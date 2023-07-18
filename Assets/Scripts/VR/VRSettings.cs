using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSettings : MonoBehaviour
{
    public static VRSettings Instance { get { return _instance; } }
    private static VRSettings _instance;

    public bool UseTeleportMovement { get { return useTeleportMovement; } }
    public bool UseIncrementalRotate { get { return useIncrementalRotate; } }
    public int PrimaryHand { get { return !isLeftHanded ? 0 : 1; } }

    private bool useTeleportMovement, useIncrementalRotate;
    private bool isLeftHanded;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: set initial values based on save data
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMovementType(bool isTeleport)
    {
        useTeleportMovement = isTeleport;
    }

    public void SetRotateSetting(bool isIncremental)
    {
        useTeleportMovement = isIncremental;
    }

    public void SetHandedness(bool isLeft)
    {
        isLeftHanded = isLeft;
    }
}
