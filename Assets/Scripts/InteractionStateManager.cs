using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionStateManager : MonoBehaviour {

	public enum InteractionState {
		SELECT,
		ADDPOINT,
		REMOVEPOINT,
		DUPLICATECLOTH,
		UNFOLDCLOTH,
		REMOVECLOTH,
		SEW,
		ROTATE,
		NONE
	}

	public InteractionState currentState;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void OnToggleChange(GameObject toggle){
		Debug.Log ("Toggle: " + toggle.tag);
		var newState = GetInteractionState (toggle.tag);

		if (newState != currentState) {
			currentState = newState;
		} else {
			currentState = InteractionState.NONE;
		}
	}

	private InteractionState GetInteractionState(string tag){
		InteractionState temp = InteractionState.NONE;
		switch (tag) {
		case "Select":
			temp = InteractionState.SELECT;
			break;
		case "AddPoint":
			temp = InteractionState.ADDPOINT;
			break;
		case "RemovePoint":
			temp = InteractionState.REMOVEPOINT;
			break;
		case "UnfoldCloth":
			temp = InteractionState.UNFOLDCLOTH;
			break;
		case "DuplicateCloth":
			temp = InteractionState.DUPLICATECLOTH;
			break;
		case "RemoveCloth":
			temp = InteractionState.REMOVECLOTH;
			break;
		case "Sew":
			temp = InteractionState.SEW;
			break;
		case "Rotate":
			temp = InteractionState.ROTATE;
			break;
		}
		return temp;
	}
}
