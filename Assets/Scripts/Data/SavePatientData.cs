using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

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

        public PatientDataEntry(int e, int numAttempts = 3)
        {
            exercise = e;
            attempts = new PatientAttempt[numAttempts];
        }
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
    private int recentExercise;
    private int currentAttempt = 0;
    private int maxAttempts = 3;
    private int totalExercises = 0;


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
        this.totalExercises = totalExercises;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        patientDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "patient_data.csv";
        ciDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "ci_data.csv";
        FileDownload(newGame);
    }

    private void FileDownload(bool newGame)
    {
        patientData = new List<PatientDataEntry>();
        if (!CreateFile(patientDataFile, out patientData, newGame))
        {
            UploadPatientData();
        }
        ciData = new List<PatientDataEntry>();
        CreateFile(ciDataFile, out ciData, true, 15);
    }

    // TODO : make sure our CIData matches up with levels, modules, exercises
    private List<PatientDataEntry> InitializeCIData()
    {
        List<PatientDataEntry> data = new List<PatientDataEntry>();
        for (int i = 0; i < initialCIData.data.Count; i++)
        {
            PatientDataEntry entry = new PatientDataEntry(i, 15);
            for (int level = 0; level < 15; level++)
            {
                entry.attempts[level].time = Mathf.Round(initialCIData.data[i].attempts[0].time * (1f + (float)level / 10f));
                entry.attempts[level].successes = initialCIData.data[i].attempts[0].successes;
                entry.attempts[level].misses = Mathf.CeilToInt(initialCIData.data[i].attempts[0].successes * (0.05f + (float)level / 15f)); // when level is 15, minimum acc = 50. Meaning misses = successes
            }
            data.Add(entry);
        }
        return data;
    }

    // TODO : are we using the returned bool?, return true if file existed prior to creation
    private bool CreateFile(string path, out List<PatientDataEntry> data, bool newGame, int numAttempts = 3)
    {
        data = new List<PatientDataEntry>();
        try
        {
            if (!newGame && File.Exists(path))
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
                else
                {
                    // Dont fill file with empty data because it misrepresents how many modules the user has completed in dashboard
                    for (int i = 0; i < totalExercises; i++)
                    {
                        PatientDataEntry entry = new PatientDataEntry(i, numAttempts);
                        data.Add(entry);
                    }
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
                /*
                 * These columns are checked for proper types
                 */
                bool validRow = true;

                if (path == patientDataFile) // TODO : change if allowing more than three attempts
                {
                    validRow = int.TryParse(parts[0], out entry.exercise) &&
                                   float.TryParse(parts[1], out entry.attempts[0].time) &&
                                   int.TryParse(parts[2], out entry.attempts[0].successes) &&
                                   int.TryParse(parts[3], out entry.attempts[0].misses) &&
                                   float.TryParse(parts[4], out entry.attempts[1].time) &&
                                   int.TryParse(parts[5], out entry.attempts[1].successes) &&
                                   int.TryParse(parts[6], out entry.attempts[1].misses) &&
                                   float.TryParse(parts[7], out entry.attempts[2].time) &&
                                   int.TryParse(parts[8], out entry.attempts[2].successes) &&
                                   int.TryParse(parts[9], out entry.attempts[2].misses);
                }
                else if (path == ciDataFile)
                {
                    entry = new PatientDataEntry(index, 15);
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
                    //Debug.Log("ex: " + entry.exercise + " \n\tt0: " + entry.attempts[0].time + " \tm0: " + entry.attempts[0].misses + " \n\tt1: " + entry.attempts[1].time + " \tm1: " + entry.attempts[1].misses + " \n\tt2: " + entry.attempts[2].time + " \tm2: " + entry.attempts[2].misses);
                }
            }
            readFile.Close();
        }

        // There isn't enough data, so the file must be bad. Get a new one
        if (data.Count < 2)
        {
            //Debug.Log("deleting " + path);
            File.Delete(path);
            CreateFile(path, out data, true, path == ciDataFile ? 15 : 3);
            return data;
        }
        return data;
    }

    private void SaveFile()
    {
        //string header = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", "Exercise", "Time 1", "Successes 1", "Misses 1", "Time 2", "Successes 2", "Misses 2", "Time 3", "Successes 3", "Misses 3");
        string header = "Exercise";
        //header = "";
        for (int i = 0; i < maxAttempts; i++)
        {
            header += $",Time {i + 1},Successes {i + 1},Misses {i + 1}";
        }
        try
        {
            using (var w = new StreamWriter(patientDataFile))
            {
                if (patientData != null)
                {
                    //File.WriteAllText(patientDataFile, header);
                    w.WriteLine(header);
                    w.Flush();
                    for (int i = 0; i < patientData.Count; i++)
                    {
                        PatientDataEntry entry = patientData[i];
                        //string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", entry.exercise, entry.attempts[0].time, entry.attempts[0].successes, entry.attempts[0].misses, entry.attempts[1].time, entry.attempts[1].successes, entry.attempts[1].misses, entry.attempts[2].time, entry.attempts[2].successes, entry.attempts[2].misses);
                        string line = entry.exercise.ToString();
                        for (int a = 0; a < entry.attempts.Length; a++)
                        {
                            line += $",{entry.attempts[a].time},{entry.attempts[a].successes},{entry.attempts[a].misses}";
                        }
                        //File.AppendAllText(patientDataFile, line);
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

        if (ciData != null)
        {
            try
            {
                using (var w = new StreamWriter(ciDataFile))
                {
                    //File.WriteAllText(ciDataFile, header);
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
                        //line += "\n";
                        //File.AppendAllText(ciDataFile, line);
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
                bool newEntry = false;
                for (int j = 0; j < patientData[i].attempts.Length; j++)
                {
                    if (patientData[i].attempts[j].time == 0)
                    {
                        patientData[i].attempts[j].time = time;
                        patientData[i].attempts[j].successes = successes;
                        patientData[i].attempts[j].misses = misses;
                        newEntry = true;
                        currentAttempt = j;
                        break;
                    }
                }
                if (!newEntry)
                {
                    // overwrite row 3
                    patientData[i].attempts[2].time = time;
                    patientData[i].attempts[2].successes = successes;
                    patientData[i].attempts[2].misses = misses;
                    currentAttempt = 2; // TODO : probs don't need to reset here but test, then replace 2 above with this
                }
                break;
            }
        }
        if (!found)
        {
            // add all new rows
            int dataCt = patientData.Count;
            for (int i = dataCt; i < exerciseNum; i++)
            {
                patientData.Add(new PatientDataEntry(i));
            }
            PatientDataEntry entry = new PatientDataEntry(exerciseNum);
            entry.attempts[0].time = time;
            entry.attempts[0].successes = successes;
            entry.attempts[0].misses = misses;
            patientData.Add(entry);
        }
        recentExercise = exerciseNum;
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
        Debug.Log("uploading patient data");
        StorageManager.Instance.StartCSVUpload(patientDataFile);
    }

    // returns most recently played module this session
    public int RecentModule()
    {
        return recentExercise / 7;
    }

    public int LastModulePlayed(int attempt = -1) //
    {
        Debug.Log("getting last mod played");
        int lastExercise = LastExercisePlayed(attempt);
        // TODO : make function for this, cant just divide by 7
        int mod = Mathf.FloorToInt(lastExercise / 7); // TODO : dividing by 7 bc (6 exercises + walking to module) // 3 levels * 5 modules * (6 exercises + walking to module) 
        Debug.Log($"last played M{mod} E{lastExercise}");
        return mod;
    }

    public int LastExercisePlayed(int attempt = -1)
    {
        int[] lastPlayed = new int[maxAttempts];  // last exercise played on each attempt
        if (patientData == null)
        {
            patientData = Load(patientDataFile);
        }
        for (int i = 0; i < patientData.Count; i++)
        {
            for (int j = 0; j < patientData[i].attempts.Length; j++)
            {
                if (patientData[i].attempts[j].time != 0) // TODO : better way to detect this?
                {
                    lastPlayed[j] = patientData[i].exercise + 1; // +1 accounts for leaving the hospital
                }
            }
        }
        if (attempt != -1 && attempt < maxAttempts)
        {
            return lastPlayed[attempt];
        }

        for (int i = 0; i < lastPlayed.Length; i++)
        {
            Debug.Log($"last played on attempt {i} = {lastPlayed[i]}");

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
