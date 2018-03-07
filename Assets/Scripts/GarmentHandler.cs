using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentHandler : MonoBehaviour {

    public Material garmentMaterial;
    public DeformManager deformManager;

    public AttachmentPointsHandler attachMentPointsHandler;

    public List<GameObject> clothPieces = new List<GameObject>();
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.S))
        {
            StartSimulation();
        }
	}



    public void LoadCloth(GameObject cloth)
    {

        GameObject go = new GameObject("A piece of cloth");
        
        go.transform.parent = deformManager.transform.parent;

        Transform t = attachMentPointsHandler.getSelectedAttachmentPoint();
        if(t != null)
        {
            AttachCloth(go, t);
        }
        else
        {
            go.transform.localPosition = new Vector3(0, 5, 0);
            go.transform.localRotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
        }
        
        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.sharedMesh = cloth.GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        renderer.material = garmentMaterial;

        clothPieces.Add(go);
    }

    public void AttachCloth(GameObject go, Transform t)
    {
        go.transform.localPosition = t.localPosition;
        go.transform.forward = t.up;
    }




    public void StartSimulation()
    {
        foreach(GameObject o in clothPieces)
         {
            Mesh mesh = o.GetComponent<MeshFilter>().sharedMesh;
            DeformObject deformObject = o.AddComponent<DeformObject>();
            deformObject.SetMesh(mesh);
            deformObject.SetMaterial(garmentMaterial);
        }
        
        deformManager.Reset();

    }
}
