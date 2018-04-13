using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPieceBehaviour : Behaviour {


	public GarmentHandler garmentHandler;

    public Vector3 originalPosition { set; get; }
    public Quaternion originalRotation { set; get; }


    void Awake(){
		garmentHandler = FindObjectOfType<GarmentHandler> ();
	}

	void OnMouseUp(){
		if (Input.GetKey (KeyCode.D)) {
			garmentHandler.UnloadCloth (gameObject);
		}
	}
    
}
