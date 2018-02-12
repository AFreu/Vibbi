using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour {

	public float speedX;
	public float speedY;

	private Ray mousePositionRay;

	public Material highlightMaterial;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		mousePositionRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		
	}

	void OnMouseDrag() {

		int layerMask = 1 << 8;

		RaycastHit hit;
		if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
			gameObject.transform.position = hit.point;
		}else {
			Debug.Log ("RAY missed modelplane");
		}
	}

	public void Selected(){
		gameObject.GetComponent<Renderer> ().material = highlightMaterial;
	}
	
}
