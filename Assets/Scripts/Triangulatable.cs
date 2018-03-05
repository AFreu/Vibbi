using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulatable : MonoBehaviour {


	public Triangulator triangulator;

	List<Vector2> coords = new List<Vector2>();
	List<List<Vector2>> holeCoords = new List<List<Vector2>>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (triangulator.autoTriangulate) {
			Triangulate ();
		}	


	}

	public void Triangulate(){

		UpdateCoords ();

		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

		if (mesh == null)
			Debug.Log ("Mesh is null");

		triangulator.Triangulate(mesh, coords, holeCoords);

		GetComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter> ().mesh;
	}

	protected virtual void UpdateCoords(){
		coords.Clear ();
		holeCoords.Clear ();

		BoundaryPointsHandler bph = gameObject.GetComponent<BoundaryPointsHandler> ();

		foreach(GameObject o in bph.boundaryPoints){
			var t = o.transform.localPosition;
			coords.Add (new Vector2 (t.x, t.y));
		}

		Vector2 clothPosition = transform.position;


		foreach (GameObject d in bph.darts) {
			Vector2 dartPosition = d.transform.position;

			List<Vector2> temp = new List<Vector2> ();

			foreach(Vector2 v in d.GetComponent<BoundaryPointsHandler> ().GetPointPositions ()){
				temp.Add (v - clothPosition + dartPosition);
			}

			holeCoords.Add (temp);
		}
	}




}
