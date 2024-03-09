using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase.Auth;
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

    [System.NonSerialized] public UnityEvent<bool> downloadStatusEvent = new UnityEvent<bool>();

    public PlayerContent[] playerContents, dropdownContents;
    public PlayerContent portraitContent;
    public UnityEvent contentDownloadedEvent = new UnityEvent();

    private string contentDir;
    private int totalFiles, filesDownloaded;
    
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
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
            yield return new WaitUntil(() => download_task.IsCompleted);

            if (download_task.Exception != null)
            {
                Debug.LogWarning($"ERROR downloading {firebasePath}: {download_task.Exception}");
                File.WriteAllBytes(localPath, file_contents);   // creates file to prevent errors later
            }
            else
            {
                Debug.Log($"Downloaded file {firebasePath} size: {download_task.Result.Length}");
                File.WriteAllBytes(localPath, download_task.Result);
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
            Debug.Log($"{Profiler.Instance.currentUser.username}/{filename} uploaded with status witout exception");
    }

    /// <summary>
    /// function originally written by Surya Prakash https://suryaprakash.net/2011/09/17/convert-csv-file-data-into-byte-array/
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private byte[] ReadByteArrayFromFile(string fileName)
    {
        // declare byte array variable
        byte[] buff = null;

        // open the file with read access by declaring FileStream object
        FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

        // pass FileStream object to BinaryReader
        BinaryReader br = new BinaryReader(fs);

        // get the file length
        long numBytes = new FileInfo(fileName).Length;

        // convert binary reader object data to ByteArray, using following statement
        buff = br.ReadBytes((int)numBytes);

        // return byte array object
        return buff;
    }

    public void StartCSVDownload(string path)
    {
        StartCoroutine(DownloadCSV(path));
    }

    private IEnumerator DownloadCSV(string localPath)
    {
        var storage = FirebaseStorage.DefaultInstance;
        //var csv_reference = storage.GetReference($"/data/{Profiler.Instance.currentUser.username}/patient_data.csv");
        string firebasePath = $"/data/{Profiler.Instance.currentUser.username}/patient_data.csv";

        // dowload the file
        /*
        const long maxDownloadSize = 1024 * 1024;
        var downloadTask = csv_reference.GetBytesAsync(maxDownloadSize);
        yield return new WaitUntil(() => downloadTask.IsCompleted);

        bool status = true;
        if (downloadTask.Exception != null)
        {
            Debug.LogError($"CSV Download failed: {downloadTask.Exception}");
            status = false;
        }
        else
        {
            // save csv locally
            byte[] fileContents = downloadTask.Result;
            status = writeByteArrayToFile(fileContents, filename);
        }*/
        if (File.Exists(localPath))
            File.Delete(localPath);
        CoroutineWithData cd = new CoroutineWithData(this, DownloadFile(firebasePath, localPath));
        yield return cd.coroutine;
        bool status = writeByteArrayToFile((byte[])cd.result, localPath);
        Debug.Log($"csv download complete. fb path: {firebasePath} write status? {status} file bytes: {((byte[])cd.result).Length}");
        downloadStatusEvent.Invoke(status);
    }

    /// <summary>
    /// function originally written by Surya Prakash https://suryaprakash.net/2011/09/17/convert-csv-file-data-into-byte-array/
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool writeByteArrayToFile(byte[] buff, string fileName)
    {
        // define bool flag to identify success or failure of operation
        bool response = false;

        try
        {
            // define filestream object for new filename with readwrite properties
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);

            // define binary write object from file stream object
            BinaryWriter bw = new BinaryWriter(fs);

            // write byte array content using BinaryWriter object
            bw.Write(buff);

            // close binary writer object
            bw.Close();

            // set status flag as true
            response = true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
            // set status as false, if operation fails at any point
            response = false;

        }

        return response;
    }

    public float GetDownloadProgress()
    {
        float progress = (float)filesDownloaded / totalFiles;
        if (float.IsNaN(progress)) progress = 0;
        return progress;
    }
}

