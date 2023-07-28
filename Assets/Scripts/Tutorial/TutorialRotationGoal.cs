using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRotationGoal : TutorialActionSuccessCondition
{
    [SerializeField] float angleTolerance;

    public override bool IsComplete()
    {
        Vector3 dir = transform.position - TankController.Instance.transform.position;
        float angle = Vector3.Angle(TankController.Instance.transform.forward, dir);
        Debug.Log($"angle to goal: {angle}");
        return angle <= angleTolerance;
    }
}
