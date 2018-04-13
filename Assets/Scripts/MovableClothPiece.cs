using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableClothPiece : MonoBehaviour {

	//Camera for input
	private Camera cam;

	//The axis which this object should rotate around
	private Vector3 rotationAxis;

	//Used to determine the input angle
	Vector3 currentRotationDirection;

	protected InteractionStateManager interactionStateManager;

	void Awake() {
		cam = Camera.allCameras [1];
		interactionStateManager = Component.FindObjectOfType<InteractionStateManager> ();

	}

	void Start(){
		rotationAxis = transform.InverseTransformDirection(Vector3.forward);
	}

	void OnMouseDown(){

		//Save current rotation direction
		currentRotationDirection = GetRotationDirection();

	}

	void OnMouseDrag(){

		 if (interactionStateManager.currentState == InteractionStateManager.InteractionState.SELECT && !Input.GetKey(KeyCode.LeftAlt)) {
			Move ();
		}
	}

	void Move(){


		var newPosition = GetRotationDirection () - currentRotationDirection + transform.position;

		transform.position = newPosition;
        GetComponentInParent<ClothPieceBehaviour>().originalPosition = transform.position;


	}
		

	Vector3 GetRotationDirection(){

		//Calculate direction to mouse from position of object
		Vector3 directionToMouse =  MouseWorldPosition () - transform.position;

		return Vector3.ProjectOnPlane (directionToMouse, rotationAxis);

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
