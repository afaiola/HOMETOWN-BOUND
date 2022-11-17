using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
public class Greet : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] Transform player;
    [SerializeField] LookAt look;
    [SerializeField]AudioSource source;
    [SerializeField]Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        source = GetComponent<AudioSource>();
        animator = GetComponentInParent<Animator>();
        look = GetComponentInParent<LookAt>();
    }

    // Update is called once per frame
    public void OnMouseUp()
    {
        if (player.GetComponent<TankController>().enabled)
        {
            StartCoroutine(Wave());
            if (look)
                StartCoroutine(Wave2());
        }
    }

    
    bool waving;
    public IEnumerator Wave2()
    {
        if (!waving)
        {
            waving = true;
            look.lookPosition = player.transform;
            look.Look();
            animator.SetTrigger("Waving");
            yield return new WaitForSeconds(1);
            source.clip = clip;
            source.Play();
            yield return new WaitForSeconds(1);
            look.Stop();
            waving = false;
        }
    }


    IEnumerator Wave()
    {
        if (animator.GetBool("isWalking"))
        {
            transform.parent.LookAt(new Vector3(player.position.x, transform.parent.position.y, player.position.z));
            animator.SetBool("isWalking", false);
            animator.SetBool("isWaving", true);
            yield return new WaitForSeconds(1);
            source.clip = clip;
            source.Play();
            yield return new WaitForSeconds(1);
            animator.SetBool("isWaving", false);
            animator.SetBool("isWalking", true);
        }
    }
}
