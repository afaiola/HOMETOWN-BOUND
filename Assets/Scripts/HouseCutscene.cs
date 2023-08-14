using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HouseCutscene : MonoBehaviour
{
    public Transform startPos, goalPos;
    public Door door;
    public AudioSource instructions;

    private Transform player;
    private float doorOpenTime = 2f;
    private float playerMoveTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // play on start in the house scene
        Play();
        if (VRManager.Instance)
        {
            var crossSceneTPAreas = GameObject.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.TeleportationArea>();
            foreach (var tpArea in crossSceneTPAreas)
            {
                tpArea.interactionManager = VRManager.Instance.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>(true);
                tpArea.teleportationProvider = VRManager.Instance.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.TeleportationProvider>();
            }
        }
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
        TankController.Instance.GetComponent<FloatingOrigin>().canUpdate = true;
        UIManager.Instance.CloseEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime*1.5f);

        // async unload the neighborhood
        GameManager.Instance.sceneLoader.UnloadNeighborhood();

        // face player toward door
        // do this first time to know where to reposition to
        player.transform.position = startPos.position;
        player.transform.rotation = startPos.rotation;

        //TankController.Instance.GetComponent<FloatingOrigin>()?.RecenterOrigin();
        yield return new WaitForSecondsRealtime(1f);

        // do this twice to ensure player is in correct position.
        player.transform.position = startPos.position;


        UIManager.Instance.OpenEyes();
        yield return new WaitForSecondsRealtime(UIManager.Instance.blinktime);

        door.Open();
        yield return new WaitForSeconds(door.moveTime);

        // walk player into house
        timecount = 0f;
        while (timecount < playerMoveTime)
        {
            player.transform.position = Vector3.Lerp(startPos.position, goalPos.position, timecount / playerMoveTime);
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
        instructions.Play();

    }
}
