using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDisplay : MonoBehaviour, IComparable<NoteDisplay>
{
    [Serializable]
    public class NoteInfo : IComparable<NoteInfo>
    {
        public float playTime;
        public float duration;
        public int keyIndex;
        public bool isBlack;
        public MPTKEvent mptk;

        public int CompareTo(NoteInfo other) 
        {
            return playTime.CompareTo(other.playTime);
        }
    }

    public enum NoteState
    {
        ToPlay,
        ToRemove
    }

    [HideInInspector] public NoteInfo Info;
    [HideInInspector] public NoteState State;

    // Start Method
    void Start()
    {
        
    }

    // Update Method
    void Update()
    {
        
    }

    protected float ComparableFactor { get => Info.playTime + (State == NoteState.ToRemove ? Info.duration : 0f); }

    public int CompareTo(NoteDisplay other)
    {
        return ComparableFactor.CompareTo(other.ComparableFactor);
    }
}
