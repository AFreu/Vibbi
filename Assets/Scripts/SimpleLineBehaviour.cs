using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLineBehaviour : Behaviour {

	public Transform first;
	public Transform second;

    public Transform start;
    public Transform end;

	public Vector3 unitVector;
	private BoxCollider col;

	void Start(){
		addColliderToLine ();
	}
	
	// Update is called once per frame
	void Update () {
		if (first == null || second == null)
			return;
		UpdateLine ();
	}

	void UpdateLine(){
		var start = first.transform.localPosition;
		var end = second.transform.localPosition;

		Vector3[] n = { start, end };

		GetComponent<LineRenderer>().SetPositions(n);

		UpdateCollider();
		//UpdateColliderLocal ();

	}
		
	void UpdateCollider(){

		UpdateColliderSize ();

		var start = first.transform.position;
		var end = second.transform.position;

		var offset = end - start;

		Vector3 midPoint = start + offset / 2;

		//Save unit vector for other uses
		unitVector = offset.normalized;

		col.transform.position = midPoint; // setting position of collider object
		col.transform.right = unitVector;

	}

	void UpdateColliderSize(){

		var start = first.transform.localPosition;
		var end = second.transform.localPosition;

		var offset = end - start;

		col.size = new Vector3 (offset.magnitude, 0.1f, 0.1f);
	}

	public Vector3 GetMidPoint(){
		var start = first.transform.position;
		var end = second.transform.position;
		return (start + end) / 2;
	}

	private void addColliderToLine()
	{

		col = new GameObject("Collider").AddComponent<BoxCollider> ();
		col.gameObject.layer = gameObject.layer;
		col.transform.parent = transform; // Collider is added as child object of line

	}
}
