using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// success after opening or closing eyes
public class TutorialBlinkGoal : TutorialActionSuccessCondition
{
    public bool doOpen, isSitting;
    private bool hasBlinked;

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(DoBlink());
    }

    public override bool IsComplete()
    {
        return hasBlinked;
    }

    private void SetPosition()
    {
        if (isSitting)
        {
            VRManager.Instance.SetCameraSitting();
        }
        else
        {
            VRManager.Instance.SetCameraStanding();
        }
    }

    private IEnumerator DoBlink()
    {
        if (doOpen)
        {
            UIManager.Instance.OpenEyes();
            // if opening, set position before we open our eyes
            SetPosition();
        }
        else
        {
            UIManager.Instance.CloseEyes();
        }

        yield return new WaitForSeconds(UIManager.Instance.blinktime);

        SetPosition();

        hasBlinked = true;
    }
}
