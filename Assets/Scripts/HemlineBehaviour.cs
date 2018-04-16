using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HemlineBehaviour : MonoBehaviour {

	public static bool loaded = false;

	public List<GameObject> lines;


	public Color seamColor = Color.black;
	public float seamWidth = 0.1f;

	//public GameObject firstClothPiece { set; get;}
	//public List<int> lineVerticeIndices { set; get;}

	public List<List<int>> verticeIndices;
	public List<GameObject> clothPieces;


	void Update(){
		if(loaded){
			UpdateLineRenderer ();
		}

	}

	/*void UpdateLineRenderer(){
		var renderer = GetComponent<LineRenderer> ();

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		var count = lineVerticeIndices.Count;

		renderer.positionCount = count;

		Vector3 [] positions = new Vector3[count];

		for (int i = 0; i < count; i++) {

			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [i]]);

			positions [i] = start;
		}

		renderer.SetPositions (positions);
	}*/

	void UpdateLineRenderer(){
		var renderer = GetComponent<LineRenderer> ();



		for(int i = 0; i < clothPieces.Count; i++){
			var clothPiece = clothPieces [i];
			var lineVerticeIndices = verticeIndices [i];


			var meshVertices = clothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
			var count = lineVerticeIndices.Count;


			renderer.positionCount = count;

			Vector3 [] positions = new Vector3[count];

			for (int j = 0; j < count; j++) {

				var start = clothPiece.transform.TransformPoint (meshVertices [lineVerticeIndices [j]]);

				positions [j] = start;
			}

			renderer.SetPositions (positions);
		}

	}

	public void Init(List<GameObject> lines){
		this.lines = lines;
	}

	/*public void Init3D(List<int> lineVerticeIndices, GameObject firstClothPiece){

		this.lineVerticeIndices = lineVerticeIndices;
		this.firstClothPiece = firstClothPiece;

		var firstVertexPosition = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices [lineVerticeIndices [0]];
		transform.position = firstClothPiece.transform.TransformPoint (firstVertexPosition);

		loaded = true;

		var renderer = GetComponent<LineRenderer> ();
		renderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		renderer.startColor = seamColor;
		renderer.endColor = seamColor;
		renderer.startWidth = seamWidth;
		renderer.endWidth = seamWidth;

	}*/

	public void Init3D(List<List<int>> verticeIndices, List<GameObject> clothPieces){

		this.verticeIndices = verticeIndices;
		this.clothPieces = clothPieces;

		var firstVertexPosition = clothPieces[0].GetComponent<MeshFilter> ().sharedMesh.vertices [verticeIndices [0][0]];
		transform.position = clothPieces[0].transform.TransformPoint (firstVertexPosition);


		loaded = true;

		var renderer = GetComponent<LineRenderer> ();
		renderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		renderer.startColor = seamColor;
		renderer.endColor = seamColor;
		renderer.startWidth = seamWidth;
		renderer.endWidth = seamWidth;
	
	}

		
}
