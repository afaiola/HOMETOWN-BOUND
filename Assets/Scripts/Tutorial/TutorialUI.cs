using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class TutorialUI : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent anyButtonPressed;

    [SerializeField] private Text title;
    public Button[] buttons;
    [SerializeField] private Button skipButton;

    private void Start()
    {
        
    }

    public void SetupButton(int idx, UnityAction callback)
    {
        if (idx < buttons.Length)
        {
            //buttons[idx].onClick = new Button.ButtonClickedEvent();
            buttons[idx].onClick.AddListener(callback);
            buttons[idx].onClick.AddListener(SelectionMade);
        }
    }

    private void SelectionMade()
    {
        if (anyButtonPressed != null)
            anyButtonPressed.Invoke();
    }

}
