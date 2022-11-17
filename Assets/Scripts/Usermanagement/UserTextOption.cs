using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
using InputField = TMPro.TMP_InputField;
using System.Text.RegularExpressions;

public class UserTextOption : UserInputOption
{
    public string[] patterns;  // regular expression constraint on the input field.
    private InputField input;

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        input = GetComponentInChildren<InputField>();
        input.onValueChanged.AddListener(TextChanged);
    }

    public override string GetValue()
    {
        return input.text;
    }

    public override bool GetIfInputValid()
    {
        if (patterns.Length == 0) return true;
        bool[] matches = new bool[patterns.Length];
        for (int i = 0; i < patterns.Length; i++)
        {
            if (patterns[i].Contains("compareTo:"))
            {
                string comparename = patterns[i].Substring(10);
                Transform comparer = transform.parent.Find(comparename);
                if (comparer)
                {
                    UserTextOption compareOption = comparer.GetComponent<UserTextOption>();
                    matches[i] = compareOption.GetValue() == GetValue();
                }
            }
            else
            {
                Regex rx = new Regex(patterns[i], RegexOptions.Compiled | RegexOptions.IgnoreCase);
                matches[i] = rx.IsMatch(GetValue());
            }
        }

        bool valid = true;
        for (int i = 0; i < matches.Length; i++)
        {
            if (helpTexts != null)
                helpTexts[i].color = matches[i] ? Color.green : Color.red;
            if (!matches[i])
            {
                valid = false;
            }
        }

        return valid;
    }

    private void TextChanged(string str)
    {
        InputChanged();
    }
}
