using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarSensor : MonoBehaviour
{
    //[System.NonSerialized] 
    public List<GameObject> sensedObjs;
    //[System.NonSerialized] 
    public GameObject player;
    //[System.NonSerialized] 
    public bool useRaycast = true;
    public float sensorLength = 10f;
    public float sensorViewAngle = 60f;

    // Start is called before the first frame update
    void Start()
    {
        sensedObjs = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.GetComponent<CarSensor>()) return;
        GameObject obstacle = other.GetComponentInParent<Car>()?.gameObject;
        if (obstacle == null) obstacle = other.GetComponentInParent<Pedestrian>()?.gameObject;

        if (obstacle != null)
        {
            bool dup = false;
            foreach (var c in sensedObjs)
            {
                if (c == obstacle)
                {
                    dup = true;
                    break;
                }
            }
            if (!dup)
            {
                sensedObjs.Add(obstacle.gameObject);
            }
        }
        else if (other.tag == "Player")
        {
            player = other.gameObject;
        }

        CarTurner turner = other.GetComponent<CarTurner>();
        CarStopper stopper = other.GetComponent<CarStopper>();

        if (turner || stopper)
        {
            GetComponentInParent<Car>().SlowDown();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CarSensor>()) return;

        GameObject obstacle = other.GetComponentInParent<Car>()?.gameObject;
        if (obstacle == null) obstacle = other.GetComponentInParent<Pedestrian>()?.gameObject;

        if (obstacle != null)
        {
            sensedObjs.Remove(obstacle.gameObject);
        }
        else if (other.tag == "Player")
        {
            player = null;
        }
    }
}
