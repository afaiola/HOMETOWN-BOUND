using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// This script represents an excercise within a module. This script manages the excercises in modules 1 - 3 in CandyLand.
/// It's better if you didn't refactor the class name because all the corresponding scripts in the inspector will go missing.
/// </summary>
public class Exercise : MonoBehaviour
{
    public int exerciseID;
    public int score;
    public double maxTime;
    public GameObject moduleButton;
    public List<ModuleButton> buttons;
    public RawImage leftObject;
    public string nameOfObject;
    public Texture2D leftImage;
    public List<Texture2D> images;
    public List<ModuleButton> selected;
    public int _correctCount;
    public int _incorrectCount;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && Debug.isDebugBuild)
        {
            Success();
        }
    }

    /// <summary>
    /// Whether the excercise is completed
    /// </summary>
    public virtual bool CheckSuccess
    {
        get
        {
            return selected.Count == buttons.Count(z => z.correct);
        }
    }

    protected virtual void OnValidate()
    {
        leftObject = GetComponentInChildren<RawImage>();
        if (images.Count(z => z == leftImage) < 3)
            Debug.LogWarning("Not enough matching images for " + name);
        if (images.Count(z => z == leftImage) > 3)
            Debug.LogWarning("Too many matching images for " + name);
    }
    /// <summary>
    /// Arranges the UI elements in the window. Use override to create new types of excercises.
    /// </summary>
    public virtual void Arrange()
    {
        if (leftImage == null) return;
        buttons = new List<ModuleButton>();
        leftObject.texture = leftImage;
        var btns = GetComponentsInChildren<Button>();
        for (int i = 0; i < btns.Length; i++)
        {
            var mb = btns[i].GetComponent<ModuleButton>();
            mb.Set(this, images[i], images[i] == leftImage);
            buttons.Add(mb);
        }
    }

    /// <summary>
    /// When one of the images are clicked, checks to see if the excercise if finished
    /// </summary>
    /// <param name="mb"></param>
    public virtual bool Select(ModuleButton mb)
    {
        if (mb.correct)
        {
            selected.Add(mb);
            _correctCount++;
        }
        else _incorrectCount++;

        if (CheckSuccess)
        {
            StartCoroutine(Wait(2, Success));
        }

        return true;
    }

    protected virtual IEnumerator Wait(int duration, Action after)
    {
        DeactivateButtons();
        yield return new WaitForSeconds(.5f);
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.5f);
        after();
        ActivateButtons();
    }

    private void ActivateButtons()
    {
        foreach(ModuleButton mb in buttons){
            mb.ActivateButton();
        }
    }

    private void DeactivateButtons()
    {
         foreach(ModuleButton mb in buttons){
            mb.DeactivateButton();
        }
    }

    /// <summary>
    /// Tell the parent to advance to the next excercise
    /// </summary>
    protected void Success()
    {
        GetComponentInParent<Module>().Advance();
    }

    public virtual void Cleanup()
    {
        // implemented by other inheriting classes
    }

}
