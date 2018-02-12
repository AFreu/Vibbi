using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentHandler : MonoBehaviour {

    //private GameObject cloth;

    private GameObject cloth;
    private DeformObject deformObject;

    public Material garmentMaterial;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadCloth(GameObject o)
    {
        cloth = new GameObject("Garment");
        deformObject = cloth.AddComponent<DeformObject>();
        
        Mesh mesh = Mesh.Instantiate(o.GetComponent<MeshFilter>().sharedMesh);

        
        Debug.Log("Mesh" + mesh.vertices[0]);
        deformObject.material = garmentMaterial;
        deformObject.originalMesh = mesh;

        cloth.transform.parent = transform;
        cloth.transform.position = transform.position;
        //cloth = GameObject.Instantiate(o);
        //cloth.transform.position = transform.position;

        deformObject.Build();


    }

    public void Simulate()
    {
        if (cloth == null) return;

        GameObject go = GameObject.Instantiate(cloth);

    }
}
