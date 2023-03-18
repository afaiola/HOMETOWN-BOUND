using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StaticEvent.showSpeechBubble.AddListener(ActivateBubble);
        StaticEvent.hideSpeechBubble.AddListener(DeactivateBubble);
    }

    void ActivateBubble(){
        foreach(Transform child in transform){
            if (child.gameObject.name == "Text"){
                child.GetComponent<TMPro.TextMeshProUGUI>().text = child.GetComponent<TMPro.TextMeshProUGUI>().text.Replace("{name}", Profiler.Instance.currentUser.displayName);
            }
            child.gameObject.SetActive(true);
        }
    }
    void DeactivateBubble(){
        Destroy(this.gameObject);
    }
}
