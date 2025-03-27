using System.Collections;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    public float min, max;
    public float frequency;

    private new Light light;
    private float timeSinceFlash;
    private bool flashing;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        light.intensity = min;
        timeSinceFlash = Time.time;
    }

    private IEnumerator Flash()
    {
        flashing = true;
        float val = 0;
        while (flashing)
        {
            if (val < frequency)
            {
                light.intensity = Mathf.Lerp(min, max, val / frequency);
            }
            else if (val < 2f * frequency)
            {
                light.intensity = Mathf.Lerp(max, min, (val - frequency) / frequency);
            }
            else
            {
                val = 0;
            }
            yield return new WaitForEndOfFrame();
            val += Time.deltaTime;
        }

    }

    public void StartFlash()
    {
        StartCoroutine(Flash());
    }

    public void Stop()
    {
        flashing = false;
    }
}
