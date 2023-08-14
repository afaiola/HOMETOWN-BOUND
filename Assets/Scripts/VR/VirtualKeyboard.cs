using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using InputField = TMPro.TMP_InputField;

public class VirtualKeyboard : MonoBehaviour
{
    public static VirtualKeyboard Instance {get {return _instance;}}
    public static VirtualKeyboard _instance;

    [SerializeField] private GameObject rowPrefab, keyPrefab;
    [SerializeField] private Toggle shift;
    [SerializeField] private VirtualKey spaceKey;
    [SerializeField] private Button enter, clear, backspace;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip keyPressedClip, enterClip, backspaceClip, shiftClip;

    private char[] topRowLower = { '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=' };
    private char[] topRowUpper = { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+' };
    private char[] firstRowLower = { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[', ']', '\\' };
    private char[] firstRowUpper = { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '{', '}', '|' };
    private char[] secondRowLower = { 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\''};
    private char[] secondRowUpper = { 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ':', '\"'};
    private char[] thirdRowLower = { 'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/'};
    private char[] thirdRowUpper = { 'z', 'x', 'c', 'v', 'b', 'n', 'm', '<', '>', '?'};

    private VirtualKey[] keys;
    private InputField activeInput;

    private void OnValidate()
    {
        /*if (keys != null)
        {
            foreach (var key in keys)
            {
                Destroy(key);
            }
        }

        keys = new List<VirtualKey>();

        PopulateRow(thirdRowLower, thirdRowUpper);
        PopulateRow(secondRowLower, secondRowUpper);
        PopulateRow(firstRowLower, firstRowUpper);
        PopulateRow(topRowLower, topRowUpper);

        ChangeCase(false);*/
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        _instance = this;

        shift.onValueChanged.AddListener(ChangeCase);
        enter.onClick.AddListener(Close);
        clear.onClick.AddListener(Clear);
        backspace.onClick.AddListener(Backspace);

        keys = GetComponentsInChildren<VirtualKey>();

        foreach (var vKey in keys)
        {
            vKey.letterTypedEvent = new UnityEvent<char>();
            vKey.letterTypedEvent.AddListener(KeyPressed);
        }

        Close();
    }

    private void PopulateRow(char[] lowers, char[] uppers)
    {
        GameObject row = Instantiate(rowPrefab, transform);

        for (int i = 0; i < lowers.Length; i++)
        {
            VirtualKey vKey = Instantiate(keyPrefab, row.transform).GetComponent<VirtualKey>();
            if (uppers[i] >= 'a' && uppers[i] <= 'z')
                uppers[i] -= ' ';

            vKey.Initialize(lowers[i], uppers[i]);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
    }

    public void Open(InputField openedInput)
    {
        activeInput = openedInput;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        activeInput = null;
        gameObject.SetActive(false);

        PlaySound(enterClip);
    }

    private void Clear()
    {
        if (activeInput)
            activeInput.text = "";

        PlaySound(backspaceClip);
    }

    private void Backspace()
    {
        if (activeInput)
            if (activeInput.text.Length > 0)
                activeInput.text = activeInput.text.Substring(0, activeInput.text.Length - 1);

        PlaySound(backspaceClip);
    }

    private void KeyPressed(char key)
    {
        if (activeInput == null) return;
        activeInput.text += key.ToString();
        UserTextOption textOption = GetComponentInParent<UserTextOption>();
        if (textOption)
            textOption.TextChanged(activeInput.text);

        PlaySound(keyPressedClip);
    }

    public void ChangeCase(bool isUpper)
    {
        shift.GetComponent<Image>().color = isUpper ? Color.grey : Color.white;

        foreach (var key in keys)
        {
            if (!isUpper)
                key.ToLower();
            else
                key.ToUpper();
        }
        PlaySound(shiftClip);
    }
}
