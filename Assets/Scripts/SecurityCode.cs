using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SecurityEvent : UnityEvent<bool> { }

public class SecurityCode : MonoBehaviour
{
    public static SecurityCode Instance { get { return _instance; } }
    private static SecurityCode _instance;

    private InputField input;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closebutton;
    [SerializeField] private string code;

    public UnityEvent onSuccess;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        input = GetComponentInChildren<InputField>();
        submitButton.onClick.AddListener(CheckCode);
        closebutton.onClick.AddListener(Stop);
        Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckCode();
        }
    }

    private void CheckCode()
    {
        if (input.text == code)
        {
            StartCoroutine(ValidResponse());
        }
        else
        {
            StartCoroutine(InvalidResponse());
        }
    }

    private IEnumerator ValidResponse()
    {
        Image buttonImage = submitButton.GetComponent<Image>();
        Color buttonColor = buttonImage.color;
        Text submitText = submitButton.GetComponentInChildren<Text>();
        Color textColor = submitText.color;

        submitButton.interactable = false;
        submitButton.enabled = false;
        buttonImage.enabled = false;
        submitText.text = "Success!";
        //buttonImage.color = Color.green;
        submitText.color = Color.green;

        yield return new WaitForSecondsRealtime(1f);

        if (onSuccess != null)
            onSuccess.Invoke();

        submitButton.interactable = true;
        submitButton.enabled = true;
        buttonImage.enabled = true;
        submitText.text = "Submit";
        buttonImage.color = buttonColor;
        submitText.color = textColor;
        input.text = "";
        Stop();
    }

    private IEnumerator InvalidResponse()
    {
        Image buttonImage = submitButton.GetComponent<Image>();
        Color buttonColor = buttonImage.color;
        Text submitText = submitButton.GetComponentInChildren<Text>();
        Color textColor = submitText.color;

        submitButton.interactable = false;
        submitButton.enabled = false;
        buttonImage.enabled = false;
        submitText.text = "Invalid Code";
        //buttonImage.color = Color.red;
        submitText.color = Color.red;

        yield return new WaitForSecondsRealtime(2f);

        submitButton.interactable = true;
        submitButton.enabled = true;
        buttonImage.enabled = true;
        submitText.text = "Submit";
        buttonImage.color = buttonColor;
        submitText.color = textColor;
        input.text = "";
    }

    private void Stop()
    {
        canvas.SetActive(false);
    }

    public void Show()
    {
        canvas.SetActive(true);
    }
}
