using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the excercises in modules 4 and 5 in CandyLand
/// </summary>
public class HugeExercise : Exercise
{
    [SerializeField] int size;
    enum shapeType { shapes, arrows, letters }
    [SerializeField] shapeType type;
    [SerializeField] GameObject shape;
    [SerializeField] GridLayoutGroup grid;
    [SerializeField] GameObject leftPanel;
    [SerializeField] RawImage[] leftImages;
    [SerializeField] Text[] leftTexts;
    private int maxOfOneType = 3;
    private int numObjectives = 0;


    public override bool CheckSuccess
    {
        get
        {
            return numObjectives <= _correctCount;
        }
    }
    private void OnValidate()
    {
        grid = GetComponentInChildren<GridLayoutGroup>();
        if (type == shapeType.letters)
            leftTexts = leftPanel.GetComponentsInChildren<Text>();
        else
            leftImages = leftPanel.GetComponentsInChildren<RawImage>();
        if (type == shapeType.arrows)
            images = Resources.LoadAll("Arrows", typeof(Texture2D)).Select(z => z as Texture2D).ToList();
        else if(type == shapeType.shapes)
            images = Resources.LoadAll("Shapes", typeof(Texture2D)).Select(z => z as Texture2D).ToList();
    }
    public override void Arrange()
    {
        var rndList = new List<int>();
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();   
        while (rndList.Count<leftImages.Length)
        {
            var rnd = Random.Range(0, images.Count);
            if (!rndList.Contains(rnd))
                rndList.Add(rnd);
        }
        for (int i = 0; i < leftImages.Length; i++)
        {
            if (type == shapeType.letters)
            {
                leftTexts[i].text = letters[rndList[i]].ToString();
            }
            else
                leftImages[i].texture = images[rndList[i]];

        }
        string leftString = "";
        for (int i = 0; i < leftTexts.Length; i++)
        {
            leftString += leftTexts[i].text;
        }
        if (leftString.Contains("FAG"))
            leftTexts[1].text = "B";
        int[] objCounts = new int[leftImages.Length];
        // Add random buttons
        for (int i = 0; i < size; i++)
        {
            var button = GameObject.Instantiate(shape, grid.transform).GetComponent<ModuleButton>();
            var rnd = Random.Range(0, images.Count);
            while (rndList.Contains(rnd))
                rnd = Random.Range(0, images.Count);
            /*if (rndList.Contains(rnd))
            {
                for (int r = 0; r < rndList.Count; r++)
                {
                    if (rndList[r] == rnd)
                    {
                        if (objCounts[r] >= maxOfOneType)
                        {
                            // change this to a new image
                            while (rndList.Contains(rnd))
                                rnd = Random.Range(0, images.Count);
                        }
                        else
                            objCounts[r]++;
                        break;
                    }
                }
            }*/
            if (type == shapeType.letters)
                button.Set(this, letters[rnd].ToString(), rndList.Contains(rnd));
            else
                button.Set(this, images[rnd], rndList.Contains(rnd));
            buttons.Add(button);
        }

        // Change some buttons to match the left side
        for (int i = 0; i < leftImages.Length; i++)
        {
            int numThisImage = Random.Range(1, maxOfOneType + 1);
            for (int j = 0; j < numThisImage; j++)
            {
                if (type == shapeType.letters)
                {
                    var rnd = Random.Range(0, size);
                    while (buttons[rnd].correct)
                        rnd = Random.Range(0, size);
                    buttons[rnd].Set(this, leftTexts[i].text.ToString(), true);
                    objCounts[i]++;
                }
                else
                {
                    var rnd = Random.Range(0, size);
                    while (buttons[rnd].correct)
                        rnd = Random.Range(0, size);
                    buttons[rnd].Set(this, leftImages[i].texture as Texture2D, true);
                    objCounts[i]++;
                }
            }
        }

        numObjectives = objCounts.Sum();
    }
}
