using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TabularMenu : MonoBehaviour
{
    [System.Serializable]
    public struct TabInfo
    {
        public string name;
        public Button button;
        public GameObject panel;
        public UnityEvent openEvent;

        public void Activate()
        {
            panel.transform.SetAsLastSibling();
            if (openEvent != null)
                openEvent.Invoke();
        }
    }

    public TabInfo[] tabs;

    private void OnValidate()
    {
        foreach (var tab in tabs)
        {
            tab.button.GetComponentInChildren<Text>().text = tab.name;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var tab in tabs)
        {
            tab.button.onClick.AddListener(tab.Activate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
