using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class VRManager : MonoBehaviour
{
    public static VRManager Instance { get { return _instance; } }
    private static VRManager _instance;

    public bool makeSingleton = true;
    [System.NonSerialized] public bool xrDeviceOn;
    [SerializeField] private XRLoader[] loaders;
    private int activeLoader;

    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private Transform cameraOffset;
    [SerializeField] private VRBlink tunnelingController;

    [Header("Movement Sources")]
    [SerializeField] private TeleportationProvider teleportProvider;
    [SerializeField] private ActionBasedSnapTurnProvider snapTurnProvider;
    [SerializeField] private ActionBasedContinuousMoveProvider continuousMoverProvider;
    [SerializeField] private ActionBasedContinuousTurnProvider continuousTurnProvider;

    private float smoothMoveSpeed = 4;
    private float smoothRotateSpeed = 90;
    private float snapTurnAngle = 45;
    [SerializeField] private float cameraOffsetHeight = 0.7f;   // based on height of standing character while player is seated. 1.1 for in game

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
    public void StopXR()
    {
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        XRSettings.enabled = false;
    }

    public IEnumerator StartXR()
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("starting xr...");

        XRSettings.enabled = true;
        bool success = false;
        if (XRGeneralSettings.Instance == null) Debug.Log("no xr settings");
        if (XRGeneralSettings.Instance.Manager == null) XRManagerSettings.CreateInstance<XRManagerSettings>();
        if (XRGeneralSettings.Instance.Manager.activeLoaders == null) Debug.Log("no xr loaders");

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        XRGeneralSettings.Instance.Manager.TrySetLoaders(new List<XRLoader>());

        for (int i = 0; i < loaders.Length; i++)
        {
            XRGeneralSettings.Instance.Manager.TryAddLoader(loaders[i]);

            if (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
                yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Debug.Log("init successful? " + XRGeneralSettings.Instance.Manager.isInitializationComplete);

            //Check if initialization was successfull.
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);
            if (xrDisplaySubsystems.Count > 0)
            {
                success = xrDisplaySubsystems[0].running;
            }

            Debug.Log($"{loaders[i].name} active? " + success);
            xrDeviceOn = success;

            if (success)
            {
                break;
            }

            //XRGeneralSettings.Instance.Manager.TrySetLoaders(new List<XRLoader>());
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
        XRSettings.enabled = xrDeviceOn;
    }


    public void Initialize()
    {
        if (makeSingleton)
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

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

        ApplySettings();

        SetupHands();

        Debug.Log("manager register interactors");
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
        //SetCameraSitting();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseAction.action.triggered)
        {
            if (UIManager.Instance)
                UIManager.Instance.TogglePause();
        }
    }
    
    private void SetupHands()
    {
        if (TankController.Instance)
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

    public Vector3 GetHitPosition(Vector3 referencePos, bool primary = true, bool requireActiveInput = false)
    {
        if (VRSettings.Instance == null)
        {
            Debug.LogError("No vr settings");
            return Vector3.positiveInfinity;
        }

        int primaryHand = VRSettings.Instance.PrimaryHand;
        int secondaryHand = 1 - primaryHand;
        int activeHand = primary ? primaryHand : secondaryHand;

        // default to the hand position
        Vector3 worldPos = pokeInteractors[activeHand].attachTransform.position;

        RaycastResult hit;
        bool isActive = xrControllers[activeHand].activateActionValue.action.ReadValue<float>() > 0.1f;
        if (isActive)
        {
            if (rayInteractors[activeHand].TryGetCurrentUIRaycastResult(out hit))
            {
                worldPos = hit.worldPosition;
                //Debug.Log($"hitting {hit.gameObject.name} at {hit.worldPosition}");
            }
        }

        if (requireActiveInput)
        {
            // we should only use poke interactor if it is near the canvas
            // and if the action input is active
            float elementDistance = Vector3.Distance(referencePos, worldPos);
            if (elementDistance > 0.1f && !isActive)
                worldPos = Vector3.positiveInfinity;
            string handName = primary ? "primary" : "secondary";
            //Debug.Log($"ele dist: {elementDistance} input is active? {isActive} for {handName} hand gives pos {worldPos}");
        }

        return worldPos;
    }

    public void SetCameraSitting()
    {
        float camHeight = cameraOffsetHeight;
        if (xrOrigin.CurrentTrackingOriginMode == TrackingOriginModeFlags.Device)
        {
            xrOrigin.CameraYOffset = 1f;
            camHeight = 1.3f;
        }
        else if (xrOrigin.CurrentTrackingOriginMode == TrackingOriginModeFlags.Floor)
        {
            VRSettings.Instance.transform.localPosition = new Vector3(VRSettings.Instance.transform.localPosition.x, Camera.main.transform.localPosition.y, VRSettings.Instance.transform.localPosition.z);
        }
        //xrOrigin.GetComponent<CharacterController>().height = 0.1f;
        cameraOffset.transform.localPosition = new Vector3(0, camHeight - 0.3f, 0);
        Debug.Log($"sitting camera height local: {cameraOffset.transform.localPosition} and global: {cameraOffset.transform.position}");
    }

    public void SetCameraStanding()
    {
        float camHeight = cameraOffsetHeight;
        if (xrOrigin.CurrentTrackingOriginMode == TrackingOriginModeFlags.Device)
        {
            xrOrigin.CameraYOffset = 1.3f;
            camHeight = 1.6f;
        }
        else if (xrOrigin.CurrentTrackingOriginMode == TrackingOriginModeFlags.Floor)
        {
            VRSettings.Instance.transform.localPosition = new Vector3(VRSettings.Instance.transform.localPosition.x, Camera.main.transform.localPosition.y, VRSettings.Instance.transform.localPosition.z);
        }
        cameraOffset.transform.localPosition = new Vector3(0, camHeight, 0);
        //xrOrigin.GetComponent<CharacterController>().height = 0.1f;
        Debug.Log($"standing camera height local: {cameraOffset.transform.localPosition} and global: {cameraOffset.transform.position}");
        Debug.Log($"main camera height local: {Camera.main.transform.localPosition.y} and global: {Camera.main.transform.position.y}");
    }

    public void SetTunnelingSize(float value)
    {
        tunnelingController.SetAperatureSize(value);
    }
}
