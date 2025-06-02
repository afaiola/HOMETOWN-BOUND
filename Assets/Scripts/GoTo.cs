using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GoTo : MonoBehaviour
{
    public Module module;
    public GameObject moduleObject;
    static Module[] _modules;
    public UnityEngine.Events.UnityEvent onGo;

    Module[] modules
    {
        get
        {
            if (_modules == null)
                _modules = FindObjectsOfType<Module>();
            return _modules;
        }
    }

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ShowSecurity);
    }

    public void ShowSecurity()
    {
        SecurityCode.Instance.onSuccess = new UnityEngine.Events.UnityEvent();
        SecurityCode.Instance.onSuccess.AddListener(Go);
        SecurityCode.Instance.Show();

        transform.parent.parent.parent.gameObject.SetActive(false);
    }

    public void Go()
    {
        foreach (var m in modules)
        {
            if (m != module)
                if (m != null)
                    m.Hide();
        }
        if (!module.open)
        {
            UIManager.Instance.Resume();
            //UIManager.Instance.LockCursor();
            //ScoreCalculator.instance.controller.transform.position = moduleObject.transform.position;
            //StartModule();  // here to prevent collider starting the module too early.
            if (SavePatientData.Instance)
            {
                SavePatientData.Instance.SaveEntry(module.exercises[0].exerciseID - 1, 1, 0, 0);  // -1 covers the walk to exercise
            }
            GameManager.Instance.teleportEvent = new UnityEngine.Events.UnityEvent();
            GameManager.Instance.teleportEvent.AddListener(StartModule);
            Vector3 loc = TankController.Instance.transform.position;
            if (moduleObject) loc = moduleObject.transform.position;
            GameManager.Instance.TeleportPlayer(loc);
            if (onGo != null) onGo.Invoke();
            ModuleMapper mapper = GameObject.FindObjectOfType<ModuleMapper>();
            if (mapper) { mapper.SkipModules(module.ModuleNo); }
        }
    }

    private void StartModule()
    {
        //if (!GameManager.Instance.useVR)
        //    TankController.Instance.DisableMovement();
        ScoreCalculator.instance.GetStars();
        ScoreCalculator.instance.StartActivity(module.exercises[module.current].exerciseID);
        module?.Play();
        if (moduleObject)
            moduleObject.SetActive(false);
    }
}
