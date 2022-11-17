using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridFitChildren : MonoBehaviour
{
    void Awake()
    {
        var canvas = GetComponentInParent<Canvas>();
        var height = canvas.GetComponent<RectTransform>().sizeDelta.y - 100 - 50 - 50 - 20;
        Debug.Log(height);
        var grid = GetComponent<GridLayoutGroup>();
        var actualWidth = height - 2 * grid.spacing.x;
        var childWidth = actualWidth / 3;
        grid.cellSize = new Vector2(childWidth, childWidth);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
