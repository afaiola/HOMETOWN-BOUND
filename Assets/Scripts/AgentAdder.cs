using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAdder : MonoBehaviour
{
    public bool inBound;
    private NavMeshAgent agent;
    private Intersection[] intersections;
    private int dest;


    // Start is called before the first frame update
    void Start()
    {
        intersections = GameObject.FindObjectsOfType<Intersection>();

        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500f, NavMesh.AllAreas))
            gameObject.transform.position = closestHit.position;
        else
            Debug.LogError("Could not find position on NavMesh!");


        agent = gameObject.AddComponent<NavMeshAgent>();
        //agent.agentTypeID = 1;
        agent.areaMask = inBound ? 8 : 16;

        SetDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(agent.destination, transform.position) < 20f)
        {
            SetDestination();
        }
    }

    public void SetDestination()
    {
        int rand = 0;
        while (rand == dest)
        {
            rand = Random.Range(0, intersections.Length);
        }

        dest = rand;
        agent.SetDestination(intersections[dest].transform.position);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public void Go()
    {
        agent.isStopped = false;
    }
}
