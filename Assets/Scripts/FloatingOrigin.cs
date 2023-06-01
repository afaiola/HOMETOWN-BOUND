using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 100f;
    public bool canUpdate = false;

    private void LateUpdate()
    {
        if (!canUpdate) return;
        Vector3 pos = transform.position;
        List<GameObject> rootGos = new List<GameObject>();
        if (pos.magnitude > threshold)
        {
            /*for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Debug.Log("recentring " + SceneManager.GetSceneAt(i).name);
                foreach (GameObject go in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    rootGos.Add(go);
                    go.transform.position -= pos;
                }
            }*/

            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < allTransforms.Length; i++)
            {
                Transform root = allTransforms[i].root;
                if (root.hideFlags == HideFlags.None && !rootGos.Contains(root.gameObject))
                {
                    root.transform.position -= pos;
                    rootGos.Add(root.gameObject);
                }
            }

            Debug.Log("Recentering origin");
        }
    }
}
