using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercise to have player walk to a specific area.
// When area is reached, transition the player to an exact position.
public class WalkExercise : Exercise
{
    public string nameOfLocation;
    public ActivatorZone zone;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void OnValidate()
    {

    }

    public override void Arrange()
    {
        zone.enabled = true;
        zone.enterEvent = new UnityEngine.Events.UnityEvent();
        zone.enterEvent.AddListener(EnterArea);
    }

    public override bool CheckSuccess
    {
        get { return true; }
    }

    public void EnterArea()
    {
        // TODO: force to look straight ahead
        if (VRManager.Instance)
        {
            _correctCount++;
            Success();
        }
        StartCoroutine(MoveToPos());
    }

    private IEnumerator MoveToPos()
    {
        TankController.Instance.DisableMovement();
        Transform camTransform = TankController.Instance.playerCam.transform;
        float dist = Vector3.Distance(TankController.Instance.transform.position, zone.transform.position);
        float angle = Quaternion.Angle(TankController.Instance.transform.rotation, zone.transform.rotation);
        float pitch = camTransform.localEulerAngles.x;
        while (dist > 0.1f || Mathf.Abs(angle) > 0.1f)
        {
            TankController.Instance.transform.position = Vector3.MoveTowards(TankController.Instance.transform.position, zone.transform.position, 1f * Time.deltaTime);
            dist = Vector3.Distance(TankController.Instance.transform.position, zone.transform.position);

            TankController.Instance.transform.rotation = Quaternion.RotateTowards(TankController.Instance.transform.rotation, zone.transform.rotation, 25f * Time.deltaTime);
            angle = Quaternion.Angle(TankController.Instance.transform.rotation, zone.transform.rotation);

            pitch = 180f - camTransform.localEulerAngles.x;
            if (Mathf.Abs(pitch) < 179f)
            {
                float direction = pitch < 0 ? -0.1f : 0.1f;
                Debug.Log($"pitch: {pitch} dir: {direction}");

                //camTransform.localEulerAngles = new Vector3(pitch - (direction), 0, 0);
                TankController.Instance.RotateCharacterUpDown(direction);
            }

            yield return new WaitForEndOfFrame();
        }
        _correctCount++;
        Success();
    }


}
