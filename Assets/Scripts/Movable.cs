using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class Movable : MonoBehaviour {


	public bool snapToGrid = false;
	public float gridWidth = 0.3f;

	private static GameObject currentlyDragged;

	private Ray mousePositionRay;
	private Vector3 currentPosition;
	private Vector3 offset;

	protected ActionManager actionManager;
	protected InteractionStateManager interactionStateManager;

	void Awake() {
		actionManager = Component.FindObjectOfType<ActionManager> ();
		interactionStateManager = Component.FindObjectOfType<InteractionStateManager> ();
	}

	// Use this for initialization
	void Start () {
		currentPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		mousePositionRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		
	}

	void OnMouseDown() {
		if (interactionStateManager.currentState != InteractionStateManager.InteractionState.SELECT)
			return;
		
		if (currentlyDragged == null) {
			currentlyDragged = gameObject;
			SaveCurrentState ();

			RaycastHit hit;
			if (Physics.Raycast (mousePositionRay, out hit, 30f, LayerMask.GetMask("ModelPlane"))) {
				offset = transform.position - hit.point ;
			}
		}
	}

	void OnMouseUp(){
		if (interactionStateManager.currentState != InteractionStateManager.InteractionState.SELECT)
			return;

		if (currentlyDragged == gameObject) {
			currentlyDragged = null;

			Debug.Log ("M: OnMouseUp");
			RecordChangeOfState ();
		}
	}

	void OnMouseDrag() {
		if (interactionStateManager.currentState != InteractionStateManager.InteractionState.SELECT)
			return;
		
		if (currentlyDragged != gameObject)
			return;

		RaycastHit hit;
		if (Physics.Raycast (mousePositionRay, out hit, 30f, LayerMask.GetMask("ModelPlane"))) {

			Move (hit.point + offset);
		}else {
			Debug.Log ("RAY missed modelplane");
		}
	}

	public virtual void SaveCurrentState(){
		currentPosition = transform.position;
	}

	public virtual void RecordChangeOfState(){
		//Record a move action if the point has moved since last call to MovePoint
		if (currentPosition != transform.position) {
			Debug.Log("m: OldPosition != NewPosition");
			MoveAction mpa = new MoveAction (gameObject, currentPosition, transform.position);
			actionManager.RecordAction (mpa);
		}
	}

	public virtual void Move(Vector3 position){
		if (snapToGrid) {
			gameObject.transform.position = SnapToGrid (position);
		} else {
			gameObject.transform.position = position;
		}


	}

	protected Vector3 SnapToGrid(Vector3 pt){

		var X = pt.x;
		var Y = pt.y;

		if (pt.x % gridWidth < gridWidth / 2) {
			X = pt.x - pt.x % gridWidth;
		} else {
			X = pt.x + (gridWidth - pt.x % gridWidth);
		}


		if (pt.y % gridWidth < gridWidth / 2) {
			Y = pt.y - pt.y % gridWidth;
		} else {
			Y = pt.y + (gridWidth - pt.y % gridWidth);
		}
	
		//This depends on uneven Gridwidth
		X -= gridWidth/2;
		Y -= gridWidth/2;

		return new Vector3 (X, Y);
	}


	private class MoveAction : AbstractAction{


		public MoveAction(GameObject o, Vector3 oldPos, Vector3 newPos){
			Debug.Log("Creating MoveAction");
			gameObject = o;
			oldPosition = oldPos;
			newPosition = newPos;

		}

		public GameObject gameObject { get; set;}
		public Vector3 newPosition { get; set;}
		public Vector3 oldPosition { get; set;}

		protected override void ExecuteCore()
		{
			Debug.Log ("M: ExecuteCore");
			gameObject.transform.position = newPosition;

		}

		protected override void UnExecuteCore()
		{
			Debug.Log ("M: UnExecuteCore");
			gameObject.transform.position = oldPosition;

		}

	}
	
}
