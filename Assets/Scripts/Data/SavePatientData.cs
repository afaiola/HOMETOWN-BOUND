using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SavePatientData : MonoBehaviour
{
    [Serializable]
    public struct PatientDataEntry
    {
        [Serializable]
        public struct PatientAttempt
        {
            public float time;
            public int misses;
            public int successes;
        }

        public int exercise;
        public PatientAttempt[] attempts;

        public PatientDataEntry(int e, int numAttempts = 150)
        {
            exercise = e;
            attempts = new PatientAttempt[numAttempts];
        }
    }

    public static SavePatientData Instance { get { return _instance; } }
    private static SavePatientData _instance;
    public PatientDataObject initialCIData;
    private string patientDataFile;
    private string ciDataFile;
    private List<PatientDataEntry> patientData;
    public List<PatientDataEntry> ciData;
    private int currentAttempt = 0;
    private int maxAttempts = 150;
    private int totalExercises = 0;
    private bool newGame = false;


    private const int ciCognitiveLevels = 15;


    public List<PatientDataEntry> PatientData { get => patientData; }
    public List<PatientDataEntry> CIData { get => ciData; }
    public int CurrentAttempt { get => currentAttempt; }


    // set up the save file and format it to hold entries for all exercises
    public void Initialize(bool newGame, int totalExercises)
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        this.newGame = newGame;
        this.totalExercises = totalExercises;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        patientDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "patient_data.csv";
        ciDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "ci_data.csv";
        FileDownload();
    }

    private void FileDownload()
    {
        patientData = new List<PatientDataEntry>();
        if (!CreateFile(patientDataFile, out patientData))
        {
            UploadPatientData();
        }
        ciData = new List<PatientDataEntry>();
        CreateFile(ciDataFile, out ciData);
    }

    private List<PatientDataEntry> InitializeCIData()
    {
        List<PatientDataEntry> data = new List<PatientDataEntry>();
        for (int i = 0; i < initialCIData.data.Count; i++)
        {
            PatientDataEntry entry = new PatientDataEntry(i, ciCognitiveLevels);
            for (int level = 0; level < ciCognitiveLevels; level++)
            {
                entry.attempts[level].time = Mathf.Round(initialCIData.data[i].attempts[0].time * (1f + level / 10f));
                entry.attempts[level].successes = initialCIData.data[i].attempts[0].successes;
                entry.attempts[level].misses = Mathf.CeilToInt(initialCIData.data[i].attempts[0].successes * (0.05f + level / 15f)); // when level is 15, minimum acc = 50. Meaning misses = successes
            }
            data.Add(entry);
        }
        return data;
    }

    // return true if file existed prior to creation
    private bool CreateFile(string path, out List<PatientDataEntry> data)
    {
        data = new List<PatientDataEntry>();
        try
        {
            if (File.Exists(path))
            {
                data = Load(path);
                return true;
            }
            else
            {
                if (path == ciDataFile)
                {
                    data = InitializeCIData();
                }
                SaveFile();
            }
        }
        catch (Exception e)
        {
            PlatformSafeMessage("Failed to Save: " + e.Message);
        }
        return false;
    }

    // fill the data structure with the existing data
    // TODO: load from firebase
    private List<PatientDataEntry> Load(string path)
    {
        List<PatientDataEntry> data = new List<PatientDataEntry>();
        using (var readFile = new StreamReader(path))
        {
            string line;
            string[] parts;
            int index = 0;

            while ((line = readFile.ReadLine()) != null)
            {
                parts = line.Split(',');
                index += 1;
                PatientDataEntry entry = new PatientDataEntry(index);
                if (parts == null)
                {
                    break;
                }
                index += 1;
                // Skip first row which in this case is a header with column names
                if (index <= 1) continue;
                bool validRow = true;
                if (path == patientDataFile)
                {
                    validRow = int.TryParse(parts[0], out entry.exercise);
                    int partsIndex = 1;
                    for (int i = 0; i < maxAttempts; i++)
                    {
                        int secondIndex = partsIndex + 1;
                        int thirdIndex = partsIndex + 2;
                        validRow = float.TryParse(parts[partsIndex], out entry.attempts[i].time) &&
                                   int.TryParse(parts[secondIndex], out entry.attempts[i].successes) &&
                                   int.TryParse(parts[thirdIndex], out entry.attempts[i].misses);

                        if (!validRow)
                        {
                            break;
                        }
                        partsIndex = partsIndex + 3;
                    }
                }
                else if (path == ciDataFile)
                {
                    entry = new PatientDataEntry(index, ciCognitiveLevels);
                    validRow = int.TryParse(parts[0], out entry.exercise) &&
                               float.TryParse(parts[1], out entry.attempts[0].time) &&
                               int.TryParse(parts[2], out entry.attempts[0].successes) &&
                               int.TryParse(parts[3], out entry.attempts[0].misses) &&
                               float.TryParse(parts[4], out entry.attempts[1].time) &&
                               int.TryParse(parts[5], out entry.attempts[1].successes) &&
                               int.TryParse(parts[6], out entry.attempts[1].misses) &&
                               float.TryParse(parts[7], out entry.attempts[2].time) &&
                               int.TryParse(parts[8], out entry.attempts[2].successes) &&
                               int.TryParse(parts[9], out entry.attempts[2].misses) &&
                               float.TryParse(parts[10], out entry.attempts[3].time) &&
                               int.TryParse(parts[11], out entry.attempts[3].successes) &&
                               int.TryParse(parts[12], out entry.attempts[3].misses) &&
                               float.TryParse(parts[13], out entry.attempts[4].time) &&
                               int.TryParse(parts[14], out entry.attempts[4].successes) &&
                               int.TryParse(parts[15], out entry.attempts[4].misses) &&
                               float.TryParse(parts[16], out entry.attempts[5].time) &&
                               int.TryParse(parts[17], out entry.attempts[5].successes) &&
                               int.TryParse(parts[18], out entry.attempts[5].misses) &&
                               float.TryParse(parts[19], out entry.attempts[6].time) &&
                               int.TryParse(parts[20], out entry.attempts[6].successes) &&
                               int.TryParse(parts[21], out entry.attempts[6].misses) &&
                               float.TryParse(parts[22], out entry.attempts[7].time) &&
                               int.TryParse(parts[23], out entry.attempts[7].successes) &&
                               int.TryParse(parts[24], out entry.attempts[7].misses) &&
                               float.TryParse(parts[25], out entry.attempts[8].time) &&
                               int.TryParse(parts[26], out entry.attempts[8].successes) &&
                               int.TryParse(parts[27], out entry.attempts[8].misses) &&
                               float.TryParse(parts[28], out entry.attempts[9].time) &&
                               int.TryParse(parts[29], out entry.attempts[9].successes) &&
                               int.TryParse(parts[30], out entry.attempts[9].misses) &&
                               float.TryParse(parts[31], out entry.attempts[10].time) &&
                               int.TryParse(parts[32], out entry.attempts[10].successes) &&
                               int.TryParse(parts[33], out entry.attempts[10].misses) &&
                               float.TryParse(parts[34], out entry.attempts[11].time) &&
                               int.TryParse(parts[35], out entry.attempts[11].successes) &&
                               int.TryParse(parts[36], out entry.attempts[11].misses) &&
                               float.TryParse(parts[37], out entry.attempts[12].time) &&
                               int.TryParse(parts[38], out entry.attempts[12].successes) &&
                               int.TryParse(parts[39], out entry.attempts[12].misses) &&
                               float.TryParse(parts[40], out entry.attempts[13].time) &&
                               int.TryParse(parts[41], out entry.attempts[13].successes) &&
                               int.TryParse(parts[42], out entry.attempts[13].misses) &&
                               float.TryParse(parts[43], out entry.attempts[14].time) &&
                               int.TryParse(parts[44], out entry.attempts[14].successes) &&
                               int.TryParse(parts[45], out entry.attempts[14].misses);
                }

                if (validRow)
                {
                    data.Add(entry);
                }

            }
            if (patientData != null)
            {
                FindCurrentAttempt();
            }
            readFile.Close();
        }

        // There isn't enough data, so the file must be bad. Get a new one
        if (data.Count < 2)
        {
            File.Delete(path);
            CreateFile(path, out data);
            return data;
        }
        return data;
    }

    private void FindCurrentAttempt()
    {
        int firstEmptyAttempt = FindFirstEmptyAttempt();
        currentAttempt = Mathf.Clamp(newGame ? firstEmptyAttempt : firstEmptyAttempt - 1, 0, maxAttempts - 1);
    }

    private int FindFirstEmptyAttempt()
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            // Check each exercise is empty for current attempt
            bool isEmptyAttempt = true;
            for (int j = 0; j < patientData.Count; j++)
            {
                if (patientData[j].attempts[i].time != 0)
                {
                    isEmptyAttempt = false;
                    break;
                }
            }
            if (isEmptyAttempt) { return i; }
        }
        return maxAttempts - 1; // overwrite last attempt if all attempts all filled
    }

    private void SaveFile()
    {
        string patientDataHeader = "Exercise";
        for (int i = 0; i < maxAttempts; i++)
        {
            patientDataHeader += $",Time {i + 1},Successes {i + 1},Misses {i + 1}";
        }
        try
        {
            using (var w = new StreamWriter(patientDataFile))
            {
                if (patientData != null)
                {
                    w.WriteLine(patientDataHeader);
                    w.Flush();
                    for (int i = 0; i < patientData.Count; i++)
                    {
                        PatientDataEntry entry = patientData[i];
                        string line = entry.exercise.ToString();
                        for (int a = 0; a < maxAttempts; a++)
                        {
                            line += $",{entry.attempts[a].time},{entry.attempts[a].successes},{entry.attempts[a].misses}";
                        }
                        w.WriteLine(line);
                        w.Flush();
                    }
                    w.Close();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("fail write to patient file: " + e.Message);
        }

        string ciDataHeader = "Exercise";
        for (int i = 0; i < ciCognitiveLevels; i++)
        {
            ciDataHeader += $",Time {i + 1},Successes {i + 1},Misses {i + 1}";
        }
        if (ciData != null)
        {
            try
            {
                using (var w = new StreamWriter(ciDataFile))
                {
                    w.WriteLine(ciDataHeader);
                    w.Flush();
                    for (int i = 0; i < ciData.Count; i++)
                    {
                        PatientDataEntry entry = ciData[i];
                        string line = entry.exercise.ToString();
                        for (int j = 0; j < ciCognitiveLevels; j++)
                        {
                            line += "," + entry.attempts[j].time + "," + entry.attempts[j].successes + "," + entry.attempts[j].misses;
                        }
                        w.WriteLine(line);
                        w.Flush();
                    }
                    w.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log("fail write to CI file: " + e.Message);
            }
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //Debug.Log("syncing files");
            //SyncFiles();
        }
    }

    public void SaveEntry(int exerciseNum, float time, int successes, int misses)
    {
        time = Mathf.Round(time * 1000f) / 1000f;
        bool found = false;
        for (int i = 0; i < patientData.Count; i++)
        {
            if (patientData[i].exercise == exerciseNum)
            {
                found = true;
                for (int j = 0; j < maxAttempts; j++)
                {
                    patientData[i].attempts[currentAttempt].time = time;
                    patientData[i].attempts[currentAttempt].successes = successes;
                    patientData[i].attempts[currentAttempt].misses = misses;
                }
            }
        }

        SaveFile();
    }

    public PatientDataEntry GetCIEntry(int exerciseID)
    {
        PatientDataEntry entry = new PatientDataEntry();
        if (exerciseID < ciData.Count)
        {
            entry = ciData[exerciseID];
        }

        return entry;
    }

    public float GetPatientTimeAverage()
    {
        return AverageTime(patientData);
    }

    public float GetPatientAccuracyAverage()
    {
        return AverageAccuracy(patientData);
    }

    // give no value for parameter attempt to get average over all attempts
    public float AverageTime(List<PatientDataEntry> data, int attempt = -1)
    {
        float sum = 0;
        int count = 0;
        foreach (var entry in data)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                if (entry.attempts[i].time > 0 && (i == attempt || attempt == -1))
                {
                    sum += entry.attempts[i].time;
                    count++;
                }
            }
        }

        return sum / (float)count;
    }

    // give no value for parameter attempt to get average over all attempts
    public float AverageAccuracy(List<PatientDataEntry> data, int attempt = -1)
    {
        float sum = 0;
        int count = 0;
        foreach (var entry in data)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                if (entry.attempts[i].time > 0 && (i == attempt || attempt == -1))
                {
                    float acc = (float)entry.attempts[i].successes / (entry.attempts[i].successes + entry.attempts[i].misses) * 100f;
                    if (!float.IsNaN(acc))
                    {
                        sum += acc;
                        count++;
                    }
                }
            }

        }

        return sum / (float)count;
    }

    public void UploadPatientData()
    {
        StorageManager.Instance.StartCSVUpload(patientDataFile);
    }

    public int LastExercisePlayed()
    {
        int[] lastPlayed = new int[maxAttempts];  // last exercise played on each attempt
        if (patientData == null)
        {
            patientData = Load(patientDataFile);
        }
        for (int i = 0; i < patientData.Count; i++)
        {
            for (int j = 0; j < maxAttempts; j++)
            {
                if (patientData[i].attempts[j].time != 0)
                {
                    lastPlayed[j] = patientData[i].exercise + 1; // +1 accounts for leaving the hospital
                }
            }
        }
        if (currentAttempt != -1 && currentAttempt < maxAttempts)
        {
            return lastPlayed[currentAttempt];
        }
        for (int i = 0; i < lastPlayed.Length; i++)
        {
            if (lastPlayed[i] < totalExercises)
            {
                return lastPlayed[i];
            }
        }
        return 0;
    }


    private static void PlatformSafeMessage(string message)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //WindowAlert(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

}
