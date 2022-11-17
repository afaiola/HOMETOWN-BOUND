using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Snap : ModuleButton
{
    [SerializeField] bool checking;
    public bool stringCmp = false;
    public TMPro.TextMeshProUGUI tmpText;

    private void Update()
    {
        if (checking && DragImage.dragged)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Any(z => z.gameObject == gameObject))
            {
                if (stringCmp)
                {
                    if (DragImage.dragged.GetComponentInChildren<TMPro.TextMeshProUGUI>().text == tmpText.text)
                    {
                        tmpText.gameObject.SetActive(true);
                        Click();
                    }
                }
                else
                {
                    if (DragImage.dragged.GetComponent<RawImage>().texture == rightTexture)
                    {
                        Click();
                    }
                }
                
            }
        }
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
}
