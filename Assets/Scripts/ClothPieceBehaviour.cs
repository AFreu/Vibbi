using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPieceBehaviour : Behaviour {


	public GarmentHandler garmentHandler;

    public Vector3 originalPosition { set; get; }
    public Quaternion originalRotation { set; get; }
    public int id { set; get; }
    public bool isBent { set; get; }

    public Mesh initialMesh { set; get; }

    private MeshCollider meshCollider;
    

	void Awake(){
		garmentHandler = FindObjectOfType<GarmentHandler> ();
    }

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
       // initialMesh = meshCollider.sharedMesh;
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
