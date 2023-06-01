using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Modules3 : Module
{
    public TMPro.TextMeshProUGUI helpText;
    public float speedInc = 0.5f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float minFlipTime = 3f;
    public float maxFlipTime = 10f;
    public float timeInc = 0.25f;
    public float timeActive = 5f;
    public FlyAround.FlyPattern flyPattern;

    public override void OnValidate()
    {
        base.OnValidate();
        /*foreach (var ex in exercises)
        {
            CardExercise cx = (CardExercise)ex;
            cx.flyPattern = flyPattern;
            foreach (var card in cx.GetComponentsInChildren<FlyAround>())
            {
                card.pattern = flyPattern;
            }
        }*/
        helpAudio = helpText.transform.parent.GetComponentInChildren<AudioSource>();
    }

    protected override void RunFirstModule()
    {
        exercises[current].gameObject.SetActive(true);
        (exercises[current] as CardExercise).flyPattern = flyPattern;
        (exercises[current] as CardExercise).flyPattern = flyPattern;
        (exercises[current] as CardExercise).minSpeed = minSpeed;
        (exercises[current] as CardExercise).maxSpeed = maxSpeed;
        (exercises[current] as CardExercise).timeActive = timeActive;
        (exercises[current] as CardExercise).minFlipTime = minFlipTime;
        (exercises[current] as CardExercise).maxFlipTime = maxFlipTime;
        (exercises[current] as CardExercise).Arrange();

        helpText.text = "Click on the picture of " + (exercises[current] as CardExercise).nameOfObject;
        if (helpAudio == null)
            helpAudio = helpText.transform.parent.GetComponentInChildren<AudioSource>();
        helpAudio.clip = exercises[current].customContent ? exercises[current].instructionsCustom : exercises[current].instructionsDefault;
        helpAudio.Play();
    }

    public override void Advance()
    {
        minSpeed += speedInc;
        maxSpeed += speedInc;
        timeActive -= timeInc;
        minFlipTime -= timeInc;
        maxFlipTime -= timeInc;
        if (minFlipTime < 1f) minFlipTime = 1f;
        if (current + 1 < exercises.Count)
        {
            (exercises[current + 1] as CardExercise).flyPattern = flyPattern;
            (exercises[current + 1] as CardExercise).minSpeed = minSpeed;
            (exercises[current + 1] as CardExercise).maxSpeed = maxSpeed;
            (exercises[current + 1] as CardExercise).timeActive = timeActive;
            (exercises[current + 1] as CardExercise).minFlipTime = minFlipTime;
            (exercises[current + 1] as CardExercise).maxFlipTime = maxFlipTime;
        }
        base.Advance();
        if (current < exercises.Count)
        {
            helpText.text = "Click on the picture of " + (exercises[current] as CardExercise).nameOfObject;
            helpAudio.clip = exercises[current].customContent ? exercises[current].instructionsCustom : exercises[current].instructionsDefault;
            helpAudio.Play();
        }
    }
}
