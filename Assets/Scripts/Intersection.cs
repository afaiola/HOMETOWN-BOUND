using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public LightChanger[] verticalLights;
    public LightChanger[] horizontalLights;
    public GameObject[] verticalCarStoppers;
    public GameObject[] horizontalCarStoppers;
    public float cycleTime = 20f;
    public float timeSinceLastCycle;
    public bool verticalOn;

    // Start is called before the first frame update
    void Start()
    {
        SwitchLights();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeSinceLastCycle > cycleTime)
        {
            SwitchLights();
        }
    }

    public void SwitchLights()
    {
        verticalOn = !verticalOn;
        foreach (var light in verticalLights)
        {
            light.ChangeLight(verticalOn);
        }
        
        foreach (var light in horizontalLights)
        {
            light.ChangeLight(!verticalOn);
        }

        foreach (var stopper in verticalCarStoppers)
        {
            stopper.SetActive(!verticalOn);
        }

        foreach (var stopper in horizontalCarStoppers)
        {
            stopper.SetActive(verticalOn);
        }

        timeSinceLastCycle = Time.time;
    }
}
