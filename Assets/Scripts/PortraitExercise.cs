using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitExercise : DragExercise
{
    public GameObject dragPrefab, snapPrefab;
    public string[] familyMembers;
    public bool splitRows = true;
    public bool physicalFrame = false;
    public GameObject pictureFrame;

    private bool initialized;
    // flying panel is drag spawn
    // snap panel is snap spawn 
    protected override void OnValidate()
    {

    }

    private void Start()
    {
    }

    public void Initialize(string[] names)
    {
        if (initialized) return;
        initialized = true;
        if (names != null)
        {
            if (names.Length > 0)
            {
                familyMembers = names;
                splitRows = false;
                physicalFrame = true;
            }
        }

        
        float portraitWidth = leftObject.texture.width;
        float portraitHeight = leftObject.texture.height;
        float portraitRatio = portraitWidth / portraitHeight;
        float screenRatio = (float)Screen.width / (float)Screen.height; 

        AspectRatioFitter portraitFitter = leftObject.gameObject.AddComponent<AspectRatioFitter>();
        portraitFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        portraitFitter.aspectRatio = portraitRatio;

        float ratioRatio = 16f / 9f / screenRatio;
        
        Debug.Log($"screenRatio: {screenRatio} ratioRatio: {ratioRatio} ");
        RectTransform snapRect = snapPanel.GetComponent<RectTransform>();   // 4:3 = 258.806 --- 16:9 = 154.3284
        Vector2 adjustedSize = new Vector2(snapRect.rect.height * portraitRatio * ratioRatio + 25, snapRect.rect.height);

        pictureFrame.SetActive(physicalFrame);
        if (physicalFrame)
        {
            // top and bottom always correct
            float frameWidth = 0.05f + portraitRatio / 2f;
            float frameLength = 0.9f + portraitRatio / 2f;
            pictureFrame.transform.GetChild(0).localPosition = new Vector3(0, -frameWidth, 0);
            pictureFrame.transform.GetChild(2).localPosition = new Vector3(0, frameWidth, 0);
            pictureFrame.transform.GetChild(1).localScale = new Vector3(0.1f, frameLength, 0.1f);
            pictureFrame.transform.GetChild(3).localScale = new Vector3(0.1f, frameLength, 0.1f);
        }
        else
        {
            adjustedSize *= 0.8f;
            //adjustedSize = new Vector2((adjustedSize.x - 25f) * 0.75f, adjustedSize.y * 0.75f);  // cut out the frame 
        }

        snapRect.anchoredPosition = new Vector2(0, 5f); // looks generally good
        if (GameManager.Instance.useVR)
        {
            adjustedSize = new Vector2(128, 60);
            snapRect.anchoredPosition = new Vector2(0, -5f); // looks generally good
        }

        snapRect.sizeDelta = adjustedSize;

        leftImages = new RawImage[familyMembers.Length];
        flyingImages = new RawImage[familyMembers.Length];
        snapButtons = new Snap[familyMembers.Length];

        Rect rect = snapPanel.GetComponent<RectTransform>().rect;
        float separationDist = adjustedSize.x / familyMembers.Length; 
        if (splitRows) separationDist *= 2f;

        List<string> namesRemaining = new List<string>();
        for (int i = 0; i < familyMembers.Length; i++) namesRemaining.Add(familyMembers[i]);

        // spawn the flying images and snaps
        for (int i = 0; i < familyMembers.Length; i++)
        {
            RawImage flyer = Instantiate(dragPrefab, flyingPanel.transform).GetComponent<RawImage>();
            if (GameManager.Instance.useVR)
            {
                flyer.rectTransform.sizeDelta = new Vector2(25, 10);
            }
            //flyer.rectTransform.sizeDelta = new Vector2(flyer.rectTransform.sizeDelta.x * ratioRatio, flyer.rectTransform.sizeDelta.y);
            string randName = namesRemaining[Random.Range(0, namesRemaining.Count)];
            namesRemaining.Remove(randName);

            float x = i - familyMembers.Length / 2;
            float y = rect.height / 2f;
            if (splitRows && x >= 0)
            {
                x -= familyMembers.Length / 2;
                y *= -1f;// + 10;// little +10 offset fits frame better
            }
            x *= separationDist;
            x += 10;    // just makes this one fit better. May need to change if image changes
            if (splitRows) x += adjustedSize.x / 2f;    // increased separation dist causes objects to be too far left

            // if row of names too congested, stagger odd numbered labels
            if (separationDist < flyer.rectTransform.rect.width)
                if (i % 2 == 1)
                    y -= flyer.rectTransform.rect.height;

            flyer.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = randName;
            flyer.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y/2f);    // y/2f to put the flyers more in the center

            Snap snap = Instantiate(snapPrefab, snapPanel.transform).GetComponent<Snap>();
            snap.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            snap.tmpText.text = familyMembers[i]; // be sure the text is not visible
            snap.tmpText.gameObject.SetActive(false);
            snap.rightTexture = flyer.texture as Texture2D;
            snap.GetComponent<RectTransform>().sizeDelta = flyer.rectTransform.sizeDelta;

            flyingImages[i] = flyer;
            snapButtons[i] = snap;
        }
    }

    public override void Arrange()
    {
        if (!initialized) Initialize(null);
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
