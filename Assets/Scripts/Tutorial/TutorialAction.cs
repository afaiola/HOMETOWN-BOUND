using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAction : MonoBehaviour
{
    public int id;
    public TutorialAction nextAction;

    [System.Serializable]
    public struct TutorialObject
    {
        public GameObject obj;
        public bool isAnimated;
        public string animationName;
        public bool hasSound;
    }

    [SerializeField] TutorialObject[] tutorialObjects;
    [SerializeField] TutorialActionSuccessCondition[] successConditions;
    [SerializeField] int successRequired;

    private void OnValidate()
    {
        successRequired = successConditions.Length;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Run()
    {
        foreach (var tut in tutorialObjects)
        {
            tut.obj.SetActive(true);

            if (tut.isAnimated)
            {
                Animator anim = tut.obj.GetComponent<Animator>();
                anim.Play(tut.animationName);
            }

            if (tut.hasSound)
            {
                AudioSource audio = tut.obj.GetComponent<AudioSource>();
                audio.Play();
            }
        }
    }

    public void Finish()
    {
        foreach (var tut in tutorialObjects)
        {
            tut.obj.SetActive(false);
        }
    }

    public virtual bool IsComplete()
    {
        int successCt = 0;
        foreach (var condition in successConditions)
        {
            if (condition.IsComplete())
            {
                successCt++;
            }
        }

        return successCt >= successRequired;
    }
}
