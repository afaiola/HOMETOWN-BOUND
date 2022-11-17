using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : WaypointCharacterController
{
    public bool crossing;
    private float footstepTime;
    private float footstepDelay = 0.45f;

    private AudioSource audio;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (canMove)
        {
            footstepTime += Time.deltaTime;
            if (footstepTime > footstepDelay)
            { 
                audio.Play();
                footstepTime = 0;
            }
        }
    }

    public override void Go()
    {
        base.Go();
    }

    public override void Stop()
    {
        if (crossing) return;
        base.Stop();
    }

}
