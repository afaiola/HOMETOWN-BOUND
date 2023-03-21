using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get { return _instance; } }
    private static UIManager _instance;

    public Text moduleName;
    public GameObject pauseMenu;
    public GoToMenu gotoMenu;
    public RectTransform topLid, bottomLid;
    public float blinktime = 1f;

    public EndUI endUI;
    public GameObject refocusObj;

    public bool paused, inCutscene, requestRefocus;
    CursorLockMode prevMode;
    bool prevVisible;

    // Start is called before the first frame update
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

        DontDestroyOnLoad(gameObject);
        if (refocusObj)
            refocusObj.SetActive(false);
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (requestRefocus && Input.GetMouseButtonDown(0))
        {
            var view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
            if (!isOutside)
            {
                requestRefocus = false;
                refocusObj.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                TankController.Instance.speed = 15f;
                TankController.Instance.turnSpeed = 180f;
            }
        }
        if (inCutscene) return;
        if (Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused && !requestRefocus)
            {
                Pause();
            }
            else
            {
                //Resume();
                // when button is pressed down, resume, lock cursor on release
            }
        }
    }

    public void Pause()
    {
        pauseMenu.GetComponent<MusicManager>();
        pauseMenu.SetActive(true);
        paused = true;
        //menu.SetActive(true);
        /*
        prevMode = Cursor.lockState;
        prevVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;*/
        if (!ScoreCalculator.instance.exercising && !inCutscene)
            TankController.Instance.DisableMovement();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
        pauseMenu.GetComponentInChildren<Text>().text = "Paused";
    }

    public void AFKMenu()
    {
        Pause();
        pauseMenu.GetComponentInChildren<Text>().text = "Still playing?";
    }

    // called on mouse release of pause button to lock cursor in game window
    public void LockCursor()
    {
        if (!ScoreCalculator.instance.exercising)
        {
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;

            if (!inCutscene)
                TankController.Instance.EnableMovement();
        }

        //if (!ScoreCalculator.instance.exercising && !inCutscene)
        //    TankController.Instance.EnableMovement();

        pauseMenu.SetActive(false);
        PromptGameWindowFocus();
    }

    public void Resume()
    {
        Time.timeScale = 1;

        paused = false;
        Menu.Instance.gotoMenu.SetActive(false);
        pauseMenu.SetActive(false);
        LockCursor();
        //menu.SetActive(false);
        //Cursor.lockState = prevMode;
        //Cursor.visible = prevVisible;


        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;  
    }

    // only needed for webgl
    public void PromptGameWindowFocus()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer) return;
        if (ScoreCalculator.instance.exercising) return;
        TankController.Instance.speed = 0f; // disallow movement while unfocused
        TankController.Instance.turnSpeed = 0f;
        refocusObj.SetActive(true);
        requestRefocus = true;
    }

    public void UpdateModuleName(string str)
    {
        moduleName.text = str;
    }

    public void CloseEyes()
    {
        StartCoroutine(Blink(0, Screen.height / 2));
    }

    public void OpenEyes()
    {
        StartCoroutine(Blink(Screen.height / 2, 0));
    }

    private IEnumerator Blink(int start, int goal)
    {
        float blinking = 0;
        float timecount = 0;

        topLid.sizeDelta = new Vector2(topLid.sizeDelta.x, start);
        bottomLid.sizeDelta = new Vector2(bottomLid.sizeDelta.x, start);

        while (timecount < blinktime)
        {
            blinking = Mathf.Lerp(start, goal, timecount);
            topLid.sizeDelta = new Vector2(topLid.sizeDelta.x, blinking);
            bottomLid.sizeDelta = new Vector2(bottomLid.sizeDelta.x, blinking);
            timecount += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        TankController.Instance.playerCam.transform.localRotation = new Quaternion();   // reset view
        topLid.sizeDelta = new Vector2(topLid.sizeDelta.x, goal);
        bottomLid.sizeDelta = new Vector2(bottomLid.sizeDelta.x, goal);
    }

    public void ShowEndScreen(bool on = true)
    {
        endUI.gameObject.SetActive(on);
        endUI.GetScores();
        Menu.Instance.gotoButton.onValueChanged.RemoveAllListeners();
        Menu.Instance.gotoButton.onValueChanged.AddListener(ShowEndScreen);
        TankController.Instance.DisableMovement();
    }

}
