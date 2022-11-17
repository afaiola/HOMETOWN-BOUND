using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ExitHospitalCutscene : MonoBehaviour
{
    [SerializeField] Transform point2;
    [SerializeField] PlayableDirector director;

    public void PlayCutscene()
    {
        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene()
    {
        UIManager.Instance.inCutscene = true;
        TankController.Instance.DisableMovement();
        UIManager.Instance.CloseEyes();
        yield return new WaitForSeconds(UIManager.Instance.blinktime);

        director.Play();
        UIManager.Instance.OpenEyes();
        yield return new WaitForSeconds((float)director.duration);

        ScoreCalculator.instance.StartActivity(0);
        ScoreCalculator.instance.exercising = false;
        TankController.Instance.EnableMovement();
        UIManager.Instance.inCutscene = false;
        UIManager.Instance.PromptGameWindowFocus();
    }

    /*
    private void Update()
    {
        if (walking)
            if (!fadeOut)
            {
                blinking = Mathf.Lerp(0,Screen.height / 2, t);
                topLid.sizeDelta = new Vector2(topLid.sizeDelta.x, blinking);
                bottomLid.sizeDelta = new Vector2(bottomLid.sizeDelta.x, blinking);
                t +=  Time.deltaTime;
                if (blinking == Screen.height/2)
                {
                    t = 0;
                    fadeOut = true;
                    player.transform.position = point2.position;
                    player.transform.rotation = point2.rotation;
                }
            }
            else
            {
                blinking = Mathf.Lerp(Screen.height / 2, 0, t);
                topLid.sizeDelta = new Vector2(topLid.sizeDelta.x, blinking);
                bottomLid.sizeDelta = new Vector2(bottomLid.sizeDelta.x, blinking);
                t +=  Time.deltaTime;
                if (blinking == 0)
                {
                    walking = false;
                    director.Play();
                }
            }
    }

    private void StartActivity()
    {
        ScoreCalculator.instance.StartActivity(0);
    }*/
}
