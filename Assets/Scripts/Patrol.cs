using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] Transform p2;
    [SerializeField] float speed;
    Animator animator;
    UnityEngine.CharacterController controller;
    Vector3 p1;
    Vector3 current;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", true);
        controller = GetComponent<UnityEngine.CharacterController>();
        p1 = transform.localPosition;
        current = p1;

    }

    int counter = 0;
    Vector3 oldPosition;
    void FixedUpdate()
    {
        if(transform.localPosition == oldPosition)
        {
            counter++;
            if (counter == 20)
            {
                counter = 0;
                current = current == p1 ? p2.localPosition : p1;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("isWalking")){
            if (Vector3.Distance(transform.localPosition, current) < 2)
            {
                current = current == p1 ? p2.localPosition : p1;
            }
            //transform.LookAt(current);
            //controller.Move(movDir * Time.deltaTime - Vector3.up * 0.1f);

            var movDir = (current - transform.localPosition).normalized * speed * 0.5f;
            oldPosition = transform.localPosition;
            transform.localPosition += movDir * Time.deltaTime;
            
            Vector3 lookDir = movDir - new Vector3(0, movDir.y, 0);
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, 100f * speed * Time.deltaTime);
        }
    }
}