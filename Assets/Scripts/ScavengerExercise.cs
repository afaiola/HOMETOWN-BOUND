using System.Collections.Generic;
using UnityEngine;

public class ScavengerExercise : Exercise
{
    public GameObject goalPrefab, pedestrianPrefab;
    public List<Transform> goalLocations;
    public bool isPedestrian;

    protected GameObject goalObj;

    protected override void OnValidate()
    {
        // Empty to override base class
    }

    public override bool CheckSuccess
    {
        get => true;
    }

    private GameObject SpawnGoal()
    {
        GameObject obj;
        if (isPedestrian)
        {
            obj = Instantiate(pedestrianPrefab, goalLocations[Random.Range(0, goalLocations.Count)]);
            Pedestrian ped = goalObj.GetComponent<Pedestrian>();
            ped.models[0] = goalPrefab;

            // find it a path to walk on
            WaypointEntitySpawner spawner = GameObject.FindObjectOfType<WaypointEntitySpawner>();
            Transform child = spawner.waypointRoot.GetChild(Random.Range(0, spawner.waypointRoot.childCount));

            ped.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
        }
        else
        {
            obj = Instantiate(goalPrefab, goalLocations[Random.Range(0, goalLocations.Count)]);
        }
        return obj;
    }

    public void AddInteractable(GameObject obj)
    {
        obj.AddComponent<cakeslice.Outline>();
        Interact goalInteract = obj.AddComponent<Interact>();
        goalInteract.interactEvent = new UnityEngine.Events.UnityEvent();
        goalInteract.interactEvent.AddListener(Select);
    }

    public override void Arrange()
    {
        goalObj = SpawnGoal();

        AddInteractable(goalObj);
    }

    public virtual void Select()
    {
        Destroy(goalObj);
        if (CheckSuccess)
        {
            StartCoroutine(Wait(2, Success));
        }
    }

    public override bool Select(ModuleButton mb)
    {
        return false;
    }

    public override void Cleanup()
    {
        Destroy(goalObj);
    }
}
