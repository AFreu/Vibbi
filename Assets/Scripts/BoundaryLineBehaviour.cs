using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryLineBehaviour : MonoBehaviour, ICopyable<GameObject> {

	public Transform first;
	public Transform second;

	private Ray mousePositionRay;
	private Vector3 unitVector;


	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (first == null || second == null)
			return;
		UpdateLine ();

		mousePositionRay = Camera.main.ScreenPointToRay (Input.mousePosition);

	}

	void OnMouseOver(){
		if (Input.GetKeyUp (KeyCode.A)) {
			int layerMask = 1 << 8;
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 30f, layerMask)) {
				gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().AddBoundaryPoint(gameObject, hit.point);
			}
			


		}
	}

	void OnMouseDrag(){

		int layerMask = 1 << 8;
		RaycastHit hit;
		if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {

			//Calculate vector rejection of vector 'a' going from line to raycast hit (mouse position on model plane).
			Vector3 a = hit.point - first.transform.position;
			Vector3 a2 = a - Vector3.Dot(a , unitVector) * unitVector;

			//Add the rejection vector to both boundary points of this line
			first.transform.position += a2;
			second.transform.position += a2;

			//Love linear algebra! 
		}else {
			Debug.Log ("RAY missed modelplane");
		}
	}

	void OnMouseEnter(){
		Debug.Log ("Enter Boundary");
	}

	void UpdateLine(){
		var start = first.transform.position;
		var end = second.transform.position;

		var parentScaleCompensation = transform.parent.transform.localScale.x;
		var offset = end - start;
		var scale = new Vector3(offset.magnitude/parentScaleCompensation, 1, 1);
		var position = start;

		//Save unit vector for other uses
		unitVector = offset.normalized;

		transform.position = position;
		transform.right = offset;
		transform.localScale = scale;
	}

	public GameObject Copy(Transform parent){
		GameObject copy = Instantiate (gameObject, parent) as GameObject;
		copy.transform.position = transform.position;
		copy.transform.rotation = transform.rotation;
		copy.transform.localScale = transform.localScale;

		return copy;
	}
}
