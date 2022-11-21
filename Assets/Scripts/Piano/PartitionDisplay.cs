using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartitionDisplay : MonoBehaviour
{
    // Input Parameters
    public GameObject NotePrefab;
    public GameObject NoteContainer;
    public float AdvertTime;
    public float AdvertHeight;
    public GameObject Limiter;
    public List<float> KeysWidth;
    public MidiFilePlayer MidiPlayer;
    public MidiStreamPlayer MidiStream;

    // Private Members
    private List<NoteDisplay.NoteInfo> notesInfo = new();
    private float internalTimer;
    private List<NoteDisplay> notesToPlay = new(), notesToRemove = new();

    // Baked Info
    internal List<float> __cummulativeKeysWidth = new();
    internal float __keysWidthSum;

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
        float widthSum = 0f;
        foreach (float width in KeysWidth)
        {
            __cummulativeKeysWidth.Add(widthSum + 0.5f * width);
            widthSum += width;
        }
        __keysWidthSum = widthSum;

        // Limiter Scaling
        Limiter.transform.Rescale(widthSum);
    }

    private void NotesToPlay(List<MPTKEvent> notes)
    {
        foreach (MPTKEvent note in notes)
        {
            if (note.Command == MPTKCommand.NoteOn && note.Duration > 0)
            {
                if (note.Value >= 36 && note.Value < 36 + KeysWidth.Count)
                {
                    // TODO: make this better
                    notesInfo.AddSorted(new NoteDisplay.NoteInfo()
                    {
                        playTime = internalTimer + AdvertTime,
                        duration = 1e-3f * note.Duration,
                        keyIndex = note.Value - 36,
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
            noteGO.transform.position = NoteContainer.transform.position + new Vector3(
                __cummulativeKeysWidth[note.keyIndex] - __keysWidthSum / 2f,
                (note.playTime - internalTimer + note.duration) * AdvertRatio);
            noteGO.transform.rotation = NoteContainer.transform.rotation;

            // Scaling
            noteGO.transform.Rescale(KeysWidth[note.keyIndex], note.duration * AdvertRatio);

            // Velocity
            if (noteGO.TryGetComponent(out Rigidbody noteRB))
            {
                noteRB.velocity = -noteGO.transform.up * AdvertRatio;
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
            MidiStream.MPTK_PlayEvent(note.Info.mptk);

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
