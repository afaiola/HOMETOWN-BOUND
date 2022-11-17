using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class StarComponent : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] Color color;
    [SerializeField] List<Image> images;

    private void OnValidate()
    {
        source = GetComponent<AudioSource>();
    }
    public void Set(int stars)
    {
        for (int i = 0; i < stars; i++)
        {
            images[i].color = color;
        }
        source.Play();
    }
}
