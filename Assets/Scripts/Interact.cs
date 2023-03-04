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
    private bool hovering;

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
}
