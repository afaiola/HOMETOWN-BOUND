using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScavengerModule : Module
{
    public Text helpText;

    protected override void RunFirstModule()
    {
        (exercises[0] as ScavengerExercise).Arrange();
        helpText.text = "Find and click on " + (exercises[0] as ScavengerExercise).nameOfObject;
    }

    public override void Play()
    {
        base.Play();
        TankController.Instance.EnableMovement();
    }

    public override void Advance()
    {
        base.Advance();
        if (current < exercises.Count)
            helpText.text = "Find and click on " + (exercises[current] as ScavengerExercise).nameOfObject;
        ScoreCalculator.instance.exercising = false;
    }

}
