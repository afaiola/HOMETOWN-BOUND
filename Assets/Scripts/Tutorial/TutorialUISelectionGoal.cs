using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUISelectionGoal : TutorialActionSuccessCondition
{
    [SerializeField] private TutorialUI linkedUI;

    private bool selectionMade;

    private void Start()
    {
    }

    private void AnyPressed()
    {
        selectionMade = true;
    }

    public override void Activate()
    {
        base.Activate();
        linkedUI.anyButtonPressed = new UnityEngine.Events.UnityEvent();
        linkedUI.anyButtonPressed.AddListener(AnyPressed);
        linkedUI.gameObject.SetActive(true);
    }

    public override bool IsComplete()
    {
        return selectionMade;
    }
}
