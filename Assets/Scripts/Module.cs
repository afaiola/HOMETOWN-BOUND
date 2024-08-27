using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This represents a module that manages its 6 children (excercises)
/// </summary>
public class Module : MonoBehaviour
{
    /// <summary>
    /// Score to get if done faster than maxTime
    /// </summary>
    public int bonusScore;
    /// <summary>
    /// Maximum amount of time to complete module
    /// </summary>
    public double maxTime;
    /// <summary>
    /// Maximum amount of score 
    /// </summary>
    public int maxScore;
    /// <summary>
    /// Bonus for reaching the module faster than timeToReachModule
    /// </summary>
    public int arrivedScore;
    /// <summary>
    /// Maximum amount of time to reach the module
    /// </summary>
    public double timeToReachModule;

    public int current;
    public List<Exercise> exercises;
    public bool playAudio;
    [SerializeField] public int lvl;
    [SerializeField] public int ModuleNo;
    public bool open;
    [SerializeField] Button closeButton;
    protected AudioSource helpAudio;

    private QuantumTek.QuantumUI.QUI_Bar progressBar;
    public virtual void OnValidate()
    {
        exercises = GetComponentsInChildren<Exercise>(true).ToList();
    }
    protected void Start()
    {
        closeButton.onClick.AddListener(ShowSecurity);
    }

    /// <summary>
    /// Does necessary updates to advance to the next excercise
    /// </summary>
    public virtual void Advance()
    {
        //score for completing current module
        ScoreCalculator.instance.EndActivity(exercises[current]._correctCount, exercises[current]._incorrectCount);
        exercises[current++].gameObject.SetActive(false);
        progressBar.SetFill((float)current / exercises.Count);

        //if all excercises are done
        if (current == exercises.Count)
        {
            Profiler.Instance.currentUser.totalPlayTime = ScoreCalculator.instance.totalDuration;
            var stars = ScoreCalculator.instance.GetStars();
            transform.GetChild(0).gameObject.SetActive(false);
            var starPanel = transform.GetChild(1).gameObject;
            starPanel.SetActive(true);
            starPanel.GetComponent<StarComponent>().Set(stars);
            StartCoroutine(Wrap());
            Profiler.Instance.UpdateUserProfile();
            if (SavePatientData.Instance)
                SavePatientData.Instance.UploadPatientData();
            StaticEvent.moduleEnded();
        }
        else
        {
            if (helpAudio)
            {
                helpAudio.clip = exercises[current].customContent ? exercises[current].instructionsCustom : exercises[current].instructionsDefault;
            }
            exercises[current].Arrange();
            exercises[current].gameObject.SetActive(true);
            foreach (var layoutGroup in this.GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }
            //next module
            ScoreCalculator.instance.StartActivity(exercises[current].exerciseID);
            Menu.Instance.UpdateModuleName(string.Format("Level {0} - Module {1} - Exercise {2}", lvl, ModuleNo, current + 1));
        }
        
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    /// <summary>
    /// Shows the stars window for 4 seconds before closing
    /// </summary>
    /// <returns></returns>
    public IEnumerator Wrap()
    {
        yield return new WaitForSeconds(Debug.isDebugBuild ? 0.5f : 4f );
        Wrap2();
    }

    public void Wrap2()
    {
        gameObject.SetActive(false);
        TankController.Instance.EnableMovement();
        //walking to the next module
        ScoreCalculator.instance.StartActivity(exercises[exercises.Count-1].exerciseID+1);
        End();
        open = false;
    }

    public virtual void End()
    {
        GameManager.Instance.inModule = false;

        if (GameManager.Instance.useVR)
        {
            VRManager.Instance.moveMultiplier = 1;
            VRManager.Instance.ApplySettings();
        }

        UIManager.Instance.PromptGameWindowFocus();
        // Destroy all goal objects if they exist
        foreach (var ex in exercises)
        {
            ex.Cleanup();
        }
        NewLevelIndicator nli = GetComponent<NewLevelIndicator>();
        if (nli)
        {
            nli.ShowHint();
        }
    }

    public void Hide()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        open = false;
    }

    /// <summary>
    /// This is called when the floating object in the world is clicked
    /// </summary>
    public virtual void Play()
    {
        GameManager.Instance.inModule = true;
        open = true;
        UIManager.Instance.UpdateModuleName(string.Format("Level {0} - Module {1} - Exercise {2}", lvl, ModuleNo, current + 1));
        if (!GameManager.Instance.useVR)
            TankController.Instance.DisableMovement();
        else
        {
            TankController.Instance.EnableMovement();
            VRManager.Instance.moveMultiplier = 0.5f;
            VRManager.Instance.ApplySettings();
        }
        transform.GetChild(0).gameObject.SetActive(true);
        foreach (var layoutGroup in this.GetComponentsInChildren<LayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
        if (playAudio)
            GetComponent<AudioSource>().Play();

        progressBar = GetComponentInChildren<QuantumTek.QuantumUI.QUI_Bar>();
        progressBar.SetFill((float)current / exercises.Count);

        //reached the module in time
        if (current == 0)
            ScoreCalculator.instance.EndActivity(1, 0);
        if (exercises.Count > 0)
        {
            RunFirstModule();
            //first module
            ScoreCalculator.instance.StartActivity(exercises[current].exerciseID);
        }
        else
        {
            StartCoroutine(Wrap());
        }
        
        if (GameManager.Instance.useVR)
            MoveVRCanvas();
    }

    protected virtual void MoveVRCanvas()
    {
        UIManager.Instance.MoveToPosition();
    }

    /// <summary>
    /// Override this to change how the first excercise is shown
    /// </summary>
    protected virtual void RunFirstModule()
    {
        exercises[current].Arrange();
        exercises[current].gameObject.SetActive(true);
    }

    protected virtual void ExitEarly()
    {
        Exit();
        gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);
        ModuleMapper modMap = GameObject.FindObjectOfType<ModuleMapper>();
        modMap.ResetModule(this);
    }

    protected virtual void Exit()
    {
        ScoreCalculator.instance.EndActivity(0, 0);
        TankController.Instance.EnableMovement();
        //gameObject.SetActive(false);
        foreach (var ex in exercises) ex.gameObject.SetActive(false);
        gameObject.SetActive(false);
        End();
        open = false;
    }

    public virtual void ShowSecurity()
    {
        SecurityCode.Instance.onSuccess = new UnityEngine.Events.UnityEvent();
        SecurityCode.Instance.onSuccess.AddListener(ExitEarly);
        SecurityCode.Instance.Show();
    }
}

