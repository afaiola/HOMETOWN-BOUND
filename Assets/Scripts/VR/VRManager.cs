using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
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

    [Header("Interactors")]
    [SerializeField] private XRRayInteractor[] rayInteractors;
    [SerializeField] private XRRayInteractor[] rayTeleporters;
    // direct interactors are for grabbing objects directly with the controllers
    //[SerializeField] private XRDirectInteractor[] directInteractors;

    // TODO: get hand avatars from Assets/Models/VR Hands FP Arms/Prefabs
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

        VRSettings vrSettings = GameObject.FindObjectOfType<VRSettings>();
        if (vrSettings)
        {
            vrSettings.onMovementTypeChange = new UnityEvent();
            vrSettings.onMovementTypeChange.AddListener(ApplyMovementType);
            vrSettings.onRotateTypeChange = new UnityEvent();
            vrSettings.onRotateTypeChange.AddListener(ApplyMovementType);
            vrSettings.onHandednessChange = new UnityEvent();
            vrSettings.onHandednessChange.AddListener(ApplyHandedness);

            vrSettings.LoadSettings();
        }
        ApplyHandedness();
        ApplyMovementType();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyHandedness()
    {
        // by default primary interactors are right hand.
        int primaryHand = VRSettings.Instance.PrimaryHand;
        int secondaryHand = 1 - primaryHand;

        rayTeleporters[secondaryHand].gameObject.SetActive(true && VRSettings.Instance.UseTeleportMovement);
        rayTeleporters[primaryHand].gameObject.SetActive(false);

        // TODO: if using the handheld settings, change the hands.
    }

    public void ApplyMovementType()
    {
        teleportProvider.enabled = VRSettings.Instance.UseTeleportMovement;
        snapTurnProvider.enabled = VRSettings.Instance.UseIncrementalRotate;

        continuousMoverProvider.enabled = !VRSettings.Instance.UseTeleportMovement;
        continuousTurnProvider.enabled = !VRSettings.Instance.UseIncrementalRotate;
    }


    public RaycastResult GetPrimaryRaycastResult()
    {
        RaycastResult hit;

        rayInteractors[VRSettings.Instance.PrimaryHand].TryGetCurrentUIRaycastResult(out hit);
        return hit;
    }

    public Vector3 GetPrimaryHitPosition()
    {
        RaycastResult hit;
        if (VRSettings.Instance == null) return Vector3.positiveInfinity;
        if (!rayInteractors[VRSettings.Instance.PrimaryHand].TryGetCurrentUIRaycastResult(out hit))
        {
            return Vector3.positiveInfinity;
        }
        
        return hit.worldPosition;
    }
}
