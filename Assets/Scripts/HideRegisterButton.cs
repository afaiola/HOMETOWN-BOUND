using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRegisterButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("USERNAME"))){
            GameObject.Find("UsernameField").GetComponent<TextPopulator>().PopulateUsername();
            this.gameObject.SetActive(false);
        }
    }
}
