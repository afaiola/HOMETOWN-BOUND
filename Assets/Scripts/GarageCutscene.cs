using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCutscene : MonoBehaviour
{
    [SerializeField]
    public Transform peopleParent;
    [SerializeField]
    public Door door;
    [SerializeField]
    public ActivatorZone activator;
    [SerializeField]
    public GameObject garageUI;
    [SerializeField]
    public AudioSource crowdAudio;
    [SerializeField]
    public AudioClip whispers;
    [SerializeField]
    private AudioClip cheer;
    [SerializeField]
    private CyclicFlash path;


    protected void Start()
    {
        activator.EnterEvent = new UnityEngine.Events.UnityEvent();
        activator.EnterEvent.AddListener(OpenDoor);
        garageUI.SetActive(false);
        path.gameObject.SetActive(false);
    }


    public void Activate()
    {
        activator.enabled = true;
        activator.gameObject.SetActive(true);
        //garageUI.SetActive(true);
        Whispers();
        path.gameObject.SetActive(true);
    }

    public void OpenDoor()
    {
        door.Open();
    }

    public void Whispers()
    {
        crowdAudio.clip = whispers;
        crowdAudio.Play();
    }

    public void AnimateGuests()
    {
        crowdAudio.clip = cheer;
        crowdAudio.Play();

        Animator[] animators = peopleParent.GetComponentsInChildren<Animator>();
        foreach (var a in animators)
        {
            a.SetInteger("ID", Random.Range(0, 3));
            a.SetBool("Cheering", true);
            a.SetFloat("Offset", Random.Range(0f, 1f));
            a.GetComponentInParent<LookAt>().Look();
        }
        door.moveTime = 0.1f;
        door.Close();
        garageUI.SetActive(false);
        Invoke("EndGame", 7f);
    }


    private void EndGame()
    {
        UIManager.Instance.ShowEndScreen();
    }
}
