using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InitializeTeleportArea : MonoBehaviour
{
    public TeleportationArea tpArea;

    public void Activate()
    {
        var managers = GameObject.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
        tpArea.interactionManager = GameObject.FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
        tpArea.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
