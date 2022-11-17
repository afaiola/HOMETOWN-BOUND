using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bind : MonoBehaviour
{
    [SerializeField]Toggle toggle;

    private void OnValidate()
    {
        toggle = GetComponent<Toggle>();
    }
    private void Update()
    {
        toggle.isOn = UIManager.Instance.paused;
    }
}
