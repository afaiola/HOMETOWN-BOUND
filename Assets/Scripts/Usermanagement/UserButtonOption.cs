using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserButtonOption : UserInputOption
{
    public ButtonOption[] buttons;

    private int activeOption;

    protected override void OnValidate()
    {
        base.OnValidate();
        buttons = GetComponentsInChildren<ButtonOption>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].ID = i;
            buttons[i].optionEvent = new UnityEngine.Events.UnityEvent<int>();
            buttons[i].optionEvent.AddListener(OptionSelected);
        }
    }

    public override string GetValue()
    {
        return activeOption.ToString();
    }

    private void OptionSelected(int val)
    {
        activeOption = val;
        // TODO: something to show which is currently active
        InputChanged();
    }
}
