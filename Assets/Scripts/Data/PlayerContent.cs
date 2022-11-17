using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerContent", menuName = "ScriptableObjects/PlayerContent", order = 1)]
public class PlayerContent : ScriptableObject
{
    public string pictureName;  // assigned in editor
    public string details;      // things like the name of the person
    public int exerciseID;      // assigned in editor
    public string remotePath;   // filled by script
    public string localPath;    // filled by script
    public Texture2D image;       // used by exercise
    public bool valid;
}
