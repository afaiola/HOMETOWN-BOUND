using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToPlay : MonoBehaviour
{
    public void Play()
    {
        GetComponent<AudioSource>().Play();
    }
}
