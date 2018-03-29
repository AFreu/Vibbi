using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentSeamBehaviour : MonoBehaviour {

	public int firstMesh { set; get;}
	public int secondMesh{ set; get;}

	public GameObject firstClothPiece { set; get;}
	public GameObject secondClothPiece{ set; get;}

	public List<int> lineVerticeIndices { set; get;}

	private GameObject seam;


	// Update is called once per frame
	void Update () {
		UpdateLineRenderer ();
	}


	//Called when triangulating
	public void UpdateIndices(){

		lineVerticeIndices = VibbiMeshUtils.DefineSeamFromLines (seam.GetComponent<SeamBehaviour>().GetFirstLine (), seam.GetComponent<SeamBehaviour>().GetSecondLine()); 

	}


	private void UpdateLineRenderer(){
		var renderer = GetComponent<LineRenderer> ();

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		var count = lineVerticeIndices.Count;

		renderer.positionCount = count;

		Vector3 [] positions = new Vector3[count];
		bool alt = true;
		for (int i = 0; i < count; i = i+2) {


			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [lineVerticeIndices [i+1]]);

			if (alt) {
				alt = false;
				positions [i] = start;
				positions [i + 1] = end;
			} else {
				alt = true;
				positions [i] = end;
				positions [i + 1] = start;
			}
		}
			
		renderer.SetPositions (positions);
	}

	public void Init(int firstMesh, int secondMesh, List<int> lineVerticeIndices, GameObject firstClothPiece, GameObject secondClothPiece, GameObject seam){
		this.firstMesh = firstMesh;
		this.secondMesh = secondMesh;
		this.lineVerticeIndices = lineVerticeIndices;
		this.firstClothPiece = firstClothPiece;
		this.secondClothPiece = secondClothPiece;

		this.seam = seam;
	}

}
