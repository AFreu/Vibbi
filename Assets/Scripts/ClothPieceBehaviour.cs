using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPieceBehaviour : Behaviour {


	public GarmentHandler garmentHandler;

    public Mesh initialMesh;

    private MeshCollider meshCollider;

	void Awake(){
		garmentHandler = FindObjectOfType<GarmentHandler> ();
	}

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        initialMesh = meshCollider.sharedMesh;
    }

    void LateUpdate()
    {
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    void OnMouseUp(){
		if (Input.GetKey (KeyCode.D)) {
			garmentHandler.UnloadCloth (gameObject);
		}
	}
}
