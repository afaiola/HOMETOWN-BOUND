using QuantumTek.QuantumUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: move this to its own loading scene rather than doing it on the main menu
public class Load : MonoBehaviour
{
    public Transform loadingBar;
    bool menuUnloaded, scenesLoaded;
    AsyncOperation gameLoad;

    public void Transition()
    {
        Debug.Log("Scenes loaded");
        gameLoad.allowSceneActivation = true;
        scenesLoaded = true;
    }

    void Update()
    {
        if (!menuUnloaded && scenesLoaded)
        {
            Debug.Log("Unloading mainmenu");
            menuUnloaded = true;
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    }

    public void LoadGame()
    {
        Debug.Log("load game");
        VRHandler vrHandler = GameObject.FindObjectOfType<VRHandler>();
        if (vrHandler)
            vrHandler.GameLoading();
        StartCoroutine(LoadSceneAsync());
    }

    protected IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSecondsRealtime(.2f);

        //AsyncOperation candylandLoad = SceneManager.LoadSceneAsync("Candyland", LoadSceneMode.Additive);
        AsyncOperation cityLoad = SceneManager.LoadSceneAsync("City", LoadSceneMode.Additive);
        AsyncOperation neighborhoodLoad = SceneManager.LoadSceneAsync("Neighborhood", LoadSceneMode.Additive);
        gameLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

        gameLoad.allowSceneActivation = false;

        while (!gameLoad.isDone && !cityLoad.isDone && !neighborhoodLoad.isDone)
        {
            float loadProgress = gameLoad.progress + cityLoad.progress + neighborhoodLoad.progress;
            loadProgress /= 3f;
            loadProgress = (loadProgress + StorageManager.Instance.GetDownloadProgress()) / 2f; // maybe separate this into a separate bar
            //Debug.Log("progress: " + loadProgress);
            if (loadingBar)
            {
                loadingBar.localScale = new Vector3(loadProgress, 1, 1);
            }

            yield return null;
        }
        Transition();
    }
}
