using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPieceBehaviour : Behaviour {


	public GarmentHandler garmentHandler;

	void Awake(){
		garmentHandler = FindObjectOfType<GarmentHandler> ();
	}

	void OnMouseUp(){
		if (Input.GetKey (KeyCode.D)) {
			garmentHandler.UnloadCloth (gameObject);
		}
	}
}
