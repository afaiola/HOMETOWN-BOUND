using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public static LightController Instance { get { return _instance; } }
    private static LightController _instance;

    private Light dirLight;
    // Start is called before the first frame update
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        dirLight = GetComponentInChildren<Light>();
        //dirLight.enabled = false;
    }
}
