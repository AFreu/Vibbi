using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour {

	private Vector3 lastPosition;
	public float mouseSensitivity = 1.0f;

	private Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 point = cam.ScreenToViewportPoint(Input.mousePosition);
		bool cursorInViewPort = point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1;
		if (!cursorInViewPort)
			return;
		
		//var position = cam.ScreenToViewportPoint (Input.mousePosition);

		if (Input.GetMouseButtonDown(2))
		{
			lastPosition = point;
		}

		if (Input.GetMouseButton(2))
		{

			Vector3 delta = point - lastPosition;

			transform.Translate(-delta.x * mouseSensitivity * cam.orthographicSize, -delta.y * mouseSensitivity * cam.orthographicSize, 0);

			lastPosition = point;
		}
	}
}
