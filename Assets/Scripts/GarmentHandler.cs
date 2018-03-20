﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentHandler : MonoBehaviour {

    public Material garmentMaterial;
    public DeformManager deformManager;

    public AttachmentPointsHandler attachMentPointsHandler;

    public List<GameObject> clothPieces = new List<GameObject>();
	private List<GameObject> garmentSeams = new List<GameObject>();
    

	
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
        go.transform.position = t.position;
        go.transform.forward = t.up;
    }

	public void LoadSeam(GameObject seam){
		Debug.Log ("Load Seam");
		var sb = seam.GetComponent<SeamBehaviour> ();
		int firstLineMeshIndex = -1;
		int secondLineMeshIndex = -1;
		bool firstMeshFound = false;
		bool secondMeshFound = false;
		for (int index = 0; index < clothPieces.Count; index++) {
			if (clothPieces[index].GetComponent<MeshFilter> ().sharedMesh.Equals (sb.GetFirstMesh ())) {
				Debug.Log ("Mesh 1 is previously loaded");
				firstLineMeshIndex = index;
				firstMeshFound = true;
			}

			if (clothPieces[index].GetComponent<MeshFilter> ().sharedMesh.Equals (sb.GetSecondMesh ())) {
				Debug.Log ("Mesh 2 is previously loaded");
				secondLineMeshIndex = index;
				secondMeshFound = true;
			}
		
		}

		if (firstMeshFound && secondMeshFound) {
			List<int> LineVerticeIndices = VibbiMeshUtils.DefineSeamFromLines (sb.GetFirstLine (), sb.GetSecondLine()); 
			//List<int> secondLineVerticeIndices = VibbiMeshUtils.VerticesFromLine (sb.GetSecondLine());
			if (LineVerticeIndices.Count <= 0 ) {
				Debug.Log ("Seam edge contains 0 vertices, aborting!");
				return;
			}
			CreateSeam (firstLineMeshIndex, secondLineMeshIndex, LineVerticeIndices);
		}
	}

	private void CreateSeam(int firstClothPieceIndex, int secondClothPieceIndex, List<int> lineVerticeIndices){
		GameObject garmentSeam = new GameObject ("GarmentSeam");
		garmentSeam.transform.parent = transform;
		var seam = garmentSeam.AddComponent<GarmentSeamBehaviour> ();
		seam.Init (firstClothPieceIndex, secondClothPieceIndex, lineVerticeIndices, clothPieces[firstClothPieceIndex], clothPieces[secondClothPieceIndex]);
	
		garmentSeams.Add(garmentSeam);

	}

    private IDictionary<int, int> idToPositonInList = new Dictionary<int, int>();
    private int totalNumberOfVertices = 0;

    public void StartSimulation()
    {
        foreach(GameObject o in clothPieces)
         {
            Mesh mesh = o.GetComponent<MeshFilter>().sharedMesh;
            DeformObject deformObject = o.AddComponent<DeformObject>();

            deformObject.originalMesh = mesh;
            deformObject.material = garmentMaterial;
            deformObject.AddToSimulation();
        }
        //seams
        foreach (GameObject seam in garmentSeams)
        {

            int id1 = seam.GetComponent<GarmentSeamBehaviour>().firstClothPiece.GetComponent<DeformObject>().GetId();
            int id2 = seam.GetComponent<GarmentSeamBehaviour>().secondClothPiece.GetComponent<DeformObject>().GetId();

            Debug.Log(id1);
            Debug.Log(id2);
        }
        deformManager.Reset();

    }

    public void setIDs()
    {
        //gå baklänges
        for (int i = clothPieces.Count-1; i > -1; i--)
        {
            idToPositonInList.Add(clothPieces[i].GetComponent<DeformObject>().GetId(), totalNumberOfVertices); //so that we can get global index when sewing
            totalNumberOfVertices += clothPieces[i].GetComponent<MeshFilter>().sharedMesh.vertexCount;  
        }
    }
		
}
