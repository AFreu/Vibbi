using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class BoundaryLineBehaviour : Behaviour{

	public Transform first;
	public Transform second;

	public Vector3 unitVector;
    

	// Update is called once per frame
	void Update () {
		if (first == null || second == null)
			return;
		UpdateLine ();

	}

	Vector3 MouseWorldPosition(){

		//Get mouse position on screen
		Vector3 mousePos = Input.mousePosition;

		//Adjust mouse position 
		mousePos.z = transform.position.z - Camera.main.transform.position.z;

		//Get a world position for the mouse
		return Camera.main.ScreenToWorldPoint(mousePos);

	}

	void OnMouseUp(){

		var hit = MouseWorldPosition ();

		if (Input.GetKey (KeyCode.A) || interactionStateManager.currentState == InteractionStateManager.InteractionState.ADDPOINT) {
			GetComponentInParent<BoundaryPointsHandler> ().AddPoint(gameObject, hit);

		}else if(Input.GetKey(KeyCode.U) || interactionStateManager.currentState == InteractionStateManager.InteractionState.UNFOLDCLOTH){
			GetComponentInParent<BoundaryPointsHandler> ().Unfold(gameObject);

		}else if(Input.GetKey(KeyCode.K) || interactionStateManager.currentState == InteractionStateManager.InteractionState.SEW){
            //switch first and second depending on hit position
			//shortest way to which point?
			Transform tmp = first;
			if ((first.position -hit).magnitude > (second.position - hit).magnitude) //if the length between first point and clicked point is larger than the second point and clicked point
			{
				first = second;
				second = tmp;
			}
            //init sewing
            GetComponentInParent<ClothModelHandler>().InitSewing(this.gameObject);


		}
	}

	void UpdateLine(){
		var start = first.transform.position;
		var end = second.transform.position;

		var parentScaleCompensation = transform.parent.transform.localScale.x;
		var offset = end - start;
		var scale = new Vector3(offset.magnitude/parentScaleCompensation, 1, 1);
		var position = start;

		//Save unit vector for other uses
		unitVector = offset.normalized;

		transform.position = position;
		transform.right = offset;
		transform.localScale = scale;
	}
}
