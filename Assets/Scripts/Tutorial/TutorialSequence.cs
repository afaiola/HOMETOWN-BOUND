using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialSequence : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent sequenceComplete;
    // go through a sequence of actions.
    // actions can have success conditions and are reset on failure
    [SerializeField] TutorialAction[] actions;
    private int currAction;
    private bool done;

    private void OnValidate()
    {
        actions = GetComponentsInChildren<TutorialAction>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currAction = -1;
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].id = i;
        }    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (currAction < 0) return;

        if (currAction >= actions.Length)
        {
            // sequence complete!
            if (sequenceComplete != null && !done)
            {
                done = true;
                sequenceComplete.Invoke();
            }
            return;
        }

        if (actions[currAction].IsComplete())
        {
            if (actions[currAction].GetNextAction() != null)
                currAction = actions[currAction].GetNextAction().id;
            else
                currAction++;
            if (currAction < actions.Length)
                actions[currAction].Run();
        }
    }

    public void StartSequence()
    {
        Debug.Log("Starting sequence: " + name);
        currAction = 0;
        actions[0].Run();
    }
}
