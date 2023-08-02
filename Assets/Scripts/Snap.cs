using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Snap : ModuleButton, IMoveHandler
{
    [SerializeField] bool checking;
    public bool stringCmp = false;
    public TMPro.TextMeshProUGUI tmpText;
    private float autoSnapDistance = 7.5f;

    private void Start()
    {
        if (VRManager.Instance)
            autoSnapDistance = 0.025f;

    }

    private void Update()
    {
        if (DragImage.dragged)
        {
            if (DragImage.dragged.GetComponent<RawImage>().texture == rightTexture)
            {
                float dist = Vector2.Distance(DragImage.dragged.transform.position, transform.position);
                if (dist < autoSnapDistance)
                {
                    TryDrop();
                }
            }
        }
    }

    public bool TryDrop()
    {
        if (stringCmp)
        {
            if (DragImage.dragged.GetComponentInChildren<TMPro.TextMeshProUGUI>().text == tmpText.text)
            {
                tmpText.gameObject.SetActive(true);
                Click();
                return true;
            }
        }
        else
        {
            if (DragImage.dragged.GetComponent<RawImage>().texture == rightTexture)
            {
                Click();
                return true;
            }
        }
        return false;
    }

    public override void Click()
    {
        checking = false;
        DragImage.StopDrag();
        image.texture = rightTexture;
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = rightClip;
        audioSource.Play();
        correct = true;
        GetComponentInParent<DragExercise>().Select(this);
        image.color = new Color(1, 1, 1, 1);
    }

    public void OnMove(AxisEventData eventData)
    {
        if (checking && DragImage.dragged)
        {
            List<RaycastResult> results = new List<RaycastResult>();

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = DragImage.dragged.transform.position;
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            
            if (results.Any(z => z.gameObject == gameObject))
            {
                float dist = Vector2.Distance(DragImage.dragged.transform.position, transform.position);
                if (dist < 5f)
                {
                    TryDrop();
                }

            }
        }
    }
}
