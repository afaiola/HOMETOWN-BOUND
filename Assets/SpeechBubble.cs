using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI messageText;
    public string defaultMessage;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        messageText.text = defaultMessage;
        Invoke("Close", 5f);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ShowText(string message)
    {
        gameObject.SetActive(true);
        messageText.text = message;
    }

}
