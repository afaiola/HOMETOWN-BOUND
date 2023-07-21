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
    private bool usingPrimaryHand;

    // Start is called before the first frame update
    void Start()
    {
        CalculateCanvasRange();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateCanvasRange()
    {
        StartCoroutine(DelayedCalculateRange());
    }

    private IEnumerator DelayedCalculateRange()
    {
        // wait a frame to give canvas time to recalculate position
        yield return new WaitForEndOfFrame();

        RectTransform rectTransform = GetComponent<RectTransform>();
        Rect rect = rectTransform.rect;

        float worldOffsetX = Mathf.Cos(Mathf.Deg2Rad * transform.root.eulerAngles.y) * (rect.width / 2) * transform.root.localScale.x;
        float worldOffsetY = rect.height / 2 * transform.root.localScale.y;
        float worldOffsetZ = -Mathf.Sin(Mathf.Deg2Rad * transform.root.eulerAngles.y) * (rect.width / 2) * transform.root.localScale.z;

        /*float worldMinX = transform.root.position.x - worldOffsetX;
        float worldMinY = transform.root.position.y - worldOffsetY;
        float worldMinZ = transform.root.position.z - worldOffsetZ;

        float worldMaxX = transform.root.position.x + worldOffsetX;
        float worldMaxY = transform.root.position.y + worldOffsetY;
        float worldMaxZ = transform.root.position.z + worldOffsetZ;
        worldMin = new Vector3(worldMinX, worldMinY, worldMinZ);
        worldMax = new Vector3(worldMaxX, worldMaxY, worldMaxZ);*/

        //Vector3 worldOffset = new Vector3(worldOffsetX, worldOffsetY, worldOffsetZ);
        Vector3 worldOffset = new Vector3(
            Mathf.Abs(worldOffsetX), 
            Mathf.Abs(worldOffsetY),
            Mathf.Abs(worldOffsetZ)
            );
        worldMin = transform.root.position - worldOffset;
        worldMax = transform.root.position + worldOffset;

        Debug.Log($"angle {transform.root.eulerAngles.y} gives range {worldMin}-{worldMax} offset: {worldOffset}");
    }

    public bool GetCanvasWorldPosition(ref Vector3 resultPos, bool requireActive=false)
    {
        // TODO: get whichever pointer is active. 
        // if both active, pick whichever was there first
        
        resultPos = VRManager.Instance.GetHitPosition(true, requireActive);
        if (Single.IsInfinity(resultPos.x))
        {
            resultPos = VRManager.Instance.GetHitPosition(false, requireActive);
            if (Single.IsInfinity(resultPos.x))
            {
                return false;
            }
            // active hand failed, but secondary succeeded. switch
            //usingPrimaryHand = !usingPrimaryHand;
        }

        ClampWorldPosToCanvas(ref resultPos);
        return true;
    }

    public void ClampWorldPosToCanvas(ref Vector3 pos)
    {
        Debug.Log($"pre clamp: {pos}");
        pos = new Vector3(
            Mathf.Clamp(pos.x, worldMin.x, worldMax.x),
            Mathf.Clamp(pos.y, worldMin.y, worldMax.y),
            Mathf.Clamp(pos.z, worldMin.z, worldMax.z)
        );
        Debug.Log($"post clamp: {pos}");

    }
}
