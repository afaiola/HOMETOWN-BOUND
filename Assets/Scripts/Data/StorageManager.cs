using System.Collections;
using UnityEngine;
using Firebase.Storage;
using System.IO;
using System;
using UnityEngine.Events;

/// <summary>
/// Downloads and uploads patient pictures and csv files
/// </summary>
public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance { get => _instance; }
    private static StorageManager _instance;


    [NonSerialized] public UnityEvent<bool> downloadStatusEvent = new UnityEvent<bool>();
    public PlayerContent[] playerContents, dropdownContents;
    public PlayerContent portraitContent;
    public UnityEvent contentDownloadedEvent = new UnityEvent();


    private string contentDir;
    private int totalFiles, filesDownloaded;


    protected void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    public void StartContentDownload()
    {
        contentDir = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}usercontent{Path.DirectorySeparatorChar}{Profiler.Instance.currentUser.username}";
        if (!Directory.Exists(contentDir)) Directory.CreateDirectory(contentDir);

        StartCoroutine(DownloadContent());
    }

    private IEnumerator DownloadFile(string firebasePath, string localPath)
    {
        var storage = FirebaseStorage.DefaultInstance;

        byte[] file_contents = new byte[1];
        bool fileExists = File.Exists(localPath);
        if (fileExists) file_contents = File.ReadAllBytes(localPath);
        if (!fileExists || file_contents.Length < 16)
        {
            var content_ref = storage.GetReference(firebasePath);
            //var download_task = content_ref.GetFileAsync(localPath);    // may want to check if file already exists
            const long maxDownloadSize = 1024 * 1024 * 16;      // 16MB (form limit is 10MB)
            var download_task = content_ref.GetBytesAsync(maxDownloadSize);    // may want to check if file already exists
            float timeout = 3f;
            while (!download_task.IsCompleted && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            //yield return new WaitUntil(() => download_task.IsCompleted);

            if (timeout < 0)
            {
                Debug.LogWarning("download timeout : " + firebasePath);
            }
            else if (download_task.Exception != null)
            {
                Debug.LogWarning($"ERROR downloading {firebasePath}: {download_task.Exception}");
                File.WriteAllBytes(localPath, file_contents);   // creates file to prevent errors later
            }
            else
            {
                Debug.Log($"Downloaded file {firebasePath} size: {download_task.Result.Length}");
                if (download_task.Result.Length > 0)
                {
                    File.WriteAllBytes(localPath, download_task.Result);
                }
                file_contents = download_task.Result;
            }
        }

        yield return file_contents;
    }

    private IEnumerator DownloadContent()
    {
        filesDownloaded = 0;
        totalFiles = playerContents.Length + 1;
        var storage = FirebaseStorage.DefaultInstance;

        // waiting for csv to download
        while (SavePatientData.Instance == null)
            yield return new WaitForEndOfFrame();

        string firebasePath = $"/data/{Profiler.Instance.currentUser.username}/content_map";
        string localPath = $"{contentDir}{Path.DirectorySeparatorChar}content_map.json";

        CoroutineWithData cd = new CoroutineWithData(this, DownloadFile(firebasePath, localPath));
        yield return cd.coroutine;

        // delete the json string
        // parse the json
        // save the json

        filesDownloaded++;

        foreach (var content in playerContents)
        {
            firebasePath = $"/data/{Profiler.Instance.currentUser.username}/{content.pictureName}";
            localPath = $"{contentDir}{Path.DirectorySeparatorChar}{content.pictureName}.png";
            // potentially don't download if file already exists
            content.remotePath = firebasePath;
            content.localPath = localPath;
            cd = new CoroutineWithData(this, DownloadFile(firebasePath, localPath));
            yield return cd.coroutine;
            byte[] result = (byte[])cd.result;
            if (result == null) continue;
            //Debug.Log($"{content.pictureName} size: {result.Length}");
            if (result.Length < 16)
            {
                content.valid = false;
                continue;
            }
            content.valid = true;
            content.image.LoadImage(result);
            content.image.Apply();
            filesDownloaded++;
            float downloadPercent = 10000f * (float)filesDownloaded / (float)totalFiles / 100f;
            //Debug.Log($"Download progress: {downloadPercent}%");
        }
        contentDownloadedEvent.Invoke();
    }

    // download patient images
    private IEnumerator DownloadPictures()
    {
        //var storage = FirebaseStorage.DefaultInstance;

        foreach (var content in playerContents)
        {
            string firebasePath = $"/data/{Profiler.Instance.currentUser.username}/{content.pictureName}";
            string localPath = $"{contentDir}/{content.pictureName}.png";
            // potentially don't download if file already exists
            content.remotePath = firebasePath;
            content.localPath = localPath;
            CoroutineWithData cd = new CoroutineWithData(this, DownloadFile(firebasePath, localPath));
            yield return cd.coroutine;
            content.image.LoadImage((byte[])cd.result);
            content.image.Apply();
        }
        contentDownloadedEvent.Invoke();
    }

    public void StartCSVUpload(string filename)
    {
        StartCoroutine(UploadCSV(filename));
    }

    private IEnumerator UploadCSV(string filename)
    {
        // 1. make storage reference (to retreive file later) 
        var storage = FirebaseStorage.DefaultInstance;
        var csv_reference = storage.GetReference($"/data/{Profiler.Instance.currentUser.username}/patient_data.csv");
        // 2. convert to bytes to allow internet to handle it
        var bytes = ReadByteArrayFromFile(filename);
        // 3. add metadata

        var uploadTask = csv_reference.PutBytesAsync(bytes);
        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if (uploadTask.Exception != null)
        {
            Debug.LogError($"Failed to upload because {uploadTask.Exception}");
            yield break;
        }
        else
        {
            Debug.Log($"{Profiler.Instance.currentUser.username}/{filename} uploaded with status witout exception");
        }
    }

    private byte[] ReadByteArrayFromFile(string fileName)
    {
        byte[] buff;
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        using (BinaryReader br = new BinaryReader(fs))
        {
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
        }
        return buff;
    }

    public void StartCSVDownload(string path)
    {
        StartCoroutine(DownloadCSV(path));
    }

    private IEnumerator DownloadCSV(string localPath)
    {
        string firebasePath = $"/data/{Profiler.Instance.currentUser.username}/patient_data.csv";
        if (File.Exists(localPath))
        {
            File.Delete(localPath);
        }
        CoroutineWithData cd = new CoroutineWithData(this, DownloadFile(firebasePath, localPath));
        yield return cd.coroutine;
        bool status = false;
        var data = (byte[])cd.result;
        if (data.Length > 1)
        {
            status = WriteByteArrayToFile(data, localPath);
        }
        Debug.Log($"csv download complete. fb path: {firebasePath} write status? {status} file bytes: {((byte[])cd.result).Length}");
        downloadStatusEvent.Invoke(status);
    }

    public bool WriteByteArrayToFile(byte[] buff, string fileName)
    {
        try
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(buff);
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
            return false;
        }
    }

    public float GetDownloadProgress()
    {
        float progress = (float)filesDownloaded / totalFiles;
        if (float.IsNaN(progress)) progress = 0;
        return progress;
    }
}

