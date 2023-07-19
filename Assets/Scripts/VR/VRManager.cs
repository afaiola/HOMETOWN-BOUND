using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VRManager : MonoBehaviour
{
    public static VRManager Instance { get { return _instance; } }
    private static VRManager _instance;

    [Header("Movement Sources")]
    [SerializeField] private TeleportationProvider teleportProvider;
    [SerializeField] private ActionBasedSnapTurnProvider snapTurnProvider;
    [SerializeField] private ActionBasedContinuousMoveProvider continuousMoverProvider;
    [SerializeField] private ActionBasedContinuousTurnProvider continuousTurnProvider;

    private float smoothMoveSpeed, smoothRotateSpeed, snapTurnAngle;

    [SerializeField] private ActionBasedController[] xrControllers;

    [Header("Interactors")]
    [SerializeField] private XRRayInteractor[] rayInteractors;
    [SerializeField] private XRRayInteractor[] rayTeleporters;
    // direct interactors are for grabbing objects directly with the controllers
    //[SerializeField] private XRDirectInteractor[] directInteractors;

    // TODO: use poke interactor for UI
    
    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
        XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        VRSettings vrSettings = GameObject.FindObjectOfType<VRSettings>();
        if (vrSettings)
        {
            vrSettings.onMovementTypeChange = new UnityEvent();
            vrSettings.onMovementTypeChange.AddListener(ApplySettings);
            vrSettings.onRotateTypeChange = new UnityEvent();
            vrSettings.onRotateTypeChange.AddListener(ApplySettings);
            vrSettings.onHandednessChange = new UnityEvent();
            vrSettings.onHandednessChange.AddListener(ApplySettings);

            vrSettings.LoadSettings();
        }

        smoothMoveSpeed = continuousMoverProvider.moveSpeed;
        smoothRotateSpeed = continuousTurnProvider.turnSpeed;
        snapTurnAngle = snapTurnProvider.turnAmount;

        ApplySettings();

        Invoke("SetupHands", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupHands()
    {
        TankController.Instance.SetHandModel();
    }

    public void ApplySettings()
    {
        /*teleportProvider.enabled = VRSettings.Instance.UseTeleportMovement;
        snapTurnProvider.enabled = VRSettings.Instance.UseIncrementalRotate;

        continuousMoverProvider.enabled = !VRSettings.Instance.UseTeleportMovement;
        continuousTurnProvider.enabled = !VRSettings.Instance.UseIncrementalRotate;*/

        //int primaryHand = VRSettings.Instance.PrimaryHand;
        //int secondaryHand = 1 - primaryHand;

        rayTeleporters[1].gameObject.SetActive(true && VRSettings.Instance.UseTeleportMovement);
        rayTeleporters[0].gameObject.SetActive(false);

        snapTurnProvider.turnAmount = VRSettings.Instance.UseIncrementalRotate ? snapTurnAngle : 0;

        continuousMoverProvider.moveSpeed = VRSettings.Instance.UseTeleportMovement ? 0 : smoothMoveSpeed;
        continuousTurnProvider.turnSpeed = VRSettings.Instance.UseIncrementalRotate ? 0 : smoothRotateSpeed;
    }


    public RaycastResult GetPrimaryRaycastResult()
    {
        RaycastResult hit;

        rayInteractors[VRSettings.Instance.PrimaryHand].TryGetCurrentUIRaycastResult(out hit);
        return hit;
    }

    public Vector3 GetHitPosition(bool primary = true, bool requireActive = false)
    {
        RaycastResult hit;
        if (VRSettings.Instance == null) return Vector3.positiveInfinity;

        int primaryHand = VRSettings.Instance.PrimaryHand;
        int secondaryHand = 1 - primaryHand;

        if (xrControllers[primary ? primaryHand : secondaryHand].activateActionValue.action.ReadValue<float>() < 0.1f && requireActive)
            return Vector3.positiveInfinity;

        if (!rayInteractors[primary ? primaryHand : secondaryHand].TryGetCurrentUIRaycastResult(out hit))
            return Vector3.positiveInfinity;
        
        return hit.worldPosition;
    }
}
