using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    public WaypointCharacterController controller;
    public Waypoint currentWaypoint;
    public bool randomDirection, bidirectionalBranches;
    private float direction = 1f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<WaypointCharacterController>();
        controller.SetDestination(currentWaypoint.GetPosition());
        if (randomDirection)
            direction = Random.Range(-1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.destinationReached)
        {
            bool shouldBranch = false;

            if (currentWaypoint.branches != null)
            { 
                if (currentWaypoint.branches.Count > 0)
                    shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
                // need to move in direction of branch
                // if next waypoint has a branch, go in other direction
                Waypoint next = direction > 0 ? currentWaypoint.nextWaypoint : currentWaypoint.prevWaypoint;
                if (next)
                {
                    if (next.branches.Count > 0 && bidirectionalBranches)
                    {
                        direction *= -1f;
                    }
                }
            }
            else
            {
                if (direction < 0)
                {
                    if (currentWaypoint.prevWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.prevWaypoint;
                    }
                    else if (randomDirection)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        direction *= -1f;
                    }
                }
                else
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    else if (randomDirection)
                    {
                        currentWaypoint = currentWaypoint.prevWaypoint;
                        direction *= -1f;
                    }
                }
            }
            controller.SetDestination(currentWaypoint.GetPosition());
        }



    }
}
