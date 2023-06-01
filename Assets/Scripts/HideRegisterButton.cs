using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRegisterButton : MonoBehaviour
{
    public TextPopulator username;
    // Start is called before the first frame update
    void Awake()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("USERNAME"))){
            if (username)
                username.PopulateUsername();
            this.gameObject.SetActive(false);
        }
    }
}
