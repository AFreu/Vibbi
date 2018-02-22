﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class Movable : MonoBehaviour {
	
	private Ray mousePositionRay;

	private static GameObject currentlyDragged;

	private Vector3 currentPosition;

	protected ActionManager actionManager;

	void Awake() {
		actionManager = Component.FindObjectOfType<ActionManager> ();
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
		if (currentlyDragged == null) {
			currentlyDragged = gameObject;
			SaveCurrentState ();

		}
	}

	void OnMouseUp(){
		if (currentlyDragged == gameObject) {
			currentlyDragged = null;

			Debug.Log ("M: OnMouseUp");
			RecordChangeOfState ();
		}
	}

	void OnMouseDrag() {
		if (currentlyDragged != gameObject)
			return;


		int layerMask = 1 << 8;

		RaycastHit hit;
		if (Physics.Raycast (mousePositionRay, out hit, 30f, layerMask)) {
			Move (hit.point);
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
		gameObject.transform.position = position;
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
