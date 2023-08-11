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

    private void NegativeButtonPressed()
    {
        isSuccess = false;
    }

    private void PositiveButtonPressed()
    {
        isSuccess = true;
    }

    private void AnyPressed()
    {
        Debug.Log("Any pressed with success? " + isSuccess);
        selectionMade = true;
    }

    public override void Activate()
    {
        base.Activate();
        TutorialUI[] allTutUIs = linkedUI.transform.parent.GetComponentsInChildren<TutorialUI>();
        foreach (var ui in allTutUIs)
            ui.gameObject.SetActive(false);

        linkedUI.gameObject.SetActive(true);
        linkedUI.SetupButton(0, NegativeButtonPressed);
        if (linkedUI.buttons.Length > 1)
            linkedUI.SetupButton(1, PositiveButtonPressed);
            
        linkedUI.anyButtonPressed = new UnityEngine.Events.UnityEvent();
        linkedUI.anyButtonPressed.AddListener(AnyPressed);
        linkedUI.gameObject.SetActive(true);
    }

    public override bool IsComplete()
    {
        return selectionMade;
    }
}
