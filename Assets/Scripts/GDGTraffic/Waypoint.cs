using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint prevWaypoint, nextWaypoint;

    [Range(0, 25)]
    public float width = 5f;

    public List<Waypoint> branches;

    [Range(0f, 1f)]
    public float branchRatio = 0.5f;

    public Vector3 GetPosition()
    {
        Vector3 min = transform.position + (transform.right * width / 2f);
        Vector3 max = transform.position - (transform.right * width / 2f);

        return Vector3.Lerp(min, max, Random.Range(0f, 1f));
    }
}
