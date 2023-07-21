using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class DragImage : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private float dragFollowSpeed = 1f;
    public static Button dragged;
    [SerializeField]Button image;
    [SerializeField] FlyAround fly;
    public UnityEvent dropEvent;

    private VRCanvasHelper canvasHelper;
    private float clickTime;
    private float clickDelay = 0.5f;

    private void OnValidate()
    {
        image = GetComponent<Button>();
        fly = GetComponent<FlyAround>();
    }

    private void Update()
    {
        Vector3 inputPos = Input.mousePosition;
        if (canvasHelper)
        {
            if (!canvasHelper.GetCanvasWorldPosition(ref inputPos, dragged != image)) return;
        }
        SetDraggedPosition(inputPos);

    }

    public void SetDraggedPosition(Vector3 pos)
    {
        if (dragged == image)
        {
            pos = Vector3.Lerp(transform.position, pos, Time.deltaTime * dragFollowSpeed);
            Debug.Log("Setting pos to " + pos);
            transform.position = pos;
        }
    }

    public void Click(PointerEventData eventData)
    {
        // prevents OnPointerUp selects occuring right after OnPointerDown events
        // occurs when trying to release the dragged image
        if (canvasHelper == null && VRManager.Instance)
            canvasHelper = GetComponentInParent<VRCanvasHelper>();

        if (Time.time - clickTime < clickDelay) return; 

        clickTime = Time.time;
        if (dragged)
        {
            if (dragged == image)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                eventData.position = transform.position;
                EventSystem.current.RaycastAll(eventData, results);
                foreach (var result in results)
                {
                    Snap snap = result.gameObject.GetComponent<Snap>();
                    if (snap)
                        if (snap.TryDrop())
                            return;
                }
            }
            dragged = null;
            fly.enabled = true;
            if (dropEvent != null)
            {
                dropEvent.Invoke();
            }
        }
        else
        {
            dragged = image;
            fly.enabled = false;
        }
    }

    internal static void StopDrag()
    {
        dragged.enabled = false;
        dragged.gameObject.SetActive(false);
        dragged = null;
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("pointer down");
        Click(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // checking time here to prevent dropping right after picking up
        // this allows to drag and drop and for click on pickup and putdown

        //image.OnPointerClick(eventData);
        Debug.Log("pointer up");
        Click(eventData);

    }

}
