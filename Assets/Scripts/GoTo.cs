using System;
using UnityEngine;
using UnityEngine.UI;

public class GoTo : MonoBehaviour
{
    public Module goToModule;
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
        foreach (var module in modules)
        {
            if (module != goToModule && module != null)
            {
                module.Hide();
            }
        }
        if (!goToModule.open)
        {
            UIManager.Instance.Resume();
            //UIManager.Instance.LockCursor();
            //ScoreCalculator.instance.controller.transform.position = moduleObject.transform.position;
            //StartModule();  // here to prevent collider starting the module too early.

            if (SavePatientData.Instance)
            {
                int exerciseId = goToModule.exercises[0].exerciseID - 1;  // -1 covers the walk to exercise
                string exerciseName = GameManager.Instance.CreateExerciseNameFromExerciseId(exerciseId);
                SavePatientData.Instance.SaveEntry(exerciseId, exerciseName, DateTimeOffset.Now, 1, 0, 0);
            }
            GameManager.Instance.teleportEvent = new UnityEngine.Events.UnityEvent();
            GameManager.Instance.teleportEvent.AddListener(StartModule);
            Vector3 loc = TankController.Instance.transform.position;
            if (moduleObject) loc = moduleObject.transform.position;
            GameManager.Instance.TeleportPlayer(loc);
            if (onGo != null) onGo.Invoke();
            ModuleMapper mapper = FindObjectOfType<ModuleMapper>();
            if (mapper) { mapper.SkipModules(goToModule.ModuleNo); }
        }
    }

    private void StartModule()
    {
        //if (!GameManager.Instance.useVR)
        //    TankController.Instance.DisableMovement();
        ScoreCalculator.instance.GetStars();
        // TODO : got null ref here after having completed module then trying to jump pack to said module
        ScoreCalculator.instance.StartActivity(goToModule.exercises[goToModule.current].exerciseID);
        goToModule?.Play();
        if (moduleObject)
        {
            moduleObject.SetActive(false);
        }
    }
}
