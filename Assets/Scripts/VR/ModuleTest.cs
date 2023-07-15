using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class ModuleTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //fix
        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
        XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
