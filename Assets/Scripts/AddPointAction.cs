using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;

public class AddPointAction : AbstractAction{


	public AddPointAction(BoundaryPointsHandler handler, GameObject line, Vector3 pos){
		Debug.Log("Creating AddPointAction");
		boundaryLine = line;
		position = pos;
		boundaryPointsHandler = handler;

	}

	public GameObject boundaryPoint { get; set; }
	public GameObject boundaryLine { get; set; }
	public Vector3 position { get; set;}
	public BoundaryPointsHandler boundaryPointsHandler { get; set; }

	protected override void ExecuteCore()
	{
		Debug.Log ("AP: ExecuteCore");
		if (boundaryPoint == null) {
			boundaryPoint = boundaryPointsHandler.AddBoundaryPoint (boundaryLine, position);
		} else {
			boundaryPointsHandler.ActivateBoundaryPoint (boundaryPoint, boundaryLine, position);
		}


	}

	protected override void UnExecuteCore()
	{
		Debug.Log ("AP: UnExecuteCore");
		boundaryLine = boundaryPointsHandler.DeactivateBoundaryPoint (boundaryPoint);
	}

}