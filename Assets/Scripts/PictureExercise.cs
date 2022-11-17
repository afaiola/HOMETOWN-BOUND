using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT DOING PICTURE MODULES ANYMORE BUT THIS MIGHT BE NICE TO HAVE
public class PictureExercise : ScavengerExercise
{
    public SelectablePicture picture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Arrange()
    {
        goalObj = picture.gameObject;
        picture.moduleButton.Set(this, leftImage, true) ;

        //AddInteractable();
    }

    protected override IEnumerator Wait(int duration, Action after)
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        after.Invoke();
    }

    public override bool CheckSuccess
    {
        get
        {
            return _correctCount == 1;
        }
    }

    public override void Select()
    {
     // do nothing
    }

    public override bool Select(ModuleButton mb)
    {
        if (mb.correct)
        {
            selected.Add(mb);
            _correctCount++;
        }
        else _incorrectCount++;

        if (CheckSuccess)
        {
            //mb.Set(this, leftImage, false);
            StartCoroutine(Wait(2, Success));
        }

        return true;
    }

    public override void Cleanup()
    {
        Destroy(picture.GetComponent<Interact>());
        Destroy(picture.GetComponent<cakeslice.Outline>());
        picture.moduleButton.Set(this, leftImage, false);
    }
}
