using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPositionGoal : TutorialActionSuccessCondition
{
    private bool entered;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public override void Activate()
    {
        GetComponent<AudioSource>().Play();
        base.Activate();
    }

    public override bool IsComplete()
    {
        return entered;
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: may need to change this to look for a valid component
        if (other.tag == "Player")
        {
            entered = true;
            gameObject.SetActive(false);
            GetComponent<AudioSource>().Stop();
        }
    }
}
