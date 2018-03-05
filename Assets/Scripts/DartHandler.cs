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

		if (Input.GetMouseButtonDown(0)) {
			
			int layerMask = LayerMask.GetMask("ModelPlane");


			if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
				makingDart = true;
				start = hit.point;
			}

			startOnCloth = TryFindCloth ();
		}

		if (Input.GetMouseButton (0)) {

			int layerMask = LayerMask.GetMask("ModelPlane");
			if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
				end = hit.point;
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			makingDart = false;
			Debug.Log ("Dart making");

			endOnCloth = TryFindCloth ();

			var bothInside = endOnCloth && startOnCloth;

			if (startOnCloth || endOnCloth) {

				Debug.Log ("AddDart");

				if (!bothInside) {
					if (Physics.Linecast (start, end, out hit, LayerMask.GetMask ("BoundaryLine"))) {
						var bl = hit.transform.gameObject.GetComponent<SimpleLineBehaviour> ();


						//TODO: what happens when dart is made over boundary line
						GameObject d = CreateDart (transform.InverseTransformPoint (start), transform.InverseTransformPoint (end));
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
				GameObject d = CreateDart(start, end);
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

		if(hit)
		{
			Debug.Log ("Found cloth");
			cloth = hit.transform.gameObject;
			return true;
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
		lr.SetColors(color, color);
		lr.SetWidth(0.1f, 0.1f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}

	public GameObject CreateDart(Vector3 start, Vector3 end){
	
		GameObject d = Instantiate (dartPrefab, cloth.transform) as GameObject;

		var position = start + (end - start) / 2;
		Debug.Log ("Dart position: " + position);
		d.transform.Translate (position);

		Vector2 p1 = start;
		Vector2 p2 = end;

		var v = p2 - p1;
		var dv = v.normalized;
		var m = v.magnitude;

		var p = p1 + v / 2; 

		var n = new Vector2 (v.y, -v.x);

		Vector2 p3 = p + n/3;
		Vector2 p4 = p - n/3;

		BoundaryPointsHandler bph = d.GetComponent<BoundaryPointsHandler> ();
		bph.boundaryPointPrefab = dartPointPrefab;
		bph.boundaryLinePrefab = simpleLinePrefab;
		bph.InitQuad ();

		bph.boundaryPoints [0].transform.position = p1;
		bph.boundaryPoints [1].transform.position = p3;
		bph.boundaryPoints [2].transform.position = p2;
		bph.boundaryPoints [3].transform.position = p4;

		foreach (GameObject o in bph.boundaryPoints) {
			o.GetComponent<Movable1D> ().origin = position;
		}

		return d;
	}
}
