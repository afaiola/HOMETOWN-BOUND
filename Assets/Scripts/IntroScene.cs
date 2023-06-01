using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class IntroScene : MonoBehaviour
{
    [SerializeField] GameObject dr,nurse;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip introClip, continuePlayingClip;
    [SerializeField] SpeechBubble speechBubble;
    [SerializeField] float blinkSpeed;
    [SerializeField] Transform start;
    [SerializeField] GameObject guy;
    public GameObject point2;
    public bool skipped;
    private IEnumerator activeCoroutine;

    public UnityEvent onComplete;
    private string nextActionMessage = "Leave the hospital.";

    private void OnValidate()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void SetDialogue(bool firstTime)
    {
        string name = Profiler.Instance.currentUser.displayName;
        if (name != "") name = $", {name}";
        string subtitles = $"How are we doing today{name}? Everything looks good. Feel free to get up, get dressed, and go home from the hospital.";
        
        source.clip = introClip;
        nextActionMessage = "Leave the hospital.";
        if (!firstTime)
        {
            source.clip = continuePlayingClip;
            nextActionMessage = "Loading Checkpoint...";
            subtitles = $"How are we doing today{name}? Everything looks good. Let's pick up where we left off.";
        }

        if (speechBubble)
            speechBubble.ShowText(subtitles);
    }

    public void PlayCutscene(bool firstTime)
    {
        SetDialogue(firstTime); 
        activeCoroutine = Cutscene();
        StartCoroutine(activeCoroutine);
    }

    private IEnumerator Cutscene()
    {
        //UIManager.Instance.canPause = false;
        UIManager.Instance.inCutscene = true;
        UIManager.Instance.OpenEyes();
        yield return new WaitForSecondsRealtime(1f);
        StaticEvent.ActivateSpeechBubble();
        TankController.Instance.DisableMovement();
        TankController.Instance.enabled = false;
        
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        dr.GetComponent<Animator>().SetBool("Talking", true);
        source.Play();
        yield return new WaitForSecondsRealtime(source.clip.length);

        dr.GetComponent<Animator>().SetBool("Talking", false);
        StaticEvent.DeactivateSpeechBubble();
        UIManager.Instance.CloseEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        TankController.Instance.transform.position = start.position;
        TankController.Instance.transform.rotation = start.rotation;
        guy.SetActive(false);
        dr.SetActive(false);
        nurse.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);
        speechBubble.Close();

        UIManager.Instance.OpenEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        TankController.Instance.enabled = true;
        TankController.Instance.EnableMovement();
        TankController.Instance.transform.position = start.position;
        TankController.Instance.transform.rotation = start.rotation;
        UIManager.Instance.inCutscene = false;
        //UIManager.Instance.canPause = true;
        UIManager.Instance.PromptGameWindowFocus();
        Menu.Instance.UpdateModuleName(nextActionMessage);
        // TODO: turn off the speech bubble
        skipped = true;
        FloatingOrigin floatingOrigin = GameObject.FindObjectOfType<FloatingOrigin>();
        if (floatingOrigin)
            floatingOrigin.canUpdate = true;
        if (onComplete != null)
            onComplete.Invoke();
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
        speechBubble.Close();

        UIManager.Instance.OpenEyes();
        UIManager.Instance.inCutscene = false;
        //UIManager.Instance.canPause = true;
        Menu.Instance.UpdateModuleName(nextActionMessage);
        // TODO: turn off the speech bubble 
        FloatingOrigin floatingOrigin = GameObject.FindObjectOfType<FloatingOrigin>();
        if (floatingOrigin)
            floatingOrigin.canUpdate = true;
        skipped = true;
        if (onComplete != null)
            onComplete.Invoke();
    }
}
