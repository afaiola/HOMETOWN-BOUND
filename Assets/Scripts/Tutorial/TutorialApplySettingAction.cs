using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialApplySettingAction : TutorialAction
{
    [SerializeField] private bool useTeleport, useIncrementalRotate;

    public override void Run()
    {
        base.Run();
        VRSettings.Instance.SetMovementType(useTeleport);
        VRSettings.Instance.SetRotateSetting(useIncrementalRotate);
    }
}
