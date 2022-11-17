using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterDefinitionSaver : MonoBehaviour
{
    public UnityEngine.UI.InputField field;
    public SaveAndLoadSample saver;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveNewData()
    {
#if UNITY_EDITOR
        CharacterDefinitionData data = new CharacterDefinitionData();
        data.name = field.text;
        data.saveString = saver.saveString;
        data.compressedString = saver.compressedString;
        data.avatarString = saver.avatarString;
        data.useAvatarDefinition = saver.useAvatarDefinition;
        data.useCompressedString = saver.useCompressedString;
        AssetDatabase.CreateAsset(data, "Assets/Resources/CharacterRecipes/" + data.name.Replace(" ", "") + ".asset");
        AssetDatabase.SaveAssets();
#endif
    }

}
