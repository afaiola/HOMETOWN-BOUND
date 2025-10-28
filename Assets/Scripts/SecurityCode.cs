using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;
using InputField = TMPro.TMP_InputField;

public class SecurityEvent : UnityEvent<bool> { }

public class SecurityCode : MonoBehaviour
{
    public static SecurityCode Instance { get { return _instance; } }
    private static SecurityCode _instance;

    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Text title;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closebutton;
    [SerializeField] private Button clearButton;
    [SerializeField] private string code;

    [SerializeField] private IntegerButton[] keypadNumbers;

    public UnityEvent onSuccess;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialize()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Destroy security code");
            Destroy(gameObject);
            return;
            
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);

        if (inputField == null)
            inputField = GetComponentInChildren<InputField>();
        submitButton.onClick.AddListener(CheckCode);
        closebutton.onClick.AddListener(Stop);
        clearButton.onClick.AddListener(ClearCode);
        Stop();

        for (int i = 0; i < keypadNumbers.Length; i++)
        {
            keypadNumbers[i].Setup(i);
            keypadNumbers[i].sendIntEvent.AddListener(EnterNumber);
        }

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            inputField.shouldHideMobileInput = false;
            inputField.shouldHideSoftKeyboard = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckCode();
        }
    }

    private void EnterNumber(int value)
    {
        inputField.text += value.ToString();
    }

    private void ClearCode()
    {
        inputField.text = "";
    }

    private void CheckCode()
    {
        if (inputField.text == code)
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
        //Text submitText = submitButton.GetComponentInChildren<Text>();
        Color textColor = title.color;

        submitButton.interactable = false;
        submitButton.enabled = false;
        buttonImage.enabled = false;
        title.text = "Code Valid";
        //buttonImage.color = Color.green;
        title.color = Color.green;

        yield return new WaitForSecondsRealtime(1f);
        StaticEvent.moduleEnded();
        if (onSuccess != null)
            onSuccess.Invoke();

        submitButton.interactable = true;
        submitButton.enabled = true;
        buttonImage.enabled = true;
        title.text = "Security Code";
        //buttonImage.color = buttonColor;
        title.color = textColor;
        inputField.text = "";
        Stop();
    }

    private IEnumerator InvalidResponse()
    {
        Image buttonImage = submitButton.GetComponent<Image>();
        Color buttonColor = buttonImage.color;
        //Text submitText = submitButton.GetComponentInChildren<Text>();
        Color textColor = title.color;

        submitButton.interactable = false;
        submitButton.enabled = false;
        buttonImage.enabled = false;
        title.text = "Invalid Code";
        //buttonImage.color = Color.red;
        title.color = Color.red;

        yield return new WaitForSecondsRealtime(2f);

        submitButton.interactable = true;
        submitButton.enabled = true;
        buttonImage.enabled = true;
        title.text = "Security Code";
        //buttonImage.color = buttonColor;
        title.color = textColor;
        inputField.text = "";
    }

    private void Stop()
    {
        canvas.SetActive(false);
    }

    public void Show()
    {
        canvas.SetActive(true);
        inputField.SetTextWithoutNotify("");
    }
}
