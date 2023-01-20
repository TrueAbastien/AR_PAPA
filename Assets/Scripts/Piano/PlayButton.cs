using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class PlayButton : MonoBehaviour
{
    private bool IsPlaying = false;
    public GameObject Piano;
    public PianoTrackSelector TrackSelector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayPiano(ButtonConfigHelper helper)
    {
        PartitionDisplay partition = this.Piano.GetComponent<PartitionDisplay>();
        if (IsPlaying)
        {
            this.IsPlaying = false;
            helper.MainLabelText = "Play";
            helper.SetSpriteIconByName("play");
            partition.Stop();
        }
        else
        {
            this.IsPlaying = true;
            helper.MainLabelText = "Stop";
            helper.SetSpriteIconByName("stop");
            partition.Play(TrackSelector.SelectedTrack);
        }
    }
}
