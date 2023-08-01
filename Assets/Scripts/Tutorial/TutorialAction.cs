using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAction : MonoBehaviour
{
    [System.NonSerialized] public int id;
    public TutorialAction onSuccessAction, onFailAction;

    private bool isSuccess;

    [System.Serializable]
    public struct TutorialObject
    {
        public string name;
        public GameObject obj;
        public Transform objLocation;
        public bool isObjActive;
        public bool isAnimated;
        public AnimatorControllerParameterType animtionParameterType;
        public string animationName;
        public AudioClip audioClip;
        public string subtitle;
    }

    [SerializeField] protected TutorialObject[] tutorialObjects;
    [SerializeField] protected TutorialActionSuccessCondition[] successConditions;
    [SerializeField] protected int successRequired;

    protected void OnValidate()
    {
        successConditions = GetComponentsInChildren<TutorialActionSuccessCondition>();
        successRequired = successConditions.Length;

        for (int i = 0; i < tutorialObjects.Length; i++)
        {
            if (tutorialObjects[i].obj)
                tutorialObjects[i].name = tutorialObjects[i].obj.name;
        }    
    }

    public virtual void Run()
    {
        // TODO: start on delay
        Debug.Log("starting action " + name);
        foreach (var tut in tutorialObjects)
        {
            if (tut.obj)
            {
                tut.obj.SetActive(tut.isObjActive);
                if (tut.objLocation)
                {
                    tut.obj.transform.position = tut.objLocation.position;
                    tut.obj.transform.rotation = tut.objLocation.rotation;
                    Debug.Log($"moving {tut.obj.name} to {tut.objLocation.name}");
                }
            }

            if (tut.isAnimated)
            {
                Animator anim = tut.obj.GetComponent<Animator>();
                switch (tut.animtionParameterType)
                {
                    case AnimatorControllerParameterType.Bool:
                        bool boolVal = anim.GetBool(tut.animationName);
                        anim.SetBool(tut.animationName, !boolVal);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        anim.SetTrigger(tut.animationName);
                        break;
                    default:
                        break;
                }
            }

            if (tut.audioClip)
            {
                AudioSource audio = tut.obj.GetComponent<AudioSource>();
                audio.clip = tut.audioClip;
                audio.Play();

                SpeechBubble speechBubble = tut.obj.GetComponentInChildren<SpeechBubble>(true);
                if (speechBubble)
                    speechBubble.ShowText(tut.subtitle);//, tut.audioClip.length);
            }
        }

        foreach (var condition in successConditions)
        {
            condition.Activate();
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
        isSuccess = true;
        foreach (var condition in successConditions)
        {
            if (condition.IsComplete())
            {
                successCt++;
                isSuccess &= condition.Successful;
            }
        }
        return successCt >= successRequired;
    }

    public TutorialAction GetNextAction()
    {
        TutorialAction nextAction = null;
        if (isSuccess)
            nextAction = onSuccessAction;
        else 
            nextAction = onFailAction;
        //if (nextAction != null)
        //    Debug.Log("Get next action " + nextAction.name);
        return nextAction;
    }
}
