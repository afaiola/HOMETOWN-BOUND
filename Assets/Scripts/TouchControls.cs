using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
    private int moveVal, lookVal;

    // Start is called before the first frame update
    void Start()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld && !Debug.isDebugBuild)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TankController.Instance)
        {
            TankController.Instance.MoveCharacterForwardBack(moveVal);
            TankController.Instance.RotateCharacterLeftRight(lookVal);
        }
    }

    public void MoveForward()
    {
        moveVal = 1;
    }

    public void MoveBackward()
    {
        moveVal = -1;
    }

    public void StopMoving()
    {
        moveVal = 0;
    }

    public void LookLeft()
    {
        lookVal = -1;
    }

    public void LookRight()
    {
        lookVal = 1;
    }

    public void LookStraight()
    {
        lookVal = 0;
    }

}
