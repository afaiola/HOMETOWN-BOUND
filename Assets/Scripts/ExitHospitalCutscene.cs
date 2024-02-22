using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ExitHospitalCutscene : MonoBehaviour
{
    [SerializeField] Transform point1, point2;
    [SerializeField] PlayableDirector director;

    public void PlayCutscene()
    {
        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene()
    {
        UIManager.Instance.inCutscene = true;
        TankController.Instance.DisableMovement();
        Transform player = TankController.Instance.transform;
        UIManager.Instance.CloseEyes();
        yield return new WaitForSeconds(UIManager.Instance.blinktime);
        UIManager.Instance.OpenEyes();

        director.Play();
        // face player toward door
        player.transform.position = point1.position;
        player.transform.rotation = point1.rotation;

        yield return new WaitForSeconds(2.08f);

        float timecount = 0;
        float playerMoveTime = 3.86666666666667f;
        while (timecount < playerMoveTime)
        {
            player.transform.position = Vector3.Lerp(point1.position, point2.position, timecount / playerMoveTime);
            timecount += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        timecount = 0;
        playerMoveTime = 8.73333333333333f;
        while (timecount < playerMoveTime)
        {
            player.transform.rotation = Quaternion.Lerp(point1.rotation, point2.rotation, timecount / playerMoveTime);
            timecount += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds((float)director.duration);

        ScoreCalculator.instance.StartActivity(0);
        ScoreCalculator.instance.exercising = false;
        TankController.Instance.EnableMovement();
        UIManager.Instance.inCutscene = false;
        UIManager.Instance.PromptGameWindowFocus();

        yield return new WaitForEndOfFrame();
        TankController.Instance.ForceControllerCollision();
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
