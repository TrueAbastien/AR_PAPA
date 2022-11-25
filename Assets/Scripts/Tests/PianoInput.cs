using MidiJack;
using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PianoInput : MonoBehaviour
{
    public MidiStreamPlayer MidiStream;

    // Start is called before the first frame update
    void Start()
    {
        MidiMaster.noteOnDelegate = onNoteOn;
        MidiMaster.noteOffDelegate = onNoteOff;
    }

    private int noteToKeyboard(int note)
    {
        return note - 12;
    }

    private void onNoteOn(MidiChannel channel, int note, float velocity)
    {
        Debug.Log($"CH:{channel} N:{note} V:{velocity}");

        MidiStream.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Channel = (int)channel,
            Velocity = (int)(velocity * 127),
            Value = noteToKeyboard(note)
        });
    }

    private void onNoteOff(MidiChannel channel, int note)
    {
        MidiStream.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.NoteOff,
            Channel = (int)channel,
            Value = noteToKeyboard(note)
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
