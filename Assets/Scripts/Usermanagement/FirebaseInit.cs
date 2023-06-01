using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using UnityEngine.Events;

public class FirebaseInit : MonoBehaviour
{
    public UnityEvent onFirebaseInitalized = new UnityEvent();
    
    // Start is called before the first frame update
    void Start()
    {
        onFirebaseInitalized.AddListener(FBInitialized);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            onFirebaseInitalized.Invoke();
        });
        //FirebaseApp.CheckandFix
    }

    private void FBInitialized()
    {
        Debug.Log("Firebase Initialized");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
