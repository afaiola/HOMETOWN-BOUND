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
    private UnityEngine.CharacterController controller;

    public float speed = 15;
    public float turnSpeed = 180;
    [SerializeField] private AudioClip[] m_FootstepSounds;
    public float afkTime = 60f;
    List<Animation> handAnimations;
    private float m_StepCycle;
    private float m_NextStep;
    [SerializeField] private float m_StepInterval;
    private AudioSource m_AudioSource;

    public Material[] skinColors;

    private int displayWidth, displayHeight;
    private float timeSinceLastMove = 0;

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

        handAnimations = GetComponentsInChildren<Animation>().ToList();
        controller = GetComponent<UnityEngine.CharacterController>();
        m_AudioSource = GetComponent<AudioSource>();
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;

        displayWidth = Display.main.renderingWidth / 2;
        displayHeight = Display.main.renderingHeight / 2;

        foreach (var rend in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            rend.sharedMaterial = skinColors[Profiler.Instance.currentUser.skin_id];
        }
    }

    private void Update()
    {

    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (Time.time - timeSinceLastMove > afkTime && !UIManager.Instance.paused)
        {
            UIManager.Instance.AFKMenu();
            timeSinceLastMove = Time.time;
        }

        //var y = Input.GetAxis("Mouse Y");
        RotateCharacter(Input.GetAxis("Mouse X"));
        MoveCharacter(Input.GetAxis("Mouse ScrollWheel") * 10f);
        MoveCharacter(Input.GetAxis("Mouse Y"));

        MoveCharacter(Input.GetAxis("Vertical") * 0.5f);
        RotateCharacter(Input.GetAxisRaw("Horizontal") * 0.35f);

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

    public void RotateCharacter(float value)
    {
        if (value == 0) return;
        transform.Rotate(0, value * turnSpeed * Time.deltaTime, 0);
    }

    public void MoveCharacter(float value)
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

    private void PlayFootStepAudio()
    {
        if (!controller.isGrounded)
        {
            return;
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
        controller.enabled = true;
        canMove = true;
        StartCoroutine(FixMouse(CursorLockMode.Locked));
    }

    public void DisableMovement()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        controller.enabled = false;
        canMove = false;
        StartCoroutine(FixMouse(CursorLockMode.Confined));
    }

    IEnumerator FixMouse(CursorLockMode cursorSetting)
    {
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(Time.deltaTime * 2);
        Cursor.lockState = cursorSetting;
    }
}
