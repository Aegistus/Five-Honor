using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    SoundManager soundManager;
    int footstepSoundID;

    private void Start()
    {
        soundManager = SoundManager.Instance;
        footstepSoundID = soundManager.GetSoundID("Footstep");
    }

    private void Step()
    {
        soundManager.PlaySoundAtPosition(footstepSoundID, transform.position);
    }
}
