using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class IntroScene : MonoBehaviour
{
    [SerializeField] GameObject dr,nurse;
    [SerializeField] AudioSource source;
    [SerializeField] float blinkSpeed;
    [SerializeField] Transform start;
    [SerializeField] GameObject guy;
    public GameObject point2;
    public bool skipped;
    private IEnumerator activeCoroutine;

    private void OnValidate()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        activeCoroutine = Cutscene();   
        StartCoroutine(activeCoroutine);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.M) && Debug.isDebugBuild)
        {
            ScoreCalculator.instance.StartActivity(0);
            Interrupt();
        }
        if (skipped)
        {
            source.Stop();
        }
    }

    private IEnumerator Cutscene()
    {
        //UIManager.Instance.canPause = false;
        UIManager.Instance.inCutscene = true;
        UIManager.Instance.OpenEyes();
        yield return new WaitForSecondsRealtime(1f);
        TankController.Instance.DisableMovement();
        TankController.Instance.enabled = false;
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        dr.GetComponent<Animator>().SetBool("Talking", true);
        source.Play();
        yield return new WaitForSecondsRealtime(source.clip.length);

        dr.GetComponent<Animator>().SetBool("Talking", false);
        UIManager.Instance.CloseEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        TankController.Instance.transform.position = start.position;
        TankController.Instance.transform.rotation = start.rotation;
        guy.SetActive(false);
        dr.SetActive(false);
        nurse.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);

        UIManager.Instance.OpenEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        TankController.Instance.enabled = true;
        TankController.Instance.EnableMovement();
        TankController.Instance.transform.position = start.position;
        TankController.Instance.transform.rotation = start.rotation;
        UIManager.Instance.inCutscene = false;
        //UIManager.Instance.canPause = true;
        UIManager.Instance.PromptGameWindowFocus();
        Menu.Instance.UpdateModuleName("Leave the hospital.");
    }

    public void Interrupt()
    {
        source.Stop();
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
        TankController.Instance.transform.position = point2.transform.position;
        TankController.Instance.EnableMovement();
        UIManager.Instance.OpenEyes();
        UIManager.Instance.inCutscene = false;
        //UIManager.Instance.canPause = true;
        Menu.Instance.UpdateModuleName("Leave the hospital.");
    }
}
