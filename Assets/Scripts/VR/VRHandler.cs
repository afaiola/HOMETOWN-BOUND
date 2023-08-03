using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class VRHandler : MonoBehaviour
{
    public static VRHandler Instance { get { return _instance; } }
    public static VRHandler _instance;

    [SerializeField] GameObject vrRig;
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
                    vrActive = success;

                    if (success)
                    {
                        break;
                    }

                    //XRGeneralSettings.Instance.Manager.TrySetLoaders(new List<XRLoader>());
                    XRGeneralSettings.Instance.Manager.StopSubsystems();
                    XRGeneralSettings.Instance.Manager.DeinitializeLoader();
                }
            }
        }
        Initialize();
    }

    public void Initialize()
    {
        Debug.Log("init menu handler");
        if (!vrActive)
        {
            Destroy(gameObject);
            Destroy(vrRig);
            return;
        }

        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (var cam in cameras)
        {
            //if (cam != tank.playerCam)
            if (cam.transform.parent == null)
                cam.gameObject.SetActive(false);
        }

        EventSystem[] allSystems = GameObject.FindObjectsOfType<EventSystem>();
        foreach (var system in allSystems)
        {
            if (system.transform.parent == null)
                Destroy(system.gameObject);
        }

        ConvertCanvassesToWorldSpace();
        //if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        //    MainMenuLoaded();
    }

    private void ConvertCanvassesToWorldSpace()
    {
        foreach (var canvas in GameObject.FindObjectsOfType<Canvas>())
        {
            var trackedDeviceRaycaster = canvas.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
            if (trackedDeviceRaycaster != null || worldUILocaiton == null) continue;

            canvas.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();

            canvas.renderMode = RenderMode.WorldSpace;
            //canvas.transform.parent = worldUILocaiton;
            canvas.transform.position = worldUILocaiton.position;
            canvas.transform.rotation = worldUILocaiton.rotation;
            canvas.transform.localScale = worldUILocaiton.localScale;
        }
    }

    public void GameLoading()
    {
        Debug.Log("game loading");
        Destroy(vrRig);
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
