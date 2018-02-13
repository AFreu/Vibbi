using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentHandler : MonoBehaviour {

    //private GameObject cloth;

    private GameObject cloth;
    private DeformObject deformObject;

    public Material garmentMaterial;
    public DeformManager deformManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadCloth(GameObject o)
    {
        cloth = new GameObject("Garment");
        deformObject = cloth.AddComponent<DeformObject>() as DeformObject;
        
        Mesh mesh = Mesh.Instantiate(o.GetComponent<MeshFilter>().sharedMesh) as Mesh;

        
        //Debug.Log("Mesh" + mesh.vertices[0]);
        deformObject.material = garmentMaterial;
        deformObject.originalMesh = mesh;

        //deformObject.Build();


        cloth.transform.parent = transform;
        cloth.transform.position = transform.position;


        //cloth = GameObject.Instantiate(o);
        //cloth.transform.position = transform.position;


    }

    public void Simulate()
    {
        if (cloth == null) return;

        Instantiate(cloth, new Vector3(-0.09f, -0.01f, 4.6f), Quaternion.identity);

       // deformManager.Reset();

        //GameObject go = new GameObject("hej");
        //go.AddComponent<DeformObject>();

    }
}
