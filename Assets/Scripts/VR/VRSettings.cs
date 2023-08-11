using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRSettings : MonoBehaviour
{
    public static VRSettings Instance { get { return _instance; } }
    private static VRSettings _instance;

    public GameObject settingsMenu;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: open the settings with a button

    public void LoadSettings()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        useTeleportMovement = PlayerPrefs.GetInt(movementSaveKey, 0) == 1;
        useIncrementalRotate = PlayerPrefs.GetInt(rotateSaveKey, 0) == 1;
        isLeftHanded = PlayerPrefs.GetInt(handednessSaveKey, 0) == 1;

        teleportToggle.SetValue(useTeleportMovement);
        rotateToggle.SetValue(useIncrementalRotate);
        handednessToggle.SetValue(isLeftHanded);

        teleportToggle.onValueChange = new UnityEvent<bool>();
        teleportToggle.onValueChange.AddListener(SetMovementType);
        rotateToggle.onValueChange = new UnityEvent<bool>();
        rotateToggle.onValueChange.AddListener(SetRotateSetting);
        handednessToggle.onValueChange = new UnityEvent<bool>();
        handednessToggle.onValueChange.AddListener(SetHandedness);
    }

    public void SetMovementType(bool isTeleport)
    {
        useTeleportMovement = isTeleport;
        PlayerPrefs.SetInt(movementSaveKey, !useTeleportMovement ? 0 : 1);
        if (onMovementTypeChange != null)
            onMovementTypeChange.Invoke();
    }

    public void SetRotateSetting(bool isIncremental)
    {
        useIncrementalRotate = isIncremental;
        PlayerPrefs.SetInt(rotateSaveKey, !useIncrementalRotate ? 0 : 1);
        if (onRotateTypeChange != null)
            onRotateTypeChange.Invoke();
    }

    public void SetHandedness(bool isLeft)
    {
        Debug.Log("set hand " + isLeft);
        isLeftHanded = isLeft;
        PlayerPrefs.SetInt(rotateSaveKey, PrimaryHand);
        if (onHandednessChange != null)
            onHandednessChange.Invoke();
    }
}
