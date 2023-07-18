using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRSettings : MonoBehaviour
{
    public static VRSettings Instance { get { return _instance; } }
    private static VRSettings _instance;

    [System.NonSerialized]
    public UnityEvent onMovementTypeChange, onRotateTypeChange, onHandednessChange;

    [SerializeField] private ToggleOption teleportToggle, rotateToggle, handednessToggle;

    public bool UseTeleportMovement { get { return useTeleportMovement; } }
    public bool UseIncrementalRotate { get { return useIncrementalRotate; } }
    public int PrimaryHand { get { return !isLeftHanded ? 0 : 1; } }

    private bool useTeleportMovement, useIncrementalRotate;
    private bool isLeftHanded;

    // player pref keys
    private string movementSaveKey = "vr_tp_move";
    private string rotateSaveKey = "vr_inc_rot";
    private string handednessSaveKey = "vr_hand";

    // Start is called before the first frame update
    void Start()
    {
        // TODO: set initial values based on save data
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: open the settings with a button

    public void LoadSettings()
    {
        /*if (!PlayerPrefs.HasKey(movementSaveKey))
            PlayerPrefs.SetInt(movementSaveKey, 0);
        if (!PlayerPrefs.HasKey(rotateSaveKey))
            PlayerPrefs.SetInt(rotateSaveKey, 0);
        if (!PlayerPrefs.HasKey(handednessSaveKey))
            PlayerPrefs.SetInt(handednessSaveKey, 0);*/
        useTeleportMovement = PlayerPrefs.GetInt(movementSaveKey, 0) == 1;
        useIncrementalRotate = PlayerPrefs.GetInt(rotateSaveKey, 0) == 1;
        isLeftHanded = PlayerPrefs.GetInt(handednessSaveKey, 0) == 1;

        teleportToggle.SetValue(useTeleportMovement);
        rotateToggle.SetValue(useIncrementalRotate);
        handednessToggle.SetValue(isLeftHanded);

        teleportToggle.onValueChange = new UnityEvent<bool>();
        teleportToggle.onValueChange.AddListener(SetMovementType);
        rotateToggle.onValueChange = new UnityEvent<bool>();
        rotateToggle.onValueChange.AddListener(SetMovementType);
        handednessToggle.onValueChange = new UnityEvent<bool>();
        handednessToggle.onValueChange.AddListener(SetMovementType);
    }

    public void SetMovementType(bool isTeleport)
    {
        Debug.Log("setting movement type setting");
        useTeleportMovement = isTeleport;
        PlayerPrefs.SetInt(movementSaveKey, !useTeleportMovement ? 0 : 1);
        if (onMovementTypeChange != null)
            onMovementTypeChange.Invoke();
    }

    public void SetRotateSetting(bool isIncremental)
    {
        useTeleportMovement = isIncremental;
        PlayerPrefs.SetInt(rotateSaveKey, !useTeleportMovement ? 0 : 1);
        if (onRotateTypeChange != null)
            onRotateTypeChange.Invoke();
    }

    public void SetHandedness(bool isLeft)
    {
        isLeftHanded = isLeft;
        PlayerPrefs.SetInt(rotateSaveKey, PrimaryHand);
        if (onHandednessChange != null)
            onHandednessChange.Invoke();
    }
}
