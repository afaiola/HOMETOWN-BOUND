using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public struct DataPoint
{
    public float x;
    public float y;

    public DataPoint(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
}

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get { return _instance; } }
    private static StatisticsManager _instance;

    public GameObject canvas;
    // Text objects in the Top section
    public Text patientText, patientCIText, dateOfRecentDataText, timeOfRecentDataText;
    // Text objects in the summary
    public DataFilterOptions dataFilterOptions;
    public Text patientIDText, dayStartedText, timesLoggedOnText, moduleCompletedText, exerciseCompletedText, fromModule1, fromExercise1, meanTimeText, fromModule2, fromExercise2, meanAccuracyText;
    
    // TODO: Add text objects in patient projections

    private List<SavePatientData.PatientDataEntry> patientData, ciData;

    public LineGraph timeGraph, timeBase;
    public Graph accuracyGraph, accuracyBase;

    private enum LevelOfDetail { Level, Module, Exercise };
    private LevelOfDetail lod;
    private int numberOption;
    private int activeAttempt;
    private int ciLevel;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        dataFilterOptions.optionsUpdatedEvent = new UnityEvent();
        dataFilterOptions.optionsUpdatedEvent.AddListener(OptionsUpdated);

        if (Profiler.Instance)
        {
            ciLevel = Profiler.Instance.currentUser.ciLevel;
            patientText.text = Profiler.Instance.currentUser.username;
            patientCIText.text = ciLevel.ToString();
        }

        dateOfRecentDataText.text = System.DateTime.Now.ToString("MM/dd/yyyy");
        timeOfRecentDataText.text = "  " + System.DateTime.Now.ToString("hh:mm");

        canvas.SetActive(false);

        timeGraph.Initialize();
        timeBase.Initialize();
        accuracyGraph.Initialize();
        accuracyBase.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu()
    {
        StartCoroutine(OpenRoutine());
    }
    private IEnumerator OpenRoutine()
    {
        // TODO: load player data and:
        // 1. see when they started the game
        // 2. see how many times they logged on

        List<SavePatientData.PatientDataEntry> allData = SavePatientData.Instance.GetPatientData();
        int lastExercise = allData.Count-1;
        int lastModule = lastExercise / 7 + 1;
        lastExercise = lastExercise % 7;
        moduleCompletedText.text = lastModule.ToString();
        exerciseCompletedText.text = lastExercise.ToString();
        dayStartedText.text = Profiler.Instance.currentUser.startDate.ToString("d");
        timesLoggedOnText.text = Profiler.Instance.currentUser.timesLoggedIn.ToString();
        dateOfRecentDataText.text = Profiler.Instance.currentUser.lastLoginDate.ToString("d");


        // refresh all the graphs
        canvas.SetActive(true);
        // wait a frame to allow canvas to auto size
        yield return new WaitForEndOfFrame();

        dataFilterOptions.UpdateCIDropdown();
        DrawExerciseGraph();
        DrawAccuracyGraph();

    }

    public void DrawExerciseGraph()
    {
        if (lod == LevelOfDetail.Exercise)
        {
            SpecificExerciseGraph();
            return;
        }

        ChooseDataRange();

        List<DataPoint> data = new List<DataPoint>();
        List<DataPoint> cidata = new List<DataPoint>();

        for (int i = 0; i < patientData.Count; i++)
        {
            float x = 0;
            float y = 0;
            x = patientData[i].exercise;
            y = patientData[i].attempts[activeAttempt].time;

            DataPoint point = new DataPoint(x, y);
            data.Add(point);

            y = ciData[i].attempts[ciLevel].time;

            point = new DataPoint(x, y);
            cidata.Add(point);
        }

        timeBase.forceMax = false;
        timeGraph.forceMax = false;

        timeBase.DrawGraph(cidata, "", "", "");
        timeGraph.DrawGraph(data, "", "Cog. Exercise Number", "Time in Seconds");
    
        // scale the graphs to one another

        if (timeGraph.YMax > timeBase.YMax)
        {
            timeBase.forceMax = true;
            timeBase.forcedYMax = timeGraph.YMax;
        }
        else
        {
            timeGraph.forceMax = true;
            timeGraph.forcedYMax = timeBase.YMax;
        }

        string title = "Module " + (numberOption + 1) + " Exercise Times (Session: " + (activeAttempt + 1) + ")";
        timeBase.DrawGraph(cidata, "", "", "");
        timeGraph.DrawGraph(data, title, "Cog. Exercise Number", "Time in Seconds");

    }

    public void DrawAccuracyGraph()
    {
        if (lod == LevelOfDetail.Exercise)
        {
            SpecificExerciseGraph();
            return;
        }

        ChooseDataRange();

        List<DataPoint> data = new List<DataPoint>();
        List<DataPoint> cidata = new List<DataPoint>();

        for (int i = 0; i < patientData.Count; i++)
        {
            float x = 0;
            float y = 0;
            x = patientData[i].exercise;
            y = (float)patientData[i].attempts[activeAttempt].successes / (patientData[i].attempts[activeAttempt].successes + patientData[i].attempts[activeAttempt].misses) * 100f;

            DataPoint point = new DataPoint(x, y);
            data.Add(point);

            y = (float)ciData[i].attempts[ciLevel].successes / (ciData[i].attempts[ciLevel].successes + ciData[i].attempts[ciLevel].misses) * 100f;
            point = new DataPoint(x, y);

            cidata.Add(point);
        }

        string title = "Module " + (numberOption + 1) + " Exercise Accuracy (Session: " + (activeAttempt + 1) + ")";
        accuracyBase.DrawGraph(cidata, "", "", "");
        accuracyGraph.DrawGraph(data, title, "Cog. Exercise Number", "Percent Accuracy");

    }

    public void SpecificExerciseGraph()
    {
        ChooseDataRange();
        patientData = SavePatientData.Instance.GetPatientData();
        ciData = SavePatientData.Instance.GetCiData();

        if (numberOption > patientData.Count) return;

        List<DataPoint> timeData = new List<DataPoint>();
        List<DataPoint> ciTimes = new List<DataPoint>();
        List<DataPoint> accData = new List<DataPoint>();
        List<DataPoint> ciAcc = new List<DataPoint>();


        for (int i = 0; i < 3; i++)
        {
            int x = i;
            float y = patientData[numberOption].attempts[i].time;
            DataPoint timePoint = new DataPoint(x, y);

            y = ciData[numberOption].attempts[ciLevel].time;
            DataPoint baseTimePoint = new DataPoint(x, y);

            y = (float)patientData[numberOption].attempts[i].successes / (patientData[numberOption].attempts[i].successes + patientData[numberOption].attempts[i].misses) * 100f;
            DataPoint accPoint = new DataPoint(x, y);

            y = (float)ciData[numberOption].attempts[ciLevel].successes / (ciData[numberOption].attempts[ciLevel].successes + ciData[numberOption].attempts[ciLevel].misses) * 100f;
            DataPoint baseAccPoint = new DataPoint(x, y);

            timeData.Add(timePoint);
            ciTimes.Add(baseTimePoint);
            accData.Add(accPoint);
            ciAcc.Add(baseAccPoint);
        }

        timeBase.forceMax = false;
        timeGraph.forceMax = false;

        timeBase.DrawGraph(ciTimes, "", "", "");
        timeGraph.DrawGraph(timeData, "", "Cog. Exercise Number", "Time in Seconds");

        // scale the graphs to one another
        if (timeGraph.YMax > timeBase.YMax)
        {
            timeBase.forceMax = true;
            timeBase.forcedYMax = timeGraph.YMax;
        }
        else
        {
            timeGraph.forceMax = true;
            timeGraph.forcedYMax = timeBase.YMax;
        }

        timeBase.DrawGraph(ciTimes, "", "", "");
        accuracyBase.DrawGraph(ciAcc, "", "", "");

        timeGraph.DrawGraph(timeData, "Comparison of Exersice #" + (numberOption + 1) + " Times Over Several Play Sessions", "Play Session", "Time in Seconds", true); 
        accuracyGraph.DrawGraph(accData, "Comparison of Exersice #" + (numberOption + 1) + " Accuracy Over Several Play Sessions", "Play Session", "Percent Accuracy", true);
    }

    public void ChooseDataRange()
    {
        patientData = SavePatientData.Instance.GetPatientData();
        ciData = SavePatientData.Instance.GetCiData();

        int start = 0;
        int end = patientData.Count;

        if (lod == LevelOfDetail.Level)
        {
            start = numberOption * 7 * 5;
            end = start + 7 * 5;
        }
        else if (lod == LevelOfDetail.Module)
        {
            start = numberOption * 7;
            end = start + 7;
        }
        else if (lod == LevelOfDetail.Exercise)
        {
            start = numberOption;
            end = numberOption + 1;
        }

        List<SavePatientData.PatientDataEntry> data = new List<SavePatientData.PatientDataEntry>();
        List<SavePatientData.PatientDataEntry> cidata = new List<SavePatientData.PatientDataEntry>();
        for (int i = start; i < end; i++)
        {
            if (i < patientData.Count)
            {
                data.Add(patientData[i]);
                cidata.Add(ciData[i]);
            }
        }
        //Debug.Log("range: " + start + ", " + end);
        patientData = data;
        ciData = cidata;

        // Fill out the patient summary fields
        int exNo = start;
        int modNo = exNo / 7 + 1;
        exNo = exNo  % 7;

        fromModule1.text = modNo.ToString();
        fromExercise1.text = exNo.ToString();

        exNo = end-1;
        modNo = exNo / 7 + 1;
        exNo = exNo % 7;

        fromModule2.text = modNo.ToString();
        fromExercise2.text = exNo.ToString();

        float timeAvg = SavePatientData.Instance.AverageTime(patientData, activeAttempt);
        meanTimeText.text = timeAvg.ToString("0.00") + " seconds";

        float accAvg = SavePatientData.Instance.AverageAccuracy(patientData, activeAttempt);
        meanAccuracyText.text = accAvg.ToString("0.00") + "%";

    }

    private void OptionsUpdated()
    {
        numberOption = dataFilterOptions.numberDropdown.value;
        activeAttempt = dataFilterOptions.attemptDropdown.value;
        ciLevel = dataFilterOptions.ciDropdown.value;

        LevelOfDetail prevLOD = lod;
        lod = (LevelOfDetail)dataFilterOptions.lodDropdown.value;

        if (lod != prevLOD)
            numberOption = 0;

        int range = 0;

        if (lod == LevelOfDetail.Level)
        {
            range = 3;              // TODO: change to 3
            timeGraph.xTickCount = 5;
            accuracyGraph.xTickCount = 5;
        }
        else if (lod == LevelOfDetail.Module)
        {
            range = 14;             // TODO: Change to 15
            timeGraph.xTickCount = 7;
            accuracyGraph.xTickCount = 7;
        }
        else if (lod == LevelOfDetail.Exercise)
        {
            range = 3 * 5 * 7;      // TODO: Change to 7*5*3
            timeGraph.xTickCount = 3;
            accuracyGraph.xTickCount = 3;
            activeAttempt = -1;
        }

        dataFilterOptions.UpdateCIDropdown();
        dataFilterOptions.UpdateDropdownOptions(range, numberOption);
        OpenMenu();
    }

}
