using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointEntitySpawner : MonoBehaviour
{
    public GameObject entityPrefab;
    public int maxEntities;
    public Transform waypointRoot;
    public bool spawnOnStart;

    public List<GameObject> entities;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnOnStart) SpawnEntities();
    }

    public void SpawnEntities()
    {
        if (entities != null)
        {
            if (entities.Count != 0) return;
        }
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        entities = new List<GameObject>();
        int spawned = 0;
        List<int> locationsUsed = new List<int>();
        for (int i = 0; i < maxEntities; i++)
        {
            int idx = Random.Range(0, waypointRoot.childCount);
            while (locationsUsed.Contains(idx))
            {
                idx = Random.Range(0, maxEntities);
            }
            locationsUsed.Add(idx);
            if (locationsUsed.Count >= maxEntities) locationsUsed = new List<int>();

            //idx = idx % waypointRoot.childCount;

            Transform child = waypointRoot.GetChild(idx);

            GameObject obj = Instantiate(entityPrefab, transform);
            obj.name += " " + spawned.ToString();
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.position = child.transform.position;
            obj.transform.forward = child.transform.forward;
            entities.Add(obj);
            spawned++;
            yield return new WaitForEndOfFrame();
        }
    }

    public void DestroyEntities()
    {
        foreach (var entity in entities)
        {
            Destroy(entity);
        }
        entities = new List<GameObject>();
        waypointRoot.gameObject.SetActive(false);
    }
}
