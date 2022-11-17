using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float moveTime = 2f;
    public AudioClip doorOpenClip, doorCloseClip;

    private AudioSource source;
    private BoxCollider boxCol;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        boxCol = GetComponentInChildren<BoxCollider>();
        source.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        source.clip = doorOpenClip;
        source.Play();
        StartCoroutine(MoveDoor(1));
    }

    public void Close()
    {
        boxCol.enabled = false ;
        source.clip = doorCloseClip;
        StartCoroutine(MoveDoor(-1));
    }

    private IEnumerator MoveDoor(float direction)
    {
        float timecount = 0;
        Quaternion doorStart = transform.localRotation;
        Quaternion doorGoal = Quaternion.Euler(transform.localEulerAngles - new Vector3(0, direction*90f, 0));

        while (timecount < moveTime)
        {
            transform.localRotation = Quaternion.Lerp(doorStart, doorGoal, timecount / moveTime);
            timecount += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (direction < 0)
        {
            source.Play();
            boxCol.enabled = true;
        }
    }
}
