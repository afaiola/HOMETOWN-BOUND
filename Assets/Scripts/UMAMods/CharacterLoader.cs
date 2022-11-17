using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA.CharacterSystem;

// Put on a UMADynamicCharacterAvatar object to load a preset avatar on Start
public class CharacterLoader : MonoBehaviour
{
    public CharacterDefinitionData characterData;
    // Start is called before the first frame update
    void Start()
    {
        string saveString = characterData.saveString;
        string compressedString = characterData.compressedString;
        string avatarString = characterData.avatarString;
        bool useCompressedString = characterData.useCompressedString;
        bool useAvatarDefinition = characterData.useAvatarDefinition;

        DynamicCharacterAvatar avatar = GetComponent<DynamicCharacterAvatar>();
        if (string.IsNullOrEmpty(saveString))
            return;
        if (useCompressedString)
        {
            AvatarDefinition adf = AvatarDefinition.FromCompressedString(compressedString, '|');
            avatar.LoadAvatarDefinition(adf);
            avatar.BuildCharacter(false); // don't restore old DNA...
        }
        else if (useAvatarDefinition)
        {
            avatar.LoadAvatarDefinition(avatarString);
            avatar.BuildCharacter(false); // We must not restore the old DNA
        }
        else
        {
            avatar.LoadFromRecipeString(saveString);
        }
    }


}
