using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Plays distracting audio around the player while doing modules.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get { return _instance; } }
    private static SoundManager _instance;

    public DistractingAudioData data;
    [SerializeField] public AudioMixerGroup distractionMixer;
    public float spawnRadius;       // How far away the audio distractions will spawn 
    private GameObject player;
    private bool done;

    // Start is called before the first frame update
    void Start()
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

        player = GameObject.FindGameObjectWithTag("Player");
        ScoreCalculator.instance.activityStart = new IntEvent();
        ScoreCalculator.instance.activityStart.AddListener(StartDistracting);
        ScoreCalculator.instance.activityEnd = new UnityEngine.Events.UnityEvent();
        ScoreCalculator.instance.activityEnd.AddListener(StopDistracting);
    }

    public void StartDistracting(int exercise)
    {
        int module = Mathf.FloorToInt((exercise) / 7);
        int ex = exercise - 1 - module * 7;
        if (ex < 0) return;
        //Debug.Log("ID: " + exercise + " mod: " + module +" idx: " + ex);
        if (module >= data.modules.Length) return;
        DistractingAudioData.ExerciseDistraction currDistract = data.modules[module].exercises[ex];
        AudioClip[] clips = data.modules[module].clips;
        StartCoroutine(Distract(clips, currDistract));
    }

    public void StopDistracting()
    {
        done = true;
        StopAllCoroutines();
    }

    private IEnumerator Distract(AudioClip[] clips, DistractingAudioData.ExerciseDistraction distraction)
    {

        if (clips.Length > 0 && distraction.distractionAmount > 0)
        {
            done = false;

            float frequency = 10f / distraction.distractionAmount;
            DistractionObject lastSpawned;

            while (!done)
            {
                yield return new WaitForSeconds(frequency);

                float randX = Random.Range(1f, spawnRadius);
                float randY = 1f;//Random.Range(1f, 1f);
                float randZ = Random.Range(1f, spawnRadius);
                Vector3 location = new Vector3(randX, randY, randZ);
                GameObject distractionObj = new GameObject();
                distractionObj.transform.position = TankController.Instance.transform.position + location;
                AudioSource source = distractionObj.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = distractionMixer;
                source.maxDistance = spawnRadius;
                lastSpawned = distractionObj.AddComponent<DistractionObject>();
                lastSpawned.Initialize(clips[Random.Range(0, clips.Length)]);
            }
        }
    }
}
