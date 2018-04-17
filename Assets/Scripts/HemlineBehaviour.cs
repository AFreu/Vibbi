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


	public List<int> clothPieceIDs;



		

	void Update(){
		if (loaded) {
			UpdateLineRenderer ();
		} else {
			GetComponent<LineRenderer> ().enabled = false;
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

		List<Vector3> newPositions = new List<Vector3>();

		Vector3 sumOfPositions = Vector3.zero;
		int numberOfPositions = 0;

		//Fetch vertices for every cloth pice
		for(int i = 0; i < clothPieces.Count; i++){
			var clothPiece = clothPieces [i];

			//Indices of the vertices in question
			var lineVerticeIndices = verticeIndices [i];

			//Fetch all vertices
			var meshVertices = clothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;


			for (int j = 0; j < lineVerticeIndices.Count; j++) {

				var position = clothPiece.transform.TransformPoint (meshVertices [lineVerticeIndices [j]]);

				newPositions.Add (position);

				sumOfPositions += position;
				numberOfPositions++;
			}


		}

		renderer.positionCount = numberOfPositions;
		renderer.SetPositions (newPositions.ToArray());

		var averagePosition = sumOfPositions / numberOfPositions;
		transform.position = averagePosition;




	}

	public void Init(List<GameObject> lines){
		this.lines = lines;

		foreach (GameObject line in lines) {
			this.clothPieceIDs.Add(line.GetComponentInParent<ClothModelBehaviour>().id);
		}
	}
		

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

		renderer.enabled = true;


	
	}

		
}
