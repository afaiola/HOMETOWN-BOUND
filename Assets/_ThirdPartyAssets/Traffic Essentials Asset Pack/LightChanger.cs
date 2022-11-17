using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChanger : MonoBehaviour
{
    [SerializeField] public Material offMat, posMat, negMat, transMat;
    [SerializeField] public int posMatIdx;
    [SerializeField] public int negMatIdx;
    [SerializeField] public int transMatIdx = -1;

    [SerializeField] public MeshRenderer meshRend;
    [SerializeField] public bool isOn;
    private AudioSource audioSource;
    public LightChanger companion;

    // Start is called before the first frame update
    void Start()
    {
        //renderer = GetComponentInChildren<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateIndices()
    {
        // restore current LOD to 
        // get current mesh rend
        for (int i = 0; i < meshRend.materials.Length; i++)
        {
            if (meshRend.materials[i].name == posMat.name)
            {
                posMatIdx = i;
            }
            else if (meshRend.materials[i].name == negMat.name)
            {
                negMatIdx = i;
            }
            else if (transMat != null)
            {
                if (transMat.name == meshRend.materials[i].name)
                    transMatIdx = i;
            }

        }
    }

    public void ChangeLight(bool on)
    {
        isOn = on;
        if (audioSource)
        {
            PlayAudio();
        }
        StartCoroutine(ChangeLightRoutine(on));
    }

    private IEnumerator ChangeLightRoutine(bool turningOn)
    {
        Material[] currMats = meshRend.materials;
        if (transMatIdx > -1 && !turningOn)
        {
            currMats[negMatIdx] = offMat;
            currMats[posMatIdx] = offMat;
            currMats[transMatIdx] = transMat;
            meshRend.materials = currMats;
            yield return new WaitForSeconds(1f);

            currMats[transMatIdx] = offMat;
            meshRend.materials = currMats;
        }

        yield return new WaitForEndOfFrame();

        currMats[posMatIdx] = turningOn ? posMat : offMat;
        currMats[negMatIdx] = !turningOn ? negMat : offMat;
        meshRend.materials = currMats;
    }

    public void PlayAudio()
    {
        StartCoroutine(AudioDelay());
    }

    public void StopAudio()
    {
        if (audioSource)
            audioSource.Stop();
    }

    private IEnumerator AudioDelay()
    {
        if (isOn)
        {
            audioSource.Stop();
            audioSource.Play();
            if (companion != null)
            {
                companion.StopAudio();
                yield return new WaitForSeconds(0.5f);
                companion.PlayAudio();
            }
        }
        else
        {
            StopAudio();
        }
    }
}
