using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem;

public class HandAnimatorManagerVR : MonoBehaviour
{
	public StateModel[] stateModels;
	Animator handAnimator;

	public int currentState = 100;
	int lastState = -1;

	public bool action = false;
	public bool hold = false;

	[SerializeField] InputActionReference buttonAction;
	[SerializeField] InputActionReference triggerAction;
	[SerializeField] InputActionReference gripAction;

	public int numberOfAnimations = 8;

	// Use this for initialization
	void Start ()
	{
		handAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (gripAction.action.ReadValue<float>() > 0.1f)
			hold = true;
		else
			hold = false;

		if (triggerAction.action.ReadValue<float>() > 0.1f)
			action = true;
		else
			action = false;


		if (lastState != currentState) {
			lastState = currentState;
			handAnimator.SetInteger ("State", currentState);
			TurnOnState (currentState);
		}

		handAnimator.SetBool ("Action", action);
		//handAnimator.SetBool ("Hold", hold);

	}

	void TurnOnState (int stateNumber)
	{
		foreach (var item in stateModels) {
			if (item.go == null) continue;
			if (item.stateNumber == stateNumber && !item.go.activeSelf)
				item.go.SetActive (true);
			else if (item.go.activeSelf)
				item.go.SetActive (false);
		}
	}
}

