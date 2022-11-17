using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrafficManager : MonoBehaviour
{
    public NavMeshBuildSettings settings;
    private List<NavMeshBuildSource> sources;
    private NavMeshData data;

    public GameObject crosswalks;
    public GameObject roads;

    private float timeSinceLastBake;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastBake = Time.time;
        sources = new List<NavMeshBuildSource>();
        //data = NavMeshData

        MeshRenderer[] walkMesh = crosswalks.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in walkMesh)
        {
            NavMeshBuildSource src = new NavMeshBuildSource();
            src.transform = mesh.transform.localToWorldMatrix;
            src.shape = NavMeshBuildSourceShape.Box;
            src.size = mesh.transform.localScale;
            sources.Add(src);
        }

        MeshRenderer[] roadMesh = crosswalks.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in roadMesh)
        {
            NavMeshBuildSource src = new NavMeshBuildSource();
            src.transform = mesh.transform.localToWorldMatrix;
            src.shape = NavMeshBuildSourceShape.Box;
            src.size = mesh.transform.localScale;
            sources.Add(src);
        }

        //crosswalks.SetActive(false);
        roads.SetActive(false);
        //StartCoroutine(BakeRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeSinceLastBake > 1f)
        {
            //data = NavMeshBuilder.BuildNavMeshData(settings, sources, new Bounds(transform.position, new Vector3(200, 200, 200)), Vector3.zero, Quaternion.identity);
            //NavMeshBuilder.UpdateNavMeshData(data, settings, sources, new Bounds(transform.position, new Vector3(200, 200, 200)));
            timeSinceLastBake = Time.time;
            //Debug.Log("BAKE");
        }

    }

    private IEnumerator BakeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            crosswalks.SetActive(true);
            roads.SetActive(true);
            yield return new WaitForEndOfFrame();
            crosswalks.SetActive(false);
            roads.SetActive(false);
        }
    }
}
