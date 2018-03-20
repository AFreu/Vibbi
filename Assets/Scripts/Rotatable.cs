using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour {

	public Camera cam;

	private float angle;

	void Update(){


	}

	void OnMouseDown(){
		Vector3 directionToMouse =  MouseWorldPosition () - transform.position;

		Vector3 projection = Vector3.ProjectOnPlane (directionToMouse, transform.forward);


		angle = Vector3.SignedAngle (transform.right, projection, transform.forward);

		Debug.DrawLine (transform.position, transform.position + projection, Color.blue, 100);
		Debug.DrawLine (transform.position, transform.position + transform.right, Color.red, 100);
		Debug.Log ("angle: " + angle);




	}

	void OnMouseDrag(){
		Rotate ();
	}

	Vector3 MouseWorldPosition(){
		//Get mouse position on screen
		Vector3 mousePos = Input.mousePosition;

		//Adjust mouse position
		mousePos.z =  transform.position.z - cam.transform.position.z;  

		//Get a world position for the mouse
		return cam.ScreenToWorldPoint(mousePos);  
	}


	void Rotate(){
		
		Vector3 directionToMouse =  MouseWorldPosition () - transform.position;

		Vector3 projection = Vector3.ProjectOnPlane (directionToMouse, transform.forward);

		//transform.right = Vector3.Slerp (transform.right, projection, 0.1f); 
		Debug.Log ("oldproj: " +  projection); 
		Debug.Log ("newproj: " + Quaternion.AngleAxis (-angle, transform.forward) * projection); 
		Debug.Log ("right: " + transform.right);


		transform.right = Quaternion.AngleAxis (-angle, transform.forward) * projection;
	}


}
