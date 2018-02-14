﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointBehaviour : MonoBehaviour{

	private Ray mousePositionRay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		mousePositionRay = Camera.main.ScreenPointToRay (Input.mousePosition);
	}

	void OnMouseOver(){
		
		if (Input.GetKeyUp (KeyCode.D)) {
			gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().RemoveBoundaryPoint(gameObject);
		}

	}

	void OnMouseDrag() {

		int layerMask = 1 << 8;

		RaycastHit hit;
		if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
			
			gameObject.transform.position = hit.point;
		}else {
			Debug.Log ("RAY missed modelplane");
		}
	}

	/*public GameObject Copy(Transform parent){
		GameObject copy = Instantiate (gameObject, parent) as GameObject;
		copy.transform.position = transform.position;

		return copy;
	}*/
}
