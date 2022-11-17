using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCharacterController : MonoBehaviour
{
    public float speedRangeLow = 1.75f;
    public float speedRangeHigh = 2.5f;
    public float moveSpeed = 2f;
    public float stopDistance = 2.5f;
    public float turnFactor = 1f;
    public string moveAnim;
    public GameObject[] models;
    public bool adaptiveTurning;

    protected Animator animator;
    protected Vector3 destination;
    public bool destinationReached;

    protected float turnSpeed = 120f;
    protected float distToDest;
    protected bool canMove;
    protected bool firstDest;
    protected GameObject model;
    protected Vector3 modelPos;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Randomize the look of the model
        model = Instantiate(models[Random.Range(0, models.Length)], transform);
        modelPos = model.transform.localPosition;
        animator = GetComponentInChildren<Animator>();
        Rigidbody modelRb = model.GetComponent<Rigidbody>();
        if (modelRb)
        {
            Destroy(modelRb);
        }
        moveSpeed = Random.Range(speedRangeLow, speedRangeHigh);
        firstDest = true;
        Go();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        model.transform.localPosition = modelPos;
        if (!canMove) return;
        Vector3 dir = destination - transform.position;
        //dir.y = 0;
        
        //Debug.DrawRay(transform.position, dir.normalized, Color.cyan);
        distToDest = Vector3.Distance(transform.position, destination);
        if (distToDest > stopDistance)
        {
            destinationReached = false;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            if (moveSpeed > 0)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * turnFactor * Time.deltaTime);
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            destinationReached = true;
            firstDest = false;
        } 
    }

    public virtual void SetDestination(Vector3 dest)
    {
        float dist = Vector3.Distance(transform.position, dest);
        if (!firstDest && adaptiveTurning)
            turnSpeed = 1900f / dist;
        if (turnSpeed < 20f) turnSpeed = 20f;
        destination = dest;
    }

    public virtual void Go()
    {
        canMove = true;
        if (animator && moveAnim != "")
        {
            animator.SetBool(moveAnim, true);
        }
    }

    public virtual void Stop()
    {
        canMove = false;
        if (animator)
        {
            animator.SetBool(moveAnim, false);
        }
    }
}
