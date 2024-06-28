using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum Levels { LOLLIPOP, DOWN, HOME};
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return _instance; } }
    private static GameManager _instance;

    public UnityEvent teleportEvent;
    public GameObject scavengerObjects;
    public SceneLoader sceneLoader;

    public VRManager vrManager;
    public bool useVR;
    // includes player, menu, and security code
    [SerializeField] GameObject[] vrObjects, desktopObjects;

    // Start is called before the first frame update
    void Start()
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
            DontDestroyOnLoad(scavengerObjects);
        DontDestroyOnLoad(gameObject);
        
        VRHandler vrHandler = GameObject.FindObjectOfType<VRHandler>();
        if (vrManager && useVR)
        {
            yield return vrManager.StartXR();
            useVR = vrManager.xrDeviceOn;
        }

        //if (!useVR)
        //    vrManager.StopXR();

        // make sure each of these items are disabled prior to this start call to prevent any unwanted initialization
        for (int i = 0; i < vrObjects.Length; i++)
        {
            if (vrObjects[i])
                vrObjects[i].SetActive(useVR);
        }
        for (int i = 0; i < desktopObjects.Length; i++)
            desktopObjects[i].SetActive(!useVR);

        // wait for objects to activate before initializing them
        yield return new WaitForEndOfFrame();

        GameObject.FindObjectOfType<TankController>().Initialize();
        GameObject.FindObjectOfType<Menu>().Initialize();
        GameObject.FindObjectOfType<UIManager>().Initialize();
        GameObject.FindObjectOfType<SecurityCode>().Initialize();
        GameObject.FindObjectOfType<StatisticsManager>().Initialize();

        //VRManager vrManager = GameObject.FindObjectOfType<VRManager>();
        if (vrManager)
        {
            if (useVR)
            {
                Debug.Log("manager register groups");   
                /*
                UnityEngine.XR.Interaction.Toolkit.XRInteractionGroup[] interactionGroups = GameObject.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.XRInteractionGroup>();
                foreach (var group in interactionGroups)
                {
                    group.interactionManager = GameObject.FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
                }
                yield return new WaitForEndOfFrame();
                */
                yield return vrManager.Initialize();
                var crossSceneTPAreas = GameObject.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.TeleportationArea>();
                foreach (var tpArea in crossSceneTPAreas)
                {
                    tpArea.interactionManager = vrManager.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>(true);
                    tpArea.teleportationProvider = vrManager.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.TeleportationProvider>();
                }

                // VR is sometimes considered a mobile device. Clean this up if it still exists
                TouchControls touchControls = GameObject.FindObjectOfType<TouchControls>();
                if (touchControls)
                    Destroy(touchControls.gameObject);
                yield return new WaitForEndOfFrame();
                vrManager.xrOrigin.gameObject.SetActive(true);
            }
            else
            {
                Destroy(vrManager.gameObject);
            }
        }
        yield return new WaitForEndOfFrame();   // so tank controller can be found


        foreach (var looker in GameObject.FindObjectsOfType<LookAt>())
            looker.Initialize();

        // Each scene has its own modules. Wait until all are loaded before matching modules up to their respective interactibles.
        ModuleMapper moduleMapper = GameObject.FindObjectOfType<ModuleMapper>();
        if (moduleMapper)
        {
            moduleMapper.MapModules();
        }
        // Enable the gotos so the module mapper can find them. Disable after mapping
        Menu.Instance.gotoMenu.SetActive(false);
        //UIManager.Instance.gotoMenu.gameObject.SetActive(false);
        ScoreCalculator.instance.SetImpairmentLevel(Profiler.Instance.currentUser.ciLevel);
        moduleMapper.interactables[moduleMapper.interactables.Length - 1].interactEvent.AddListener(sceneLoader.LoadHouseInterior);
        //moduleMapper.interactables[moduleMapper.interactables.Length - 1].GetComponent<ActivatorZone>().enterEvent.AddListener(sceneLoader.LoadHouseInterior);
        moduleMapper.gotos[moduleMapper.gotos.Length - 1].onGo = new UnityEvent();
        moduleMapper.gotos[moduleMapper.gotos.Length - 1].onGo.AddListener(sceneLoader.LoadHouseInterior);

        // load the downloaded images into the exercises
        if (StorageManager.Instance == null)
        {
            StorageManager storage = GameObject.FindObjectOfType<StorageManager>();
            storage.Initialize();
        }

        StorageManager.Instance.downloadStatusEvent = new UnityEvent<bool>();
        StorageManager.Instance.downloadStatusEvent.AddListener(LoadModule);
        StorageManager.Instance.StartCSVDownload(Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "patient_data.csv");

        StorageManager.Instance.contentDownloadedEvent.AddListener(moduleMapper.MapPlayerContent);
        StorageManager.Instance.contentDownloadedEvent.AddListener(ContentMapped);
        //StorageManager.Instance.contentDownloadedEvent.AddListener(LoadModule);
        StorageManager.Instance.StartContentDownload();

        IntroScene intro = GameObject.FindObjectOfType<IntroScene>();

        bool firstTime = true;
        if (Profiler.Instance)
            firstTime = Profiler.Instance.currentUser.newGame;

        if (VRManager.Instance && firstTime && useVR)
        {
            TutorialManager tutorial = GameObject.FindObjectOfType<TutorialManager>();
            tutorial.BeginTutorial();
        }
        else
        {
            intro.PlayCutscene(firstTime);
        }

        SetCameraClipDist(175f);
    }

    private void ContentMapped()
    {
        //SavePatientData.Instance.Initialize();  // ensure patient data is downloaded
        //StorageManager.Instance.downloadStatusEvent = new UnityEngine.Events.UnityEvent<bool>();
        //GameObject.FindObjectOfType<SavePatientData>().Initialize();
        //LoadModule(true);
        //StorageManager.Instance.downloadStatusEvent.AddListener(LoadModule);
    }

    private void LoadModule(bool status)
    {
        GameObject.FindObjectOfType<SavePatientData>().Initialize();
        IntroScene intro = GameObject.FindObjectOfType<IntroScene>();
        int lastModulePlayed = SavePatientData.Instance.LastModulePlayed();
        if (Profiler.Instance.currentUser.newGame) lastModulePlayed = 0;
        if (lastModulePlayed == 0)
        {
            intro.SetDialogue(true);
            return;
        }

        ModuleMapper moduleMapper = GameObject.FindObjectOfType<ModuleMapper>();

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
        IntroScene intro = GameObject.FindObjectOfType<IntroScene>();
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
        //TankController.Instance.EnableMovement();
        //TankController.Instance.GetComponent<FloatingOrigin>()?.RecenterOrigin();
        if (teleportEvent != null) teleportEvent.Invoke();
    }

    // need function for handling when a new level is loaded. map the modules to the newly found interactables
    public void LevelLoaded(int level)
    {
        // 
    }

    public void SetCameraClipDist(float dist)
    {
        if (TankController.Instance)
            TankController.Instance.SetCullDistance(dist);
    }
    public void Quit()
    {
        //Application.Quit();
        if (VRManager.Instance)
            Destroy(VRManager.Instance.gameObject);
        SceneLoader.Instance.LoadMainMenu();
    }
}
