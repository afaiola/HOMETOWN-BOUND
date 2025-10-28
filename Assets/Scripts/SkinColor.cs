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
                if (value > 0 && value < skinColors.Length)
                {
                    item.material = skinColors[value];
                }
            }

        }
    }

    [SerializeField]
    Material[] skinColors;


    void Awake()
    {
        skin = 0;
    }

}
