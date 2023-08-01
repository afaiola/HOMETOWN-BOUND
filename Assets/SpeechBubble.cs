using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI messageText;
    public string defaultMessage;

    private float messageTime, messageDuration;
    private bool messageActive;

    private void Start()
    {
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (messageActive && Time.time - messageTime > messageDuration)
        {
            Close();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        messageText.text = defaultMessage;
        Invoke("Close", 5f);
    }

    public void Close()
    {
        messageActive = false;
        gameObject.SetActive(false);

        Animator animator = GetComponentInParent<Animator>();
        if (animator)
        {
            animator.SetBool("Talking", false);
        }
    }

    public void ShowText(string message, float timeActive=10f)
    {
        messageActive = true;
        messageTime = Time.time;
        messageDuration = timeActive;

        gameObject.SetActive(true);
        messageText.text = message;

        Animator animator = GetComponentInParent<Animator>();
        if (animator)
        {
            animator.SetBool("Talking", true);
        }
    }

}
