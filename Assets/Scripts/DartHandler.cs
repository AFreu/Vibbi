using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartHandler : MonoBehaviour {

	public GameObject dartPrefab;

	public GameObject dartPointPrefab;
	public GameObject simpleLinePrefab;

	public List<GameObject> dartPoints = new List<GameObject> ();
	public List<GameObject> dartLines = new List<GameObject> ();

	private Vector3 start;
	private Vector3 end;

	private bool startOnCloth = false;
	private bool endOnCloth = false;

	private bool makingDart = false;

	Ray mousePositionRay;

	private GameObject cloth;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		mousePositionRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		HandleInput ();

		if (makingDart) {
			Debug.DrawLine (start, end, Color.red);
			DrawLine (start, end, Color.red);
		}

	}

	void HandleInput(){

		if (!Input.GetKey (KeyCode.G)) {
			makingDart = false;
			return;
		}

		RaycastHit hit;

		var position = MouseWorldPosition ();

		if (Input.GetMouseButtonDown(0)) {
			
			/*int layerMask = LayerMask.GetMask("ModelPlane");


			if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
				makingDart = true;
				start = hit.point;
			}*/

			makingDart = true;
			start = position;

			startOnCloth = TryFindCloth ();
		}

		if (Input.GetMouseButton (0)) {

			/*int layerMask = LayerMask.GetMask("ModelPlane");
			if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
				end = hit.point;
			}*/
			end = position;
		}

		if (Input.GetMouseButtonUp (0)) {
			makingDart = false;
			Debug.Log ("Dart making");

			endOnCloth = TryFindCloth ();

			var bothInside = endOnCloth && startOnCloth;

			if (startOnCloth || endOnCloth) {

				if (!bothInside) {

					Debug.Log ("Over boundary");
					if (Physics.Linecast (start, end, out hit, LayerMask.GetMask ("BoundaryLine"))) {
						var bl = hit.transform.GetComponent<SimpleLineBehaviour> ();

						Debug.Log ("Found boundary");
						GameObject d = CreateDart (transform.InverseTransformPoint (start), transform.InverseTransformPoint (hit.point), bl);
						cloth.GetComponent<BoundaryPointsHandler> ().AddDart (d);

					} else {
						Debug.Log ("Did not find boundary line between dart start and end");
						return;
					}


				} else {
					GameObject d = CreateDart (start, end);
					//GameObject d = CreateDart (transform.InverseTransformPoint (start), transform.InverseTransformPoint (end));
					cloth.GetComponent<BoundaryPointsHandler> ().AddDart (d);
				}
			} else {
				//Create dart model
				CreateDart(start, end);
				//TODO: what happens when dart is made outside a cloth
				//No Cloth to add it to :(

			}
		}
	}

	bool TryFindCloth(){

		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 5; //Distance to camera

		Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

		RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);

		if (hit) {
			Debug.Log ("Found cloth");
			cloth = hit.transform.gameObject;
			return true;
		} else {
			Debug.Log ("Did not find cloth");
		}

		return false;
	}

	void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.1f)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.startColor = color;
		lr.endColor = color;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}

	public GameObject CreateDart(Vector3 start, Vector3 end){


		Vector2 position = start + (end - start) / 2;

		GameObject d = Instantiate (dartPrefab, cloth.transform) as GameObject;
		d.transform.Translate (position);

		var positions = InitPositions (position);

		BoundaryPointsHandler bph = d.GetComponent<BoundaryPointsHandler> ();
		InitBoundaryPointsHandler (bph, positions);

		return d;
	}

	public GameObject CreateDart(Vector3 start, Vector3 end, SimpleLineBehaviour simpleLine){

		Vector2 position = end;

		GameObject o = new GameObject ();
		o.transform.parent = simpleLine.transform;
		o.transform.localPosition = Vector3.zero;

		GameObject d = Instantiate (dartPrefab, o.transform) as GameObject;
		d.transform.Translate (position);

		var positions = InitPositions (position);

		BoundaryPointsHandler bph = d.GetComponent<BoundaryPointsHandler> ();

		InitBoundaryPointsHandler (bph, positions);

		bph.DeactivateBoundaryPoint (bph.boundaryPoints[2]);

		Destroy (d.GetComponent<Movable>());
		var movable = d.AddComponent<Movable1D> ();
		movable.Init (simpleLine.first, simpleLine.second);

		return d;
	}

	void InitBoundaryPointsHandler(BoundaryPointsHandler bph, Vector2[] positions){
		
		bph.boundaryPointPrefab = dartPointPrefab;
		bph.boundaryLinePrefab = simpleLinePrefab;
		bph.InitQuad ();

		bph.boundaryPoints [0].transform.position = positions[0];
		bph.boundaryPoints [1].transform.position = positions[2];
		bph.boundaryPoints [2].transform.position = positions[1];
		bph.boundaryPoints [3].transform.position = positions[3];

		bph.boundaryPoints [0].GetComponent<Movable1D> ().Init (bph.boundaryPoints [2].transform, bph.boundaryPoints [0].transform); 
		bph.boundaryPoints [1].GetComponent<Movable1D> ().Init (bph.boundaryPoints [3].transform, bph.boundaryPoints [1].transform); 
		bph.boundaryPoints [2].GetComponent<Movable1D> ().Init (bph.boundaryPoints [0].transform, bph.boundaryPoints [2].transform); 
		bph.boundaryPoints [3].GetComponent<Movable1D> ().Init (bph.boundaryPoints [1].transform, bph.boundaryPoints [3].transform); 
	}
		

	Vector2[] InitPositions(Vector2 position){

		Vector2[] positions = new Vector2 [4];
		positions[0] = start;
		positions[1] = end;
		var v = positions[1] - positions[0];
		var n = new Vector2 (v.y, -v.x);
		positions[2] = position + n/3;
		positions[3] = position - n/3;
		return positions;
	}

	Vector3 MouseWorldPosition(){

		//Get mouse position on screen
		Vector3 mousePos = Input.mousePosition;

		//Adjust mouse position 
		mousePos.z = transform.position.z - Camera.main.transform.position.z;

		//Get a world position for the mouse
		return Camera.main.ScreenToWorldPoint(mousePos);

	}
}
