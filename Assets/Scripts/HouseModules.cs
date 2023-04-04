using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Text as TMPro.TextMeshProUGUI;

// Modules that take place in the house.
// Need to be able to walk to the next exercise between exercises. maybe that is an additional exercise?
public class HouseModules : Module
{
    public TMPro.TextMeshProUGUI helpText;
    public AudioClip commotionClip;

    protected override void RunFirstModule()
    {
        (exercises[0] as WalkExercise).Arrange();
        helpText.text = "Go to the <color=#79B251>" + (exercises[0] as WalkExercise).nameOfLocation;
        helpAudio = helpText.transform.parent.GetComponentInChildren<AudioSource>();
        //helpAudio.clip = exercises[0].instructionsDefault;
        //helpAudio.Play();
    }

    public override void Advance()
    {
        base.Advance();
        helpAudio.clip = exercises[current].instructionsDefault;
        helpAudio.Play();
        if (current < exercises.Count)
        {
            if (current % 2 == 0)
            {
                helpText.text = "Go to the <color=#79B251>" + (exercises[current] as WalkExercise).nameOfLocation;
                TankController.Instance.EnableMovement();
                ScoreCalculator.instance.exercising = false;    // just walking to the next exercise should not be considered exercising
            }
            else
            {
                TankController.Instance.DisableMovement();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                if (current == 1)
                    helpText.text = "Arrange <color=#79B251>names</color> on the <color=#79B251>portrait</color> from left to right";
                else if (current == 3)
                {
                    // get a random channel to be goal
                    TVExercise tv = exercises[current] as TVExercise;
                    //tv.goalChannel = Random.Range(0, tv.channels.Length); // now set by the player 
                    helpText.text = "Flip to the <color=#79B251>" + tv.channels[tv.goalChannel].name + "</color> channel. Press <color=#79B251>'OK'</color> to confirm.";
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
        helpAudio.clip = commotionClip;
        helpAudio.Play();
    }
}
