using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;

public class UserInputOption : MonoBehaviour
{
    [System.NonSerialized] public int id;
    public string optionName;
    public GameObject helper;   // shows when current option is highlighted
    public Text[] helpTexts;    // various texts on the helper object to give detailed info on the inputs
    public Image statusImage;    // shows if current input is valid or not  // may not need these
    public string errorfieldname;
    [SerializeField] private Sprite validIcon, invalidIcon;

    [System.NonSerialized] public UnityEvent inputChanged;
    [System.NonSerialized] public UnityEvent<int> selected;


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
        bool valid = GetIfInputValid();
        if (statusImage)
            statusImage.sprite = valid ? validIcon : invalidIcon;
        Highlight(valid);
    }

    public virtual void Highlight(bool on)
    {
        if (helper)
            helper.SetActive(on);
    }

    protected void OnSelect()
    {
        if (selected != null)
            selected.Invoke(id);
    }

}
