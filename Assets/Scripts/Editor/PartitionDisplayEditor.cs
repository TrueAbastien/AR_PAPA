using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PartitionDisplay), isFallback = true)]
public class PartitionDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PartitionDisplay partition = (target as PartitionDisplay);

        if (GUILayout.Button("Play"))
        {
            partition.Stop();
            partition.Play();
        }
        else if (GUILayout.Button("Stop"))
        {
            partition.Stop();
        }
    }
}
