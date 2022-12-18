using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PianoRecord : MonoBehaviour
{
    public string FilePath;


    // Start Function
    void Start()
    {
        File.Create($@"{FilePath}");
    }

    // Update Function
    void Update()
    {
    }
}
