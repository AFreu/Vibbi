using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentSeamBehaviour : MonoBehaviour {

	public int firstMesh { set; get;}
	public int secondMesh{ set; get;}

	public GameObject firstClothPiece { set; get;}
	public GameObject secondClothPiece{ set; get;}

	public List<int> lineVerticeIndices { set; get;}

	private List<GameObject> connections = new List<GameObject>();

	
	// Update is called once per frame
	void Update () {
		
		UpdateConnections ();
	}

	void UpdateConnections(){


		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;


		for (int i = 0; i < connections.Count; i++) {


			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [2 * i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [lineVerticeIndices [2 * i + 1]]);

			var lineRenderer = connections[i].GetComponent<LineRenderer> ();
			lineRenderer.SetPosition (0, start);
			lineRenderer.SetPosition (1, end);
		}
	}

	public void Init(int firstMesh, int secondMesh, List<int> lineVerticeIndices, GameObject firstClothPiece, GameObject secondClothPiece){
		this.firstMesh = firstMesh;
		this.secondMesh = secondMesh;
		this.lineVerticeIndices = lineVerticeIndices;
		this.firstClothPiece = firstClothPiece;
		this.secondClothPiece = secondClothPiece;

		InitConnections ();


	}

	private void InitConnections(){

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		for (int i = 0; i < lineVerticeIndices.Count; i = i+2) {


			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [lineVerticeIndices [i+1]]);

			connections.Add(VibbiUtils.CreateLine (start, end, Color.red, transform));

		}

	}

}
