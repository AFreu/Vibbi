using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLineBehaviour : Behaviour {

	public Transform first;
	public Transform second;

	public Vector3 unitVector;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (first == null || second == null)
			return;
		UpdateLine ();
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
}
