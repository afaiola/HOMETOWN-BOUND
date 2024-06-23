using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] Animator animator;
    public Transform lookPosition;
    public float lookWeight = 0;
    [SerializeField] bool lockAt1, lookOnStart;
    private Vector3 lookOffset =  new Vector3(0, 2.8f, 0);

    private void Start()
    {
    }

    public void Initialize()
    {
        if (lookOnStart)
            lookPosition = GameObject.FindGameObjectWithTag("Player").transform;

    }

    private void OnValidate()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layer)
    {
        if (lookPosition == null)
        {
            lookPosition = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (lookPosition)
        {
            animator.SetLookAtPosition(lookPosition.position + lookOffset);
            // this is rotating, but gets really weird looking
            /*
            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            Vector3 forward = (lookPosition.position - head.position).normalized;
            Vector3 up = Vector3.Cross(forward, transform.right);
            Quaternion rotation = Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(forward, up);
            animator.SetBoneLocalRotation(HumanBodyBones.Head, rotation);
            */
        }

        if (lockAt1)
            animator.SetLookAtWeight(1, 0, 1, Mathf.Min(1, 1 * 2), 1);
        else
            animator.SetLookAtWeight(lookWeight, 0, lookWeight, Mathf.Min(1, lookWeight * 2), 1);

    }

    public void Look()
    {
        if (lookPosition == null)
        {
            // find the player
            lookPosition = GameObject.FindGameObjectWithTag("Player").transform;
        }
        lookWeight = 1;
        stopping = false;
    }
    bool stopping;
    float t = 0;
    internal void Stop()
    {
        t = 0;
        stopping = true;
    }
    private void LateUpdate()
    {
        if (stopping)
        {
            if (lookWeight > 0)
            {
                lookWeight = Mathf.Lerp(1, 0, t);
                t += 0.1f * Time.deltaTime;
            }
            else
            {
                stopping = false;
            }
        }
    }
}
