using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// complete when an audio source has completed
public class TutorialAudioGoal : TutorialActionSuccessCondition
{

    [SerializeField] private AudioSource audioSource;

    private bool audioStarted;

    public override void Activate()
    {
        base.Activate();
        audioStarted = true;
    }

    public override bool IsComplete()
    {
        return !audioSource.isPlaying && audioStarted;
    }
}
