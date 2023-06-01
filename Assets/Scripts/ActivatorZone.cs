using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ActivatorZone : MonoBehaviour
{
    [SerializeField] public UnityEvent enterEvent;
    [SerializeField] public UnityEvent exitEvent;
    [SerializeField] public bool oneTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enterEvent != null)
        {
            if (other.tag == "Player")
            {
                enterEvent.Invoke();
                if (oneTime)
                    gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (exitEvent != null)
        {
            if (other.tag == "Player")
            {
                exitEvent.Invoke();
                if (oneTime)
                    gameObject.SetActive(false);
            }
        }
    }
}
