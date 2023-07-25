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

    private AudioSource audio;
    [SerializeField] AudioClip clip_missedModule, clip_in, clip_goBack;
    [SerializeField] AudioClip[] numbers, levels;

    private bool playingWarning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;
        audio = GetComponent<AudioSource>();
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (!playingWarning)
            StartCoroutine(AudioSequence(warningClips));
    }

    private IEnumerator AudioSequence(List<AudioClip> clips)
    {
        playingWarning = true;
        for (int i = 0; i < clips.Count; i++)
        {
            audio.clip = clips[i];
            audio.Play();
            yield return new WaitForSeconds(clips[i].length);
        }
        playingWarning = false;
    }
}
