using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosswalkTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Pedestrian ped = other.GetComponentInParent<Pedestrian>();

        if (ped != null)
        {
            ped.crossing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Pedestrian ped = other.GetComponentInParent<Pedestrian>();

        if (ped != null)
        {
            ped.crossing = false;
        }
    }
}
