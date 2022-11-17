using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLevelIndicator : MonoBehaviour
{
    public Transform hintLocation;
    public GameObject hintPrefab;
    public string hint;

    private GameObject spawnedHint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHint()
    {
        if (hintLocation != null && hintPrefab != null)
        {
            spawnedHint = Instantiate(hintPrefab, hintLocation);
        }
        Menu.Instance.UpdateModuleName(hint);
    }

    // when player has entered new area
    public void HideHint()
    {
        if (spawnedHint)
            Destroy(spawnedHint);
    }

}
