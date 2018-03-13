﻿using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour {

	public float zoomSpeed = 1.0f;
	public float min = 1.0f;
	public float max = 20.0f;

	private Camera cam;

	void Start(){
		cam = GetComponent<Camera> ();
	}

	void Update () {

		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		float value = cam.orthographicSize;

		if (scroll > 0) {
			value = cam.orthographicSize+zoomSpeed;
		} else if (scroll < 0) {
			value = cam.orthographicSize-zoomSpeed;
		}

		cam.orthographicSize = Mathf.Clamp(value, min, max);

	}
}
