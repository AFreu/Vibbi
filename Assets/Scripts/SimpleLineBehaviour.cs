using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLineBehaviour : Behaviour {

	public Transform first;
	public Transform second;

	public Vector3 unitVector;

	[SerializeField]
	private BoxCollider col;

	protected virtual void Start(){

		if (col == null) {
			addColliderToLine ();
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (first == null || second == null)
			return;
		UpdateLine ();
	}

	void UpdateLine(){
		var startPosition = first.transform.localPosition;
		var endPosition = second.transform.localPosition;

		Vector3[] n = { startPosition, endPosition };

		GetComponent<LineRenderer>().SetPositions(n);

		UpdateCollider();
		//UpdateColliderLocal ();

	}
		
	void UpdateCollider(){

		UpdateColliderSize ();

		var startPosition = first.transform.position;
		var endPosition = second.transform.position;

		var offset = endPosition - startPosition;

		Vector3 midPoint = startPosition + offset / 2;

		//Save unit vector for other uses
		unitVector = offset.normalized;

		col.transform.position = midPoint; // setting position of collider object
		col.transform.right = unitVector;

	}

	void UpdateColliderSize(){

		var startPosition = first.transform.localPosition;
		var endPosition = second.transform.localPosition;

		var offset = endPosition - startPosition;

		col.size = new Vector3 (offset.magnitude, 0.1f, 0.1f);
	}

	public Vector3 GetMidPoint(){
		var startPosition = first.transform.position;
		var endPosition = second.transform.position;
		return (startPosition + endPosition) / 2;
	}

	private void addColliderToLine()
	{

		col = new GameObject("Collider").AddComponent<BoxCollider> ();
		col.gameObject.layer = gameObject.layer;
		col.transform.parent = transform; // Collider is added as child object of line

	}
}
