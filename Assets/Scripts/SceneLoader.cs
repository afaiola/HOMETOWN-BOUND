using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get { return _instance; } }
    private static SceneLoader _instance;


    public ActivatorZone candylandDeactivator, cityDeactivator;


    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        candylandDeactivator.EnterEvent.AddListener(UnloadCandyland);
        cityDeactivator.EnterEvent.AddListener(UnloadCity);
    }


    public void UnloadCandyland()
    {
        if (GetIfSceneLoaded("Game"))
        {
            SceneManager.UnloadSceneAsync("Game");
        }
        UIManager.Instance.gotoMenu.HideOptions(1);
    }

    public void UnloadCity()
    {
        if (GetIfSceneLoaded("City"))
        {
            SceneManager.UnloadSceneAsync("City");
        }
        UIManager.Instance.gotoMenu.HideOptions(2);
    }

    public void UnloadNeighborhood()
    {
        if (GetIfSceneLoaded("Neighborhood"))
            SceneManager.UnloadSceneAsync("Neighborhood");
    }

    public void LoadHouseInterior()
    {
        //GameManager.Instance.LevelLoaded(3);
        UIManager.Instance.CloseEyes();
        UIManager.Instance.followPlayer = false;
        SceneManager.LoadSceneAsync("House", LoadSceneMode.Additive);
    }

    private bool GetIfSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("SplashScreen");
        Destroy(MusicManager.Instance.gameObject);
        Destroy(TankController.Instance.gameObject);
        Destroy(LightController.Instance.gameObject);
        Destroy(StatisticsManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        Destroy(SavePatientData.Instance.gameObject);
        Destroy(SecurityCode.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        Destroy(GameManager.Instance.scavengerObjects);
        Destroy(GameManager.Instance.gameObject);
        Destroy(Profiler.Instance.gameObject);
        Destroy(StorageManager.Instance.gameObject);
        Destroy(gameObject);
    }
}
