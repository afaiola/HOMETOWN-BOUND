using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CyclicFlash : MonoBehaviour
{
    public Image[] images;
    public int numOn = 1;
    public float step = 0.5f;
    public Color onColor;
    public Color offColor;
    
    private int space;
    private int leader; // last active number on

    private void OnValidate()
    {
        images = GetComponentsInChildren<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        space = images.Length / numOn;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = offColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Cycle()
    {
        while (enabled)
        {
            //for (int i = 0; i < images.Length; i++)
            //{
            //    images[i].color = offColor;
            //}

            for (int i = 0; i < numOn; i++)
            {
                int idx = leader - i * space;
                SetImage(idx, true);
                SetImage(idx - 1, false);
            }
            leader++;
            if (leader >= images.Length) leader -= space;   // 10 images, last is idx 9. num on = 1 space = 10. leader = 10, now = 0. we are good.
            yield return new WaitForSeconds(step);
        }
    }

    private void SetImage(int idx, bool on)
    {
        if (idx >= images.Length) idx = 0;
        if (idx < 0) idx = images.Length-1;

        images[idx].color = on ? onColor : offColor;
    }

    private void OnEnable()
    {
        leader = images.Length;
        StartCoroutine(Cycle());
    }
}
