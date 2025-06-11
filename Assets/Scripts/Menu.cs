using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu Instance { get { return _instance; } }
    private static Menu _instance;

    public Text moduleName;
    public GameObject gotoMenu;
    public Toggle gotoButton;
    [SerializeField]
    private AudioClip clip_missedModule;
    [SerializeField]
    private AudioClip clip_in;
    [SerializeField]
    private AudioClip clip_goBack;
    [SerializeField]
    private AudioClip[] numbers;
    [SerializeField]
    private AudioClip[] levels;
    [SerializeField]
    private AudioSource audioSource;


    private bool playingWarning;


    public void Initialize()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Destroy menu");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void UpdateModuleName(string str)
    {
        moduleName.text = str;
    }

    public void MissedModuleWarning(int modNo, Levels level)
    {
        List<AudioClip> warningClips = new List<AudioClip>();
        warningClips.Add(clip_missedModule);
        warningClips.Add(numbers[modNo - 1]);
        warningClips.Add(clip_in);
        warningClips.Add(levels[(int)level]);
        warningClips.Add(clip_goBack);
        if (!playingWarning) { StartCoroutine(AudioSequence(warningClips)); }
    }


    private IEnumerator AudioSequence(List<AudioClip> clips)
    {
        playingWarning = true;
        for (int i = 0; i < clips.Count; i++)
        {
            audioSource.clip = clips[i];
            audioSource.Play();
            yield return new WaitForSeconds(clips[i].length);
        }
        playingWarning = false;
    }
}
