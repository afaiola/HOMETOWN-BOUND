using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Interact : MonoBehaviour/*, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler*/
{
    public UnityEvent<GameObject> onSelected;
    [SerializeField]Outline outline;
    public Module correspondingModule;
    public UnityEvent interactEvent;

    private GameObject player;
    private bool hovering, resetting;

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
            zone.oneTime = false;
            return;
        }

        // if previous module not played, prompt user to play the correct previous module
        // TODO: figure out if we want to use last module with saved info or for the current play session
        int currAttempt = SavePatientData.Instance.GetActiveAttempt();
        int lastModule = SavePatientData.Instance.LastModulePlayed(currAttempt);
        int lastLevel = lastModule / 5;
        int modNum = correspondingModule.ModuleNo + (correspondingModule.lvl-1) * 5;
        Debug.Log($"Interact with {modNum}. Last module on attempt {currAttempt} was {lastModule}");
        if (modNum - lastModule > 1)
        {
            Menu.Instance.UpdateModuleName($"Sorry. You missed Module <b>{(lastModule+1) % 5}</b> in <b>{((Levels)lastLevel).ToString()} TOWN</b>. Please go back and complete before you proceed");
            Menu.Instance.MissedModuleWarning((lastModule + 1) % 5, (Levels)lastLevel);
            zone.oneTime = false;
            return;
        }
        if (typeof(HouseModules) == correspondingModule.GetType())
        {
            SceneLoader.Instance.LoadHouseInterior();
        }
        zone.oneTime = true;
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

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponentInChildren<Outline>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
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

    public void Reset()
    {
        resetting = true;
        ActivatorZone zone = GetComponent<ActivatorZone>();
        zone.oneTime = false;
        zone.exitEvent.AddListener(ResetOnExit);
        gameObject.SetActive(true);
    }

    private void ResetOnExit()
    {
        resetting = false;
        ActivatorZone zone = GetComponent<ActivatorZone>();
        zone.oneTime = false;
    }
}
