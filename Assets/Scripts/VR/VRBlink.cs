using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; 

// adjusts parameters of vignette shader to make blink effect
public class VRBlink : MonoBehaviour
{

    private MeshRenderer m_MeshRender;
    MeshFilter m_MeshFilter;
    Material m_SharedMaterial;

    MaterialPropertyBlock m_VignettePropertyBlock;
    VignetteParameters m_CurrentParameters;

    static class ShaderPropertyLookup
    {
        public static readonly int apertureSize = Shader.PropertyToID("_ApertureSize");
        public static readonly int featheringEffect = Shader.PropertyToID("_FeatheringEffect");
        public static readonly int vignetteColor = Shader.PropertyToID("_VignetteColor");
        public static readonly int vignetteColorBlend = Shader.PropertyToID("_VignetteColorBlend");
    }

    const string k_DefaultShader = "VR/TunnelingVignette";

    public void Initialize()
    {
        m_CurrentParameters = new VignetteParameters();
        SetAperatureSize(0);
    }

    bool TrySetUpMaterial()
    {
        if (m_MeshRender == null)
            m_MeshRender = GetComponent<MeshRenderer>();
        if (m_MeshRender == null)
            m_MeshRender = gameObject.AddComponent<MeshRenderer>();

        if (m_VignettePropertyBlock == null)
            m_VignettePropertyBlock = new MaterialPropertyBlock();

        if (m_MeshFilter == null)
            m_MeshFilter = GetComponent<MeshFilter>();
        if (m_MeshFilter == null)
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();

        if (m_MeshFilter.sharedMesh == null)
        {
            Debug.LogWarning("The default mesh for the TunnelingVignetteController is not set. " +
                "Make sure to import it from the Tunneling Vignette Sample of XR Interaction Toolkit.", this);
            return false;
        }

        if (m_MeshRender.sharedMaterial == null)
        {
            var defaultShader = Shader.Find(k_DefaultShader);
            if (defaultShader == null)
            {
                Debug.LogWarning("The default material for the TunnelingVignetteController is not set, and the default Shader: " + k_DefaultShader
                    + " cannot be found. Make sure they are imported from the Tunneling Vignette Sample of XR Interaction Toolkit.", this);
                return false;
            }

            Debug.LogWarning("The default material for the TunnelingVignetteController is not set. " +
                "Make sure it is imported from the Tunneling Vignette Sample of XR Interaction Toolkit. + " +
                "Try creating a material using the default Shader: " + k_DefaultShader, this);

            m_SharedMaterial = new Material(defaultShader)
            {
                name = "DefaultTunnelingVignette",
            };
            m_MeshRender.sharedMaterial = m_SharedMaterial;
        }
        else
        {
            m_SharedMaterial = m_MeshRender.sharedMaterial;
        }

        return true;
    }

    void UpdateTunnelingVignette(VignetteParameters parameters)
    {
        if (TrySetUpMaterial())
        {
            m_MeshRender.GetPropertyBlock(m_VignettePropertyBlock);
            m_VignettePropertyBlock.SetFloat(ShaderPropertyLookup.apertureSize, parameters.apertureSize);
            m_VignettePropertyBlock.SetFloat(ShaderPropertyLookup.featheringEffect, parameters.featheringEffect);
            m_VignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColor, parameters.vignetteColor);
            m_VignettePropertyBlock.SetColor(ShaderPropertyLookup.vignetteColorBlend, parameters.vignetteColorBlend);
            m_MeshRender.SetPropertyBlock(m_VignettePropertyBlock);
        }

        // Update the Transform y-position to match apertureVerticalPosition
        var thisTransform = transform;
        var localPosition = thisTransform.localPosition;
        if (!Mathf.Approximately(localPosition.y, parameters.apertureVerticalPosition))
        {
            localPosition.y = parameters.apertureVerticalPosition;
            thisTransform.localPosition = localPosition;
        }

        m_CurrentParameters = parameters;
    }

    public void SetAperatureSize(float value)
    {
        m_CurrentParameters.apertureSize = value;
        m_CurrentParameters.featheringEffect = value;
        UpdateTunnelingVignette(m_CurrentParameters);
    }
}
