using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureModule : ScavengerModule
{
    protected override void RunFirstModule()
    {
        (exercises[0] as PictureExercise).Arrange();
        helpText.text = "Find and click on the picture of your " + (exercises[0] as ScavengerExercise).nameOfObject;
    }

    public override void Play()
    {
        base.Play();
        for (int i = 0; i < exercises.Count; i++)
        {
            PictureExercise px = (exercises[i] as PictureExercise);

            px.picture.ChangePicture(px.leftImage);
            px.picture.moduleButton.Set(exercises[i], px.leftImage, i == 0);
            px.AddInteractable(px.picture.gameObject);
            px.picture.GetComponent<Interact>().onSelected = new UnityEngine.Events.UnityEvent<GameObject>();
            px.picture.GetComponent<Interact>().onSelected.AddListener(PictureSelected);
        }
    }

    public override void Advance()
    {
        base.Advance();
        if (current < exercises.Count)
        {
            helpText.text = "Find and click on the picture of your " + (exercises[current] as PictureExercise).nameOfObject;
            // set each interactable event return to call select on current exercise
            for (int i = 0; i < exercises.Count; i++)
            {
                PictureExercise px = (exercises[i] as PictureExercise);
                px.picture.moduleButton.Set(exercises[i], px.leftImage, i == current);
                //px.picture.GetComponent<Interact>().onSelected = new UnityEngine.Events.UnityEvent<GameObject>();
                //px.picture.GetComponent<Interact>().onSelected.AddListener(PictureSelected);
            }
        }
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void PictureSelected(GameObject selected)
    {
        SelectablePicture picture = selected.GetComponent<SelectablePicture>();
        picture.moduleButton.Click();
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
