using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Text = TMPro.TextMeshProUGUI;

public enum InputStatus { VALID, INVALID, ENABLED, DISABLED }   // GREEN, RED, BLUE, GREY for indexing the statusColors array

public class UserInputPanel : MonoBehaviour
{
    public enum MenuType
    {
        SignIn,
        Register
    }

    public Button submitButton;
    public Button registerButton;
    [SerializeField]
    private Toggle saveLoginInfoToggle = null;
    [SerializeField]
    private UserInputOption[] userInputOptions;
    public Color[] statusColors;
    [SerializeField]
    private MenuType menuType = MenuType.SignIn;

    private TabSelector tabSelector;
    private string buttonText;

    [System.NonSerialized]
    public UnityEvent<Dictionary<string, string>> submitAction;


    protected void Start()
    {
        tabSelector = GetComponentInChildren<TabSelector>();
        tabSelector.selectEvent = new UnityEvent<int>();
        tabSelector.selectEvent.AddListener(OptionSelected);

        int id = 0;
        foreach (var option in userInputOptions)
        {
            option.inputChanged = new UnityEvent();
            option.inputChanged.AddListener(CheckAllOptions);
            option.selected = new UnityEvent<int>();
            option.selected.AddListener(OptionSelected);
            option.id = id;
            id++;
        }

        if (menuType == MenuType.SignIn && saveLoginInfoToggle && saveLoginInfoToggle.isOn)
        {
            var signInField = userInputOptions[0].GetComponentInChildren<TMPro.TMP_InputField>();
            signInField.SetTextWithoutNotify(PlayerPrefs.GetString("USERNAME", ""));
            var passwordField = userInputOptions[1].GetComponentInChildren<TMPro.TMP_InputField>();
            passwordField.SetTextWithoutNotify(PlayerPrefs.GetString("PASSWORD", ""));
        }

        submitButton.onClick.AddListener(OnSubmit);
        buttonText = submitButton.GetComponentInChildren<Text>().text;
        if (saveLoginInfoToggle)
        {
            saveLoginInfoToggle.onValueChanged.AddListener(FullyLogout);
        }

        CheckAllOptions();
    }

    private void OptionSelected(int idx)
    {
        foreach (var option in userInputOptions)
        {
            option.Highlight(false);
        }
        if (userInputOptions.Length > idx)
            userInputOptions[idx].Highlight(true);
    }

    private void CheckAllOptions()
    {
        // check if all inputs are valid. if any aren't, disable the submit button.
        submitButton.interactable = true;
        submitButton.image.color = statusColors[(int)InputStatus.VALID];

        foreach (var option in userInputOptions)
        {
            if (!option.GetIfInputValid())
            {
                submitButton.interactable = false;
                submitButton.image.color = statusColors[(int)InputStatus.DISABLED];
            }
        }
    }

    protected void OnSubmit()
    {
        if (menuType == MenuType.SignIn && saveLoginInfoToggle && saveLoginInfoToggle.isOn)
        {
            PlayerPrefs.SetString("USERNAME", userInputOptions[0].GetValue());
            PlayerPrefs.SetString("PASSWORD", userInputOptions[1].GetValue());
        }
        else if (menuType == MenuType.Register)
        {
            PlayerPrefs.SetString("USERNAME", userInputOptions[0].GetValue());
            PlayerPrefs.SetString("PASSWORD", userInputOptions[3].GetValue());
        }
        bool valid = true;
        foreach (UserInputOption u in userInputOptions)
        {
            if (u.GetValue() == "")
            {
                valid = false;
                SubmitFail("Please enter your " + u.errorfieldname);
                break;
            }
        }

        if (valid)
        {
            Dictionary<string, string> userOptions = new Dictionary<string, string>();
            for (int i = 0; i < userInputOptions.Length; i++)
            {
                userOptions.Add(userInputOptions[i].optionName, userInputOptions[i].GetValue());
            }
            submitButton.interactable = false;
            submitAction.Invoke(userOptions);
        }
    }

    public void SubmitFail(string message)
    {
        submitButton.image.color = statusColors[(int)InputStatus.INVALID];
        submitButton.GetComponentInChildren<Text>().text = message;
        Debug.Log("Fail with message: " + message);
        if (registerButton != null)
        {
            registerButton.gameObject.SetActive(true);
        }
        Invoke("PanelReset", 2f);
    }

    public void FullyLogout(bool toggleState)
    {
        if (!toggleState)
        {
            PlayerPrefs.DeleteKey("USERNAME");
            PlayerPrefs.DeleteKey("PASSWORD");
        }
        else
        {
            PlayerPrefs.SetString("USERNAME", userInputOptions[0].GetValue());
            PlayerPrefs.SetString("PASSWORD", userInputOptions[1].GetValue());
        }
    }

    private void PanelReset()
    {
        CheckAllOptions();
        foreach (var option in userInputOptions)
        {
            option.Highlight(!option.GetIfInputValid());
        }

        submitButton.GetComponentInChildren<Text>().text = buttonText;
        submitButton.interactable = true;
    }
}
