using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineGraphElement : GraphElement
{
    public Transform connectingPoint;
    public Image backwardsLine, forwardsLine;

    public void ConnectPoints(LineGraphElement prev)
    {
        StartCoroutine(ConnectionRoutine(prev));
    }

    private IEnumerator ConnectionRoutine(LineGraphElement prev)
    {
        // wait a frame so the layout group has time to position the elements
        yield return new WaitForEndOfFrame();

        // angle the line towards the prev point
        Vector2 dir = prev.connectingPoint.position - connectingPoint.position;
        //Debug.DrawRay(connectingPoint.position, connectingPoint.up, Color.red, 3f);
        //Debug.DrawRay(connectingPoint.position, dir, Color.blue, 3f);
        float angle = Vector2.Angle(connectingPoint.up, dir);
        //float angle = Vector3.Angle(connectingPoint.up, dir);
        backwardsLine.transform.localEulerAngles = new Vector3(0, 0, angle);

        float dist = Vector3.Distance(connectingPoint.position, prev.connectingPoint.position);
        //if (Screen.fullScreen) 
        dist /= 2f;
        backwardsLine.rectTransform.sizeDelta = new Vector2(backwardsLine.rectTransform.rect.width, dist);

        prev.forwardsLine.transform.localEulerAngles = new Vector3(0, 0, angle-180f);
        prev.forwardsLine.rectTransform.sizeDelta = new Vector2(prev.forwardsLine.rectTransform.rect.width, dist);
    }
}
