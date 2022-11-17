using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Modules for the City
/// </summary>
public class Modules2 : Module
{
    public float speedInc = 0.5f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    protected override void RunFirstModule()
    {
        (exercises[0] as DragExercise).Arrange();
    }

    public override void Advance()
    {
        minSpeed += speedInc;
        maxSpeed += speedInc;
        if (current + 1 < exercises.Count)
        {
            (exercises[current + 1] as DragExercise).minSpeed = minSpeed;
            (exercises[current + 1] as DragExercise).maxSpeed = maxSpeed;
        }
        base.Advance();

    }
}
