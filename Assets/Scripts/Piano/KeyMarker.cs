using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyMarker : MonoBehaviour, IComparable<KeyMarker>
{
    public string key;

    public bool isBlack { get => key.Contains('#'); }
    public Vector3 upper { get => -transform.forward; }

    private int keyCode(KeyMarker key)
    {
        return HelperNoteLabel.ListNote.FirstOrDefault(e => e.Label == key.name)?.Midi ?? int.MaxValue;
    }
    public int CompareTo(KeyMarker other)
    {
        return keyCode(this).CompareTo(keyCode(other));
    }
}
