using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToMenu : MonoBehaviour
{
    [SerializeField] private GoTo[] firstGotos, secondGotos, thirdGotos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideOptions(int option)
    {
        switch (option)
        {
            case 1:
                foreach (var go in firstGotos) go.gameObject.SetActive(false);
                break;
            case 2:
                foreach (var go in secondGotos) go.gameObject.SetActive(false);
                break;
            case 3:
                foreach (var go in thirdGotos) go.gameObject.SetActive(false);
                break;
        }
    }
}
