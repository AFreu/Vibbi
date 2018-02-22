using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class BoundaryLineBehaviour : MonoBehaviour{

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

	void OnMouseOver(){
		if (Input.GetKeyUp (KeyCode.A)) {
			int layerMask = 1 << 8;
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 30f, layerMask)) {
				gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().AddPoint(gameObject, hit.point);
			}
			


		}else if(Input.GetKeyUp(KeyCode.U)){
			gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().Unfold(gameObject);
		}
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

	/*public Ray getRayRepresentation(){
		return new Ray (new Vector3(first.position.x, first.position.y, first.position.z), new Vector3(unitVector.x, unitVector.y, unitVector.z));
	}*/


	/*private class MoveLineAction : AbstractAction{

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
			lineHandler.MoveLine (newFirst, newSecond);

		}

		protected override void UnExecuteCore()
		{
			Debug.Log ("RP: UnExecuteCore");
			lineHandler.MoveLine (oldFirst, oldSecond);

		}

	}*/
		
}
