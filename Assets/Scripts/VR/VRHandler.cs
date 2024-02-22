using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;

public class VRHandler : MonoBehaviour
{
    public static VRHandler Instance { get { return _instance; } }
    public static VRHandler _instance;

    [SerializeField] VRManager vrRig;
    [SerializeField] Transform worldUILocaiton;
    [SerializeField] private XRLoader[] loaders;

    public bool vrActive;

    // Start is called before the first frame update
    void Start()
    {
        /*if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);*/

        StartCoroutine(StartXR());
    }

    public IEnumerator StartXR()
    {
        yield return new WaitForEndOfFrame();
        if (!vrActive)
        {
            Debug.Log("starting xr...");

            XRSettings.enabled = true;
            bool success = false;
            if (XRGeneralSettings.Instance == null) Debug.Log("no xr settings");
            if (XRGeneralSettings.Instance.Manager == null) XRManagerSettings.CreateInstance<XRManagerSettings>();
            if (XRGeneralSettings.Instance.Manager.activeLoaders == null) Debug.Log("no xr loaders");

            var startingSubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(startingSubsystems);
            if (startingSubsystems.Count > 0)
            {
                Debug.Log("XR already loaded");
                success = startingSubsystems[0].running;
                /*if (XRGeneralSettings.Instance.Manager.activeLoader != null)
                {
                    XRGeneralSettings.Instance.Manager.StopSubsystems();
                    XRGeneralSettings.Instance.Manager.DeinitializeLoader();
                }
                XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
                XRGeneralSettings.Instance.Manager.StartSubsystems();*/
            }

            if (!success)
            {
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
                    

                    if (success)
                    {
                        vrActive = success;
                        break;
                    }

                    //XRGeneralSettings.Instance.Manager.TrySetLoaders(new List<XRLoader>());
                    XRGeneralSettings.Instance.Manager.StopSubsystems();
                    XRGeneralSettings.Instance.Manager.DeinitializeLoader();
                }
            }
            vrActive = success;
            Initialize();
        }
    }

    public void Initialize()
    {
        if (!vrActive)
        {
            Destroy(gameObject);
            Destroy(vrRig.gameObject);
            return;
        }

        TankController tankController = GameObject.FindObjectOfType<TankController>();
        //if (tankController)
            //tankController.Initialize();
        vrRig.Initialize();
        //vrRig.DisableMovement();

        vrRig.SetCameraSitting();
        if (gameObject)
            ForceInteractorRegister();

        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            //if (cam != tank.playerCam)
            if (cam.transform.parent == null)
                cam.gameObject.SetActive(false);
        }

        // THIS DOESN'T WORK. ALL OTHER EVENT SYSTEMS MUST BE DISABLED ON START
        EventSystem[] allSystems = GameObject.FindObjectsOfType<EventSystem>();
        foreach (var system in allSystems)
        {
            //if (system.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>())
            if (system.transform.parent != null)
            {
                //Debug.Log("valid event system found " + system.name);
                EventSystem.current = system;
            }
            else
            {
                Destroy(system.gameObject);
            }
        }
        ConvertCanvassesToWorldSpace();

        Debug.Log("handler register groups");
        XRInteractionGroup[] interactionGroups = GameObject.FindObjectsOfType<XRInteractionGroup>();
        foreach (var group in interactionGroups)
        {
            group.interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();
        }

        //ForceInteractorRegister();
    }

    private void ForceInteractorRegister()
    {
        Debug.Log("register interactors with handler");
        XRRayInteractor[] rayInteractors = GameObject.FindObjectsOfType<XRRayInteractor>();
        foreach (var ray in rayInteractors)
        {
            ray.interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();
            ray.enableUIInteraction = false;
            ray.enableUIInteraction = true;
        }

        XRPokeInteractor[] pokeInteractors = GameObject.FindObjectsOfType<XRPokeInteractor>();
        for (int i = 0; i < pokeInteractors.Length; i++)
        {
            pokeInteractors[i].interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();
            pokeInteractors[i].enableUIInteraction = false;
            pokeInteractors[i].enableUIInteraction = true;
        }
    }

    private void ConvertCanvassesToWorldSpace()
    {
        foreach (var canvas in GameObject.FindObjectsOfType<Canvas>())
        {
            var trackedDeviceRaycaster = canvas.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
            if (trackedDeviceRaycaster != null || worldUILocaiton == null) continue;

            trackedDeviceRaycaster = canvas.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
            //var canvasHelp = canvas.gameObject.AddComponent<VRCanvasHelper>();
           
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = GameObject.FindObjectOfType<Camera>(); // only active one at this point should be the vr one
            //canvas.transform.parent = worldUILocaiton;
            canvas.transform.position = worldUILocaiton.position;
            canvas.transform.rotation = worldUILocaiton.rotation;
            canvas.transform.localScale = worldUILocaiton.localScale;
        }
    }

    public void GameLoading()
    {
        //Debug.Log("game loading");
        Destroy(vrRig.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        Debug.Log("Handler destroyed");
    }

    void StopXR()
    {
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("app quit");
        StopXR();
    }


}
