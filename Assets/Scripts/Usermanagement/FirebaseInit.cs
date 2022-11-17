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
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            onFirebaseInitalized.Invoke();
        });
        //FirebaseApp.CheckandFix
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
