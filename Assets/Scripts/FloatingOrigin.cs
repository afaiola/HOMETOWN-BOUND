using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 100f;
    public bool canUpdate = false;
    [SerializeField] GameObject[] nonSceneObjects;

    private Vector3 totalOffset;
    private List<RootObjectOffsets> rootGos = new List<RootObjectOffsets>();

    public struct RootObjectOffsets
    {
        public GameObject go;
        public Vector3 originalPos;

        public RootObjectOffsets(GameObject _go)
        {
            go = _go;
            originalPos = go.transform.position;
        }
    }

    private void Start()
    {
        rootGos = new List<RootObjectOffsets>();
        foreach (var go in nonSceneObjects)
        {
            RootObjectOffsets offset = new RootObjectOffsets(go);
            rootGos.Add(offset);
        }
    }

    private bool RootContainsGameObject(GameObject go)
    {
        foreach (var offset in rootGos)
        {
            if (offset.go == go)
                return true;
        }    
        return false;
    }

    public void RecenterOrigin()
    {
        if (!canUpdate) return;
        Vector3 pos = transform.position;
        //Debug.Log($"pos {pos} ({pos.magnitude})");
        if (pos.magnitude > threshold)
        {
            TankController.Instance.Recenter(pos);
            totalOffset += pos;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                //Debug.Log("recentring " + SceneManager.GetSceneAt(i).name);
                foreach (GameObject go in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    if (!RootContainsGameObject(go))
                    {
                        RootObjectOffsets offset = new RootObjectOffsets(go);
                        rootGos.Add(offset);
                    }
                    /*if (!rootGos.Contains(go))
                    {
                        go.transform.position -= pos;
                        rootGos.Add(go);
                    }*/

                }
            }

            foreach (var go in rootGos)
            {
                if (go.go != null)
                {
                    go.go.transform.position = go.originalPos - totalOffset;
                }
            }

            CableProceduralStatic[] cables = GameObject.FindObjectsOfType<CableProceduralStatic>();
            foreach (var cable in cables)
                cable.Draw();

            WaypointCharacterController[] waypointers = GameObject.FindObjectsOfType<WaypointCharacterController>();
            foreach (var character in waypointers)
                character.SetOffset(-pos);

            Debug.Log($"Recentering origin to {transform.position} affecting {rootGos.Count} objects");
        }
    }

    private void LateUpdate()
    {
        RecenterOrigin();
    }
}
