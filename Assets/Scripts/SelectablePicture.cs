using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectablePicture : MonoBehaviour
{
    public RawImage image;
    private float maxWidth = 100;
    private float maxHeight = 150;
    public ModuleButton moduleButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePicture(Texture2D tex)
    {
        // if sprite w > h turn 90 deg
        // fit image width if portrait
        // fit image height if landscape
        float ratio = (float)tex.width / (float)tex.height;

        if (ratio > 1)
        {
            transform.localEulerAngles += new Vector3(0, 0, 90);
            image.transform.localEulerAngles += new Vector3(0, 0, 90);
            image.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            image.rectTransform.sizeDelta = new Vector2(maxHeight, maxWidth);
        }
        image.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
        image.texture = tex;

    }
}
