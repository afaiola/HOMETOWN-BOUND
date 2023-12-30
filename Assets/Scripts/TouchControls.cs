using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TouchControls : MonoBehaviour
{
    private int moveVal, lookVal;
    private CanvasGroup cg;

    // Start is called before the first frame update
    void Start()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)// && !Debug.isDebugBuild)
        {
            Destroy(gameObject);
        }
        cg = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TankController.Instance && TankController.Instance.canMove)
        {
            if (cg)
            {
                cg.alpha = 1f;
                cg.blocksRaycasts = true;
            }
            TankController.Instance.MoveCharacterForwardBack(moveVal * 0.4f);
            TankController.Instance.RotateCharacterLeftRight(lookVal * 0.35f);
        }
        else if (cg)
        {
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
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
