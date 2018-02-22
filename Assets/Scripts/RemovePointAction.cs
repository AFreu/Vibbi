using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class RemovePointAction : AbstractAction{


	public RemovePointAction(BoundaryPointsHandler handler, GameObject point){
		Debug.Log("Creating RemovePointAction");
		boundaryPoint = point;
		boundaryPointsHandler = handler;

	}

	public GameObject boundaryPoint { get; set; }
	public GameObject boundaryLine { get; set; }
	public Vector3 position { get; set;}
	public BoundaryPointsHandler boundaryPointsHandler { get; set; }

	protected override void ExecuteCore()
	{
		Debug.Log ("RP: ExecuteCore");
		position = boundaryPoint.transform.position;
		boundaryLine = boundaryPointsHandler.DeactivateBoundaryPoint (boundaryPoint);

	}

	protected override void UnExecuteCore()
	{
		Debug.Log ("RP: UnExecuteCore");
		boundaryPoint = boundaryPointsHandler.ActivateBoundaryPoint (boundaryPoint, boundaryLine, position);

	}

}
