using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

	public Material selectedMaterial;
	public Material highlightMaterial;
	public Material normalMaterial;


	private bool highlighted = false;
	private bool selected = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//bool leftMouseButtonDown = Input.GetMouseButtonDown (0);
		bool leftMouseButtonUp = Input.GetMouseButtonUp (0);
		bool leftShift = Input.GetKey (KeyCode.LeftShift);

		if (highlighted && leftMouseButtonUp)
			Select (true);

		if (!highlighted && leftMouseButtonUp && !leftShift)
			Select (false);

	}
		

	void OnMouseEnter(){
		Highlight (true);
	
	}

	void OnMouseExit(){
		Highlight (false);
	}

	void Highlight(bool on){
		highlighted = on;
		if (selected)
			return;
		if (on) {
			GetComponent<Renderer> ().material = highlightMaterial;
		} else {
			GetComponent<Renderer> ().material = normalMaterial;
		}
	}

	void Select(bool on){
		selected = on;
		if (on) {
			GetComponent<Renderer> ().material = selectedMaterial;
		} else {
			GetComponent<Renderer> ().material = normalMaterial;
		}
	}
		
}
