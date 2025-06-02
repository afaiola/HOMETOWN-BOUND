using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Interact : MonoBehaviour
{
    [SerializeField]
    private ActivatorZone zone;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private Outline outline;
    public UnityEvent<GameObject> onSelected;
    public Module correspondingModule;
    public UnityEvent interactEvent;


    private GameObject player;
    private bool hovering;
    private bool resetting;


    protected void Start()
    {
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
        if (GameManager.Instance.inModule)
        {
            Menu.Instance.UpdateModuleName("Please complete the current module before attempting the next one.");
            zone.OneTime = false;
            return;
        }
        int lastModuleCompletedIndex = GameManager.Instance.GetModuleIndexLastCompleted();
        int moduleIndex = Array.IndexOf(GameManager.Instance.ModuleMapper.modules, correspondingModule);
        int skippedModuleIndex = lastModuleCompletedIndex + 1;
        var skippedModule = GameManager.Instance.ModuleMapper.modules[skippedModuleIndex];
        if (moduleIndex - lastModuleCompletedIndex > 1)
        {
            int skippedModuleLevelIndex = skippedModule.lvl - 1;
            int skippedModuleNumber = skippedModule.ModuleNo;
            Menu.Instance.UpdateModuleName($"Sorry. You missed Module <b>{skippedModuleNumber}</b> in <b>{((Levels)skippedModuleLevelIndex).ToString()} TOWN</b>. Please go back and complete before you proceed");
            Menu.Instance.MissedModuleWarning(skippedModuleNumber, (Levels)skippedModuleLevelIndex);
            zone.OneTime = false;
            return;
        }
        if (typeof(HouseModules) == correspondingModule.GetType())
        {
            SceneLoader.Instance.LoadHouseInterior();
        }
        zone.OneTime = true;
        audioSource.Play();
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
        zone.OneTime = false;
        zone.ExitEvent.AddListener(ResetOnExit);
        gameObject.SetActive(true);
    }


    private void ResetOnExit()
    {
        resetting = false;
        zone.OneTime = false;
    }
}
