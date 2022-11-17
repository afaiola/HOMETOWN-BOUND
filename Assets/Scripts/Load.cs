using QuantumTek.QuantumUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: move this to its own loading scene rather than doing it on the main menu
public class Load : MonoBehaviour
{
    public Transform loadingBar;
    // Start is called before the first frame update

    bool startedLoading;
    void Start()
    {
    }

    void Update()
    {
        if (!startedLoading)
        {
            startedLoading = true;
            DontDestroyOnLoad(gameObject);
            //StartCoroutine(LoadSceneAsync());
        }
    }

    bool ready, finish;
    AsyncOperation gameLoad;
    public void Transition()
    {
        gameLoad.allowSceneActivation = true;
        finish = true;
    }

    void FixedUpdate()
    {
        if (ready == true && finish == true)
        {
            enabled = false;
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    }

    public void LoadGame()
    {
        Debug.Log("load game");
        StartCoroutine(LoadSceneAsync());
    }

    protected IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSeconds(.2f);

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
        ready = true;
        Transition();
    }
}
