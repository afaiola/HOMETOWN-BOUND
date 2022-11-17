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

    [DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);

    public static SavePatientData Instance { get { return _instance; } }
    private static SavePatientData _instance;

    public PatientDataObject initialCIData;
    private string patientDataFile;
    private string ciDataFile;
    private List<PatientDataEntry> patientData;
    public List<PatientDataEntry> ciData;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // set up the save file and format it to hold entries for all exercises
    public void Initialize()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        patientDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "patient_data.csv";
        ciDataFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "ci_data.csv";
        if (File.Exists(patientDataFile))
            File.Delete(patientDataFile);
        // try fetching patientDataFile.
        StorageManager.Instance.downloadStatusEvent = new UnityEngine.Events.UnityEvent<bool>();
        StorageManager.Instance.downloadStatusEvent.AddListener(FileDownload);
        StorageManager.Instance.StartCSVDownload(patientDataFile);
    }

    private void FileDownload(bool status)
    {
        Debug.Log("File download success? " + status);
        patientData = new List<PatientDataEntry>();
        if (!CreateFile(patientDataFile, out patientData))
            UploadPatientData();

        ciData = new List<PatientDataEntry>();
        CreateFile(ciDataFile, out ciData, 15);
    }

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

    // return true if file existed prior to creation
    private bool CreateFile(string path, out List<PatientDataEntry> data, int numAttempts=3)
    {
        data = new List<PatientDataEntry>();
        try
        {
            if (File.Exists(path))
            {
                Debug.Log("load existing " + Path.GetFileName(path));
                data = Load(path);
                return true;
            }
            else
            {
                // Create new data
                if (path == ciDataFile)
                {
                    Debug.Log("create new: " + Path.GetFileName(ciDataFile));
                    data = InitializeCIData();
                }
                else
                {
                    /*
                    // Dont fill file with empty data because it misrepresents how many modules the user has completed in dashboard
                    // 3 levels * 5 modules * (6 exercises + walking to module)     
                    for (int i = 0; i < 3 * 5 * 7; i++)
                    {
                        PatientDataEntry entry = new PatientDataEntry(i, numAttempts);
                        data.Add(entry);
                    }*/
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

                if (path == patientDataFile)
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
        }

        // There isn't enough data, so the file must be bad. Get a new one
        if (data.Count < 15)
        {
            Debug.Log("deleting " + path);
            File.Delete(path);
            CreateFile(path, out data, path == ciDataFile ? 15 : 3);
            return data;
        }
        return data;
    }

    private void SaveFile()
    {
        string header = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", "Exercise", "Time 1", "Successes 1", "Misses 1", "Time 2", "Successes 2", "Misses 2", "Time 3", "Successes 3", "Misses 3");

        try
        {
            using (var w = new StreamWriter(patientDataFile))
            {
                if (patientData != null)
                {
                    //Debug.Log("saving: " + patientDataFile);
                    //File.WriteAllText(patientDataFile, header);
                    w.WriteLine(header);
                    w.Flush();
                    for (int i = 0; i < patientData.Count; i++)
                    {
                        PatientDataEntry entry = patientData[i];
                        string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", entry.exercise, entry.attempts[0].time, entry.attempts[0].successes, entry.attempts[0].misses, entry.attempts[1].time, entry.attempts[1].successes, entry.attempts[1].misses, entry.attempts[2].time, entry.attempts[2].successes, entry.attempts[2].misses);
                        //File.AppendAllText(patientDataFile, line);
                        w.WriteLine(line);
                        w.Flush();
                    }
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
                    //Debug.Log("saving: " + ciDataFile);
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
                }
            }
            catch (Exception e)
            {
                Debug.Log("fail write to CI file: " + e.Message);
            }
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("syncing files");
            SyncFiles();
        }

        // TODO: Upload to firebase
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
                        break;
                    }
                }

                if (!newEntry)
                {
                    // overwrite some row
                    patientData[i].attempts[2].time = time;
                    patientData[i].attempts[2].successes = successes;
                    patientData[i].attempts[2].misses = misses;
                }
                break;
            }
        }
        if (!found)
        {
            // add new row
            PatientDataEntry entry = new PatientDataEntry(exerciseNum);
            entry.attempts[0].time = time;
            entry.attempts[0].successes = successes;
            entry.attempts[0].misses = misses;
            patientData.Add(entry);
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

    public List<PatientDataEntry> GetPatientData()
    {
        return patientData;
    }

    public List<PatientDataEntry> GetCiData()
    {
        return ciData;
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
    public float AverageTime(List<PatientDataEntry> data, int attempt=-1)
    {
        float sum = 0;
        int count = 0;
        foreach (var entry in data)
        {
            for (int i = 0; i < 3; i++)
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
    public float AverageAccuracy(List<PatientDataEntry> data, int attempt=-1)
    {
        float sum = 0;
        int count = 0;
        foreach (var entry in data)
        {
            for (int i = 0; i < 3; i++)
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

    private static void PlatformSafeMessage(string message)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WindowAlert(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

}
