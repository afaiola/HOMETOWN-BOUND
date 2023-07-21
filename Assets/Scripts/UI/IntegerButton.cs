using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntegerButton : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent<int> sendIntEvent = new UnityEvent<int>();
    private Button button;
    private TMPro.TextMeshProUGUI text;
    private int storedInt;

    public void Setup(int value)
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        storedInt = value;
        text.text = storedInt.ToString();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        sendIntEvent.Invoke(storedInt);
    }


}
