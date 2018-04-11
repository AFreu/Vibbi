using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour {

	//Camera for input
	private Camera cam;

	//The axis which this object should rotate around
	private Vector3 rotationAxis;

	//Used to determine the input angle
	Vector3 currentRotationDirection;

	protected InteractionStateManager interactionStateManager;

	void Awake() {
		
		interactionStateManager = Component.FindObjectOfType<InteractionStateManager> ();
		cam = interactionStateManager.GetCamera (tag);
	}

	void Start(){
		rotationAxis = transform.InverseTransformDirection(Vector3.forward);
	}

	void OnMouseDown(){
		
		//Save current rotation direction
		currentRotationDirection = GetRotationDirection();

	}

	void OnMouseDrag(){

		if (Input.GetKey(KeyCode.R) || interactionStateManager.currentState == InteractionStateManager.InteractionState.ROTATE) {
			Rotate ();
		} 
	}

	Vector3 GetRotationDirection(){

		//Calculate direction to mouse from position of object
		Vector3 directionToMouse =  MouseWorldPosition () - transform.position;

		return Vector3.ProjectOnPlane (directionToMouse, rotationAxis);

	}


	void Rotate(){

		//Get new rotation direction according to mouse
		var newRotationDirection = GetRotationDirection ();

		//Calculate angle between old and new rotation direction
		var rotationAngle = Vector3.SignedAngle (currentRotationDirection, newRotationDirection, rotationAxis);

		//Rotate object according to angle
		transform.RotateAround (transform.position, rotationAxis, rotationAngle);

		//Update current rotation direction
		currentRotationDirection = newRotationDirection;

	}

	Vector3 MouseWorldPosition(){
		
		//Get mouse position on screen
		Vector3 mousePos = Input.mousePosition;

		//Adjust mouse position
		mousePos.z =  (transform.position - cam.transform.position).magnitude;  

		//Get a world position for the mouse
		return cam.ScreenToWorldPoint(mousePos);

	}

}
