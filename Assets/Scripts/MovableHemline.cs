using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableHemline : Movable {

	public List<GameObject> lines;


	void Start(){
		this.lines = GetComponent<HemlineBehaviour> ().lines;
	}

	public override void SaveCurrentState(){
		currentPosition = transform.position;
		offset = transform.position - MouseWorldPosition ();

		foreach (GameObject line in lines) {
			line.GetComponent<MovableLine> ().SaveCurrentState ();
		}

	}

	public override void RecordChangeOfState(){
		//Record a move action if the point has moved since last call to MovePoint
		if (currentPosition != transform.position) {
			Debug.Log("m: OldPosition != NewPosition");


		}
	}

	public override void Move(Vector3 position){

		//Update position of hemline
		transform.position = position;

		var translation = position - currentPosition;

		//Update position of all lines
		foreach (GameObject line in GetComponent<HemlineBehaviour> ().lines) {
			var movableLine = line.GetComponent<MovableLine> ();

			movableLine.Move (translation + movableLine.currentPosition);
		}
	}

}
