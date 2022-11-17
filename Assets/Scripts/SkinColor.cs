using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinColor : MonoBehaviour
{
    public int skin
    {
        set
        {
            foreach (var item in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var color = 0;
                if (value > 0 && value < skinColors.Length)
                    color = value;
                item.material = skinColors[color];
            }
            
        }
    }
    [SerializeField] Material[] skinColors;
    // Start is called before the first frame update

    void Awake()
    {
        skin = 0;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

}
