using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class Movable : MonoBehaviour {


	public bool snapToGrid = false;
	public float gridWidth = 0.3f;

	private static GameObject currentlyDragged;

	private Vector3 currentPosition;
	protected Vector3 offset;

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

	void OnMouseDown() {
		if (interactionStateManager.currentState != InteractionStateManager.InteractionState.SELECT && interactionStateManager.currentState != InteractionStateManager.InteractionState.ADDPOINT )
			return;

        if (interactionStateManager.currentState == InteractionStateManager.InteractionState.ADDPOINT &&
            gameObject.GetComponent<BoundaryLineBehaviour>() != null) //if we are in add point interaction state we don't want to move the lines around, just the points
        {
            return;
        }


		if (currentlyDragged == null) {
			currentlyDragged = gameObject;
			SaveCurrentState ();

		}
	}

	void OnMouseUp(){
		if (interactionStateManager.currentState != InteractionStateManager.InteractionState.SELECT && interactionStateManager.currentState != InteractionStateManager.InteractionState.ADDPOINT)
			return;

		if (currentlyDragged == gameObject) {
			currentlyDragged = null;
			RecordChangeOfState ();
		}
	}

	void OnMouseDrag() {
		if (interactionStateManager.currentState != InteractionStateManager.InteractionState.SELECT && interactionStateManager.currentState != InteractionStateManager.InteractionState.ADDPOINT)
			return;
		
		if (currentlyDragged != gameObject)
			return;

		Move (MouseWorldPosition() + offset);

	}

	protected Vector3 MouseWorldPosition(){

		//Get mouse position on screen
		Vector3 mousePos = Input.mousePosition;

		//Adjust mouse position 
		mousePos.z = transform.position.z - Camera.main.transform.position.z;
			
		//Get a world position for the mouse
		return Camera.main.ScreenToWorldPoint(mousePos);

	}

	public virtual void SaveCurrentState(){
		currentPosition = transform.position;
		offset = transform.position - MouseWorldPosition ();
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
			transform.position = SnapToGrid (position);
		} else {
			transform.position = position;

            //polygon has been edited if a point or line was moved
            if (gameObject.tag.Equals("BoundaryPoint"))
            {
                GetComponentInParent<ClothModelBehaviour>().editedAndNotTriangulated = true;
            }

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
