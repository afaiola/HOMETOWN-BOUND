using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitExercise : DragExercise
{
    public GameObject dragPrefab, snapPrefab;
    private string[] familyMembers;
    // flying panel is drag spawn
    // snap panel is snap spawn 
    protected override void OnValidate()
    {

    }

    public void Initialize(string[] names)
    {
        familyMembers = names;
        leftImages = new RawImage[names.Length];
        flyingImages = new RawImage[names.Length];
        snapButtons = new Snap[names.Length];

        Rect rect = flyingPanel.GetComponent<RectTransform>().rect;
        float separationDist = rect.width / names.Length;
        List<string> namesRemaining = new List<string>();
        for (int i = 0; i < names.Length; i++) namesRemaining.Add(names[i]);
        // spawn the flying images and snaps
        for (int i = 0; i < names.Length; i++)
        {
            RawImage flyer = Instantiate(dragPrefab, flyingPanel.transform).GetComponent<RawImage>();
            string randName = namesRemaining[Random.Range(0, namesRemaining.Count)];
            namesRemaining.Remove(randName);
            flyer.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = randName;
            flyer.GetComponent<RectTransform>().anchoredPosition = new Vector2((i-names.Length/2) * separationDist, rect.height / 2f);

            Snap snap = Instantiate(snapPrefab, snapPanel.transform).GetComponent<Snap>();
            snap.GetComponent<RectTransform>().anchoredPosition = new Vector2((i-names.Length / 2) * separationDist, -rect.height / 2f);
            snap.tmpText.text = names[i]; // be sure the text is not visible
            snap.tmpText.gameObject.SetActive(false);
            snap.rightTexture = flyer.texture as Texture2D;

            flyingImages[i] = flyer;
            snapButtons[i] = snap;
        }
    }

    public override void Arrange()
    {
        if (familyMembers.Length == 0)
        {
            Success();
        }
        // destroy the horiz fitter
        for (int i = 0; i < flyingImages.Length; i++)
        {
            flyingImages[i].GetComponent<FlyAround>().speed = Random.Range(minSpeed, maxSpeed);
            flyingImages[i].GetComponent<DragImage>().dropEvent = new UnityEngine.Events.UnityEvent();
            flyingImages[i].GetComponent<DragImage>().dropEvent.AddListener(ImageDropped);
            snapButtons[i].GetComponent<RawImage>().texture = flyingImages[i].GetComponent<RawImage>().texture;
            //flyingImages[i].GetComponent<RectTransform>().sizeDelta = snapButtons[i].GetComponent<RectTransform>().sizeDelta;
        }
        flyingPanel.SetActive(true);
        snapPanel.SetActive(true);
    }

}
