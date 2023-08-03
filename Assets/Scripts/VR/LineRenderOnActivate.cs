using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LineRenderOnActivate : MonoBehaviour
{
    [SerializeField] private InputActionReference activateAction;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private XRInteractorLineVisual lineVisual;

    private void Start()
    {
        activateAction.action.Enable();
        
        activateAction.action.started += OnActivate;
        //activateAction.action.performed += context => Debug.Log($"{context.action} performed");
        activateAction.action.canceled += OnDeactivate;

        OnDeactivate(new InputAction.CallbackContext());
    }

    private void Update()
    {
        //if (!activateAction.enabled) return;
    }

    public void OnActivate(InputAction.CallbackContext context)
    {
        if (lineRenderer == null) return;
        lineRenderer.enabled = true;
        lineVisual.enabled = true;
    }

    public void OnDeactivate(InputAction.CallbackContext context)
    {
        if (lineRenderer == null) return;
        lineRenderer.enabled = false;
        lineVisual.enabled = false;

    }
}
