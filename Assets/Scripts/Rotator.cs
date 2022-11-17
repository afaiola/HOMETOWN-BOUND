using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rotator : MonoBehaviour
{
    [SerializeField] public float angle = 90f;
    [SerializeField] public Vector3 direction = new Vector3(0, 1, 0);
    [SerializeField] public float time = 1f;
    private bool rotated = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate()
    {
        rotated = true;
        StartCoroutine(Rotating(1f));
    }

    public void RotateBack()
    {
        Debug.Log("rotate back");
        if (rotated)
        {
            rotated = false;
            StartCoroutine(Rotating(-1f));
        }
    }

    private IEnumerator Rotating(float reverse)
    {
        Quaternion rot = transform.localRotation;
        Quaternion goal = Quaternion.Euler(transform.localEulerAngles + direction * angle * reverse);
        float elapsed = 0;

        while (elapsed < time)
        {
            transform.localRotation = Quaternion.Slerp(rot, goal, elapsed / time);
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
    }
}
