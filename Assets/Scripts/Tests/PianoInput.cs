using MidiJack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MidiMaster.noteOnDelegate = onNoteOn;
    }

    private void onNoteOn(MidiChannel channel, int note, float velocity)
    {
        Debug.Log($"CH:{channel} N:{note} V:{velocity}");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
