using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModuleButton:MonoBehaviour
{
    public Exercise exercise;
    public RawImage image;
    public Text text;
    public RawImage overlay;
    public bool correct;
    public AudioClip rightClip, wrongClip;
    public Texture2D rightTexture, wrongTexture;

    public void OnValidate()
    {
        text = GetComponentInChildren<Text>();
        image = GetComponentInChildren<RawImage>();
        exercise = GetComponentInParent<Exercise>();
    }

    public void Reset()
    {
        GetComponent<Button>().interactable = true;
        overlay.enabled = false;
    }

    public void Set(Exercise module, Texture2D texture, bool correct)
    {
        overlay.enabled = false;
        GetComponent<Button>().interactable = true;
        this.exercise = module;
        image.texture = texture;
        this.correct = correct;
    }

    public void Set(Exercise module, string text, bool correct)
    {
        this.exercise = module;
        this.text.text = text;
        this.correct = correct;
    }

    public virtual void Click()
    {
        if (UIManager.Instance.paused) return;

        if (!exercise.Select(this)) return;

        var audioSource = GetComponent<AudioSource>();
        GetComponent<Button>().interactable = false;
        if (correct)
        {
            audioSource.clip = rightClip;
            audioSource.Play();
            overlay.texture = rightTexture;
        }
        else
        {
            audioSource.clip = wrongClip;
            audioSource.Play();
            overlay.texture = wrongTexture;
        }

        overlay.enabled = true;
    }
}
