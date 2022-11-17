using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car : WaypointCharacterController
{
    public GameObject[] driverModels;
    public float maxSpeed = 25f; 
    public float slowSpeed = 15f;
    public float acceleration;
    public bool forceGO;
    public bool brake, stopped;

    private Transform minLoc;
    private CarSensor carSensor;
    private float timeStopped;
    private float destSpeed;
    private float brakeFactor = 2f;

    [Header("Audio")]
    public AudioSource primaryAudio;
    public AudioSource secondaryAudio;  // used for extra sounds like honking and screeching tires
    public AudioClip idleSFX;
    public AudioClip accelerateSFX;
    public AudioClip driveSFX;
    public AudioClip decelerateSFX;
    public AudioClip honkSFX;
    public AudioClip tireScreech;
    private AudioClip nextClip;
   
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // spawn the driver
        Transform driverSpawnPoint = model.transform.Find("Driver");
        if (driverSpawnPoint)
        {
            GameObject driver = null;
            if (driverSpawnPoint.childCount == 0)
            {
                driver = Instantiate(driverModels[Random.Range(0, driverModels.Length)], driverSpawnPoint);
            }
            else
            {
                driver = driverSpawnPoint.GetChild(0).gameObject;
            }

            Animator driverAnim = driver.GetComponentInChildren<Animator>();
            driverAnim.SetBool("isDriving", true);
            
        }

        forceGO = true;         // pushes cars through the intersection if they spawn in one
        carSensor = GetComponentInChildren<CarSensor>();
        moveSpeed = 0f;
        Go();
    }

    // Update is called once per frame
    protected override void Update()
    {
        ObjectSafe();
        if (stopped) brakeFactor = 2f;
        if (stopped && moveSpeed <= 0f) Stop();

        if (brake)
        {
            timeStopped += Time.deltaTime;
            if (timeStopped > 20f)
            {
                secondaryAudio.clip = honkSFX;
                secondaryAudio.Play();
                timeStopped -= 7f;
            }
        }
        else
        {
            timeStopped = 0f;
        }

        if (forceGO && !stopped)
        {
            bool atLight = false;
            foreach (var sensed in carSensor.sensedObjs)
            {
                Car car = sensed.GetComponent<Car>();

                if (car)
                {
                    if (car.stopped) 
                    {
                        atLight = true;
                        break;
                    }
                }
            }
            // only force go if the car in front is not stopped
            if (!atLight)
                Go();
            else
            {
                stopped = true;
                Brake();
            }
        }

        if ((moveSpeed < destSpeed && !brake) || (forceGO && !stopped))
        {
            moveSpeed += Time.deltaTime * acceleration;
            nextClip = accelerateSFX;
            primaryAudio.loop = false;
            if (forceGO) destSpeed = maxSpeed;
            if (moveSpeed > destSpeed) moveSpeed = destSpeed;
        }
        else if (moveSpeed > destSpeed)
        {
            moveSpeed -= Time.deltaTime * acceleration * 2f * brakeFactor;
            nextClip = decelerateSFX;
            primaryAudio.loop = false;
        }

        if (moveSpeed <= 0f)
        {
            moveSpeed = 0f;
            nextClip = idleSFX;
            primaryAudio.loop = true;
        }
        else if (moveSpeed > 0)
        {
            nextClip = driveSFX;
            primaryAudio.loop = true;
        }

        if (minLoc != null)
        {
            if (Vector3.Distance(minLoc.position, transform.position) < 9f)
            {
                brakeFactor = 3f;
                Brake();
                if (secondaryAudio.gameObject.activeSelf)
                {
                    if (secondaryAudio.clip != tireScreech)
                    {
                        secondaryAudio.Stop();
                    }

                    if (!secondaryAudio.isPlaying)
                    {
                        secondaryAudio.clip = tireScreech;
                        secondaryAudio.Play();
                    }
                }
            }
        }

        if (nextClip != primaryAudio.clip)
        {
            primaryAudio.Stop();
            primaryAudio.clip = nextClip;
            primaryAudio.Play();
        }

        base.Update();

    }

    public override void Stop()
    {
        base.Stop();
        stopped = true;
        //moveSpeed = 0;
    }

    public void Brake()
    {
        brake = true;
        destSpeed = 0f;
        // TODO: Calculate how far away we should stop and adjust the speed accordingly
    }

    public override void Go()
    {
        base.Go();
        brakeFactor = 1f;
        stopped = false;
        brake = false;
        destSpeed = maxSpeed;
    }

    public void SlowDown()
    {
        destSpeed = slowSpeed;
    }

    public void Turn()
    {
        Go();
        destSpeed = slowSpeed;
    }

    private void ObjectSensed()
    {
        //if (brake) return; // we are already stopped, no need to do anything else

        float playerDist = 100f;
        float obstacleDist = 100f;
        int closestObstacle = 0;

        float stopDist = 35f;
        //brakeFactor = 2f;
        if (turnSpeed > 60f)
        {
            brakeFactor = 3f;
            stopDist = 18f;
        }

        if (carSensor.player != null)
        {
            playerDist = Vector3.Distance(transform.position, carSensor.player.transform.position);
        }

        if (carSensor.sensedObjs.Count > 0)
        {
            bool carsStopped = false;
            for (int i = 0; i < carSensor.sensedObjs.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, carSensor.sensedObjs[i].transform.position);
                if (dist < obstacleDist)
                {
                    closestObstacle = i;
                    obstacleDist = dist;

                    Car car = carSensor.sensedObjs[i].GetComponent<Car>();

                    if (car)
                    {
                        if (car.stopped) carsStopped = true;
                    }
                }
            }
            // if all cars sensed are no longer stopped, I can unstop
            if (!carsStopped && stopped)
            {
                stopped = false;
            }
        }

        //Debug.Log(name + " stop dist: " + stopDist + " obstacle dist = " + obstacleDist);
        if (playerDist < stopDist || obstacleDist < stopDist)
        {   
            minLoc = null;
            Car car = null;
            Pedestrian ped = null;
            if (carSensor.sensedObjs.Count > 0)
            {
                minLoc = carSensor.sensedObjs[closestObstacle].transform;
                car = carSensor.sensedObjs[closestObstacle].GetComponent<Car>();
            }

            if (playerDist < obstacleDist)
                minLoc = carSensor.player.transform;
            else if (car != null)
            {
                // for driving behind another car
                if (!car.brake && !brake && car.moveSpeed > 0)
                {
                    //Debug.Log(name + " follow " + car.name);
                    //moveSpeed = car.moveSpeed - 1f;
                    destSpeed = car.moveSpeed;
                    brakeFactor = 3f;
                    return;
                }
            }
            //else if (ped != null)
            //{
            //    if (obstacleDist > 4f) return;
            //}

            if (minLoc != null)
            {
                brakeFactor = 2f;
                Brake();
                //stopped = true;
                //Debug.Log(name + " sensed " + (playerDist < obstacleDist ? "player " + playerDist : "car " + obstacleDist) + " m away");
                if (car != null && !forceGO)
                {
                    // if the car forced to go senses stopped cars, dont force go.
                    car.forceGO = true;
                }
            }
        }
    }

    private void ObjectSafe()
    {
        if (carSensor.sensedObjs.Count == 0 && carSensor.player == null && !stopped)
        {
            forceGO = false;
            Go();
        }
        else
        {
            ObjectSensed();
        }
    }
}
