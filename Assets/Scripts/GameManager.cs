using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum Levels { LOLLIPOP, DOWN, HOME };
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ModuleMapper moduleMapper;


    public static GameManager Instance { get { return _instance; } }
    private static GameManager _instance;

    public UnityEvent teleportEvent;
    public GameObject scavengerObjects;
    public SceneLoader sceneLoader;
    public VRManager vrManager;

    public bool useVR;
    // includes player, menu, and security code
    [SerializeField] GameObject[] vrObjects, desktopObjects;

    [HideInInspector] public bool inModule;


    public ModuleMapper ModuleMapper { get => moduleMapper; }


    protected void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitForEndOfFrame();
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        transform.parent = null;
        if (scavengerObjects)
        {
            DontDestroyOnLoad(scavengerObjects);
        }
        DontDestroyOnLoad(gameObject);

        //VRHandler vrHandler = FindObjectOfType<VRHandler>();
        if (vrManager && useVR)
        {
            //vrHandler.vrActive = useVR;
            //yield return vrHandler.StartXR();
            //useVR = vrHandler.vrActive;
            yield return vrManager.StartXR();
            useVR = vrManager.xrDeviceOn;
        }

        //if (!useVR)
        //    vrManager.StopXR();

        // make sure each of these items are disabled prior to this start call to prevent any unwanted initialization
        for (int i = 0; i < vrObjects.Length; i++)
        {
            if (vrObjects[i])
            {
                vrObjects[i].SetActive(useVR);
            }
        }
        for (int i = 0; i < desktopObjects.Length; i++)
        {
            desktopObjects[i].SetActive(!useVR);
        }

        // wait for objects to activate before initializing them
        yield return new WaitForEndOfFrame();

        // TODO: Player is getting destroyed here... meaning the VR player controller is not set active and the normal player IS set active
        if (TankController.Instance == null) { FindObjectOfType<TankController>().Initialize(); }
        FindObjectOfType<Menu>().Initialize();
        FindObjectOfType<UIManager>().Initialize();
        FindObjectOfType<SecurityCode>().Initialize();
        FindObjectOfType<StatisticsManager>().Initialize();

        if (vrManager && useVR)
        {
            UnityEngine.XR.Interaction.Toolkit.XRInteractionGroup[] interactionGroups = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.XRInteractionGroup>();
            foreach (var group in interactionGroups)
            {
                group.interactionManager = FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
            }
            yield return new WaitForEndOfFrame();
            vrManager.Initialize();
            var crossSceneTPAreas = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.TeleportationArea>();
            foreach (var tpArea in crossSceneTPAreas)
            {
                tpArea.interactionManager = vrManager.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>(true);
                tpArea.teleportationProvider = vrManager.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.TeleportationProvider>();
            }

            // VR is sometimes considered a mobile device. Clean this up if it still exists
            TouchControls touchControls = FindObjectOfType<TouchControls>();
            if (touchControls) { Destroy(touchControls.gameObject); }

        }
        else
        {
            Destroy(vrManager.gameObject);
        }
        //foreach (var looker in FindObjectsOfType<LookAt>())
        //    looker.Initialize(player.transform);

        // Each scene has its own modules. Wait until all are loaded before matching modules up to their respective interactibles.
        moduleMapper.MapModules();
        // Enable the gotos so the module mapper can find them. Disable after mapping
        if (Menu.Instance) { Menu.Instance.gotoMenu.SetActive(false); }
        //UIManager.Instance.gotoMenu.gameObject.SetActive(false);
        ScoreCalculator.instance.SetImpairmentLevel(Profiler.Instance.currentUser.ciLevel);
        moduleMapper.interactables[moduleMapper.interactables.Length - 1].interactEvent.AddListener(sceneLoader.LoadHouseInterior);
        //moduleMapper.interactables[moduleMapper.interactables.Length - 1].GetComponent<ActivatorZone>().enterEvent.AddListener(sceneLoader.LoadHouseInterior);
        moduleMapper.gotos[moduleMapper.gotos.Length - 1].onGo = new UnityEvent();
        moduleMapper.gotos[moduleMapper.gotos.Length - 1].onGo.AddListener(sceneLoader.LoadHouseInterior);

        // load the downloaded images into the exercises
        if (StorageManager.Instance == null)
        {
            StorageManager storage = FindObjectOfType<StorageManager>();
            storage.Initialize();
        }

        StorageManager.Instance.downloadStatusEvent = new UnityEvent<bool>();
        StorageManager.Instance.downloadStatusEvent.AddListener(LoadModule);
        StorageManager.Instance.StartCSVDownload(Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "patient_data.csv");

        StorageManager.Instance.contentDownloadedEvent.AddListener(moduleMapper.MapPlayerContent);
        StorageManager.Instance.StartContentDownload();

        IntroScene intro = FindObjectOfType<IntroScene>();

        bool firstTime = true;
        if (Profiler.Instance) { firstTime = Profiler.Instance.currentUser.newGame; }

        if (VRManager.Instance && firstTime && useVR)
        {
            TutorialManager tutorial = FindObjectOfType<TutorialManager>();
            tutorial.BeginTutorial();
        }
        else
        {
            intro.PlayCutscene(firstTime);
        }

        SetCameraClipDist(175f);
    }

    private void LoadModule(bool status)
    {
        bool newGame = Profiler.Instance.currentUser.newGame;
        var savePatientData = FindObjectOfType<SavePatientData>();
        if (savePatientData == null)
        {
            Debug.LogError("Could not find SavePatientData in scene.");
            return;
        }
        savePatientData.Initialize(newGame, moduleMapper);
        IntroScene intro = FindObjectOfType<IntroScene>();
        int lastModulePlayed = 0;
        if (!newGame)
        {
            lastModulePlayed = SavePatientData.Instance.GetModuleIndexLastPlayed();
            for (int i = lastModulePlayed - 1; i >= 0; i--)
            {
                moduleMapper.modules[i].IsComplete = true;
            }
        }
        if (lastModulePlayed == 0)
        {
            intro.SetDialogue(true);
            return;
        }

        if (!intro.skipped) // cutscene is still running
        {
            intro.onComplete = new UnityEvent();
            intro.onComplete.AddListener(moduleMapper.gotos[lastModulePlayed].Go);
        }
        else
        {
            moduleMapper.gotos[lastModulePlayed].Go();
        }
    }

    public void TeleportPlayer(Vector3 location)
    {
        IntroScene intro = FindObjectOfType<IntroScene>();
        if (intro != null)
        {
            if (!intro.skipped)
                intro.Interrupt();
        }
        StartCoroutine(TeleportRoutine(location));
    }

    private IEnumerator TeleportRoutine(Vector3 location)
    {
        TankController.Instance.DisableMovement();
        yield return new WaitForEndOfFrame();
        // somehow, floating origin has reset our position and shifted the world before we have set location
        TankController.Instance.transform.position = location;
        yield return new WaitForEndOfFrame();
        // gives a moment for floating origin to recenter
        TankController.Instance.EnableMovement();
        //TankController.Instance.GetComponent<FloatingOrigin>()?.RecenterOrigin();
        yield return new WaitForEndOfFrame();
        if (teleportEvent != null) teleportEvent.Invoke();
    }


    public void SetCameraClipDist(float dist)
    {
        if (TankController.Instance) { TankController.Instance.SetCullDistance(dist); }
    }

    public void Quit()
    {
        //Application.Quit();
        if (VRManager.Instance) { Destroy(VRManager.Instance.gameObject); }
        if (TankController.Instance) { Destroy(TankController.Instance.gameObject); }
        // there may be more
        SceneLoader.Instance.LoadMainMenu();
    }
}
