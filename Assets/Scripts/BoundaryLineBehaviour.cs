using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryLineBehaviour : MonoBehaviour {

	public Transform first;
	public Transform second;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (first == null || second == null)
			return;
		UpdateLine ();


	}

	void OnMouseOver(){
		Debug.Log ("OnMouseOver");
		if (Input.GetMouseButtonUp (1)) {
			Debug.Log ("RightClick");
			int layerMask = 1 << 8;
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 30f, layerMask)) {
				gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().AddBoundaryPoint(gameObject, hit.point);
			}
			


		}
	}

	void OnMouseEnter(){
		Debug.Log ("Enter Boundary");
	}

	void UpdateLine(){
		var start = first.transform.position;
		var end = second.transform.position;

		var parentScaleCompensation = transform.parent.transform.localScale.x;
		var width = 1.0f;
		var offset = end - start;
		var scale = new Vector3(offset.magnitude/parentScaleCompensation, width, width);
		var position = start;

		transform.position = position;
		transform.right = offset;
		transform.localScale = scale;
	}
}
