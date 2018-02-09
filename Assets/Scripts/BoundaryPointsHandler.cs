using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointsHandler : MonoBehaviour {

	//The lists are order dependent
	private List<GameObject> boundaryPoints = new List<GameObject> ();
	private List<GameObject> boundaryLines = new List<GameObject> ();

	public GameObject boundaryPointPrefab;
	public GameObject boundaryLinePrefab;

	public Triangulator test;
	public GameObject cloth;


	// Use this for initialization
	void Start () {

		InitQuad ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TriangulateModel() {

		var coords = new List<Vector2>();
		var holeCoords = new List<List<Vector2>> ();

		foreach(GameObject o in boundaryPoints){
			var t = o.transform.position;
			coords.Add (new Vector2 (t.x, t.y));
		}

		Mesh mesh = cloth.GetComponent<MeshFilter> ().mesh;

		test.Triangulate (mesh, coords, holeCoords);

		cloth.GetComponent<MeshCollider> ().sharedMesh = cloth.GetComponent<MeshFilter>().mesh; 
	}

	public void AddBoundaryPoint(GameObject line, Vector3 position){
		if (!boundaryLines.Contains (line)) {
			return;
		}
			
		int index = boundaryLines.IndexOf (line);

		//Get line behaviour of line at index
		var lineBehaviour = boundaryLines[index].GetComponent<BoundaryLineBehaviour> ();

		//Create new point and line
		GameObject newPoint = Instantiate (boundaryPointPrefab, position, Quaternion.identity, gameObject.transform) as GameObject;
		GameObject newLine = Instantiate (boundaryLinePrefab, gameObject.transform) as GameObject;

		//Store current line's second point reference
		var secondPointTransform = lineBehaviour.second;
		//Set current line's second point to the new point
		lineBehaviour.second = newPoint.transform;

		//Get new line behaviour
		var newLineBehaviour = newLine.GetComponent<BoundaryLineBehaviour> ();

		//Set new line's first point to the new point
		newLineBehaviour.first = newPoint.transform;

		//Set new line's second point to the stored second point reference
		newLineBehaviour.second = secondPointTransform;

		boundaryPoints.Insert (index + 1, newPoint);
		boundaryLines.Insert (index + 1, newLine);


	}

	void InitQuad(){
		//Instantiate boundar
		GameObject o1 = Instantiate (boundaryPointPrefab, gameObject.transform) as GameObject;
		GameObject o2 = Instantiate (boundaryPointPrefab, gameObject.transform) as GameObject;
		GameObject o3 = Instantiate (boundaryPointPrefab, gameObject.transform) as GameObject;
		GameObject o4 = Instantiate (boundaryPointPrefab, gameObject.transform) as GameObject;

		o1.transform.Translate (new Vector3 (0.5f, 0.5f, 0.0f));
		o2.transform.Translate (new Vector3 (-0.5f, 0.5f, 0.0f));
		o3.transform.Translate (new Vector3 (-0.5f, -0.5f, 0.0f));
		o4.transform.Translate (new Vector3 (0.5f, -0.5f, 0.0f));

		//0
		boundaryPoints.Add(o1);
		//1
		boundaryPoints.Add (o2);
		//2
		boundaryPoints.Add (o3);
		//3
		boundaryPoints.Add (o4);
		//4

		GameObject l1 = Instantiate (boundaryLinePrefab, gameObject.transform) as GameObject;
		GameObject l2 = Instantiate (boundaryLinePrefab, gameObject.transform) as GameObject;
		GameObject l3 = Instantiate (boundaryLinePrefab, gameObject.transform) as GameObject;
		GameObject l4 = Instantiate (boundaryLinePrefab, gameObject.transform) as GameObject;

		var lb1 = l1.GetComponent<BoundaryLineBehaviour> ();
		lb1.first = o1.transform;
		lb1.second = o2.transform;

		var lb2 = l2.GetComponent<BoundaryLineBehaviour> ();
		lb2.first = o2.transform;
		lb2.second = o3.transform;

		var lb3 = l3.GetComponent<BoundaryLineBehaviour> ();
		lb3.first = o3.transform;
		lb3.second = o4.transform;

		var lb4 = l4.GetComponent<BoundaryLineBehaviour> ();
		lb4.first = o4.transform;
		lb4.second = o1.transform;

		boundaryLines.Add (l1);
		boundaryLines.Add (l2);
		boundaryLines.Add (l3);
		boundaryLines.Add (l4);

	}
}
