using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragExercise : Exercise
{
    [SerializeField] protected GameObject leftPanel;
    [SerializeField] protected GameObject flyingPanel;
    [SerializeField] protected GameObject snapPanel;

    [SerializeField] protected RawImage[] leftImages;
    [SerializeField] protected RawImage[] flyingImages;
    [SerializeField] protected Snap[] snapButtons;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public bool overriderefs = false;

    public override bool CheckSuccess
    {
        get
        {
            return selected.Count == snapButtons.Length;
        }
    }
    protected virtual void OnValidate()
    {
        if (overriderefs) return;
        if (leftPanel)
            leftImages = leftPanel.GetComponentsInChildren<RawImage>();
        if (flyingPanel)
            flyingImages = flyingPanel.GetComponentsInChildren<RawImage>();
        snapButtons = GetComponentsInChildren<Snap>();
        snapPanel = snapButtons[0].transform.parent.gameObject;
        for (int i = 0; i < snapButtons.Length; i++)
        {
            //flyingImages[i].GetComponent<RectTransform>().sizeDelta = snapButtons[i].GetComponent<RectTransform>().sizeDelta;
            //flyingImages[i].transform.localPosition = snapButtons[Random.Range(0, snapButtons.Length)].transform.localPosition;
        }
    }

    public override void Arrange()
    {
        for (int i = 0; i < images.Count; i++)
        {
            flyingImages[i].texture = leftImages[i].texture = images[i];
            flyingImages[i].GetComponent<FlyAround>().speed = Random.Range(minSpeed, maxSpeed);
            snapButtons[i].rightTexture = images[i];
            flyingImages[i].GetComponent<DragImage>().dropEvent = new UnityEngine.Events.UnityEvent();
            flyingImages[i].GetComponent<DragImage>().dropEvent.AddListener(ImageDropped);
            //flyingImages[i].GetComponent<RectTransform>().sizeDelta = snapButtons[i].GetComponent<RectTransform>().sizeDelta;
            flyingImages[i].GetComponent<RectTransform>().anchoredPosition = snapButtons[Random.Range(0, snapButtons.Length)].GetComponent<RectTransform>().anchoredPosition;
        }
        flyingPanel.SetActive(true);
        snapPanel.SetActive(true);
        //_incorrectCount = -images.Count;    // a miss is a drop and technically all images are dropped at start
        //Destroy(flyingImages[0].transform.parent.GetComponent<GridLayoutGroup>());
    }

    protected void ImageDropped()
    {
        _incorrectCount++;
    }

}
