using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Modules that take place in the house.
// Need to be able to walk to the next exercise between exercises. maybe that is an additional exercise?
public class HouseModules : Module
{
    public Text helpText;

    protected override void RunFirstModule()
    {
        (exercises[0] as WalkExercise).Arrange();
        helpText.text = "Go to the " + (exercises[0] as WalkExercise).nameOfLocation;
    }

    public override void Advance()
    {
        base.Advance();
        if (current < exercises.Count)
        {
            if (current % 2 == 0)
            {
                helpText.text = "Go to the " + (exercises[current] as WalkExercise).nameOfLocation;
                TankController.Instance.EnableMovement();
                ScoreCalculator.instance.exercising = false;    // just walking to the next exercise should not be considered exercising
            }
            else
            {
                TankController.Instance.DisableMovement();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                if (current == 1)
                    helpText.text = "Arrange names on your family portrait left to right";
                else if (current == 3)
                {
                    // get a random channel to be goal
                    TVExercise tv = exercises[current] as TVExercise;
                    //tv.goalChannel = Random.Range(0, tv.channels.Length); // now set by the player 
                    helpText.text = "Flip to the " + tv.channels[tv.goalChannel].name + " channel. Press 'OK' to confirm.";
                }
            }
        }
        //ScoreCalculator.instance.exercising = false;
    }

    public override void End()
    {
        base.End();
        GarageCutscene garage = GameObject.FindObjectOfType<GarageCutscene>();
        if (garage)
        {
            garage.Activate();
            SoundManager.Instance.StopDistracting();
        }
        ScoreCalculator.instance.exercising = false;
    }
}
