using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
using Firebase.Auth;
using Firebase.Firestore;

public class UserAccountManager : MonoBehaviour
{
    public UserInputPanel signInPanel, registerPanel;
    public GameObject loadingBar, newGamePanel;
    public Emailer emailPanel;
    public UnityEvent loginSuccessEvent = new UnityEvent();

    // user option keys
    private string k_email = "email";
    private string k_pass = "password";

    // dict keys
    private string k_user = "firstname";
    private string k_ci = "ci";
    private string k_skin = "skin";
    private string k_login_ct = "timesLoggedIn";
    private string k_play_time = "playTime";
    private string k_start_date = "creationDate";
    private string k_last_login = "lastLogin";
    private string k_days_played = "consecutiveDays";

    // firestore keys
    private string k_user_collection = "users";

    private string secret = "AaBb1!2@";

    private string adminUser = "hometown.service859@gmail.com";
    private string adminPass = "hometown B0UND$";


    // Start is called before the first frame update
    void Start()
    {
        signInPanel.submitAction = new UnityEvent<Dictionary<string, string>>();
        signInPanel.submitAction.AddListener(StartSignIn);

        registerPanel.submitAction = new UnityEvent<Dictionary<string, string>>();
        registerPanel.submitAction.AddListener(StartRegistration);
        registerPanel.gameObject.SetActive(false);

        Load loader = GameObject.FindObjectOfType<Load>();
        loginSuccessEvent.AddListener(loader.LoadGame);
        loadingBar.SetActive(false);

        emailPanel.gameObject.SetActive(false);
        newGamePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartSignIn(Dictionary<string, string> userOptions)
    {
        Debug.Log("signing in");
        StartCoroutine(SignIn(userOptions));
    }

    // Using WaitUntils rather than the task.ContinueWith becuase that was causing the function to terminate 
    // after the task completed which caused chained coroutines to not execute.
    private IEnumerator SignIn(Dictionary<string, string> userOptions)
    {
        // TODO: protect from retreiving invalid keys
        string email = userOptions[k_email];    // check if this is a username instead
        string username = userOptions[k_email];   // retreive the username from firestore if not provided 
        string password = userOptions[k_pass];

        DateTime legacySigninDate = new DateTime(2024, 3, 9);
        
        var db = FirebaseFirestore.DefaultInstance;
        if (!email.Contains("@"))
        {
            // username no longer used for login purposes, fail login if email does not contain @
            signInPanel.SubmitFail("Your Email must be a valid email address");
        }
        else
        {
            // temporairly sign in for this?
            var auth = FirebaseAuth.DefaultInstance;
            var task = auth.SignInWithEmailAndPasswordAsync(adminUser, adminPass);
            yield return new WaitUntil(() => task.IsCompleted);
            Dictionary<string, object> user = null;

            if (task.Exception != null)
            {
                Debug.LogWarning("could not log into admin account to get user list");
                password += secret;
                // assume we are good?
            }
            else
            {
                var userRef = db.Collection(k_user_collection).Document(email);
                var snapshotTask = userRef.GetSnapshotAsync();
                yield return new WaitUntil(() => snapshotTask.IsCompleted);

                user = snapshotTask.Result.ToDictionary();
                if (user != null)
                {
                    DateTime createDate = DateTime.Now;
                    if (user.ContainsKey(k_start_date)) createDate = DateTime.Parse(user[k_start_date].ToString());
                    if (createDate > legacySigninDate)    // yeah no date needs to be from the account itself
                        password += secret;
                }
            }
            auth.SignOut();

            //var auth = FirebaseAuth.DefaultInstance;
            task = auth.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            }

            if (task.Exception == null)
            {
                

                if (user != null)
                {
                    string uname = "";
                    int ci = 0;
                    int skin = 0;
                    int login_ct = 0;
                    double playTime = 0;
                    DateTime startDate = DateTime.Today;
                    DateTime lastLogin = DateTime.Today;
                    int consecDays = 0;

                    if (user.ContainsKey(k_user)) uname = user[k_user].ToString();
                    if (user.ContainsKey(k_ci)) ci = int.Parse(user[k_ci].ToString());
                    if (user.ContainsKey(k_skin)) skin = int.Parse(user[k_skin].ToString());
                    if (user.ContainsKey(k_login_ct)) login_ct = int.Parse(user[k_login_ct].ToString());
                    if (user.ContainsKey(k_play_time)) playTime = double.Parse(user[k_play_time].ToString());
                    if (user.ContainsKey(k_start_date)) startDate = DateTime.Parse(user[k_start_date].ToString());
                    if (user.ContainsKey(k_last_login)) lastLogin = DateTime.Parse(user[k_last_login].ToString());
                    if (user.ContainsKey(k_days_played)) login_ct = int.Parse(user[k_days_played].ToString());

                    Profiler.Instance.UserSignedIn(email, ci, skin, login_ct, playTime, startDate, lastLogin, consecDays, uname);
                    if (login_ct <= 1)
                    {
                        signInPanel.gameObject.SetActive(false);
                        registerPanel.gameObject.SetActive(false);
                        loadingBar.SetActive(true);
                        loginSuccessEvent.Invoke();
                    }
                    else
                    {
                        newGamePanel.SetActive(true);
                    }

                }
            }
            else
            {
                signInPanel.SubmitFail(task.Exception.Message);
                // need to find where the useful content is in the exception
            }
        }
    }

    public void StartRegistration(Dictionary<string, string> userOptions)
    {
        StartCoroutine(Register(userOptions));
    }

    private IEnumerator Register(Dictionary<string, string> userOptions)
    {
        // TODO: protect from retreiving invalid keys
        string email = userOptions[k_email];
        string password = userOptions[k_pass] + secret;
        string username = userOptions[k_user];
        int ci = 0;
        int skin = int.Parse(userOptions[k_skin]);

        string message = "";
        bool success = true;
        var db = FirebaseFirestore.DefaultInstance;
        // Query for the new email already existing
        var usersRef = db.Collection(k_user_collection);
        Query query = usersRef.WhereEqualTo(k_email, email);
        var queryTask = query.GetSnapshotAsync();
        yield return new WaitUntil(() => queryTask.IsCompleted);

        foreach (var doc in queryTask.Result.Documents)
        {
            Dictionary<string, object> user = doc.ToDictionary();
            if (user.ContainsValue(email))
            {
                message = $"email already in use!";
                success = false;
            }
        }

        if (success)
        {
            var auth = FirebaseAuth.DefaultInstance;
            var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                success = false;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                success = false;
            }

            if (!success)
            {
                //message = task.Exception.InnerException.Message;//message = task.Exception.Flatten().InnerExceptions[0].Message;
                //message = task.Exception.Message;//message = task.Exception.Flatten().InnerExceptions[0].Message;
                message = task.Exception.InnerExceptions[task.Exception.InnerExceptions.Count - 1].Message;
            }
        }

        if (success)
        {
            // Add user to firestore
            DateTime date = System.DateTime.Today;
            string shortDate = date.ToString("d");
            var userRef = db.Collection(k_user_collection).Document(email);
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { k_email, email },
                { k_ci,  ci },
                { k_skin, skin },
                { k_login_ct, 1 },
                { k_play_time, 0 },
                { k_start_date, shortDate },
                { k_last_login, shortDate },
                { k_days_played, 0 },
                { k_user, username }
            };
            userRef.SetAsync(user);
            Profiler.Instance.UserSignedIn(email, ci, skin, 0, 0, date, date, 0, username);

            //loginSuccessEvent.Invoke();
            registerPanel.gameObject.SetActive(false);
            emailPanel.gameObject.SetActive(true);
            emailPanel.SendEmail(email);
            //signInPanel.gameObject.SetActive(true);
        }
        else
        {
            registerPanel.SubmitFail(message);
            // TODO: Update the proper helper messages on fail ie: username unavailable
        }
    }
    
    public void StartNewGame(bool newGame)
    {
        Profiler.Instance.currentUser.newGame = newGame;
        signInPanel.gameObject.SetActive(false);
        registerPanel.gameObject.SetActive(false);
        loadingBar.SetActive(true);
        loginSuccessEvent.Invoke();
    }
}
