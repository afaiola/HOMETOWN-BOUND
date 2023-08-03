using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class VirtualKey : MonoBehaviour
{
    [SerializeField] private Text keyText;
    [SerializeField] private Button button;

    [System.NonSerialized] public UnityEvent<char> letterTypedEvent = new UnityEvent<char>();

    [SerializeField] private char lower, upper;
    private bool isLower;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Initialize(char l, char u)
    {
        gameObject.name = l.ToString();
        lower = l;
        upper = u;
        ToLower();
    }

    public void ToUpper()
    {
        isLower = false;
        keyText.text = upper.ToString();
    }

    public void ToLower()
    {
        isLower = true;
        keyText.text = lower.ToString();
    }

    private void OnClick()
    {
        letterTypedEvent.Invoke(isLower ? lower : upper);
    }
}
