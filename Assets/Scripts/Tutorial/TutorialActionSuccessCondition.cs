using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialActionSuccessCondition : MonoBehaviour
{
    public bool Successful { get { return isSuccess; } }
    protected bool isSuccess;

    public virtual void Activate()
    {

    }

    public virtual bool IsComplete()
    {
        return true;
    }
}
