using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonOption : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent<int> optionEvent;
    private Button button;

    public int ID { get => id; set => id = value; }
    private int id;

    private void OnValidate()
    {
        button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
        button.onClick.AddListener(OptionSelected);
    }

    private void OptionSelected()
    {
        optionEvent.Invoke(id);
    }

}
