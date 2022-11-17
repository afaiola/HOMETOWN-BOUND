using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGraph : Graph
{
    protected override void SpawnDataPoint(int i)
    {
        base.SpawnDataPoint(i);

        // connect previous point to this one if it isn't the first
        LineGraphElement curr = horizDataPoints[i].GetComponent<LineGraphElement>();
        if (i <= 0)
        {
            if (curr)
                curr.backwardsLine.gameObject.SetActive(false);
            return;
        }

        LineGraphElement prev = horizDataPoints[i - 1].GetComponent<LineGraphElement>();
        curr.ConnectPoints(prev);

        if (i == graphData.Count-1) 
            curr.forwardsLine.gameObject.SetActive(false);
    }
}
