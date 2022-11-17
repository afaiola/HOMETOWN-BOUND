using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    float time;
    [SerializeField] float cooldown;
    [SerializeField] ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        time = Time.time + cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0 && time < Time.time)
        {
            foreach (var _ps in ps.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
            time = Time.time + cooldown;
        }
    }
}
