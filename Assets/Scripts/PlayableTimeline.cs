using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableTimeline : MonoBehaviour
{
    [SerializeField]PlayableDirector director;
    // Start is called before the first frame update
    void Start()
    {
        director.stopped += Director_stopped;
    }

    private void Director_stopped(PlayableDirector obj)
    {
        GetComponent<TankController>().enabled = true;
        //ScoreCalculator.instance.StartActivity(0);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
