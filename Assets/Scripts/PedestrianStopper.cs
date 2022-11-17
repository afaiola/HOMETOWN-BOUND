using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianStopper : MonoBehaviour
{
    private List<Pedestrian> peds;
    // Start is called before the first frame update
    void Start()
    {
        peds = new List<Pedestrian>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnEnable()
    {
        peds = new List<Pedestrian>();
    }

    private void OnDisable()
    {
        foreach (var ped in peds)
        {
            ped.Go();
        }
        peds = new List<Pedestrian>();
    }

    //TODO: consider moving this to the car class.
    private void OnTriggerEnter(Collider other)
    {
        Pedestrian ped = other.GetComponentInParent<Pedestrian>();
        if (ped != null)
        {
            peds.Add(ped);
            ped.Stop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Pedestrian ped = other.GetComponentInParent<Pedestrian>();
        if (ped != null)
        {
            ped.Go();
            peds.Remove(ped);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Prevents cars from leaving prematurely
        foreach (var ped in peds)
        {
            ped.Stop();
        }
    }

}
