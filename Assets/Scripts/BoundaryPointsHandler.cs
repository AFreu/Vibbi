using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointsHandler : MonoBehaviour {
	private int MINIMUM_AMOUNT_BOUNDARYPOINTS = 3; 

	//The lists are order dependent
	private List<GameObject> boundaryPoints = new List<GameObject> ();
	private List<GameObject> boundaryLines = new List<GameObject> ();

	private GameObject cloth;

	public GameObject clothPrefab;
	public GameObject boundaryPointPrefab;
	public GameObject boundaryLinePrefab;

	public Triangulator triangulator;

	public bool autoTriangulate = false;
    
	private static bool save;

	// Use this for initialization
	void Start () {

        save = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (autoTriangulate) {
			TriangulateModel ();
		}

		if (save)
		{
			save = false;
			Debug.Log("hej");
			ObjExporter.MeshToFile(cloth.GetComponent<MeshFilter>(), "meshyoyo.obj");
		}
	}

	public void TriangulateModel() {

		//Debug.Log ("Triangulate Model");

		var coords = new List<Vector2>();
		var holeCoords = new List<List<Vector2>> ();

		foreach(GameObject o in boundaryPoints){
			var t = o.transform.localPosition;
			coords.Add (new Vector2 (t.x, t.y));
		}

		//Debug.Log ("Number of coords: " + coords.Count);

		if (cloth == null)
			Debug.Log ("Cloth is null");

		Mesh mesh = cloth.GetComponent<MeshFilter>().sharedMesh;

		if (mesh == null)
			Debug.Log ("Mesh is null");

		triangulator.Triangulate(mesh, coords, holeCoords);

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
		GameObject newPoint = Instantiate (boundaryPointPrefab, position, Quaternion.identity, cloth.transform) as GameObject;
		GameObject newLine = Instantiate (boundaryLinePrefab, cloth.transform) as GameObject;


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

		//Add new point and line to lists
		//Make sure the child hierarchy has the same order as the lists
		boundaryPoints.Insert (index + 1, newPoint);
		newPoint.transform.SetSiblingIndex (index + 1);
		boundaryLines.Insert (index + 1, newLine);
		newLine.transform.SetSiblingIndex (boundaryPoints.Count + index + 1);

        
	}

	public void RemoveBoundaryPoint(GameObject point){
		if (boundaryPoints.Count <= MINIMUM_AMOUNT_BOUNDARYPOINTS) {
			Debug.Log ("Not allowed to have less boundary points!");
			return;
		}

		if (!boundaryPoints.Contains (point))
			return;

		var index = boundaryPoints.IndexOf (point);

		var bP = boundaryPoints [index];
		var bL = boundaryLines [index];

		//Store second boundary point transform
		var second = bL.GetComponent<BoundaryLineBehaviour> ().second;

		//Remove and destroy object and references
		boundaryPoints.Remove (bP);
		boundaryLines.Remove (bL);

		GameObject.Destroy (bP);
		GameObject.Destroy (bL);

		bP = null;
		bL = null;

		//Update line which had point as second
		UpdateLineSecond (index - 1, second);


	}

	private void UpdateLineSecond(int index, Transform newSecond){
		//Wrap around index
		if (index < 0) {
			index = boundaryLines.Count - 1;
		}
		var bL = boundaryLines [index];
		bL.GetComponent<BoundaryLineBehaviour> ().second = newSecond;


	}

	public void InitQuad(){

		Debug.Log ("InitQuad!");
		cloth = Instantiate (clothPrefab, gameObject.transform) as GameObject;

		//Instantiate boundary
		GameObject o1 = Instantiate (boundaryPointPrefab, cloth.transform) as GameObject;
		GameObject o2 = Instantiate (boundaryPointPrefab, cloth.transform) as GameObject;
		GameObject o3 = Instantiate (boundaryPointPrefab, cloth.transform) as GameObject;
		GameObject o4 = Instantiate (boundaryPointPrefab, cloth.transform) as GameObject;

        o1.transform.Translate (new Vector3 (0.6f, 0.6f, 0.0f));
		o2.transform.Translate (new Vector3 (-0.6f, 0.6f, 0.0f));
		o3.transform.Translate (new Vector3 (-0.6f, -0.6f, 0.0f));
		o4.transform.Translate (new Vector3 (0.6f, -0.6f, 0.0f));

        //0
        boundaryPoints.Add(o1);
		//1
		boundaryPoints.Add (o2);
		//2
		boundaryPoints.Add (o3);
		//3
		boundaryPoints.Add (o4);
        //4

        GameObject l1 = Instantiate (boundaryLinePrefab, cloth.transform) as GameObject;
		GameObject l2 = Instantiate (boundaryLinePrefab, cloth.transform) as GameObject;
		GameObject l3 = Instantiate (boundaryLinePrefab, cloth.transform) as GameObject;
		GameObject l4 = Instantiate (boundaryLinePrefab, cloth.transform) as GameObject;

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

    public void saveMesh()
    {
        Debug.Log("tjena");
        save = true;
        //Debug.Log(GetComponent<MeshFilter>().sharedMesh.vertices[0]);
        //ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "meshyoyo.obj");
    }

	public void Remove(){
		gameObject.transform.GetComponentInParent<ClothModelHandler> ().RemoveClothModel(gameObject);
	}

	//Probably some nicer way to implement this
	public void Duplicate(){
		gameObject.transform.GetComponentInParent<ClothModelHandler> ().CopyClothModel(gameObject, Vector3.zero);
	}


	public void InitCopy(){
		Debug.Log ("InitCopy");
		boundaryPoints.Clear ();
		boundaryLines.Clear ();

		var clothTransform = transform.GetChild (0);

		Debug.Log ("Adding Cloth");
		cloth = clothTransform.gameObject;

		foreach (Transform t in clothTransform) {
			switch (t.tag) {
			case "BoundaryLine":
				Debug.Log ("Adding BoundaryLine");
				boundaryLines.Add (t.gameObject);
				break;
			case "BoundaryPoint":
				Debug.Log ("Adding BoundaryPoint");
				boundaryPoints.Add (t.gameObject);
				break;
			}
		}

	}

	/*

	private void setBoundary(List<GameObject> bPs, List<GameObject> bLs){
		foreach (GameObject p in boundaryPoints) {
			GameObject.Destroy (p);
		}

		foreach (GameObject l in boundaryLines) {
			GameObject.Destroy (l);
		}

		boundaryPoints = null;
		boundaryLines = null;

		this.boundaryPoints = bPs;
		this.boundaryLines = bLs;
	}

	public void setCloth(){
		//var temp = cloths [0];

		GameObject cloth = Instantiate (clothPrefab, gameObject.transform) as GameObject;
		cloths.Add(cloth);
		//cloths.Remove (temp);
		foreach (Transform t in transform) {
			if (t.tag != "Cloth") {
				t.parent = cloth.transform;
			}
		}

		//GameObject.Destroy (temp);
		//temp = null;


		//cloths.Add (cloth);

	}
		

	public GameObject Copy(Transform parent){
		

		//Deep copy list of boundary points and lines
		List<GameObject> bPList = new List<GameObject> ();
		List<GameObject> bLList = new List<GameObject> ();

		var newCloth = Instantiate (clothPrefab, transform) as GameObject;

		var newFirst = boundaryLines [0].GetComponent<BoundaryLineBehaviour> ().first.gameObject;
		//var newFirstCopy = newFirst.GetComponent<BoundaryPointBehaviour>().Copy (transform);
		var newFirstCopy = Instantiate(boundaryPointPrefab, newCloth.transform);
		copyPoint (newFirst, newFirstCopy);

		for (int i = 0; i < boundaryLines.Count; i++) {

			bPList.Add (newFirstCopy);

			var lineBehaviour = boundaryLines [i].GetComponent<BoundaryLineBehaviour>();


			//var f = lineBehaviour.first.gameObject.GetComponent<BoundaryPointBehaviour>().Copy();
			var f = newFirstCopy;
			//var s = lineBehaviour.second.gameObject.GetComponent<BoundaryPointBehaviour>().Copy(transform);
			var s = Instantiate(boundaryPointPrefab, newCloth.transform);
			copyPoint (lineBehaviour.second.gameObject, s);

			newFirstCopy = s;

			//var lineCopy = lineBehaviour.Copy (transform);
			var lineCopy = Instantiate(boundaryLinePrefab, newCloth.transform);
			copyLine (boundaryLines [i], lineCopy);

			var lineCopyBehaviour = lineCopy.GetComponent<BoundaryLineBehaviour> ();

			lineCopyBehaviour.first = f.transform;
			lineCopyBehaviour.second = s.transform;

			bLList.Add (lineCopy);

		}


		GameObject copy = Instantiate (gameObject, parent) as GameObject;
		var copyBehaviour = copy.GetComponent<BoundaryPointsHandler> ();

		copyBehaviour.setBoundary (bPList, bLList);

		return copy;

	}


	void copyLine(GameObject line, GameObject copy){
		copy.transform.position = line.transform.position;
		copy.transform.rotation = line.transform.rotation;
		copy.transform.localScale = line.transform.localScale;
	}

	void copyPoint(GameObject point, GameObject copy){
		copy.transform.position = point.transform.position;
	}*/


		
}
