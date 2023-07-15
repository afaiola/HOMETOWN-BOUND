using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class DragImage : MonoBehaviour
{
    public static Button dragged;
    [SerializeField]Button image;
    [SerializeField] FlyAround fly;
    public UnityEvent dropEvent;

    private void OnValidate()
    {
        image = GetComponent<Button>();
        fly = GetComponent<FlyAround>();
    }
  
    private void Update()
    {
        if (dragged == image)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void Click()
    {
        // TODO: move with VR pointer position
        // TODO: drag on pointer select, drop on pointer select again.
        if (dragged)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            foreach (var result in results)
            {
                Snap snap = result.gameObject.GetComponent<Snap>();
                if (snap)
                    if (snap.TryDrop())
                        return;
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
}
