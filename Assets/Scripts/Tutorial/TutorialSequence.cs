using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSequence : MonoBehaviour
{
    // go through a sequence of actions.
    // actions can have success conditions and are reset on failure

    [SerializeField] TutorialAction[] actions;
    private int currAction;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].id = i;
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (currAction >= actions.Length)
        {
            // sequence complete!
            return;
        }

        if (actions[currAction].IsComplete())
        {
            if (actions[currAction].nextAction != null)
                currAction = actions[currAction].nextAction.id;
            else
                currAction++;
            actions[currAction].Run();
        }
    }
}
