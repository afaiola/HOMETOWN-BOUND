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
    [SerializeField] private Button[] buttons;
    [SerializeField] private Button skipButton;

    private void Start()
    {
        foreach (var button in buttons)
        {
            button.onClick.AddListener(SelectionMade);
        }
    }

    private void SelectionMade()
    {
        if (anyButtonPressed != null)
            anyButtonPressed.Invoke();
    }

}
