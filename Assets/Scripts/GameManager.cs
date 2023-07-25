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

        VRManager vrManager = GameObject.FindObjectOfType<VRManager>();
        if (vrManager)
        {
            yield return vrManager.StartXR();
            useVR = vrManager.xrDeviceOn;
        }

        // make sure each of these items are disabled prior to this start call to prevent any unwanted initialization
        for (int i = 0; i < vrObjects.Length; i++)
            vrObjects[i].SetActive(useVR);
        for (int i = 0; i < desktopObjects.Length; i++)
            desktopObjects[i].SetActive(!useVR);

        // wait for objects to activate before initializing them
        yield return new WaitForEndOfFrame();

        GameObject.FindObjectOfType<TankController>().Initialize();
        GameObject.FindObjectOfType<Menu>().Initialize();
        GameObject.FindObjectOfType<UIManager>().Initialize();
        GameObject.FindObjectOfType<SecurityCode>().Initialize();
        GameObject.FindObjectOfType<StatisticsManager>().Initialize();

        if (!useVR && vrManager)
            Destroy(vrManager.gameObject);
        else
            vrManager.Initialize();

        // Each scene has its own modules. Wait until all are loaded before matching modules up to their respective interactibles.
        ModuleMapper moduleMapper = GameObject.FindObjectOfType<ModuleMapper>();
        if (moduleMapper)
        {
            moduleMapper.MapModules();
        }
        // Enable the gotos so the module mapper can find them. Disable after mapping
        Menu.Instance.gotoMenu.SetActive(false);
        //UIManager.Instance.gotoMenu.gameObject.SetActive(false);

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

        StorageManager.Instance.contentDownloadedEvent.AddListener(moduleMapper.MapPlayerContent);
        StorageManager.Instance.contentDownloadedEvent.AddListener(ContentMapped);
        //StorageManager.Instance.contentDownloadedEvent.AddListener(LoadModule);
        StorageManager.Instance.StartContentDownload();

        IntroScene intro = GameObject.FindObjectOfType<IntroScene>();

        bool firstTime = true;
        if (Profiler.Instance)
            firstTime = Profiler.Instance.currentUser.newGame;
        intro.PlayCutscene(firstTime);
    }

    private void ContentMapped()
    {
        SavePatientData.Instance.Initialize();  // ensure patient data is downloaded
        StorageManager.Instance.downloadStatusEvent = new UnityEngine.Events.UnityEvent<bool>();
        StorageManager.Instance.downloadStatusEvent.AddListener(LoadModule);
        GameObject.FindObjectOfType<SavePatientData>().Initialize();
    }

    private void LoadModule(bool status)
    {
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
        TankController.Instance.transform.position = location;
        yield return new WaitForEndOfFrame();
        //TankController.Instance.EnableMovement();
        if (teleportEvent != null) teleportEvent.Invoke();
    }

    // need function for handling when a new level is loaded. map the modules to the newly found interactables
    public void LevelLoaded(int level)
    {
        // 
    }

    public void Quit()
    {
        //Application.Quit();
        SceneLoader.Instance.LoadMainMenu();
    }
}
