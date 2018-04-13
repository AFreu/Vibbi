using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HemlineBehaviour : MonoBehaviour {

	public static bool loaded = false;

	public List<GameObject> lines;


	public Color seamColor = Color.red;
	public float seamWidth = 0.1f;

	public int firstMesh { set; get;}
	//public int secondMesh{ set; get;}

	public GameObject firstClothPiece { set; get;}
	//public GameObject secondClothPiece{ set; get;}

	public List<int> lineVerticeIndices { set; get;}

	void Update(){
		if(loaded){
			UpdateLineRenderer ();
		}

	}

	void UpdateLineRenderer(){
		var renderer = GetComponent<LineRenderer> ();

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		var count = lineVerticeIndices.Count;
		//count = 4;

		renderer.positionCount = count;

		Vector3 [] positions = new Vector3[count];
		//bool alt = true;
		for (int i = 0; i < count; i++) {

			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [i]]);

			positions [i] = start;
		}

		renderer.SetPositions (positions);
	}

	public void Init(List<GameObject> lines){
		this.lines = lines;
	}

	public void Init3D(int firstMesh, List<int> lineVerticeIndices, GameObject firstClothPiece){


		this.firstMesh = firstMesh;
	
		this.lineVerticeIndices = lineVerticeIndices;
		this.firstClothPiece = firstClothPiece;

		var fMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;

		//transform.position = lines [0].GetComponent<BoundaryLineBehaviour> ().GetMidPoint ();
		transform.position = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [0]]);

		loaded = true;
		var renderer = GetComponent<LineRenderer> ();
		renderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		renderer.startColor = seamColor;
		renderer.endColor = seamColor;
		renderer.startWidth = seamWidth;
		renderer.endWidth = seamWidth;

	}

		
}
