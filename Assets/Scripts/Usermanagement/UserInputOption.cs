using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;

public class UserInputOption : MonoBehaviour
{
    public string optionName;
    public GameObject helper;   // shows when current option is highlighted
    public Text[] helpTexts;    // various texts on the helper object to give detailed info on the inputs
    public Image statusImage;    // shows if current input is valid or not  // may not need these
    [SerializeField] private Sprite validIcon, invalidIcon;

    [System.NonSerialized] public UnityEvent inputChanged;

    protected virtual void OnValidate()
    {
        if (helper)
            helpTexts = helper.GetComponentsInChildren<Text>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Highlight(false);
    }

    public virtual string GetValue()
    {
        return "";
    }
  
    protected void InputChanged()
    {
        inputChanged.Invoke();
        SetStatusIcon();
    }

    public virtual bool GetIfInputValid()
    {
        return true;
    }

    protected void SetStatusIcon()
    {
        if (statusImage)
            statusImage.sprite = GetIfInputValid() ? validIcon : invalidIcon;
    }

    public void Highlight(bool on)
    {
        if (helper)
            helper.SetActive(on);
    }

}
