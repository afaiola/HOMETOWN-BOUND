using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using InputField = TMPro.TMP_InputField;


public class TabSelector : MonoBehaviour
{
    public Selectable[] selectableItems;
    private int selectIdx = 0;
    EventSystem system;

    [System.NonSerialized] public UnityEvent<int> selectEvent;

    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
        system.SetSelectedGameObject(selectableItems[0].gameObject, new BaseEventData(system));

    }

    private void OnEnable()
    {
        selectIdx = 0;
        SelectNext();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selectIdx++;
            if (selectIdx >= selectableItems.Length) selectIdx = 0;
            SelectNext();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (system == null)
            {
                system = EventSystem.current;
            }
            // something has been clicked, update the index
            if (system.currentSelectedGameObject == null) return;
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>();
            if (next != null)
            {
                for (int i = 0; i < selectableItems.Length; i++)
                {
                    if (next == selectableItems[i])
                    {
                        selectIdx = i;
                        if (selectEvent != null)
                            selectEvent.Invoke(selectIdx);
                        break;
                    }
                }
            }
        }
    }

    private void SelectNext()
    {
        Selectable next = selectableItems[selectIdx];

        if (next != null)
        {
            InputField inputfield = next.GetComponent<InputField>();
            if (inputfield != null)
                inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

            if (system != null)
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));

            if (selectEvent != null)
                selectEvent.Invoke(selectIdx);
        }
    }
}
