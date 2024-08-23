using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TankController : MonoBehaviour
{
    public static TankController Instance { get { return _instance; } }
    private static TankController _instance;

    public bool canMove;
    public float maxAngle = 60;
    private UnityEngine.CharacterController controller;
    private Rigidbody rb;
    public Camera playerCam;

    public float speed = 15;
    public float turnSpeed = 180;
    [SerializeField] private AudioClip[] m_FootstepSounds;
    public float afkTime = 60f;
    List<Animation> handAnimations;
    private float m_StepCycle;
    private float m_NextStep;
    [SerializeField] private float m_StepInterval;
    private AudioSource m_AudioSource;
    private FloatingOrigin floatingOrigin;

    //public Material[] skinColors;

    private float timeSinceLastMove = 0;

    void Start()
    {
        
    }
    
    public void Initialize()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Destroy player");
            Destroy(gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);

        handAnimations = GetComponentsInChildren<Animation>().ToList();
        controller = GetComponent<UnityEngine.CharacterController>();
        rb = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;

        SetHandModel();

        floatingOrigin = GetComponent<FloatingOrigin>();
    }

    public void SetHandModel()
    {
        int skinID = 0;// Random.Range(0, skinColors.Length);
        if (Profiler.Instance)
            skinID = Profiler.Instance.currentUser.skin_id;
        var skin = GetComponent<SkinColor>();
        skin.skin = skinID;
    }

    private void Update()
    {

    }

    void FixedUpdate()
    {
        if (!canMove || VRManager.Instance != null) return;

        if (Time.time - timeSinceLastMove > afkTime && !UIManager.Instance.paused)
        {
            UIManager.Instance.AFKMenu();
            timeSinceLastMove = Time.time;
        }

        if (SystemInfo.deviceType != DeviceType.Handheld)
        { 
            //var y = Input.GetAxis("Mouse Y");
            RotateCharacterUpDown(Input.GetAxis("Mouse Y"));
            MoveCharacterForwardBack(Input.GetAxis("Mouse ScrollWheel") * 10f);
            RotateCharacterLeftRight(Input.GetAxis("Mouse X"));
        }

        MoveCharacterForwardBack(Input.GetAxis("Vertical") * 0.3f);
        float horiz = Input.GetAxisRaw("Horizontal") * 0.35f;
        if (Input.anyKey)
            MoveCharacterLeftRight(horiz);
        else
            RotateCharacterLeftRight(horiz);

        //Fall();
        /*var movDir = transform.forward * Input.GetAxis("Mouse ScrollWheel") * speed * 10;
        if (movDir == Vector3.zero)
            movDir = transform.forward * Input.GetAxis("Mouse Y") * speed;
        if (movDir != Vector3.zero)
        {
            var animation = handAnimations.First();
            if (!handAnimations.Any(z => z.isPlaying))
            {
                animation.Play();
                handAnimations.Reverse();
            }
        }
        // moves the character in horizontal direction
        controller.Move(movDir * Time.deltaTime - Vector3.up * 0.1f);

        ProgressStepCycle(speed,y);
        */
    }

    public void Recenter(Vector3 pos)
    {
        controller.enabled = false;
        transform.position -= pos;
        controller.enabled = true;
    }

    private void Fall()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -transform.up);
        if (hits.Length > 0)
        {
            Transform floor = null;
            foreach (var hit in hits)
            {
                if (hit.collider.transform.parent != transform)
                    floor = hit.collider.transform;
            }
            if (floor != null)
            {
                float distance = transform.position.y - floor.position.y;
                Debug.Log($"distance to {floor.name} = {distance}");
                var movDir = -transform.up * 9.81f;
                if (Mathf.Abs(distance -2f) > 0.1f)
                {
                    if (distance < 2)
                        movDir *= -1f;
                    transform.position += movDir * Time.deltaTime;
                }
            }
        }
    }

    public void RotateCharacterLeftRight(float value)
    {
        if (value == 0) return;
        transform.Rotate(0, value * turnSpeed * Time.deltaTime, 0);
        
    }
    public void RotateCharacterUpDown(float value)
    {
        if (value == 0) return;
        value = -value * turnSpeed * Time.deltaTime;
        value = Mathf.Clamp(value, -60f, 60f);
        Quaternion yQuat = Quaternion.AngleAxis(value, Vector3.right);
        Quaternion temp = playerCam.transform.localRotation * yQuat;
        if (Quaternion.Angle(Quaternion.identity, temp) < maxAngle)
            playerCam.transform.localRotation = temp;
    }

    public void MoveCharacterForwardBack(float value)
    {
        if (value == 0) return;
        timeSinceLastMove = Time.time;
        var movDir = transform.forward * value * speed;
        if (movDir == Vector3.zero) return;

        var animation = handAnimations.First();
        if (!handAnimations.Any(z => z.isPlaying))
        {
            animation.Play();
            handAnimations.Reverse();
        }

        // moves the character in horizontal direction
        controller.Move(movDir * Time.deltaTime - Vector3.up * 0.1f);

        ProgressStepCycle(speed, value);
    }
    public void MoveCharacterLeftRight(float value)
    {
        if (value == 0) return;
        timeSinceLastMove = Time.time;
        var movDir = transform.right * value * speed;
        if (movDir == Vector3.zero) return;

        var animation = handAnimations.First();
        if (!handAnimations.Any(z => z.isPlaying))
        {
            animation.Play();
            handAnimations.Reverse();
        }

        controller.Move(movDir * Time.deltaTime - Vector3.up * 0.1f);

        ProgressStepCycle(speed, value);
    }

    private void PlayFootStepAudio()
    {
        if (controller)
        {
            if (!controller.isGrounded)
            {
                return;
            }
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    private void ProgressStepCycle(float speed, float y)
    {
        if (controller.velocity.sqrMagnitude > 5 && (y!=0))
        {
            m_StepCycle += (controller.velocity.magnitude + (speed)) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }

    public void EnableMovement()
    {
        timeSinceLastMove = Time.time;
        enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (controller)
            controller.enabled = true;
        canMove = true;
        StartCoroutine(FixMouse(CursorLockMode.Locked));
        if (floatingOrigin)
            floatingOrigin.canUpdate = true;

        if (VRManager.Instance)
        {
            VRManager.Instance.ApplySettings();
        }
    }

    public void DisableMovement()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        //if (controller)
        //controller.enabled = false;
        if (floatingOrigin)
            floatingOrigin.canUpdate = false;

        canMove = false;
        StartCoroutine(FixMouse(CursorLockMode.Confined));

        if (VRManager.Instance)
        {
            VRManager.Instance.DisableMovement();
        }
    }

    IEnumerator FixMouse(CursorLockMode cursorSetting)
    {
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(Time.deltaTime * 2);
        Cursor.lockState = cursorSetting;
    }

    public void SetCullDistance(float dist)
    {
        playerCam.farClipPlane = dist;
    }
}
