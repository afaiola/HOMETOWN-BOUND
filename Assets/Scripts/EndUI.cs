using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EndUI : MonoBehaviour
{
    public RectTransform[] stars;
    public Text totalStarsText;
    public Button statsButton, menuButton, exitButton;

    private int[] levelScores;
    private int[] levelCount;


    // TODO : Remove magic numbers. Cannot assume each module has 7 exercises.
    public void GetScores()
    {
        statsButton.onClick = new Button.ButtonClickedEvent();
        statsButton.onClick.AddListener(StatisticsManager.Instance.OpenMenu);

        levelScores = new int[3];
        levelCount = new int[3];
        int[] exerciseScores = new int[7];
        int m = 0;
        int totalStars = 0;
        int modCt = 0;
        int activeAttempt = SavePatientData.Instance.CurrentAttempt;
        List<SavePatientData.PatientDataEntry> data = SavePatientData.Instance.PatientData;
        for (int i = 0; i < data.Count; i++)
        {
            SavePatientData.PatientDataEntry.PatientAttempt entry = data[i].attempts[activeAttempt];
            int score = ScoreCalculator.instance.GetScore(entry.time, entry.successes, entry.misses, i);

            exerciseScores[m] = score;
            m++;
            if (m >= 7)     // 7 exercises in a modules
            {
                m = 0;
                int mStars = exerciseScores.Sum() / exerciseScores.Length;
                int level = Mathf.FloorToInt(i / 35);
                levelScores[level] += mStars;
                levelCount[level]++;
                totalStars += Mathf.CeilToInt(mStars);
                exerciseScores = new int[7];
                modCt++;
            }
        }

        // last module is not a multiple of 7.
        int houseStars = exerciseScores.Sum() / 4;
        levelScores[2] += houseStars;
        levelCount[2]++;
        totalStars += Mathf.CeilToInt(houseStars);
        modCt++;

        //int totalStars = Mathf.FloorToInt(levelScores.Sum() / 14);
        for (int i = 0; i < 3; i++)
        {
            float avgStar = (float)levelScores[i] / levelCount[i];
            if (float.IsNaN(avgStar)) avgStar = 0;
            stars[i].localScale = new Vector3(avgStar / 5f, 1);
        }
        // num modules = 14
        totalStarsText.text = totalStars + "/" + 14 * 5;
    }

    public void Close()
    {
        TankController.Instance.EnableMovement();
        gameObject.SetActive(false);
    }

}
