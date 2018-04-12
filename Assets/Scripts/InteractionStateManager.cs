using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //toggles
    public GameObject selectToggle;
    public GameObject rotateToggle;
    public GameObject scaleToggle;
    public GameObject newPointToggle;
    public GameObject removePointToggle;
    public GameObject dartToggle;
    public GameObject sewToggle;
    public GameObject unfoldToggle;
    public GameObject duplicateToggle;

	//dropdowns
	public Dropdown textureDropdown;
    

    private void Start()
    {
//        selectToggle = toggleGroup.transform.GetChild(0);
    }

    private void Update()
    {
        HandleInput();

		HandleSelection ();

    }

    private void HandleInput()
    {
        //handle input that makes a toggle change
        if (Input.GetKeyUp(KeyCode.M)) selectToggle.GetComponent<Toggle>().isOn = true; // M for move
        if (Input.GetKeyUp(KeyCode.O)) rotateToggle.GetComponent<Toggle>().isOn = true; // O for shape
        if (Input.GetKeyUp(KeyCode.X)) scaleToggle.GetComponent<Toggle>().isOn = true; // X for shape of icon
        if (Input.GetKeyUp(KeyCode.A)) newPointToggle.GetComponent<Toggle>().isOn = true; // A for add
        if (Input.GetKeyUp(KeyCode.D)) removePointToggle.GetComponent<Toggle>().isOn = true; //D for destroy
        if (Input.GetKeyUp(KeyCode.H)) dartToggle.GetComponent<Toggle>().isOn = true; // H for hole
        if (Input.GetKeyUp(KeyCode.S)) sewToggle.GetComponent<Toggle>().isOn = true; // S for sew
        if (Input.GetKeyUp(KeyCode.U)) unfoldToggle.GetComponent<Toggle>().isOn = true; // U for unfold
        if (Input.GetKeyUp(KeyCode.Z)) duplicateToggle.GetComponent<Toggle>().isOn = true; // Z for copy
    }

	private void HandleSelection(){

		HideAttributes ();

		foreach(Selectable s in GetComponent<SelectableManager>().GetCurrentlySelected()){
			ShowAttributes (s);
		}
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

	public Camera GetCamera(string tag){
		Camera cam = Camera.main;
		switch (tag) {
		case "ClothPiece":
			cam = Camera.allCameras [1];
			break;

		}
		return cam;
	}
		
	public void ShowAttributes(Selectable selectable){
		switch (selectable.tag) {
		case "ClothModel":
			if (textureDropdown == null)
				return;
			//textureDropdown.transform.localScale = Vector3.one;

			break;
		}

	}
		
	public void HideAttributes(){
		if (textureDropdown == null)
			return;
		//textureDropdown.transform.localScale = Vector3.zero;
	}

	public void OnTextureDropdownChange(){

		//Set selected material to all currently selected fabricable objects
		foreach (Selectable s in GetComponent<SelectableManager>().GetCurrentlySelected()) {
			var fabricable = s.gameObject.GetComponent<Fabricable> ();
			if (fabricable != null) {
				fabricable.SetSimulationMaterial (textureDropdown.value);
			}
		}
	}

	public void Selected(Selectable selectable){
		switch (selectable.tag) {
		case "ClothModel":
			if (textureDropdown == null)
				return;
			textureDropdown.onValueChanged.SetPersistentListenerState (0, UnityEngine.Events.UnityEventCallState.Off);
			textureDropdown.value = selectable.GetComponent<Fabricable> ().GetSimulationMaterialIndex ();
			textureDropdown.onValueChanged.SetPersistentListenerState (0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);

			break;
		}
	}

	public int GetValue(string type){
		var value = 0;

		switch (type) {
		case "Selected Material":
                if (textureDropdown != null)
                value = textureDropdown.value;
			break;
		}

		return value;
	}
}
