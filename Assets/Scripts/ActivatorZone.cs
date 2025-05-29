using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ActivatorZone : MonoBehaviour
{
    [SerializeField]
    private UnityEvent enterEvent;
    [SerializeField]
    private UnityEvent exitEvent;
    [SerializeField]
    private bool oneTime;


    public UnityEvent EnterEvent { get => enterEvent; set => enterEvent = value; }
    public UnityEvent ExitEvent { get => exitEvent; set => exitEvent = value; }
    public bool OneTime { get => oneTime; set => oneTime = value; }


    private void OnTriggerEnter(Collider other)
    {
        if (enterEvent != null)
        {
            if (other.tag == "Player")
            {
                enterEvent.Invoke();
                if (oneTime) { gameObject.SetActive(false); }
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
                if (oneTime) { gameObject.SetActive(false); }
            }
        }
    }
}
