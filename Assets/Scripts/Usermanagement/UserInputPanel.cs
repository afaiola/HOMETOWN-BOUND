using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Text = TMPro.TextMeshProUGUI;

public enum InputStatus { VALID, INVALID, ENABLED, DISABLED }   // GREEN, RED, BLUE, GREY for indexing the statusColors array

public class UserInputPanel : MonoBehaviour
{
    public Button submitButton;
    public Button registerButton;
    [System.NonSerialized] public UnityEvent<Dictionary<string, string>> submitAction;
    [SerializeField] private UserInputOption[] userInputOptions;
    public Color[] statusColors;

    private TabSelector tabSelector;
    private string buttonText;


    private void OnValidate()
    {
        userInputOptions = GetComponentsInChildren<UserInputOption>();
    }

    void Start()
    {
        userInputOptions = GetComponentsInChildren<UserInputOption>();

        tabSelector = GetComponentInChildren<TabSelector>();
        tabSelector.selectEvent = new UnityEvent<int>();
        tabSelector.selectEvent.AddListener(OptionSelected);

        foreach (var option in userInputOptions)
        {
            option.inputChanged = new UnityEvent();
            option.inputChanged.AddListener(CheckAllOptions);
        }

        submitButton.onClick.AddListener(OnSubmit);
        buttonText = submitButton.GetComponentInChildren<Text>().text;

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
        PlayerPrefs.SetString("USERNAME", userInputOptions[0].GetValue());
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
        Invoke("Reset", 2f);
    }

    private void Reset()
    {
        Debug.Log(name + " reset");

        CheckAllOptions();
        foreach (var option in userInputOptions)
        {
            option.Highlight(!option.GetIfInputValid());
        }

        submitButton.GetComponentInChildren<Text>().text = buttonText;
        submitButton.interactable = true;
    }
}
