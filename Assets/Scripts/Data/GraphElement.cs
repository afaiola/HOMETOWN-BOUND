using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphElement : MonoBehaviour
{
    public Text pointInfo;
    public RectTransform fill;

    protected DataPoint point;

    // Start is called before the first frame update
    void Start()
    {
        pointInfo.transform.parent.gameObject.SetActive(false);
    }

    public void Initialize(DataPoint p, string units)
    {
        point = p;
        int exNo = (int)point.x - 1;
        int modNo = exNo / 7 + 1;
        exNo = exNo % 7;
        //if (exNo == 0) exNo = 7;
        string exString = "M" + modNo + "E" + exNo;
        pointInfo.text = "(" + exString + ", " + point.y.ToString("0.0") + " " + units +")";
    }

    public void ShowInfo()
    {
        pointInfo.transform.parent.gameObject.SetActive(true);
    }

    public void HideInfo()
    {
        pointInfo.transform.parent.gameObject.SetActive(false);
    }
}
