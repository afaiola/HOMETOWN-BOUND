using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HouseCutscene : MonoBehaviour
{
    public Transform startPos, goalPos;
    public Door door;

    private Transform player;
    private float doorOpenTime = 2f;
    private float playerMoveTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // play on start in the house scene
        Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        StartCoroutine(Cutscene());
        if (UIManager.Instance)
            UIManager.Instance.gotoMenu.HideOptions(3);
    }

    private IEnumerator Cutscene()
    {
        UIManager.Instance.inCutscene = true;
        player = TankController.Instance.transform;

        // door takes 2 seconds to open and close
        // door is held open for 3 seconds
        float timecount = 0;

        TankController.Instance.DisableMovement();
        UIManager.Instance.CloseEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime*1.5f);

        // face player toward door
        player.transform.position = startPos.position;
        player.transform.rotation = startPos.rotation;

        // async unload the neighborhood
        GameManager.Instance.sceneLoader.UnloadNeighborhood();

        yield return new WaitForSecondsRealtime(1f);

        UIManager.Instance.OpenEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        door.Open();
        yield return new WaitForSeconds(door.moveTime);

        // walk player into house
        timecount = 0f;
        Vector3 playerGoal = goalPos.position;

        while (timecount < playerMoveTime)
        {
            player.transform.position = Vector3.Lerp(startPos.position, playerGoal, timecount / playerMoveTime);
            timecount += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (MusicManager.Instance)
            MusicManager.Instance.PlayHouseMusic();

        //yield return new WaitForSecondsRealtime(0.5f);

        door.Close();
        yield return new WaitForSeconds(door.moveTime);
        TankController.Instance.EnableMovement();
        UIManager.Instance.inCutscene = false;
    }
}
