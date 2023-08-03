using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InputField = TMPro.TMP_InputField;

public class ShowKeyboard : MonoBehaviour
{
    private InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<InputField>();
        inputField.onSelect.AddListener(Selected);
    }

    private void Selected(string currString)
    {
        if (VirtualKeyboard.Instance)
            VirtualKeyboard.Instance.Open(inputField);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
