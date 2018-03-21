using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoundaryLineBehaviour : SimpleLineBehaviour{


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
}
