using System;
using GuiLabs.Undo;
using UnityEngine;


public class CopyClothAction : AbstractAction
{
	public CopyClothAction(ClothModelHandler handler, GameObject cloth, Vector3 pos){
		
		clothModel = cloth;
		position = pos;
		clothModelHandler = handler;

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


