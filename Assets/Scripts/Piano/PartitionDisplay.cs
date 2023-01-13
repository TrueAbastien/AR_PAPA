using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartitionDisplay : MonoBehaviour
{
    // Input Parameters
    public bool HasSound = true;
    public GameObject NotePrefab;
    public GameObject NoteContainer;
    public float AdvertTime;
    public float AdvertHeight;
    public GameObject Limiter;
    public MidiFilePlayer MidiPlayer;
    public MidiStreamPlayer MidiStream;
    public Color BlackColor;

    // Private Members
    private List<NoteDisplay.NoteInfo> notesInfo = new();
    private float internalTimer;
    private List<NoteDisplay> notesToPlay = new(), notesToRemove = new();
    private int noteOffset = 48;

    // Baked Info
    internal List<KeyMarker> __keys = new();

    // Properties
    private float AdvertRatio { get => AdvertHeight / AdvertTime; }

    // Start Method
    void Start()
    {
        internalTimer = 0f;
        
        // Listened to Played Music
        if (MidiPlayer != null)
        {
            if (!MidiPlayer.OnEventNotesMidi.HasEvent())
            {
                MidiPlayer.OnEventNotesMidi.AddListener(NotesToPlay);
            }
        }

        // 'Baking' Info
        foreach (KeyMarker key in Limiter.GetComponentsInChildren<KeyMarker>())
        {
            __keys.AddSorted(key);
        }
    }

    private void NotesToPlay(List<MPTKEvent> notes)
    {
        foreach (MPTKEvent note in notes)
        {
            if (note.Command == MPTKCommand.NoteOn && note.Duration > 0)
            {
                if (note.Value >= noteOffset && note.Value < noteOffset + __keys.Count)
                {
                    // TODO: make this better
                    notesInfo.AddSorted(new NoteDisplay.NoteInfo()
                    {
                        playTime = internalTimer + AdvertTime,
                        duration = 1e-3f * note.Duration,
                        keyIndex = note.Value - noteOffset,
                        mptk = note
                    });
                }
            }

            // TODO ?
        }
    }

    // Update Method
    void Update()
    {
        internalTimer += Time.deltaTime;

        // Display New Notes
        while (notesInfo.Count > 0 && internalTimer > notesInfo[0].playTime - AdvertTime)
        {
            NoteDisplay.NoteInfo note = notesInfo[0];

            // Display Note
            GameObject noteGO = GameObject.Instantiate(NotePrefab, NoteContainer.transform);
            KeyMarker refKey = __keys[note.keyIndex];

            // Position
            Vector3 position = refKey.transform.position
                + refKey.upper * (note.playTime - internalTimer + note.duration) * AdvertRatio
                + refKey.upper * Vector3.Dot(Limiter.transform.position - refKey.transform.position, refKey.upper);
            noteGO.transform.position = position;

            // Rotation
            noteGO.transform.rotation = NoteContainer.transform.rotation;

            // Scaling
            noteGO.transform.Rescale(refKey.transform.lossyScale.x * 2e-2f, note.duration * AdvertRatio);

            // Key Color
            if (refKey.isBlack)
            {
                noteGO.transform.position += noteGO.transform.forward * 1e-2f;
                noteGO.GetComponentInChildren<MeshRenderer>().material.color = BlackColor;
            }
            else
            {
                noteGO.transform.position += noteGO.transform.forward * 3e-2f;
            }

            // Velocity
            if (noteGO.TryGetComponent(out Rigidbody noteRB))
            {
                noteRB.velocity = -refKey.upper * AdvertRatio;
            }

            // Add & Remove
            if (noteGO.TryGetComponent(out NoteDisplay noteDisplay))
            {
                noteDisplay.Info = note;
                noteDisplay.State = NoteDisplay.NoteState.ToPlay;
                notesToPlay.AddSorted(noteDisplay);
            }
            notesInfo.RemoveAt(0);
        }

        // Play New Notes
        while (notesToPlay.Count > 0)
        {
            NoteDisplay note = notesToPlay[0];
            if (internalTimer < note.Info.playTime)
            {
                break;
            }

            // Play Note
            if (HasSound)
            {
                MidiStream.MPTK_PlayEvent(note.Info.mptk);
            }

            // Move Note
            note.State = NoteDisplay.NoteState.ToRemove;
            notesToPlay.RemoveAt(0);
            notesToRemove.AddSorted(note);
        }

        // Remove Old Notes
        while (notesToRemove.Count > 0)
        {
            NoteDisplay note = notesToRemove[0];
            if (internalTimer - Mathf.Epsilon <= note.Info.playTime + note.Info.duration)
            {
                break;
            }

            notesToRemove.RemoveAt(0);
            GameObject.Destroy(note.gameObject);
        }

        // Shrink Notes
        foreach (NoteDisplay note in notesToRemove)
        {
            // Shrink
            if (internalTimer > note.Info.playTime)
            {
                note.transform.Rescale(null, Mathf.Max((note.Info.duration + note.Info.playTime - internalTimer) * AdvertRatio, 0f));
            }
        }
    }

    public void Stop()
    {
        MidiPlayer?.MPTK_Stop();

        foreach (NoteDisplay note in notesToPlay)
        {
            Destroy(note.gameObject);
        }
        foreach (NoteDisplay note in notesToRemove)
        {
            Destroy(note.gameObject);
        }

        notesInfo.Clear();
        notesToPlay.Clear();
        notesToRemove.Clear();
    }

    public void Play(string filePath)
    {
        MidiPlayer?.MPTK_Play(); // TEMP

        // TODO
    }     
}

public static class Extensions
{
    public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
    {
        // No Item
        if (list.Count == 0)
        {
            list.Add(item);
            return;
        }

        // Bigger than Last
        if (list[list.Count - 1].CompareTo(item) <= 0)
        {
            list.Add(item);
            return;
        }

        // Smaller than First
        if (list[0].CompareTo(item) >= 0)
        {
            list.Insert(0, item);
            return;
        }

        // Somewhere in the Middle
        int index = list.BinarySearch(item);
        if (index < 0) index = ~index;
        list.Insert(index, item);
    }

    public static void Rescale(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(x ?? scale.x, y ?? scale.y, z ?? scale.z);
    }
}
