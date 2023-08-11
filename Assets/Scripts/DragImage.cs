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

    private RectTransform rect;
    private bool grabbedByPrimaryHand = true;

    private void OnValidate()
    {
        image = GetComponent<Button>();
        fly = GetComponent<FlyAround>();
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 inputPos = Input.mousePosition;
        if (canvasHelper)
        {
            if (dragged == image)
                if (!canvasHelper.GetCanvasWorldPosition(transform.position, ref inputPos, ref grabbedByPrimaryHand)) return;
        }
        SetDraggedPosition(inputPos);

    }

    public void SetDraggedPosition(Vector3 pos)
    {
        if (dragged == image)
        {
            pos = Vector3.Lerp(transform.position, pos, Time.deltaTime * dragFollowSpeed);
            transform.position = pos;
            rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, rect.anchoredPosition3D.y, 0);
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
                // not working for vr
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
            Vector3 inputPos = new Vector3();
            // set the hand by which this was grabbed
            canvasHelper.GetCanvasWorldPosition(transform.position, ref inputPos, ref grabbedByPrimaryHand, true);
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
        Click(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // checking time here to prevent dropping right after picking up
        // this allows to drag and drop and for click on pickup and putdown

        //image.OnPointerClick(eventData);
        Click(eventData);

    }

}
