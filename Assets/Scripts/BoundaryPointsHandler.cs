using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;


public class BoundaryPointsHandler : MonoBehaviour {
	private int MINIMUM_AMOUNT_BOUNDARYPOINTS = 3; 

	//The lists are order dependent
	private List<GameObject> boundaryPoints = new List<GameObject> ();
	private List<GameObject> boundaryLines = new List<GameObject> ();

	private PolygonCollider2D polygonCollider;
	private ActionManager actionManager;

	public GameObject boundaryPointPrefab;
	public GameObject boundaryLinePrefab;

	public Triangulator triangulator;


	public bool autoTriangulate = false;
    
	private static bool save;

	// Use this for initialization
	void Start () {

		polygonCollider = GetComponent<PolygonCollider2D> ();
		actionManager = GetComponentInParent<ActionManager> ();
        save = false;

	}
	
	// Update is called once per frame
	void Update () {
		HandleInput ();

		if (autoTriangulate) {
			TriangulateModel ();
		}

		if (save)
		{
			save = false;
			Debug.Log("hej");
			ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "meshyoyo.obj");
		}

//		UpdateCollider ();
	}

	void UpdateCollider(){
		var array = new Vector2[boundaryPoints.Count];

		//collider.points = boundaryPoints;
		float largestX = 0.0f;
		float smallestX = 0.0f;
		float largestY = 0.0f;
		float smallestY = 0.0f;

		for (int i = 0; i < boundaryPoints.Count; i++) {
			var X = boundaryPoints [i].transform.localPosition.x;
			var Y = boundaryPoints [i].transform.localPosition.y;


			if (X > largestX)
				largestX = X;
			if (X < smallestX)
				smallestX = X;
			if (Y > largestY)
				largestY = Y;
			if (Y < smallestY)
				smallestY = Y;

			array [i] = new Vector2 (X, Y);

			//collider.points [0] = boundaryPoints [0].transform.localPosition;
		}
		Vector2 mid = new Vector2 (smallestX, smallestY) + new Vector2 (largestX, largestY) / 2; 

		polygonCollider.points = array;
	}

	public void TriangulateModel() {

		var coords = new List<Vector2>();
		var holeCoords = new List<List<Vector2>> ();

		foreach(GameObject o in boundaryPoints){
			var t = o.transform.localPosition;
			coords.Add (new Vector2 (t.x, t.y));
		}

		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

		if (mesh == null)
			Debug.Log ("Mesh is null");

		triangulator.Triangulate(mesh, coords, holeCoords);

		GetComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter> ().mesh;
       
    }

	public void AddPoint(GameObject line, Vector3 position){
		AddPointAction apa = new AddPointAction (this, line, position);
		actionManager.RecordAction (apa);
	}

	public GameObject ActivateBoundaryPoint (GameObject newPoint, GameObject line, Vector3 position){
		if (!boundaryLines.Contains (line)) {
			return null;
		}

		int index = boundaryLines.IndexOf (line);

		//Get line behaviour of line at index
		var lineBehaviour = boundaryLines[index].GetComponent<BoundaryLineBehaviour> ();

		var newLine = newPoint.GetComponent<BoundaryPointBehaviour> ().line;

		newPoint.SetActive (true);
		newLine.SetActive (true);

		newPoint.transform.parent = transform;
		newLine.transform.parent = transform;

		//Set current line's second point to the new point
		lineBehaviour.second = newPoint.transform;

		//Add new point and line to lists
		//Make sure the child hierarchy has the same order as the lists
		boundaryPoints.Insert (index + 1, newPoint);
		newPoint.transform.SetSiblingIndex (index + 1);
		boundaryLines.Insert (index + 1, newLine);
		newLine.transform.SetSiblingIndex (boundaryPoints.Count + index + 1);

		return newPoint;
	}
		

	public GameObject AddBoundaryPoint(GameObject line, Vector3 position){
		if (!boundaryLines.Contains (line)) {
			return null;
		}
			
		int index = boundaryLines.IndexOf (line);

		//Get line behaviour of line at index
		var lineBehaviour = boundaryLines[index].GetComponent<BoundaryLineBehaviour> ();

		//Create new point and line
		GameObject newPoint = Instantiate (boundaryPointPrefab, position, Quaternion.identity, transform) as GameObject;
		GameObject newLine = Instantiate (boundaryLinePrefab, transform) as GameObject;

		//Store reference to line in point
		newPoint.GetComponent<BoundaryPointBehaviour> ().line = newLine;

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

		return newPoint;

        
	}

	public void RemovePoint(GameObject point){
		
		RemovePointAction rpa = new RemovePointAction (this, point);
		actionManager.RecordAction (rpa);
	}

	public GameObject DeactivateBoundaryPoint(GameObject point){
		if (boundaryPoints.Count <= MINIMUM_AMOUNT_BOUNDARYPOINTS) {
			Debug.Log ("Not allowed to have less boundary points!");
			return null;
		}

		if (!boundaryPoints.Contains (point))
			return null;

		var index = boundaryPoints.IndexOf (point);

		var bP = boundaryPoints [index];
		//var bL = boundaryLines [index];
		var bL = bP.GetComponent<BoundaryPointBehaviour>().line;

		var second = bL.GetComponent<BoundaryLineBehaviour> ().second;

		//Remove and destroy object and references
		boundaryPoints.Remove (bP);
		boundaryLines.Remove (bL);

		bP.SetActive (false);
		bL.SetActive (false);

		bL.transform.parent = transform.parent;
		bP.transform.parent = transform.parent;



		return UpdateLineSecond (index - 1, second);
	}

	public GameObject RemoveBoundaryPoint(GameObject point){
		if (boundaryPoints.Count <= MINIMUM_AMOUNT_BOUNDARYPOINTS) {
			Debug.Log ("Not allowed to have less boundary points!");
			return null;
		}

		if (!boundaryPoints.Contains (point))
			return null;

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
		return UpdateLineSecond (index - 1, second);


	}

	private GameObject UpdateLineSecond(int index, Transform newSecond){
		//Wrap around index
		if (index < 0) {
			index = boundaryLines.Count - 1;
		}
		var bL = boundaryLines [index];
		bL.GetComponent<BoundaryLineBehaviour> ().second = newSecond;
		return bL;

	}

	public void InitQuad(){

		Debug.Log ("InitQuad!");

		//Instantiate boundary
		GameObject o1 = Instantiate (boundaryPointPrefab, transform) as GameObject;
		GameObject o2 = Instantiate (boundaryPointPrefab, transform) as GameObject;
		GameObject o3 = Instantiate (boundaryPointPrefab, transform) as GameObject;
		GameObject o4 = Instantiate (boundaryPointPrefab, transform) as GameObject;

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

        GameObject l1 = Instantiate (boundaryLinePrefab, transform) as GameObject;
		GameObject l2 = Instantiate (boundaryLinePrefab, transform) as GameObject;
		GameObject l3 = Instantiate (boundaryLinePrefab, transform) as GameObject;
		GameObject l4 = Instantiate (boundaryLinePrefab, transform) as GameObject;

		o1.GetComponent<BoundaryPointBehaviour> ().line = l1;
		o2.GetComponent<BoundaryPointBehaviour> ().line = l2;
		o3.GetComponent<BoundaryPointBehaviour> ().line = l3;
		o4.GetComponent<BoundaryPointBehaviour> ().line = l4;

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

	public void InitPolygon(Points points){

		BoundaryLineBehaviour lb = null;

		foreach (Vector2 p in points.points) {
			//Make a new boundary point
			GameObject o = Instantiate (boundaryPointPrefab, transform) as GameObject;
			o.transform.Translate (new Vector3 (p.x, p.y, 0.0f));
			boundaryPoints.Add (o);

			//Make a new line
			GameObject l = Instantiate (boundaryLinePrefab, transform) as GameObject;
			boundaryLines.Add (l);
			o.GetComponent<BoundaryPointBehaviour> ().line = l;
			//Add this point as second to the previous line
			if (lb != null) {
				lb.second = o.transform;
			}
			//Get the new line behaviour
			lb = l.GetComponent<BoundaryLineBehaviour> ();

			//Add this point as first to the new line
			lb.first = o.transform;

		}

		//Set last line's second point to be the first point of the set
		lb.second = boundaryPoints [0].transform;
			
	
	}

    public void saveMesh()
    {
        Debug.Log("tjena");
        save = true;
        //Debug.Log(GetComponent<MeshFilter>().sharedMesh.vertices[0]);
        //ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "meshyoyo.obj");
    }

	public void HandleInput(){
        if (GetComponent<Selectable>() == null)
        {
			Debug.Log ("Has no Selectable");
            return;
        }
	
		if (GetComponent<Selectable> ().isSelected ()) {
			if (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.LeftControl)) {
				Duplicate();
			} else if (Input.GetKeyUp (KeyCode.D)) {
				Remove ();
			}
		}


	}

	public void Remove(){
		//gameObject.transform.GetComponentInParent<ClothModelHandler> ().RemoveClothModel(gameObject);
		gameObject.transform.GetComponentInParent<ClothModelHandler> ().RemoveCloth(gameObject);
	}

	//Probably some nicer way to implement this
	public void Duplicate(){
		//gameObject.transform.GetComponentInParent<ClothModelHandler> ().CopyClothModel(gameObject, Vector3.one);

		gameObject.transform.GetComponentInParent<ClothModelHandler> ().CopyModel(gameObject, new Vector3(1.0f, 1.0f, 0.0f));
	}


	public void InitCopy(){
		Debug.Log ("InitCopy");
		boundaryPoints.Clear ();
		boundaryLines.Clear ();

		foreach (Transform t in transform) {
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

	public void Unfold(GameObject line){
		if (!boundaryLines.Contains (line))
			return;

		//Store position and direction of line since it is changing when we add a new point to it
		var lineBehaviour = line.GetComponent<BoundaryLineBehaviour> ();
		var lineOrigin = lineBehaviour.first.transform.position;
		var end = lineBehaviour.second.transform.position;
		var lineDirection = end - lineOrigin;

		//Make a list containing position of all points except the ones the line is attached to (index && index + 1).
		List<Vector3> positions = new List<Vector3> ();
		var index = boundaryLines.IndexOf (line);
		int count = boundaryPoints.Count;	
		for (int i = 0; i < count - 2; i++) {
			/*It is important that the points are ordered, 
			 * starting with the one closest to the end of the line (index + 1)
			 * and ending with the one closest to the start of the line (index).
			 * This way ALL the points can be added to the same line, even though new lines are created each time.
			*/
			positions.Add(boundaryPoints [(index + 2 + i) % count].transform.position); 
		}

		//Add a new boundary point to the line at each position and mirror it through the original line
		foreach (Vector3 p in positions) {
			var newPoint = AddBoundaryPoint (line, p);  
			MirrorPosition(lineDirection.normalized, lineOrigin, newPoint);
		}
	}

	public void MirrorPosition(Vector3 direction, Vector3 origin, GameObject point){

		var position = point.transform.position;
		var positionOnLine = origin + (direction * Vector3.Dot ((position - origin), direction) / Vector3.Dot (direction, direction));
		var translationToLineFromPoint = (positionOnLine - position);

		/*Debug.DrawLine (p1, p1 + v, Color.green, 100);
		Debug.DrawLine (p1, p1 + d, Color.red, 100);
		Debug.DrawLine (p, p + d, Color.cyan, 100);
		Debug.DrawLine (p1, p1 + 2 * translation, Color.blue, 100);*/

		//Move point twice in magnitude and direction of the translation to the line from the point
		point.transform.position += 2 * translationToLineFromPoint;

	}
}
