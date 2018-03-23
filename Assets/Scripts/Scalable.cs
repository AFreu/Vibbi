using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalable : Behaviour {

	public float horizontalSpeed = 2.0F;
	public float verticalSpeed = 2.0F;

	private Vector3 offset;
	private Vector3 currentScale;
		

	void OnMouseDrag(){

		if (interactionStateManager.currentState != InteractionStateManager.InteractionState.SCALE)
			return;

		Vector3 newLocalScale = transform.localScale;
		float X = horizontalSpeed * Input.GetAxis("Mouse X");
		float Y = verticalSpeed * Input.GetAxis("Mouse Y");

		newLocalScale.x += X;
		newLocalScale.y += Y;

		transform.localScale = newLocalScale;
	}

	Vector3 MouseWorldPosition(){

		//Get mouse position on screen
		Vector3 mousePos = Input.mousePosition;

		//Adjust mouse position 
		mousePos.z = transform.position.z - Camera.main.transform.position.z;

		//Get a world position for the mouse
		return Camera.main.ScreenToWorldPoint(mousePos);

	}
}
