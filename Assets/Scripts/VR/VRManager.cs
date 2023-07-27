using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VRManager : MonoBehaviour
{
    public static VRManager Instance { get { return _instance; } }
    private static VRManager _instance;

    [System.NonSerialized] public bool xrDeviceOn;
    [SerializeField] private XRLoader[] loaders;
    private int activeLoader;

    [SerializeField] private Transform cameraOffset;
    [SerializeField] private VRBlink tunnelingController;

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
    [SerializeField] private XRPokeInteractor[] pokeInteractors;

    [SerializeField] private InputActionReference pauseAction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

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

        SetupHands();

        // otherwise, interactors dont interact with anything
        foreach (var ray in rayInteractors)
        {
            ray.interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();
            ray.enableUIInteraction = false;
            ray.enableUIInteraction = true;
        }

        for(int i = 0; i < xrControllers.Length; i++)
        {
            pokeInteractors[i].interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();
            pokeInteractors[i].enableUIInteraction = false;
            pokeInteractors[i].enableUIInteraction = true;

            //HandAnimatorManagerVR handAnimator = xrControllers[i].GetComponentInChildren<HandAnimatorManagerVR>();
            //pokeInteractors[i].hoverEntered.AddListener(handAnimator.InteractorHoverEnter);
            //pokeInteractors[i].hoverExited.AddListener(handAnimator.InteractHoverExit);
        }

        tunnelingController.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseAction.action.triggered)
        {
            Debug.Log("Pause Action");
            UIManager.Instance.TogglePause();
        }
    }

    public IEnumerator StartXR()
    {
        XRSettings.enabled = true;
        bool success = false;
        Debug.Log("Get if hardward on");
        if (XRGeneralSettings.Instance == null) Debug.Log("no xr settings");
        if (XRGeneralSettings.Instance.Manager == null) XRManagerSettings.CreateInstance<XRManagerSettings>();
        if (XRGeneralSettings.Instance.Manager.activeLoaders == null) Debug.Log("no xr loaders");

        Debug.Log("vr init checked");

        XRGeneralSettings.Instance.Manager.TrySetLoaders(new List<XRLoader>());

        for (int i = 0; i < loaders.Length; i++)
        {
            XRGeneralSettings.Instance.Manager.TryAddLoader(loaders[i]);
            //XRGeneralSettings.Instance.Manager.loaders.Add(loaders[i]);
            Debug.Log($"{loaders[i].name} loaded");

            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            Debug.Log($"{loaders[i].name} initialized");
            //if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
            XRGeneralSettings.Instance.Manager.StartSubsystems();

            //Check if initialization was successfull.
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);
            if (xrDisplaySubsystems.Count > 0)
            {
                success = xrDisplaySubsystems[0].running;
            }

            Debug.Log("Is " + loaders[i].name + " available? " + success);
            if (success)
            {
                activeLoader = i;
                break;
            }

            //XRGeneralSettings.Instance.Manager.TrySetLoaders(new List<XRLoader>());
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
        XRSettings.enabled = success;
        xrDeviceOn = success;

        
        /*
        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        XRGeneralSettings.Instance.Manager.StartSubsystems();*/
    }

    void StopXR()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            Camera.main.ResetAspect();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }

    private void SetupHands()
    {
        TankController.Instance.SetHandModel();
    }

    public void ApplySettings()
    {
        if (VRSettings.Instance == null) return;

        rayTeleporters[1].gameObject.SetActive(true && VRSettings.Instance.UseTeleportMovement);
        rayTeleporters[0].gameObject.SetActive(false);

        snapTurnProvider.turnAmount = VRSettings.Instance.UseIncrementalRotate ? snapTurnAngle : 0;

        continuousMoverProvider.moveSpeed = VRSettings.Instance.UseTeleportMovement ? 0 : smoothMoveSpeed;
        continuousTurnProvider.turnSpeed = VRSettings.Instance.UseIncrementalRotate ? 0 : smoothRotateSpeed;
    }

    public void DisableMovement()
    {
        foreach (var tp in rayTeleporters)
            tp.gameObject.SetActive(false);
        snapTurnProvider.turnAmount = 0;
        continuousMoverProvider.moveSpeed = 0;
        continuousTurnProvider.turnSpeed = 0;
    }


    public RaycastResult GetPrimaryRaycastResult()
    {
        RaycastResult hit;

        rayInteractors[VRSettings.Instance.PrimaryHand].TryGetCurrentUIRaycastResult(out hit);
        return hit;
    }

    public Vector3 GetHitPosition(bool primary = true, bool requireActive = false)
    {
        if (VRSettings.Instance == null)
        {
            Debug.LogError("No vr settings");
            return Vector3.positiveInfinity;
        }

        int primaryHand = VRSettings.Instance.PrimaryHand;
        int secondaryHand = 1 - primaryHand;

        bool primaryActive = xrControllers[primaryHand].activateActionValue.action.ReadValue<float>() > 0.1f;
        bool secondaryActive = xrControllers[secondaryHand].activateActionValue.action.ReadValue<float>() > 0.1f;

        if (pokeInteractors[primaryHand].isSelectActive && !primaryActive)
        {
            return pokeInteractors[primaryHand].attachTransform.position;
        }

        if (pokeInteractors[secondaryHand].isSelectActive && !primaryActive)
        {
            return pokeInteractors[secondaryHand].attachTransform.position;
        }

        RaycastResult hit;
        if (!(primary ? primaryActive : secondaryActive) && requireActive)
            return Vector3.positiveInfinity;
        if (!rayInteractors[primary ? primaryHand : secondaryHand].TryGetCurrentUIRaycastResult(out hit))
            return Vector3.positiveInfinity;
        //Debug.Log($"hitting {hit.gameObject.name} at {hit.worldPosition}"); 
        return hit.worldPosition;
    }

    public void SetCameraSitting()
    {
        cameraOffset.transform.localPosition = new Vector3(0, 1f, 0);
    }

    public void SetCameraStanding()
    {
        cameraOffset.transform.localPosition = new Vector3(0, 1.3f, 0);
    }

    public void SetTunnelingSize(float value)
    {
        tunnelingController.SetAperatureSize(value);
    }
}
