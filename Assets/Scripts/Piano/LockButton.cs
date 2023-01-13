using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class LockButton : MonoBehaviour
{
    private bool IsLocked = false;
    public GameObject objectToLock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LockPiano(ButtonConfigHelper helper)
    {
        ObjectManipulator manipulator = objectToLock.GetComponent<ObjectManipulator>();
        if (IsLocked)
        {
            manipulator.enabled = false;
            //TODO: Display Play button
            this.IsLocked = false;
            helper.SetSpriteIconByName("unlock");
        }
        else
        {
            manipulator.enabled = true;
            // TODO: Hide Play button
            this.IsLocked = true;
            helper.SetSpriteIconByName("lock");
        }
    }
}
