using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartHandler : MonoBehaviour {

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


			if (startOnCloth || endOnCloth) {
				cloth.GetComponent<BoundaryPointsHandler> ().AddDart (start, end, startOnCloth && endOnCloth);
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
}
