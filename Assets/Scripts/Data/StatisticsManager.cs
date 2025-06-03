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
    private int ciLevel;


    public void Initialize()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Destroy stats man");
            Destroy(gameObject);
            return;

        }
        _instance = this;

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

    public void OpenMenu()
    {
        StartCoroutine(OpenRoutine());
    }

    // TODO : fix null ref here if try to open without having done an exercise.
    private IEnumerator OpenRoutine()
    {
        List<SavePatientData.PatientDataEntry> allData = SavePatientData.Instance.PatientData;
        int lastExercise = allData.Count - 1; // TODO : magic number
        int lastModule = lastExercise / 7 + 1; // TODO : magic number
        lastExercise = lastExercise % 7; // TODO : magic number
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
        int sessionId = allData[0].sessionId;
        DrawExerciseGraph(sessionId);
        DrawAccuracyGraph(sessionId);

    }

    public void DrawExerciseGraph(int sessionId)
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
            y = patientData[i].time;

            DataPoint point = new DataPoint(x, y);
            data.Add(point);

            y = ciData[i].time;

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

        string title = "Module " + (numberOption + 1) + " Exercise Times (Session: " + sessionId + ")";
        timeBase.DrawGraph(cidata, "", "", "");
        timeGraph.DrawGraph(data, title, "Cog. Exercise Number", "Time in Seconds");
    }

    public void DrawAccuracyGraph(int sessionId)
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
            float x = patientData[i].exercise;
            float y = (float)patientData[i].successes / (patientData[i].successes + patientData[i].misses) * 100f;
            DataPoint point = new DataPoint(x, y);
            data.Add(point);
            y = (float)ciData[i].successes / (ciData[i].successes + ciData[i].misses) * 100f;
            point = new DataPoint(x, y);
            cidata.Add(point);
        }
        string title = "Module " + (numberOption + 1) + " Exercise Accuracy (Session: " + sessionId + ")";
        accuracyBase.DrawGraph(cidata, "", "", "");
        accuracyGraph.DrawGraph(data, title, "Cog. Exercise Number", "Percent Accuracy");
    }

    // TODO : do we need this graph anymore since we removed attempts?
    public void SpecificExerciseGraph()
    {
        ChooseDataRange();
        patientData = SavePatientData.Instance.PatientData;
        ciData = SavePatientData.Instance.CIData;

        if (numberOption > patientData.Count) return;

        List<DataPoint> timeData = new List<DataPoint>();
        List<DataPoint> ciTimes = new List<DataPoint>();
        List<DataPoint> accData = new List<DataPoint>();
        List<DataPoint> ciAcc = new List<DataPoint>();

        int x = 0;
        float y = patientData[numberOption].time;
        DataPoint timePoint = new DataPoint(x, y);
        y = ciData[numberOption].time;
        DataPoint baseTimePoint = new DataPoint(x, y);
        y = (float)patientData[numberOption].successes / (patientData[numberOption].successes + patientData[numberOption].misses) * 100f;
        DataPoint accPoint = new DataPoint(x, y);
        y = (float)ciData[numberOption].successes / (ciData[numberOption].successes + ciData[numberOption].misses) * 100f;
        DataPoint baseAccPoint = new DataPoint(x, y);

        timeData.Add(timePoint);
        ciTimes.Add(baseTimePoint);
        accData.Add(accPoint);
        ciAcc.Add(baseAccPoint);

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
        patientData = SavePatientData.Instance.PatientData;
        ciData = SavePatientData.Instance.CIData;

        int start = 0;
        int end = patientData.Count;

        if (lod == LevelOfDetail.Level)
        {
            start = numberOption * 7 * 5; // TODO : magic number
            end = start + 7 * 5; // TODO : magic number
        }
        else if (lod == LevelOfDetail.Module)
        {
            start = numberOption * 7; // TODO : magic number
            end = start + 7; // TODO : magic number
        }
        else if (lod == LevelOfDetail.Exercise)
        {
            start = numberOption; // TODO : magic number
            end = numberOption + 1; // TODO : magic number
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
        patientData = data;
        ciData = cidata;

        // Fill out the patient summary fields
        int exNo = start; // TODO : magic number
        int modNo = exNo / 7 + 1; // TODO : magic number
        exNo = exNo % 7; // TODO : magic number

        fromModule1.text = modNo.ToString();
        fromExercise1.text = exNo.ToString();

        exNo = end - 1; // TODO : magic number
        modNo = exNo / 7 + 1; // TODO : magic number
        exNo = exNo % 7; // TODO : magic number

        fromModule2.text = modNo.ToString();
        fromExercise2.text = exNo.ToString();

        float timeAvg = SavePatientData.Instance.AverageTime(patientData);
        meanTimeText.text = timeAvg.ToString("0.00") + " seconds";

        float accAvg = SavePatientData.Instance.AverageAccuracy(patientData);
        meanAccuracyText.text = accAvg.ToString("0.00") + "%";
    }

    // TODO : remove magic numbers
    private void OptionsUpdated()
    {
        numberOption = dataFilterOptions.numberDropdown.value;
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
        }

        dataFilterOptions.UpdateCIDropdown();
        dataFilterOptions.UpdateDropdownOptions(range, numberOption);
        OpenMenu();
    }
}
