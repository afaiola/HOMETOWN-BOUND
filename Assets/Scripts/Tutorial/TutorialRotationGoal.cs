using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRotationGoal : TutorialActionSuccessCondition
{
    [SerializeField] private GameObject hintArrow;
    [SerializeField] float angleTolerance;

    private void Start()
    {
        hintArrow.SetActive(false);
    }

    public override void Activate()
    {
        base.Activate();
        GetComponent<AudioSource>().Play();
        hintArrow.SetActive(true);
    }

    public override bool IsComplete()
    {
        Vector3 dir = transform.position - TankController.Instance.transform.position;
        float angle = Vector3.Angle(TankController.Instance.transform.forward, dir);
        //Debug.Log($"angle to goal: {angle}");
        bool success = angle <= angleTolerance;
        if (success)
        {
            GetComponent<AudioSource>().Stop();
            hintArrow.SetActive(false);
        }
        return success;
    }
}
