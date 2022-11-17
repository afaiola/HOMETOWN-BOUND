using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DataFilterOptions : MonoBehaviour
{
    public Dropdown lodDropdown, numberDropdown, attemptDropdown, ciDropdown;

    [System.NonSerialized] public UnityEvent optionsUpdatedEvent;

    // Start is called before the first frame update
    void Start()
    {
        lodDropdown.onValueChanged.AddListener(DropdownChanged);
        numberDropdown.onValueChanged.AddListener(DropdownChanged);
        attemptDropdown.onValueChanged.AddListener(DropdownChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDropdownOptions(int numberRange, int newValue)
    {
        StartCoroutine(RefreshNumberDropdown(numberRange, newValue));
    }

    private IEnumerator RefreshNumberDropdown(int numberRange, int newValue)
    {
        numberDropdown.onValueChanged = new Dropdown.DropdownEvent();

        yield return new WaitForEndOfFrame();
        //numberDropdown.value = 0;
        if (numberRange != numberDropdown.options.Count)
        {
            numberDropdown.ClearOptions();

            List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();

            for (int i = 1; i <= numberRange; i++)
            {
                string optionName = i.ToString();
                if (numberRange > 15)   // we are displaying exercises
                {
                    int exNo = i - 1;
                    int modNo = exNo / 7 + 1;
                    exNo = exNo % 7;
                    optionName = "M" + modNo + "E" + exNo;
                }
                /*else if (numberRange > 3)
                {
                    int levelNum = Mathf.CeilToInt((float)i / 5f);
                    int modNo = i & 5;
                    if (modNo == 0) modNo = 5;
                    optionName = "L" + levelNum.ToString() + "M" + modNo.ToString();
                }*/
                dropdownOptions.Add(new Dropdown.OptionData(optionName));
            }

            numberDropdown.AddOptions(dropdownOptions);

            yield return new WaitForEndOfFrame();
        }

        numberDropdown.value = newValue;

        yield return new WaitForEndOfFrame();
        numberDropdown.RefreshShownValue();
        numberDropdown.onValueChanged.AddListener(DropdownChanged);
    }

    public void UpdateCIDropdown()
    {
        StartCoroutine(RefreshCIRoutine());
    }

    private IEnumerator RefreshCIRoutine()
    {
        ciDropdown.onValueChanged = new Dropdown.DropdownEvent();
        int level = ciDropdown.value;

        yield return new WaitForEndOfFrame();
        List<SavePatientData.PatientDataEntry> ciData = SavePatientData.Instance.GetCiData();
        int ciLevels = ciData[0].attempts.Length;
        if (ciDropdown.options.Count != ciLevels)
        { 
            ciDropdown.ClearOptions();

            List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();

            for (int i = 1; i <= ciLevels; i++)
                dropdownOptions.Add(new Dropdown.OptionData(i.ToString()));

            ciDropdown.AddOptions(dropdownOptions);
 
            yield return new WaitForEndOfFrame();
            if (Profiler.Instance)
            {
                level = Profiler.Instance.currentUser.ciLevel - 1;
                if (level > ciDropdown.options.Count)
                {
                    level = 0;
                }
            }
        }

        ciDropdown.value = level;

        yield return new WaitForEndOfFrame();
        ciDropdown.RefreshShownValue();
        ciDropdown.onValueChanged.AddListener(DropdownChanged);

    }


    public void DropdownChanged(int idx)
    {
        optionsUpdatedEvent.Invoke();
    }
}
