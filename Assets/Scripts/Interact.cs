using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Interact : MonoBehaviour
{
    public UnityEvent<GameObject> onSelected;
    [SerializeField]
    private Outline outline;
    public Module correspondingModule;
    public UnityEvent interactEvent;


    private GameObject player;
    private bool hovering, resetting;


    protected void Start()
    {
        outline = GetComponentInChildren<Outline>();
        player = GameObject.FindGameObjectWithTag("Player");
    }


    protected void Update()
    {
        if (player == null) return;

        if (hovering && Vector3.Distance(transform.position, player.transform.position) < 15f)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }


    public void OnMouseUp()
    {
        if (outline.enabled && TankController.Instance.canMove && Vector3.Distance(transform.position, player.transform.position) < 15f)
        {
            if (interactEvent != null)
                interactEvent.Invoke();
            if (onSelected != null)
                onSelected.Invoke(gameObject);
        }
    }

    public void ModuleInteract()
    {
        if (resetting) return;
        ActivatorZone zone = GetComponent<ActivatorZone>();

        // TODO: prevent interact when another module is playing
        if (GameManager.Instance.inModule)
        {
            Menu.Instance.UpdateModuleName("Please complete the current module before attempting the next one.");
            zone.OneTime = false;
            return;
        }

        // if previous module not played, prompt user to play the correct previous module
        // TODO: figure out if we want to use last module with saved info or for the current play session
        int currAttempt = SavePatientData.Instance.CurrentAttempt;
        int lastModule = SavePatientData.Instance.LastModulePlayed(currAttempt); // TODO : passing in the correct attempt?
        int lastLevel = lastModule / 5; // TODO : magic number
        int modNum = correspondingModule.ModuleNo + (correspondingModule.lvl - 1) * 5; // TODO : magic number
        Debug.Log($"Interact with {modNum}. Last module on attempt {currAttempt} was {lastModule}");
        if (modNum - lastModule > 1)
        {
            Menu.Instance.UpdateModuleName($"Sorry. You missed Module <b>{(lastModule + 1) % 5}</b> in <b>{((Levels)lastLevel).ToString()} TOWN</b>. Please go back and complete before you proceed");
            Menu.Instance.MissedModuleWarning((lastModule + 1) % 5, (Levels)lastLevel);
            zone.OneTime = false;
            return;
        }
        if (typeof(HouseModules) == correspondingModule.GetType())
        {
            SceneLoader.Instance.LoadHouseInterior();
        }
        zone.OneTime = true;
        GetComponent<AudioSource>().Play();
        correspondingModule.Play();
        StaticEvent.moduleStarted();
        gameObject.SetActive(false);
    }

    public void OnMouseEnter()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hovering = true;
    }

    public void OnMouseExit()
    {
        hovering = false;
    }

    public void Reset()
    {
        resetting = true;
        ActivatorZone zone = GetComponent<ActivatorZone>();
        zone.OneTime = false;
        zone.ExitEvent.AddListener(ResetOnExit);
        gameObject.SetActive(true);
    }


    private void ResetOnExit()
    {
        resetting = false;
        ActivatorZone zone = GetComponent<ActivatorZone>();
        zone.OneTime = false;
    }
}
