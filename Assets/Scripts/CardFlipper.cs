using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    public RawImage face;
    public GameObject back;
    public bool flipped;
    public float minTime = 8f;
    public float maxTime = 15f;
    public float timeActive = 3f;

    private float flipTime = 10f;    // time between flips
    private float timeSinceFlip;

    // Start is called before the first frame update
    void Start()
    {
        back.gameObject.SetActive(false);
        Flip();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeSinceFlip > flipTime)
        {
            Flip();
        }
    }

    public void Flip()
    {
        timeSinceFlip = Time.time;
        flipTime = Random.Range(minTime, maxTime);
        if (flipped) flipTime += timeActive;        // might seem odd because the flipped bool is set after the animation
        StartCoroutine(FlipAnimation());
    }

    private IEnumerator FlipAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        float start = transform.localEulerAngles.y;
        float goal = (start == 0 ? 180f : 0);

        float rotateTime = 1f;
        float elapsed = 0f;

        while (elapsed < rotateTime)
        {
            float val = Mathf.Lerp(start, goal, elapsed / rotateTime);
            transform.localEulerAngles = new Vector3(0, val, 0);
            if (elapsed > (rotateTime/2))
            {
                back.SetActive(!flipped);       // might seem odd because the flipped bool is set after the animation
            }
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }

        transform.localEulerAngles = new Vector3(0, goal, 0);
        flipped = !flipped;
        if (flipped) GetComponent<ModuleButton>().Reset();
    }

    public void ColorBack(Color color)
    {
        back.GetComponent<Image>().color = color;
    }
}
