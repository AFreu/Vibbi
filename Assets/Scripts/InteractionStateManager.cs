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
		DART,
		SCALE,
		NONE
	}

	public enum SimulationState {
		SIMULATING,
		PAUSED,
		STOPPED
	}

	public InteractionState currentState;
	public SimulationState simulationState;


	public void OnToggleChange(GameObject toggle){
		Debug.Log ("Toggle: " + toggle.tag);
		var newState = GetInteractionState (toggle.tag);

		if (newState != currentState) {
			currentState = newState;
		} else {
			currentState = InteractionState.NONE;
		}
	}

	public void OnSimulationChange(GameObject toggle){
		Debug.Log ("Simulation: " + toggle.tag);
		var newState = GetSimulationState (toggle.tag);

		if (newState != simulationState) {
			simulationState = newState;
		} else {
			simulationState = SimulationState.STOPPED;
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
		case "Dart":
			temp = InteractionState.DART;
			break;
		case "Scale":
			temp = InteractionState.SCALE;
			break;
		}
		return temp;
	}

	private SimulationState GetSimulationState(string tag){
		SimulationState temp = SimulationState.STOPPED;
		switch (tag) {
		case "Simulating":
			temp = SimulationState.SIMULATING;
			break;
		case "Paused":
			temp = SimulationState.PAUSED;
			break;
		case "Stopped":
			temp = SimulationState.STOPPED;
			break;
		
		}
		return temp;
	}
}
