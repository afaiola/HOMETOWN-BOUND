using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DistractionObject : MonoBehaviour
{
    [System.NonSerialized] public AudioSource source;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Initialize(AudioClip clip)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        source = GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0.5f;
        Vector3 playerRight = -player.transform.right;
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.Normalize();
        float dot = Vector3.Dot(playerRight, directionToPlayer);
        source.panStereo = dot;

        source.Play();
        Invoke("Destruct", clip.length);
    }

    private void Destruct()
    {
        Destroy(gameObject);
    }
}
