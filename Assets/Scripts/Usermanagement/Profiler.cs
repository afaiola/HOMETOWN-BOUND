using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;

// TODO: consider using FirebaseUser to replace a lot of this
public class Profiler : MonoBehaviour
{
    [Serializable]
    public struct Profile
    {
        public string username;
        public int ciLevel;
        public int skin_id;
        public int timesLoggedIn;
        public double totalPlayTime;
        public DateTime startDate;
        public DateTime lastLoginDate;
        public int consecutiveDays;
        // TODO: possibly add a key to allow user to update the firebase

        public Profile(string _name, int _ci, int _skin, int login_ct, double _playTime, DateTime _start, DateTime _lastLogin, int _consecDays)
        {
            username = _name;
            ciLevel = _ci;
            skin_id = _skin;
            timesLoggedIn = login_ct;
            totalPlayTime = _playTime;
            startDate = _start;
            lastLoginDate = _lastLogin;
            consecutiveDays = _consecDays;
        }
    }

    [NonSerialized] public Profile currentUser;

    public static Profiler Instance { get => _instance; }
    private static Profiler _instance;

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
    }

    public void UserSignedIn(string _username, int _ci, int _skin, int login_ct, double playTime, DateTime startDate, DateTime lastLogin, int consecDays)
    {
        login_ct++;
        DateTime date = DateTime.Now;
        var timeDiff = date - lastLogin;
        if (timeDiff.TotalDays == 1)
            consecDays++;
        else if (timeDiff.TotalDays > 1)
            consecDays = 0;
        lastLogin = date;
        currentUser = new Profile(_username, _ci, _skin, login_ct, playTime, startDate, lastLogin, consecDays);
        Debug.Log($"{_username}: signed in");
    }

    public Profile GetActiveProfile()
    {
        return currentUser;
    }

    public void UpdateUserProfile()
    {
        var auth = FirebaseAuth.DefaultInstance;
        var db = FirebaseFirestore.DefaultInstance;
        var userRef = db.Collection("users").Document(auth.CurrentUser.Email);
        Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "username", currentUser.username },
                { "ci",  currentUser.ciLevel },
                { "skin", currentUser.skin_id },
                { "timesLoggedIn", currentUser.timesLoggedIn },
                { "playTime", currentUser.totalPlayTime },
                { "creationDate", currentUser.startDate.ToString("d") },
                { "lastLogin", currentUser.lastLoginDate.ToString("d") },
                { "consecutiveLogins", currentUser.consecutiveDays }
            };
        userRef.SetAsync(user);
    }
}
