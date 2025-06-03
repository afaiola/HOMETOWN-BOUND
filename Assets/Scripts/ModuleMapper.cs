using System;
using System.Collections;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

public class ModuleComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer().Compare((string)((Module)x).name, (string)((Module)y).name));
    }
}
public class InteractableComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer().Compare((string)((Interact)x).name, (string)((Interact)y).name));
    }
}
public class GoToComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer().Compare((string)((GoTo)x).name, (string)((GoTo)y).name));
    }
}

public class ModuleMapper : MonoBehaviour
{
    public Module[] modules;
    public Interact[] interactables;
    public GoTo[] gotos;


    public void MapModules()
    {
        modules = FindObjectsOfType<Module>();
        interactables = FindObjectsOfType<Interact>();
        gotos = Menu.Instance.GetComponentsInChildren<GoTo>(true);
        Array.Sort(modules, new ModuleComparer());
        Array.Sort(interactables, new InteractableComparer());
        Array.Sort(gotos, new GoToComparer());
        int totalExercises = 0;
        for (int i = 0; i < modules.Length; i++)
        {
            totalExercises++; // first exercise is walking to the module
            interactables[i].correspondingModule = modules[i];
            interactables[i].interactEvent = new UnityEngine.Events.UnityEvent();
            interactables[i].interactEvent.AddListener(interactables[i].ModuleInteract);
            gotos[i].goToModule = modules[i];// null ref here
            gotos[i].moduleObject = interactables[i].gameObject;

            for (int j = 0; j < modules[i].exercises.Count; j++)
            {
                modules[i].exercises[j].exerciseID = totalExercises;
                totalExercises++;
            }
        }
        gotos[gotos.Length - 1].moduleObject = null;    // allows house cutscene to take control of player
    }

    public void MapPlayerContent()
    {
        string json_file = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}usercontent{Path.DirectorySeparatorChar}{Profiler.Instance.currentUser.username}{Path.DirectorySeparatorChar}content_map.json";
        // if I create an object with an entry for each possible item in the json, I can tell which fields were answered and can make decisions that way
        string json = "";
        using (StreamReader r = new StreamReader(json_file))
        {
            json = r.ReadToEnd();
        }
        if (json.Length < 10) return;
        var content_map = JObject.Parse(json);
        // TODO: check if picture is valid before applying it
        foreach (var content in StorageManager.Instance.playerContents)
        {
            if (!content.valid) continue;
            int mod = Mathf.FloorToInt(content.exerciseID / 7);
            int ex = content.exerciseID % 7;
            Exercise exercise = modules[mod].exercises[ex];
            exercise.customContent = true;

            string contentDetails = "";
            if (content_map.ContainsKey(content.pictureName))
            {
                contentDetails = content_map[content.pictureName].ToString();
            }

            if (content == StorageManager.Instance.portraitContent)
            {
                PortraitExercise portraitExercise = exercise as PortraitExercise;
                char delim = ' ';
                if (contentDetails.Contains(",")) delim = ',';
                string[] names = contentDetails.Split(delim);
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = char.ToUpper(names[i][0]) + names[i].Substring(1);
                }

                portraitExercise.leftObject.texture = content.image;
                portraitExercise.Initialize(names);
            }
            else
            {
                exercise.leftImage = content.image;
                exercise.images[UnityEngine.Random.Range(0, exercise.images.Count)] = content.image;
            }

            string pictureName = content.pictureName;

            if (content.pictureName == "other") pictureName = "significant other";

            if (pictureName[pictureName.Length - 1] == '1' || pictureName[pictureName.Length - 1] == '2') pictureName = pictureName.Substring(0, pictureName.Length - 1);

            if (contentDetails != "")
            {
                contentDetails = ", " + contentDetails;
                content.details = char.ToUpper(contentDetails[0]) + contentDetails.Substring(1);
            }

            exercise.nameOfObject = $"your <color=#79B251>{pictureName}</color>{content.details}";
        }

        foreach (var content in StorageManager.Instance.dropdownContents)
        {
            content.valid = content_map.ContainsKey(content.pictureName);
            if (!content.valid) continue;
            var (moduleIndex, exerciseIndex) = GetModuleAndExerciseIndicesFromExcerciseId(content.exerciseID);
            Exercise exercise = modules[moduleIndex].exercises[exerciseIndex];
            exercise.customContent = true;
            int optionSelected = int.Parse(content_map[content.pictureName].ToString());
            content.details = optionSelected.ToString();
            if (content.pictureName == "food") content.pictureName = "type of food";
            if (content.pictureName == "restaurant") content.pictureName = "fast food restaurant";
            if (content.pictureName == "grocery") content.pictureName = "grocery store";
            exercise.nameOfObject = $"your favorite <color=#79B251> {content.pictureName}";

            if (content.pictureName == "program")
            {
                TVExercise tv = exercise as TVExercise;
                tv.goalChannel = optionSelected;
                Debug.Log("Set tv channel to <color=#79B251>" + optionSelected);
            }
            else
            {
                exercise.leftImage = exercise.images[optionSelected];
            }

            Debug.Log($"set {content.pictureName} to exercise: {exercise.name} with option: {optionSelected}");

        }
    }

    public void ResetModule(Module module)
    {
        int moduleIndex = Array.IndexOf(modules, module);
        interactables[moduleIndex].Reset();
    }

    public void SkipModules(int skipID) // module with skipID should not be skipped
    {
        for (int i = 0; i < skipID; i++)
        {
            if (interactables[i])
            {
                interactables[i].gameObject.SetActive(false);
            }
        }
    }

    public int GetModuleIndexFromExcerciseId(int excerciseId)
    {
        int currentExercise = -1;
        for (int i = 0; i < modules.Length; i++)
        {
            currentExercise++; // +1 for walk exercise
            currentExercise += modules[i].exercises.Count;
            if (excerciseId <= currentExercise)
            {
                return i;
            }
        }
        return 0;
    }

    public (int level, int module, int exercise) GetIndicesFromExcerciseId(int excerciseId)
    {
        int currentExercise = -1;
        foreach (var module in modules)
        {
            currentExercise++; // +1 for walk exercise
            if (excerciseId == currentExercise)
            {
                Debug.Log("currentExercise: " + currentExercise + " looking for: " + excerciseId + " go to exercise");
                return (module.lvl, module.ModuleNo, currentExercise);
            }
            foreach (var _ in module.exercises)
            {
                Debug.Log("currentExercise: " + currentExercise + " looking for: " + excerciseId);
                currentExercise++;
                if (excerciseId == currentExercise)
                {
                    return (module.lvl, module.ModuleNo, currentExercise);
                }
            }
        }
        Debug.LogError("Could not find module from exercise id.");
        return (0, 0, 0);
    }


    private (int, int) GetModuleAndExerciseIndicesFromExcerciseId(int excerciseId)
    {
        int currentExercise = -1;
        for (int i = 0; i < modules.Length; i++)
        {
            currentExercise++; // +1 for walk exercise
            for (int j = 0; j < modules[i].exercises.Count; j++)
            {
                if (excerciseId == currentExercise)
                {
                    return (i, j);
                }
                currentExercise++;
            }
        }
        return (0, 0);
    }
}
