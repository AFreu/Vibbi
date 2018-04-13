using System;
using GuiLabs.Undo;
using UnityEngine;


public class CopyClothAction : AbstractAction
{
	public CopyClothAction(ClothModelHandler clothModelHandler, GameObject clothModel, Vector3 position){
		
		this.clothModel = clothModel;
		this.position = position;
		this.clothModelHandler = clothModelHandler;

	}

	public GameObject clothModel { get; set; }
	public GameObject copyModel { get; set;}
	public Vector3 position { get; set;}
	public ClothModelHandler clothModelHandler { get; set;}

	protected override void ExecuteCore()
	{
		
		if (copyModel == null) {
			copyModel = clothModelHandler.CopyClothModel (clothModel, position); 
		} else {
			clothModelHandler.ActivateClothModel (copyModel);
		}


	}

	protected override void UnExecuteCore()
	{
		clothModelHandler.DeactivateClothModel (copyModel);
	}
}


