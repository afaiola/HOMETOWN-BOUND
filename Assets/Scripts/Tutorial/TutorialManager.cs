using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialSequence[] sequences;
    private int currSequence;

    private void OnValidate()
    {
        sequences = GetComponentsInChildren<TutorialSequence>();
    }

    public void BeginTutorial()
    {
        currSequence = -1;
        //foreach (var goal in GameObject.FindObjectsOfType<TutorialPositionGoal>())
        //    goal.gameObject.SetActive(false);
        //foreach (var goal in GameObject.FindObjectsOfType<TutorialRotationGoal>())
        //    goal.gameObject.SetActive(false);
        foreach (var helper in GameObject.FindObjectsOfType<TutorialSpriteHelper>())
            helper.gameObject.SetActive(false);
        foreach (var ui in GameObject.FindObjectsOfType<TutorialUI>())
            ui.gameObject.SetActive(false);

        StartNextSequence();
        TankController.Instance.DisableMovement();
    }

    private void StartNextSequence()
    {
        currSequence++;
        if (currSequence >= sequences.Length)
        {
            return;
        }
        sequences[currSequence].sequenceComplete = new UnityEngine.Events.UnityEvent();
        sequences[currSequence].sequenceComplete.AddListener(StartNextSequence);

        sequences[currSequence].StartSequence();
    }

    public void EndTutorial()
    {
        // blink
        // teleport to game start
        // remove doctor and nurse
        sequences[sequences.Length - 1].StartSequence();
        //GameObject.FindObjectOfType<IntroScene>().PlayCutscene(true);
    }
}
