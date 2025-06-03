using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SavePatientData : MonoBehaviour
{
    [Serializable]
    public struct PatientDataEntry
    {
        public int gameId;
        public int sessionId;
        public int exercise;
        public string exerciseName; // (Level.Module.Exercise)
        public DateTimeOffset timeStamp;
        public float time;
        public int misses;
        public int successes;
    }

    /*[DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);*/

    public static SavePatientData Instance { get { return _instance; } }
    private static SavePatientData _instance;
    public PatientDataObject initialCIData;
    private string patientDataFile;
    private string ciDataFile;
    private List<PatientDataEntry> patientData;
    public List<PatientDataEntry> ciData;
    private int currentGameId = 0;
    private int currentSessionId = 0;
    private int gameIdCount = 0;
    private int sessionIdCount = 0;
    private int lastExercisePlayed = 0;


    private const string patientDataHeader = "Game ID, Session ID, Exercise, Exercise Name, Time Completed, Success, Misses";


    public List<PatientDataEntry> PatientData { get => patientData; }
    public List<PatientDataEntry> CIData { get => ciData; }
    public int LastExercisePlayed { get => lastExercisePlayed; }


    // set up the save file and format it to hold entries for all exercises
    public void Initialize(bool newGame)
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        patientDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "patient_data.csv";
        ciDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "ci_data.csv";
        SetupFiles(newGame);
    }


    private void SetupFiles(bool newGame)
    {
        patientData = new List<PatientDataEntry>();
        if (!SetupFile(patientDataFile, out patientData, newGame))
        {
            UploadPatientData();
        }
        ciData = new List<PatientDataEntry>();
        // TODO : fix
        // CreateFile(ciDataFile, out ciData, true, 15);
    }

    // TODO : make sure our CIData matches up with levels, modules, exercises
    // TODO : fix
    private List<PatientDataEntry> InitializeCIData()
    {
        List<PatientDataEntry> data = new List<PatientDataEntry>();
        /* for (int i = 0; i < initialCIData.data.Count; i++)
        {
            PatientDataEntry entry = new PatientDataEntry(i, 15);
            for (int level = 0; level < 15; level++)
            {
                entry.attempts[level].time = Mathf.Round(initialCIData.data[i].attempts[0].time * (1f + (float)level / 10f));
                entry.attempts[level].successes = initialCIData.data[i].attempts[0].successes;
                entry.attempts[level].misses = Mathf.CeilToInt(initialCIData.data[i].attempts[0].successes * (0.05f + (float)level / 15f)); // when level is 15, minimum acc = 50. Meaning misses = successes
            }
            data.Add(entry);
        } */
        return data;
    }

    // return true if file existed prior to creation
    // TODO : remove 'path' if just using to load patient file
    private bool SetupFile(string path, out List<PatientDataEntry> data, bool newGame)
    {
        data = new List<PatientDataEntry>();
        if (!newGame && File.Exists(path))
        {
            Debug.Log("Went !newGame && File.Exists(path)");
            data = LoadFile(path);
            gameIdCount = data[^1].gameId;
            currentGameId = gameIdCount;
            sessionIdCount = data[^1].sessionId;
            currentSessionId = GetSessionId();
            lastExercisePlayed = 0;
            return true;
        }
        else if (newGame && File.Exists(path))
        {
            Debug.Log("Went newGame && File.Exists(path)");
            data = LoadFile(path);
            SetNextGameIdAndLastExercisePlayed();
            sessionIdCount = 0;
            currentSessionId = 0;
        }
        else // TODO : test, have to create new user to test this, delete csv on firebase, or stop download of csv from firebase
        {
            Debug.Log("Went CreateFile()");
            CreateFile();
            gameIdCount = 0;
            currentGameId = 0;
            sessionIdCount = 0;
            currentSessionId = 0;
            lastExercisePlayed = 0;
        }

        // if (!newGame && File.Exists(path))
        // {
        // data = LoadFile(path);
        // return true;
        // }
        // else
        // {
        // TODO : make seperate func for ciDataFile
        /* if (path == ciDataFile)
        {
            data = InitializeCIData();
        } */
        // CreateFile();
        // }
        return false;
    }

    private List<PatientDataEntry> LoadFile(string path)
    {
        List<PatientDataEntry> data = new List<PatientDataEntry>();
        using (var reader = new StreamReader(path))
        {
            string line;
            string[] parts;
            int index = 0;

            while ((line = reader.ReadLine()) != null)
            {
                parts = line.Split(',');
                index += 1;

                if (parts == null)
                {
                    break;
                }

                index += 1;
                // Skip first row which in this case is a header with column names
                if (index <= 1) continue;
                bool validRow = true;
                PatientDataEntry entry = new PatientDataEntry();
                if (path == patientDataFile)
                {
                    validRow = int.TryParse(parts[0], out entry.gameId) &&
                        int.TryParse(parts[1], out entry.sessionId) &&
                        int.TryParse(parts[2], out entry.exercise) &&
                        DateTimeOffset.TryParse(parts[4], out entry.timeStamp) &&
                        float.TryParse(parts[5], out entry.time) &&
                        int.TryParse(parts[6], out entry.successes) &&
                        int.TryParse(parts[7], out entry.misses);
                    entry.exerciseName = parts[3];
                }
                else if (path == ciDataFile)
                {
                    // TODO : fix
                    // Game ID, Session ID, Exercise, Exercise Name, Time Completed, Success, Misses
                    /* entry = new PatientDataEntry(index, 15);
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
                               int.TryParse(parts[45], out entry.attempts[14].misses); */
                }

                if (validRow)
                {
                    data.Add(entry);
                }
            }
            reader.Close();
        }

        // There isn't enough data, so the file must be bad. Get a new one.
        // TODO : do we need this?
        /* if (data.Count < 2)
        {
            File.Delete(path);
            CreateFile(path, out data, true, path == ciDataFile ? 15 : 3); // TODO : update so not passing in numAttempts
            return data;
        } */
        return data;
    }

    private void SetNextGameIdAndLastExercisePlayed()
    {
        using (var reader = new StreamReader(patientDataFile))
        {
            string line;
            string[] parts;
            int index = 0;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                parts = line.Split(',');
                index += 1;
                if (parts == null)
                {
                    break;
                }
                index += 1;
                // Skip first row which in this case is a header with column names
                if (index <= 1) continue;
                int.TryParse(parts[0], out gameIdCount);
                int.TryParse(parts[2], out lastExercisePlayed);
                currentGameId = GetGameId();
            }
            reader.Close();
        }
    }

    private void CreateFile()
    {
        using (var writer = new StreamWriter(patientDataFile))
        {
            Debug.Log("Create file.");
            writer.WriteLine(patientDataHeader);
        }
    }

    private void SaveEntry(PatientDataEntry entry)
    {
        Debug.Log("Exists: " + File.Exists(patientDataFile));
        Debug.Log("Is ReadOnly: " + new FileInfo(patientDataFile).IsReadOnly);
        using (var writer = new StreamWriter(patientDataFile, true))
        {
            string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", entry.gameId,
                entry.sessionId, entry.exercise, entry.exerciseName, entry.timeStamp, entry.time, entry.successes, entry.misses);
            writer.WriteLine(line);
        }

        // TODO : fix
        if (ciData != null)
        {
            /* try
            {
                using (var w = new StreamWriter(ciDataFile))
                {
                    w.WriteLine(header);
                    w.Flush();
                    for (int i = 0; i < ciData.Count; i++)
                    {
                        PatientDataEntry entry = ciData[i];
                        string line = entry.exercise.ToString();
                        for (int j = 0; j < entry.attempts.Length; j++)
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
            } */
        }
    }

    public void SaveEntry(int exerciseNumber, string exerciseName, DateTimeOffset timeStamp, float duration, int successes, int misses)
    {
        Debug.Log("Saving entry exerciseNo: " + exerciseNumber + " exerciseName: " + exerciseName);
        var entry = new PatientDataEntry();
        entry.gameId = currentGameId;
        entry.sessionId = sessionIdCount;
        entry.exercise = exerciseNumber;
        entry.exerciseName = exerciseName;
        entry.timeStamp = timeStamp;
        entry.time = duration;
        entry.successes = successes;
        entry.misses = misses;
        patientData.Add(entry);
        lastExercisePlayed = exerciseNumber;
        Debug.Log("lastExercisePlayed: " + exerciseNumber);
        SaveEntry(entry);
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


    public float AverageTime(List<PatientDataEntry> data)
    {
        float sum = 0;
        int count = 0;
        foreach (var entry in data)
        {
            sum += entry.time;
            count++;
        }

        return sum / count;
    }


    public float AverageAccuracy(List<PatientDataEntry> data)
    {
        float sum = 0;
        int count = 0;
        foreach (var entry in data)
        {
            if (entry.time > 0)
            {
                float acc = (float)entry.successes / (entry.successes + entry.misses) * 100f;
                if (!float.IsNaN(acc))
                {
                    sum += acc;
                    count++;
                }
            }
        }

        return sum / (float)count;
    }

    public void UploadPatientData()
    {
        StorageManager.Instance.StartCSVUpload(patientDataFile);
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

    private int GetGameId()
    {
        return ++gameIdCount;
    }

    private int GetSessionId()
    {
        return ++sessionIdCount;
    }

}
