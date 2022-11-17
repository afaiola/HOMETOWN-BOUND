using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTurner : MonoBehaviour
{
    public Transform[] directions;
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
        Car car = other.GetComponentInParent<Car>();

        if (car != null)
        {
            // Turn the car in one of the directions
            // Move the car to the position of the direction
            car.Turn();
        }
    }
}
