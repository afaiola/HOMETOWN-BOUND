using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinitionData", menuName = "ScriptableObjects/CharacterDefinitionData", order = 1)]
public class CharacterDefinitionData : ScriptableObject
{
    public bool useCompressedString, useAvatarDefinition;
    public string saveString, compressedString, avatarString;
}
