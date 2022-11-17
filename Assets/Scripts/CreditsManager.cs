using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class CreditsManager : MonoBehaviour
{
    public GameObject attributionElementPrefab;
    public Transform creditsPlacement;
    public TextAsset attributions;

    // Start is called before the first frame update
    void Start()
    {
        string[] credits = attributions.text.Split('\n');
        for (int i = 0; i < credits.Length; i++)
        {
            GameObject credit = Instantiate(attributionElementPrefab, creditsPlacement);
            Text creditText = credit.GetComponentInChildren<Text>();
            creditText.text = credits[i];
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
