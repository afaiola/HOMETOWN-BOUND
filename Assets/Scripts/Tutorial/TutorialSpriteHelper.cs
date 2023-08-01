using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSpriteHelper : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float swapTime = 1f;

    //[SerializeField] private Transform attachAnchor;
    [SerializeField] private Vector3 offset;

    private int currSprite;
    private bool isPlaying;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateSprite()
    {
        while (isPlaying)
        {
            image.sprite = sprites[currSprite];

            yield return new WaitForSeconds(swapTime);

            currSprite++;
            if (currSprite >= sprites.Length)
                currSprite = 0;
        }
    }

    public void Stop()
    {
        isPlaying = false;
        StopCoroutine(AnimateSprite());
    }

    private void OnEnable()
    {
        //transform.parent = attachAnchor;
        transform.localPosition = offset;
        transform.localRotation = new Quaternion();
        if (VRSettings.Instance)
        {
            float flip = 1 * VRSettings.Instance.PrimaryHand > 0 ? -1f : 1f;
            image.transform.localScale = new Vector3(flip, 1, 1);
        }

        currSprite = 0;
        isPlaying = true;
        StartCoroutine(AnimateSprite());
    }

    private void OnDisable()
    {
        Stop();
    }
}
