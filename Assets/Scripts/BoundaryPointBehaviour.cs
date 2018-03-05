using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointBehaviour : MonoBehaviour{

	public GameObject line;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver(){
		
		if (Input.GetKeyUp (KeyCode.D)) {
			gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().RemovePoint(gameObject);
		}

	}
}
