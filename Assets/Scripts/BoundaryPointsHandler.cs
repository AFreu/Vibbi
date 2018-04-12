using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;


public class BoundaryPointsHandler : MonoBehaviour {
	private int MINIMUM_AMOUNT_BOUNDARYPOINTS = 3; 

	public DartHandler dartHandler;
	public Triangulator triangulator;

	//The lists are order dependent
	[SerializeField]
	public List<GameObject> boundaryPoints = new List<GameObject> ();
	[SerializeField]
	public List<GameObject> boundaryLines = new List<GameObject> (); 
	[SerializeField]
	public List<GameObject> darts = new List<GameObject> ();

	private PolygonCollider2D polygonCollider;
	private ActionManager actionManager;

	public GameObject boundaryPointPrefab;
	public GameObject boundaryLinePrefab;
    public GameObject notchPrefab;

	private static bool save;

	[SerializeField]
	private Transform boundaries;
	[SerializeField]
	private Mesh mesh;


	// Use this for initialization
	void Start () {
		polygonCollider = GetComponent<PolygonCollider2D> ();
		actionManager = GetComponentInParent<ActionManager> ();
		dartHandler = GetComponentInParent<DartHandler> ();
        save = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (save)
		{
			save = false;
			ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "meshyoyo.obj");
		}
        
		UpdateMiddle ();
		UpdateCollider ();

		UpdateDarts ();

	}

	void UpdateMiddle(){

		if (boundaryPoints.Count <= 0) {
			return;
		}
		float largestX = boundaryPoints [0].transform.position.x;
		float smallestX = boundaryPoints [0].transform.position.x;
		float largestY = boundaryPoints [0].transform.position.y;
		float smallestY = boundaryPoints [0].transform.position.y;

		for (int i = 1; i < boundaryPoints.Count; i++) {
			var X = boundaryPoints [i].transform.position.x;
			var Y = boundaryPoints [i].transform.position.y;


			if (X > largestX)
				largestX = X;
			
			if (X < smallestX)
				smallestX = X;
			
			if (Y > largestY)
				largestY = Y;
			
			if (Y < smallestY)
				smallestY = Y;


		}

		Vector2 topCorner = new Vector3 (largestX, largestY);
		Vector2 botCorner = new Vector3 (smallestX, smallestY);
		Vector2 diagonal = topCorner - botCorner;

		Vector2 mid = botCorner + diagonal/2; 

		Debug.DrawLine (transform.position, mid, Color.blue, 100);

		UpdatePosition (new Vector3(mid.x, mid.y, 0.0f));
	}

	void UpdateCollider(){
		var array = new Vector2[boundaryPoints.Count];

		for (int i = 0; i < boundaryPoints.Count; i++) {
			var X = boundaryPoints [i].transform.localPosition.x;
			var Y = boundaryPoints [i].transform.localPosition.y;

			array [i] = new Vector2 (X, Y);

		}

		polygonCollider.points = array;
	}

	void UpdateDarts(){
		
	}

	void UpdatePosition(Vector3 position){

		Vector3 translation = position - transform.position;
		transform.Translate (translation);

		foreach (GameObject o in boundaryPoints) {
			o.transform.Translate (-translation);
		}
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
		var lineBehaviour = boundaryLines[index].GetComponent<SimpleLineBehaviour> ();

		var newLine = newPoint.GetComponent<BoundaryPointBehaviour> ().line;

		newPoint.SetActive (true);
		newLine.SetActive (true);

		newPoint.transform.parent = boundaries;
		newLine.transform.parent = boundaries;

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
		var lineBehaviour = boundaryLines[index].GetComponent<SimpleLineBehaviour> ();

		//Create new point and line
		GameObject newPoint = Instantiate (boundaryPointPrefab, position, Quaternion.identity, boundaries) as GameObject;
		GameObject newLine = Instantiate (boundaryLinePrefab, boundaries) as GameObject;

		//Store reference to line in point
		newPoint.GetComponent<BoundaryPointBehaviour> ().line = newLine;

		//Store current line's second point reference
		var secondPointTransform = lineBehaviour.second;
		//Set current line's second point to the new point
		lineBehaviour.second = newPoint.transform;

		//Get new line behaviour
		var newLineBehaviour = newLine.GetComponent<SimpleLineBehaviour> ();

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

		var second = bL.GetComponent<SimpleLineBehaviour> ().second;

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
		var second = bL.GetComponent<SimpleLineBehaviour> ().second;

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
		bL.GetComponent<SimpleLineBehaviour> ().second = newSecond;
		return bL;

	}

	private void InitBoundaryCollection(){
		GameObject boundaryCollection = new GameObject ("Boundaries");
		boundaryCollection.tag = "Boundaries";
		boundaryCollection.transform.parent = transform;
		boundaryCollection.transform.localPosition = Vector3.zero;
		boundaries = boundaryCollection.transform;


	}

	private void InitMesh(){

		Debug.Log ("InitMesh");

		List<Vector2> coords = new List<Vector2> ();
		List<List<Vector2>> holes = new List<List<Vector2>> ();

		foreach(GameObject o in boundaryPoints){
			var t = o.transform.localPosition;
			coords.Add (new Vector2 (t.x, t.y));
		}

		mesh = new Mesh ();
		mesh.name = "My mesh";

		triangulator.Triangulate (mesh, coords, holes);
		GetComponent<MeshFilter> ().sharedMesh = mesh;
	}



	public void Init(PredefinedCloth cloth){

		InitBoundaryCollection ();

		if (cloth != null) {
			InitPolygon (cloth);
		} else {
			InitQuad ();
		}

	}

	private void InitQuad(){

		Debug.Log ("InitQuad!");

		//Instantiate boundary
		GameObject o1 = Instantiate (boundaryPointPrefab, boundaries) as GameObject;
		GameObject o2 = Instantiate (boundaryPointPrefab, boundaries) as GameObject;
		GameObject o3 = Instantiate (boundaryPointPrefab, boundaries) as GameObject;
		GameObject o4 = Instantiate (boundaryPointPrefab, boundaries) as GameObject;

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

		GameObject l1 = Instantiate (boundaryLinePrefab, boundaries) as GameObject;
		GameObject l2 = Instantiate (boundaryLinePrefab, boundaries) as GameObject;
		GameObject l3 = Instantiate (boundaryLinePrefab, boundaries) as GameObject;
		GameObject l4 = Instantiate (boundaryLinePrefab, boundaries) as GameObject;

		o1.GetComponent<BoundaryPointBehaviour> ().line = l1;
		o2.GetComponent<BoundaryPointBehaviour> ().line = l2;
		o3.GetComponent<BoundaryPointBehaviour> ().line = l3;
		o4.GetComponent<BoundaryPointBehaviour> ().line = l4;

		var lb1 = l1.GetComponent<SimpleLineBehaviour> ();
		lb1.first = o1.transform;
		lb1.second = o2.transform;

		var lb2 = l2.GetComponent<SimpleLineBehaviour> ();
		lb2.first = o2.transform;
		lb2.second = o3.transform;

		var lb3 = l3.GetComponent<SimpleLineBehaviour> ();
		lb3.first = o3.transform;
		lb3.second = o4.transform;

		var lb4 = l4.GetComponent<SimpleLineBehaviour> ();
		lb4.first = o4.transform;
		lb4.second = o1.transform;

		boundaryLines.Add (l1);
		boundaryLines.Add (l2);
		boundaryLines.Add (l3);
		boundaryLines.Add (l4);

		InitMesh ();

		//AddDart (new Vector3 (0.3f, 0.4f, 0.0f), new Vector3 (0.3f, 0.1f, 0.0f));

	}

	private void InitPolygon(PredefinedCloth cloth){

		SimpleLineBehaviour lb = null;

		foreach (Vector2 p in cloth.points) {
			//Make a new boundary point
			GameObject o = Instantiate (boundaryPointPrefab, boundaries) as GameObject;
			o.transform.Translate (new Vector3 (p.x, p.y, 0.0f));
			boundaryPoints.Add (o);

			//Make a new line
			GameObject l = Instantiate (boundaryLinePrefab, boundaries) as GameObject;
			boundaryLines.Add (l);
			o.GetComponent<BoundaryPointBehaviour> ().line = l;
			//Add this point as second to the previous line
			if (lb != null) {
				lb.second = o.transform;
			}
			//Get the new line behaviour
			lb = l.GetComponent<SimpleLineBehaviour> ();

			//Add this point as first to the new line
			lb.first = o.transform;

		}

		//Set last line's second point to be the first point of the set
		lb.second = boundaryPoints [0].transform;
			
		InitMesh ();
	
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
		//GetComponentInParent starts looking in "this" object before looking into parents. 
		transform.parent.GetComponentInParent<BoundaryPointsHandler> ().RemoveDart (gameObject);
		GetComponentInParent<ClothModelHandler> ().RemoveCloth(gameObject);
	}

	public void RemoveDart(GameObject dart){
		Debug.Log ("Removing dart: " + dart);
		darts.Remove (dart);
	}

	//Probably some nicer way to implement this
	public void Duplicate(){
		GetComponentInParent<ClothModelHandler> ().CopyCloth(gameObject, new Vector3(1.0f, 1.0f, 0.0f));
	}



	public void InitCopy(){
		Debug.Log ("InitCopy");
		boundaryPoints.Clear ();
		boundaryLines.Clear ();
		darts.Clear ();

		foreach (Transform childObject in transform) {
			if (childObject.tag == "Dart") {
				Debug.Log ("Adding Dart to copy");
				darts.Add (childObject.gameObject);
			}

			//Only needed if Boundaries are not seriliazable
			/*if (childObject.tag == "Boundaries") {
				Debug.Log ("Adding Boundaries to copy");

				boundaries = childObject;
			}*/
		}

		foreach (Transform boundary in boundaries) {
			switch (boundary.tag) {
			case "BoundaryLine":
				Debug.Log ("Adding BoundaryLine to copy");
				boundaryLines.Add (boundary.gameObject);
				break;
			case "BoundaryPoint":
				Debug.Log ("Adding BoundaryPoint to copy");
				boundaryPoints.Add (boundary.gameObject);
				break;
			}
		}

		InitMesh ();
	}

	public void Unfold(GameObject line){
		if (!boundaryLines.Contains (line))
			return;

		//Store position and direction of line since it is changing when we add a new point to it
		var lineBehaviour = line.GetComponent<SimpleLineBehaviour> ();
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

    public List<GameObject> GetBoundaryPoints()
    {
        return boundaryPoints;
    }

	public List<GameObject> GetSelectedLines(){
		List<GameObject> temp = new List<GameObject> ();
		foreach (GameObject l in boundaryLines) {
			if (l.GetComponent<Selectable> ().isSelected()) {
				temp.Add (l);
			}
		}
		return temp;
	}
		
	public void AddDart(GameObject dart){
		darts.Add (dart);
	}

	public List<Vector2> GetPointPositions() {
		List<Vector2> temp = new List<Vector2> ();
		foreach (GameObject o in boundaryPoints) {
			temp.Add (o.transform.localPosition);
		}
		return temp;
	}
		
}
