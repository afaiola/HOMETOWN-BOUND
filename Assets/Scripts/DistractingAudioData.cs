using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistractingAudioData", menuName = "ScriptableObjects/DistractingAudioData", order = 1)]
public class DistractingAudioData : ScriptableObject
{
    [System.Serializable]
    public struct ExerciseDistraction
    {
        public int distractionAmount;   // lowest level should be one every 5 seconds. Highest will be 1 second
    }
    [System.Serializable]
    public struct ModuleDistraction
    {
        public string moduleName;
        public ExerciseDistraction[] exercises;
        public AudioClip[] clips;
    }

    public ModuleDistraction[] modules;
}
