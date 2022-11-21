using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDisplay : MonoBehaviour, IComparable<NoteDisplay>
{
    public class NoteInfo : IComparable<NoteInfo>
    {
        public float playTime;
        public float duration;
        public int keyIndex;
        public MPTKEvent mptk;

        //public Color? overridenColor;

        public int CompareTo(NoteInfo other) 
        {
            return playTime.CompareTo(other.playTime);
        }
    }

    [HideInInspector]
    public NoteInfo Info;

    // Start Method
    void Start()
    {
        
    }

    // Update Method
    void Update()
    {
        
    }

    public int CompareTo(NoteDisplay other)
    {
        return (Info.playTime + Info.duration).CompareTo(other.Info.playTime + other.Info.duration);
    }
}
