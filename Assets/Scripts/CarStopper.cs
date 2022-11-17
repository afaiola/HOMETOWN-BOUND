using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStopper : MonoBehaviour
{
    private List<Car> cars;
    private Collider trigger;
    // Start is called before the first frame update
    void Start()
    {
        cars = new List<Car>();
        trigger = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        cars = new List<Car>();
    }

    private void OnDisable()
    {
        foreach (var car in cars)
        {
            car.Go();
        }
        cars = new List<Car>();
    }

    //TODO: consider moving this to the car class.
    private void OnTriggerEnter(Collider other)
    {

        Car car = other.GetComponentInParent<Car>();
        if (car != null)
        {
            if (other.GetComponent<CarSensor>())
            {
                car.SlowDown();
            }
            else
            {
                car.stopped = true;
                cars.Add(car);
                car.Brake();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CarSensor>()) return;

        Car car = other.GetComponentInParent<Car>();
        if (car != null)
        {
            if (!trigger.enabled)
            {
                car.Go();
                cars.Remove(car);
            }
            else
            {
                car.Stop();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Prevents cars from leaving prematurely
        foreach (var car in cars)
        {
            car.Brake();
            if (!car.stopped)
            {
                //car.Stop();
            }
        }
    }
}
