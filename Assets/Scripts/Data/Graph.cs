using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public bool isHorizInt, isVertInt, cutoffEmptyData;
    public int xTickCount = 5;
    public int yTickCount = 5;
    public string unitsName;
    public float yBuf;
    public bool forceMax;
    public float forcedYMax;

    public Text xMaxText, yMaxText, titleText, horizAxisText, vertAxisText;

    public Transform dataContainer, horizontalAxis, verticalAxis;
    public RectTransform graphRect;

    public GameObject dataPointPrefab;
    public GameObject horizAxisTickPrefab, vertAxisTickPrefab;

    public float YMax { get { return yMax; } }

    protected List<GameObject> horizDataPoints, horizTicks, vertTicks;
    protected float xMin, xMax;
    protected float yMin, yMax;
    protected float xTick, yTick;
    protected int lastIdx;
    protected float graphHeight, graphWidth;

    protected List<DataPoint> graphData;


    // Start is called before the first frame update
    void Start()
    {
        xMaxText.text = "";
        yMaxText.text = "";
    }

    public void Initialize()
    {
        horizDataPoints = new List<GameObject>();
        horizTicks = new List<GameObject>();
        vertTicks = new List<GameObject>();
    }

    public void DrawGraph(List<DataPoint> data, string title, string xTitle, string yTitle, bool defaultAxis = false)
    {
        graphHeight = graphRect.rect.height;
        graphWidth = graphRect.rect.width;

        titleText.text = title;
        horizAxisText.text = xTitle;
        vertAxisText.text = yTitle;

        ResetGraph();
        graphData = data;
        GetGraphRange();

        lastIdx = graphData.Count;

        // Spawn the data
        for (int i = 0; i < graphData.Count; i++)
        {
            SpawnDataPoint(i);
        }
        /*GameObject dummy = new GameObject("buffer", typeof(RectTransform));
        dummy.transform.parent = dataContainer;
        horizDataPoints.Add(dummy);
        dummy.transform.SetAsFirstSibling();
        */
        if (cutoffEmptyData && lastIdx < graphData.Count)
        {
            lastIdx++;
            for (int i = lastIdx; i < graphData.Count; i++)
            {
                Destroy(horizDataPoints[i]);
            }
            horizDataPoints.RemoveRange(lastIdx, horizDataPoints.Count - lastIdx);
            graphData.RemoveRange(lastIdx, graphData.Count - lastIdx);
            GetGraphRange();
        }

        // resize elements
        foreach (var dataPoint in horizDataPoints)
        {
            //RectTransform rXform = dataPoint.GetComponent<RectTransform>();
            GraphElement element = dataPoint.GetComponent<GraphElement>();
            if (element)
            {
                RectTransform fill = element.fill;
                if (fill)
                {
                    fill.sizeDelta = new Vector2(graphWidth / horizDataPoints.Count, 1);
                    fill.anchoredPosition = new Vector2(fill.anchoredPosition.x * fill.rect.width / 6f, fill.anchoredPosition.y);   // fill object's x position must be set to + or - 1
                }
            }
        }

        xTick = (xMax - xMin) / xTickCount;
        yTick = (yMax - yMin) / yTickCount;

        // Spawn the ticks
        if (horizAxisTickPrefab == null || vertAxisTickPrefab == null) return;
        
        // if not tracking tick count to each point, first point is half as far as the tick dist
        if (xTickCount < horizDataPoints.Count)
        {
            xMin += xTick / 2;
        }

        for (int i = 0; i < xTickCount; i++)
        {
            AxisTick hTick = Instantiate(horizAxisTickPrefab, horizontalAxis).GetComponent<AxisTick>();
            string tickStr = (i+1).ToString();
            if (!defaultAxis)
            {
                int exNo = (int)(xMin + xTick * i);
                int modNo = exNo / 7 + 1;
                exNo = exNo % 7;
                //if (exNo <= 0) exNo = 7;
                tickStr = "M" + modNo + "E" + exNo;
            }
            hTick.Initialize(tickStr);
            horizTicks.Add(hTick.gameObject);
        }

        for (int i = 1; i < yTickCount; i++)
        {
            AxisTick vTick = Instantiate(vertAxisTickPrefab, verticalAxis).GetComponent<AxisTick>();
            float val = (yMin + yTick * i);
            if (isVertInt) val = Mathf.RoundToInt(val);
            string valStr = val.ToString("0.0");
            vTick.Initialize(valStr);
            vertTicks.Add(vTick.gameObject);
        }
        GameObject vertSpacer = new GameObject();
        vertSpacer.AddComponent<RectTransform>();
        vertSpacer.transform.parent = verticalAxis;
        vertTicks.Add(vertSpacer);

    }

    protected virtual void SpawnDataPoint(int i)
    {
        GameObject spawnedDataPoint = Instantiate(dataPointPrefab, dataContainer);
        GraphElement element = spawnedDataPoint.GetComponent<GraphElement>();
        element.Initialize(graphData[i], unitsName);

        RectTransform pointRect = spawnedDataPoint.GetComponent<RectTransform>();
        float height = (graphData[i].y / (yMax-yMin)) * graphHeight;
        float width = (graphData.Count / graphWidth / 2f);
        pointRect.sizeDelta = new Vector2(width, height);
        horizDataPoints.Add(spawnedDataPoint);
        if (height > 0) lastIdx = i;
    }

    private void GetGraphRange()
    {
        xMin = 1000f;
        xMax = 0f;
        yMin = 0f;
        yMax = 0f;

        foreach (var point in graphData)
        {
            if (point.x < xMin) xMin = point.x-1;
            if (point.x > xMax) xMax = point.x;
            //if (point.y < yMin) yMin = point.y;
            if (point.y > yMax) yMax = point.y;
        }

        //yMin -= yBuf;
        yMax += yBuf;

        if (yMin < 0) yMin = 0;

        if (forceMax) yMax = forcedYMax;

        //xMaxText.text = xMax.ToString(isHorizInt ? "0" : "0.0");
        //yMaxText.text = yMax.ToString(isVertInt ? "0" : "0.0");

    }

    private void ResetGraph()
    {
        if (horizDataPoints != null)
        {
            if (horizDataPoints.Count > 0)
            {
                foreach (var point in horizDataPoints) Destroy(point);
            }
        }
        horizDataPoints = new List<GameObject>();

        if (horizTicks != null)
        {
            if (horizTicks.Count > 0)
            {
                foreach (var tick in horizTicks) Destroy(tick);
            }
        }
        horizTicks = new List<GameObject>();

        if (vertTicks != null)
        {
            if (vertTicks.Count > 0)
            {
                foreach (var tick in vertTicks) Destroy(tick);
            }
        }
        vertTicks = new List<GameObject>();
    }

}
