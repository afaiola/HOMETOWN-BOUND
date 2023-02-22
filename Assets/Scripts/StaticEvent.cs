using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public static class StaticEvent 
{
    public static UnityEvent moduleStart = new UnityEvent();
    public static UnityEvent moduleEnd = new UnityEvent();
    public static UnityEvent showSpeechBubble = new UnityEvent();
    public static UnityEvent hideSpeechBubble = new UnityEvent();
    public static void moduleStarted(){
        moduleStart.Invoke();
    }
    public static void moduleEnded(){
        moduleEnd.Invoke();
    }

    public static void DeactivateSpeechBubble()
    {
       hideSpeechBubble.Invoke();    
    }

    public static void ActivateSpeechBubble()
    {
        showSpeechBubble.Invoke();
    }
    
}
