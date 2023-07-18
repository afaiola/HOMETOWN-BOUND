using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof(TrackedDeviceGraphicRaycaster))]
// converts pointer position on world space canvas to a local canvas vector2
public class VRCanvasHelper : MonoBehaviour
{
    private TrackedDeviceGraphicRaycaster trackedRaycaster;
    private Vector3 worldMin, worldMax;


    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Rect rect = rectTransform.rect;
        float worldOffsetX = -Mathf.Cos(Mathf.Deg2Rad * transform.root.eulerAngles.y) * (rect.width / 2) * transform.root.localScale.x;
        float worldOffsetY = rect.height / 2 * transform.root.localScale.y;
        float worldOffsetZ = -Mathf.Sin(Mathf.Deg2Rad * transform.root.eulerAngles.y) * (rect.width / 2) * transform.root.localScale.z;

        float worldMinX = transform.root.position.x - worldOffsetX;
        float worldMinY = transform.root.position.y - worldOffsetY;
        float worldMinZ = transform.root.position.z - worldOffsetZ;

        float worldMaxX = transform.root.position.x + worldOffsetX;
        float worldMaxY = transform.root.position.y + worldOffsetY;
        float worldMaxZ = transform.root.position.z + worldOffsetZ;

        worldMin = new Vector3(worldMinX, worldMinY, worldMinZ);
        worldMax = new Vector3(worldMaxX, worldMaxY, worldMaxZ);

        Debug.Log($"range {worldMin}-{worldMax}");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetCanvasWorldPosition(ref Vector3 resultPos)
    {
        // TODO: get whichever pointer is active. 
        // if both active, pick whichever was there first
        resultPos = VRManager.Instance.GetPrimaryHitPosition();
        if (Single.IsInfinity(resultPos.x))
            return false;
        ClampWorldPosToCanvas(ref resultPos);
        return true;
    }

    public void ClampWorldPosToCanvas(ref Vector3 pos)
    {
        pos = new Vector3(
            Mathf.Clamp(pos.x, worldMin.x, worldMax.x),
            Mathf.Clamp(pos.y, worldMin.y, worldMax.y),
            Mathf.Clamp(pos.z, worldMin.z, worldMax.z)
        );
    }
}
