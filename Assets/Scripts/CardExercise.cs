using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardExercise : Exercise
{
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float minFlipTime = 3f;
    public float maxFlipTime = 10f;
    public float timeActive = 5f;
    //[System.NonSerialized] 
    public FlyAround.FlyPattern flyPattern;


    private List<Color> cardColors;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    public override bool CheckSuccess
    {
        get
        {
            return selected.Count == 1;
        }
    }

    public override bool Select(ModuleButton mb)
    {
        CardFlipper cf = mb.GetComponent<CardFlipper>();
        if (cf.flipped) return false;

        return base.Select(mb);
    }

    private void SetUpColors()
    {
        string[] colorStrings = "#FF0000FF #FF9700FF #FFFF00FF #00FF00FF #0000FFFF #FF00FFFF #8E00FFFF ".Split(' ');
        cardColors = new List<Color>();
        for (int i = 0; i < colorStrings.Length; i++)
        {
            // convert hex string into color
            Color color = new Color();
            if (ColorUtility.TryParseHtmlString(colorStrings[i], out color))
                cardColors.Add(color);
            else
            {
                Debug.LogWarning("Color: " + colorStrings[i] + " is invalid.");
            }
        }
    }

    public override void Arrange()
    {
        if (leftImage == null) return;
        SetUpColors();

        leftObject.texture = leftImage;
        var btns = GetComponentsInChildren<Button>();
        for (int i = 0; i < btns.Length; i++)
        {
            var mb = btns[i].GetComponent<ModuleButton>();
            mb.Set(this, images[i], images[i] == leftImage);
            mb.image.texture = null;
            var cf = mb.GetComponent<CardFlipper>();
            cf.GetComponent<FlyAround>().speed = Random.Range(minSpeed, maxSpeed);
            cf.GetComponent<FlyAround>().pattern = flyPattern;
            cf.GetComponent<FlyAround>().SetDestination();
            cf.minTime = minFlipTime;
            cf.maxTime = maxFlipTime;
            cf.timeActive = timeActive;
            cf.face.texture = images[i];
            int backColor = Random.Range(0, cardColors.Count);
            cf.ColorBack(cardColors[backColor]);
            cardColors.RemoveAt(backColor);

            //Debug.Log(images[i].width + " x " + images[i].height);
            float ratio = (float)images[i].width / (float)images[i].height;
            if (ratio == 0) ratio = 1;
            cf.face.GetComponent<AspectRatioFitter>().aspectRatio = ratio;

            buttons.Add(mb);
        }

    }
}
