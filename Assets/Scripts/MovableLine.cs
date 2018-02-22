using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class MovableLine : Movable {
	private MoveLineAction mla;

	public override void SaveCurrentState(){
		//currentPosition = transform.position;
		mla = new MoveLineAction(GetComponent<BoundaryLineBehaviour> ());
	}

	public override void RecordChangeOfState(){
		if (mla.hasNewPosition()) {
			actionManager.RecordAction (mla);
		}

		/*//Record a move action if the point has moved since last call to MovePoint
		if (currentPosition != transform.position) {
			Debug.Log("m: OldPosition != NewPosition");
			MoveAction mpa = new MoveAction (gameObject, currentPosition, transform.position);
			actionManager.RecordAction (mpa);
		}*/
	}

	public override void Move(Vector3 position){

		var bL = GetComponent<BoundaryLineBehaviour> ();
		var first = bL.first;
		var second = bL.second;
		var unitVector = bL.unitVector;

		//Calculate vector rejection of vector 'a' going from line to raycast hit (mouse position on model plane).
		Vector3 a = position - first.transform.position;
		Vector3 a2 = a - Vector3.Dot(a , unitVector) * unitVector;

		//Add the rejection vector to both boundary points of this line
		first.transform.position += a2;
		second.transform.position += a2;

		//Love linear algebra! 
	}

	private class MoveLineAction : AbstractAction{

		public MoveLineAction(BoundaryLineBehaviour handler){
			lineHandler = handler;
			oldFirst = lineHandler.first.transform.position;
			oldSecond = lineHandler.second.transform.position;
		}

		public MoveLineAction(BoundaryLineBehaviour handler, Vector3 firstPosition, Vector3 secondPosition){
			Debug.Log("Creating MoveLineAction");

			lineHandler = handler;
			newFirst = lineHandler.first.transform.position;
			newSecond = lineHandler.second.transform.position;
			oldFirst = firstPosition;
			oldSecond = secondPosition;
		}

		public Vector3 oldFirst { get; set;}
		public Vector3 oldSecond { get; set;}
		public Vector3 newFirst { get; set;}
		public Vector3 newSecond { get; set;}
		public BoundaryLineBehaviour lineHandler { get; set;}

		public bool hasNewPosition(){

			newFirst = lineHandler.first.transform.position;
			newSecond = lineHandler.second.transform.position;

			return oldFirst != newFirst && oldSecond != newSecond;
		}

		protected override void ExecuteCore()
		{
			Debug.Log ("RP: ExecuteCore");
			lineHandler.first.transform.position = newFirst;
			lineHandler.second.transform.position = newSecond;
			//lineHandler.MoveLine (newFirst, newSecond);

		}

		protected override void UnExecuteCore()
		{
			Debug.Log ("RP: UnExecuteCore");
			lineHandler.first.transform.position = oldFirst;
			lineHandler.second.transform.position = oldSecond;

		}

	}
}
