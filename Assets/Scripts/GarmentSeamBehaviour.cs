using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentSeamBehaviour : MonoBehaviour {

	public int firstMesh { set; get;}
	public int secondMesh{ set; get;}

	public GameObject firstClothPiece { set; get;}
	public GameObject secondClothPiece{ set; get;}

	public List<int> firstMeshVertices { set; get;}
	public List<int> secondMeshVertices { set; get;}

	private List<GameObject> connections = new List<GameObject>();

	
	// Update is called once per frame
	void Update () {
		
		UpdateConnections ();
	}

	void UpdateConnections(){


		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;


		for (int i = 0; i < connections.Count; i++) {

			int j = i;
			if (j >= secondMeshVertices.Count) {
					j = secondMeshVertices.Count - 1;
			}

			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [firstMeshVertices [i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [secondMeshVertices [j]]);

			var lineRenderer = connections[i].GetComponent<LineRenderer> ();
			lineRenderer.SetPosition (0, start);
			lineRenderer.SetPosition (1, end);
		}
	}

	public void Init(int firstMesh, int secondMesh, List<int> firstMeshVertices, List<int> secondMeshVertices, GameObject firstClothPiece, GameObject secondClothPiece){
		this.firstMesh = firstMesh;
		this.secondMesh = secondMesh;
		this.firstMeshVertices = firstMeshVertices;
		this.secondMeshVertices = secondMeshVertices;
		this.firstClothPiece = firstClothPiece;
		this.secondClothPiece = secondClothPiece;

		InitConnections ();


	}

	private void InitConnections(){

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		for (int i = 0; i < firstMeshVertices.Count; i++) {
			int j = i;
			if (j >= secondMeshVertices.Count) {
				j = secondMeshVertices.Count - 1;
			}

			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [firstMeshVertices [i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [secondMeshVertices [j]]);

			connections.Add(VibbiUtils.CreateLine (start, end, Color.red, transform));

		}
	}

}
