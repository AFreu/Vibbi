using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointBehaviour : MonoBehaviour {

	public float speedX;
	public float speedY;

	private Ray mousePositionRay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		mousePositionRay = Camera.main.ScreenPointToRay (Input.mousePosition);
	}

	void OnMouseEnter(){
		Debug.Log ("Enter Handle");
	}

	void OnMouseDrag() {

		int layerMask = 1 << 8;

		RaycastHit hit;
		if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
			
			gameObject.transform.position = hit.point;
		}else {
			Debug.Log ("RAY missed modelplane");
		}
		//float xPos = gameObject.transform.position.x;
		//float yPos = gameObject.transform.position.x;

		//float dX = Input.GetAxis ("Mouse X") * speedX;
		//float dY = Input.GetAxis ("Mouse Y") * speedY;


		//gameObject.transform.Translate (new Vector3 (dX, dY, 0f));
		//gameObject.transform.position.x += Input.GetAxis("Mouse X");
		//gameObject.transform.position.y += Input.GetAxis ("Mouse Y");


	}
}
