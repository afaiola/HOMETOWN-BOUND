using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;
using UnityEngine.Events;

public class ToggleOption : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] string onText, offText;
    [SerializeField] Toggle toggle;

    [System.NonSerialized]
    public UnityEvent<bool> onValueChange;

    private void OnValidate()
    {
        title = GetComponentInChildren<Text>();
        toggle = GetComponentInChildren<Toggle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        toggle.onValueChanged.AddListener(SetValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(bool value)
    {
        toggle.isOn = value;
        if (title)
            title.text = value ? onText : offText;
        if (onValueChange != null)
            onValueChange.Invoke(value);
    }
}
