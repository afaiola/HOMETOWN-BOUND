using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class Menu : MonoBehaviour
{
    public static Menu Instance { get { return _instance; } }
    private static Menu _instance;

    public Text moduleName;
    public GameObject gotoMenu;

    public Toggle gotoButton;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateModuleName(string str)
    {
        moduleName.text = str;
    }
}
