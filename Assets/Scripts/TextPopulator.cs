using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPopulator : MonoBehaviour
{
    string uName;
    public void PopulateUsername(){
        uName = PlayerPrefs.GetString("USERNAME");
        GetComponent<TMPro.TMP_InputField>().text = uName;
      
    }
}
