using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using UnityEngine.UI;
using TMPro;

public class ChannelComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer().Compare((int)((TVExercise.TVChannel)x).id, (int)((TVExercise.TVChannel)y).id));
    }
}
// Flips through channels, showing different programs
// lets say 5 channels
// control remote with channel up and channel down button
// up increases channel number by 7, down decreases by 5
// channels cycle if id is out of range.
// maybe show a TV guide so player knows what channel they need to go to.

// ISSUE: Getting footage of favorite movie/tv program is going to be pretty much impossible on a per user basis
// we should get short loops of genres of programs (sports game, soap opera, sit com, news, nature documentary, cooking show)
// exercise can be to navigate to a particular channel

// ISSUE: videos are large files, might take up too much room for WebGL
public class TVExercise : Exercise
{
    // TV - in world space
    public Text channelText;   
    public VideoPlayer tv;

    // TV GUIDE - on exercise canvas
    public Transform tvGuide;
    public GameObject guideEntryPrefab;

    // REMOTE - on exercise canvas
    public Button upButton, downButton, enterButton;

    [System.Serializable]
    public struct TVChannel
    {
        public string name;
        public int id;
        public VideoClip clip;

        public TVChannel(string n, int i, VideoClip c)
        {
            name = n;
            id = i;
            clip = c;
        }
    }

    public TVChannel[] channels;
    [System.NonSerialized] public string[] originalChannelsOrder = { "News", "Sports", "Nature", "History", "Sitcom", "Classics", "Horror", "Cooking" };
    public VideoClip tvStatic;
    private int channelRange = 99;
    public int goalChannel; // index in the channels list
    private int currChannel;
    private float changeDelay = 0.25f;
    private float timeSinceChannelChange;
    private bool useClips;  // when using mp4s

    // Start is called before the first frame update
    void Start()
    {
        upButton.onClick.AddListener(IncreaseChannel);
        downButton.onClick.AddListener(DecreaseChannel);
        enterButton.onClick.AddListener(EnterSelection);
        //useClips = SystemInfo.operatingSystem.Contains("Mac");
        useClips = true;
        if (useClips) tv.clip = tvStatic;
        else tv.url = System.IO.Path.Combine(Application.streamingAssetsPath, "TV-static.ogv");
        tv.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnValidate()
    {
        // do nothing
    }

    public override void Arrange()
    {
        gameObject.SetActive(true);
        channelRange = channels.Length*2;
        List<int> usedChannels = new List<int>();
        // assign random channel ids, then sort channels list 
        for (int i = 0; i < channels.Length; i++)
        {
            int randChannel = UnityEngine.Random.Range(1, channelRange);
            while (usedChannels.Contains(randChannel))
            {
                randChannel = UnityEngine.Random.Range(1, channelRange);
            }
            usedChannels.Add(randChannel);
            channels[i] = new TVChannel(channels[i].name, randChannel, channels[i].clip);
        }
        Array.Sort(channels, new ChannelComparer());

        for (int i = 0; i < channels.Length; i++)
        {
            GameObject entry = Instantiate(guideEntryPrefab, tvGuide);
            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = channels[i].id.ToString();
            entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = channels[i].name.ToString();
        }

        for (int i = 0; i < channels.Length; i++)
        {
            if (originalChannelsOrder[goalChannel] == channels[i].name)
            {
                goalChannel = i;
                break;
            }
        }
        currChannel = 0;
        ChangeChannel();
    }

    public override bool CheckSuccess
    {
        get { return currChannel == channels[goalChannel].id; }
    }

    public void EnterSelection()
    {
        if (CheckSuccess)
        {
            _correctCount++;
            StartCoroutine(Wait(2, Success));
        }
        else
        {
            _incorrectCount++;
        }
    }

    public void IncreaseChannel()
    {
        if ((Time.time - timeSinceChannelChange) > changeDelay)
        {
            currChannel += 1;
            if (currChannel > channelRange) currChannel -= channelRange;
            ChangeChannel();
        }
    }

    public void DecreaseChannel()
    {
        if ((Time.time - timeSinceChannelChange) > changeDelay)
        {
            currChannel -= 1;
            if (currChannel <= 0) currChannel += channelRange;
            ChangeChannel();
        }
    }

    // cycling through channels too quickly may have caused crash
    private void ChangeChannel()
    {
        timeSinceChannelChange = Time.time;
        channelText.text = currChannel.ToString("00");
        //tv.Stop();
        string url = System.IO.Path.Combine(Application.streamingAssetsPath, "tv-static.ogv");
        if (useClips) tv.clip = tvStatic;

        for (int i = 0; i < channels.Length; i++)
        {
            if (channels[i].id == currChannel)
            {
                if (useClips) tv.clip = channels[i].clip;
                else url = System.IO.Path.Combine(Application.streamingAssetsPath, channels[i].name + ".ogv");
                
            }
        }
        if (!useClips)
        {
            try { tv.url = url; }
            catch { Debug.Log("Invalid URL: " + url); }
        }
        StartCoroutine(PlayVideo());
    }

    private IEnumerator PlayVideo()
    {
        Debug.Log("preparing clip: " + tv.url);
        tv.Prepare();
        while (!tv.isPrepared)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("TV clip prepared");
        tv.Play();
    }
}
