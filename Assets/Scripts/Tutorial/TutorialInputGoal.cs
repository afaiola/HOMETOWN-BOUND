using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialInputGoal : TutorialActionSuccessCondition
{
    [SerializeField] private InputActionReference primaryInput, secondaryInput;
    private bool ready, inputReceived;

    public override void Activate()
    {
        base.Activate();
        ready = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready) return;

        if (primaryInput.action.ReadValue<float>() > 0.1f)
        {
            inputReceived = true;
        }
        if (secondaryInput.action.ReadValue<float>() > 0.1f)
        {
            inputReceived = true;
        }

    }

    public override bool IsComplete()
    {
        return inputReceived;
    }
}
