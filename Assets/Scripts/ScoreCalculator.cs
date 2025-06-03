using System;
using UnityEngine;
using UnityEngine.Events;

public class IntEvent : UnityEvent<int> { }
public class ScoreCalculator
{
    static ScoreCalculator _instance;
    public static ScoreCalculator instance
    {
        get
        {
            if (_instance == null)
                _instance = new ScoreCalculator();
            return _instance;
        }
    }
    public bool exercising;
    private int totalScore;
    public double totalDuration;
    private float start;
    public IntEvent activityStart;
    public UnityEvent activityEnd;
    private int currentExerciseNo;
    private DateTimeOffset startTimeStamp;


    public void StartActivity(int exerciseID)
    {
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
        exercising = true;
        start = Time.time;
        startTimeStamp = DateTimeOffset.Now;
        Debug.Log("Starting exercise for exerciseID: " + exerciseID);
        currentExerciseNo = exerciseID;
        activityStart?.Invoke(currentExerciseNo);
    }

    public void EndActivity(int successes, int misses)
    {
        exercising = false;
        //module numbers start from 1
        if (start < 0)
        {
            Debug.LogWarning("EndActivity was called without Starting it");
            return;
        }
        var duration = Time.time - start;
        totalDuration += duration;
        start = Time.time;

        // Score exercise based on comparison to expected CI times
        totalScore += GetScore(duration, successes, misses, currentExerciseNo);
        if (SavePatientData.Instance)
        {
            string exerciseName = GameManager.Instance.CreateExerciseNameFromExerciseId(currentExerciseNo);
            SavePatientData.Instance.SaveEntry(currentExerciseNo, exerciseName, startTimeStamp, duration, successes, misses);
        }
        activityEnd?.Invoke();
    }

    // Gets the point value for an exercise. Points correlate to stars for a particular exercise
    public int GetScore(float duration, int successes, int misses, int idx)
    {
        int score = 0;
        if (SavePatientData.Instance == null) return 0;

        // TODO : fix
        /* SavePatientData.PatientDataEntry cientry = SavePatientData.Instance.GetCIEntry(idx);
        if (cientry.attempt == null) return 0;
        float compareTime = 10f;
        float compareAcc = 50f;
        float playerAcc = 100f * successes / (successes + misses);
        if (cientry.attempt[0].time != 0)  // TODO : NULL REF at end of game
        {
            for (int i = 0; i < 5; i++)
            {
                int ciIdx = impairment + i;
                ciIdx = Mathf.Clamp(ciIdx, 0, cientry.attempt.Length);
                if (ciIdx < cientry.attempt.Length)
                {
                    compareTime = cientry.attempt[ciIdx].time;
                    compareAcc = 100f * cientry.attempt[ciIdx].successes / (cientry.attempt[ciIdx].successes + cientry.attempt[ciIdx].misses);
                    compareAcc -= 5f;   // tolerance
                }
                else
                {
                    compareTime += 5f;
                    compareAcc -= 5f;
                }
                //Debug.Log("Duration: " + duration + " compared to ci[" + (impairment + 1) + "] = " + compareTime);

                if (duration < compareTime && playerAcc > compareAcc)
                {
                    Debug.Log("Time: " + Mathf.Round(duration) + "<" + Mathf.Round(compareTime) + " Acc: " + Mathf.Round(playerAcc) + ">" + Mathf.Round(compareAcc) + " gives score of: " + (5 - i));
                    score = 5 - i;
                    break;
                }
            }
        } */
        return score;
    }

    // Calculates the stars awareded based on the points earned on an entire module.
    public int GetStars(int numModules = 7) // TODO : magic number
    {
        int stars = Mathf.CeilToInt((float)totalScore / numModules);
        if (stars > 5) stars = 5;

        totalDuration = 0;
        totalScore = 0;

        return stars;
    }
}
