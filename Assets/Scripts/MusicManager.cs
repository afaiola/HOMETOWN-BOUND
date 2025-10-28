using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get { return _instance; } }
    private static MusicManager _instance;
    public AudioSource musicSource, ambienceSource;

    [Header("MUSIC")]
    public AudioClip cityMusic;
    public AudioClip homelandMusic;
    public AudioClip houseMusic;

    [Header("AMBIENCE")]
    public AudioClip cityAmbience;
    public AudioClip suburbAmbience;

    private MusicZone[] musicZones;
    private AmbienceZone[] ambienceZones;


    protected void Start()
    {
        Initialize();
    }


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

        musicZones = GameObject.FindObjectsOfType<MusicZone>();
        ambienceZones = GameObject.FindObjectsOfType<AmbienceZone>();

        foreach (var zone in musicZones)
        {
            if (zone.level == Levels.DOWN)
            {
                zone.EnterEvent = new UnityEngine.Events.UnityEvent();
                zone.EnterEvent.AddListener(PlayCityMusic);
            }
            else if (zone.level == Levels.HOME)
            {
                zone.EnterEvent = new UnityEngine.Events.UnityEvent();
                zone.EnterEvent.AddListener(PlayHomelandMusic);
            }
        }

        foreach (var zone in ambienceZones)
        {
            if (zone.level == Levels.DOWN)
            {
                zone.EnterEvent = new UnityEngine.Events.UnityEvent();
                zone.EnterEvent.AddListener(PlayCityAmbience);
            }
            else if (zone.level == Levels.HOME)
            {
                zone.EnterEvent = new UnityEngine.Events.UnityEvent();
                zone.EnterEvent.AddListener(PlaySuburbAmbience);
            }
        }

        musicSource.clip = null;
        ambienceSource.clip = null;
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        ambienceSource.Stop();
    }

    public void PlayCityMusic()
    {
        PlayNewClip(musicSource, cityMusic, 0.4f, 5f);
    }

    public void PlayHomelandMusic()
    {
        PlayNewClip(musicSource, homelandMusic, 1f, 2f);
    }

    public void PlayCityAmbience()
    {
        PlayNewClip(ambienceSource, cityAmbience, 0.4f);
    }

    public void PlaySuburbAmbience()
    {
        PlayNewClip(ambienceSource, suburbAmbience, 1f);
    }

    public void PlayHouseMusic()
    {
        PlayNewClip(musicSource, houseMusic, 0.1f);
    }


    private void PlayNewClip(AudioSource source, AudioClip clip, float volume, float delay = 0f)
    {
        if (clip == null)
        {
            source.Stop();
            return;
        }
        if (source.clip == clip) { return; }
        source.Stop();
        source.volume = volume;
        source.clip = clip;
        source.PlayDelayed(delay);
    }

}
