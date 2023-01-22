using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerControls : MonoBehaviour
{
  public KeyCode StopKey;
  public List<KeyCode> PlayKeys;
  public PartitionDisplay Partition;

  // Update Function
  private void Update()
  {
    if (Input.GetKey(StopKey))
    {
      Partition.Stop();
    }

    else
    {
      for (int ii = 0; ii < PlayKeys.Count; ++ii)
      {
        if (Input.GetKey(PlayKeys[ii]))
        {
          Partition.Play(ii);
        }
      }
    }
  }
}
