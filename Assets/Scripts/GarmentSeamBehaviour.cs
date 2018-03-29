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


	public int pooledLines = 40;
	List<GameObject> lines;

	public void InitLinePool(){
		lines = new List<GameObject> ();
		for (int i = 0; i < pooledLines; i++) {

			GameObject line = new GameObject ("Seamline");
			line.AddComponent<LineRenderer> ().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
			line.transform.parent = transform;
			line.SetActive (false);
			lines.Add (line);

		}
	}

	// Update is called once per frame
	void Update () {

		UpdateIndices ();
		DrawConnections ();
	}

	void UpdateIndices(){
		lineVerticeIndices = VibbiMeshUtils.DefineSeamFromLines (seam.GetComponent<SeamBehaviour>().GetFirstLine (), seam.GetComponent<SeamBehaviour>().GetSecondLine()); 
	}


	private void DrawConnections(){

		if (lines == null) {
			InitLinePool ();
		}

		//Deactivate all seam lines
		foreach (GameObject l in lines) {
			l.SetActive (false);
		}

		if (lines.Count < lineVerticeIndices.Count/2) {
			IncreaseLinePoolCapacity ();
		}

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		int j = 0;
		for (int i = 0; i < lineVerticeIndices.Count; i = i+2) {


			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [lineVerticeIndices [i+1]]);

			if (j < lines.Count) {
				UpdateLine (lines[j], start, end, Color.red);
			}
			j++;

		}
	}

	public void Init(int firstMesh, int secondMesh, List<int> lineVerticeIndices, GameObject firstClothPiece, GameObject secondClothPiece, GameObject seam){
		this.firstMesh = firstMesh;
		this.secondMesh = secondMesh;
		this.lineVerticeIndices = lineVerticeIndices;
		this.firstClothPiece = firstClothPiece;
		this.secondClothPiece = secondClothPiece;

		this.seam = seam;




	}

	void UpdateLine(GameObject line, Vector3 start, Vector3 end, Color color){
		
		line.transform.position = start;

		var renderer = line.GetComponent<LineRenderer> ();
		renderer.startColor = color;
		renderer.endColor = color;
		renderer.startWidth = 0.01f;
		renderer.endWidth = 0.01f;
		renderer.SetPosition(0, start);
		renderer.SetPosition(1, end);

		line.SetActive (true);
	}

	void IncreaseLinePoolCapacity (){
		Debug.Log ("Adding extra pooled lines: " + pooledLines);
		for (int i = 0; i < pooledLines; i++) {

			GameObject line = new GameObject ("ExtraSeamline");
			line.AddComponent<LineRenderer> ().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
			line.transform.parent = transform;
			line.SetActive (false);
			lines.Add (line);

		}
	}



	/*private void InitConnections(){

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		for (int i = 0; i < lineVerticeIndices.Count; i = i+2) {


			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [lineVerticeIndices [i+1]]);

			connections.Add(VibbiUtils.CreateLine (start, end, Color.red, transform));

		}
	}

	void OldUpdateConnections(){


		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;


		for (int i = 0; i < connections.Count; i++) {


			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [2 * i]]);
			var end = secondClothPiece.transform.TransformPoint (sMeshVertices [lineVerticeIndices [2 * i + 1]]);

			var lineRenderer = connections[i].GetComponent<LineRenderer> ();
			lineRenderer.SetPosition (0, start);
			lineRenderer.SetPosition (1, end);
		}
	}*/

}
