using System;
using GuiLabs.Undo;
using UnityEngine;


public class AddClothAction : AbstractAction
{
	public AddClothAction(ClothModelHandler handler, Vector3 pos){
		position = pos;
		clothModelHandler = handler;

	}

	public GameObject clothModel { get; set; }
	public Vector3 position { get; set;}
	public ClothModelHandler clothModelHandler { get; set;}

	protected override void ExecuteCore()
	{
		if (clothModel == null) {
			clothModel = clothModelHandler.AddClothModel (position); 
		} else {
			clothModelHandler.ActivateClothModel (clothModel);
		}
	}

	protected override void UnExecuteCore()
	{
		clothModelHandler.DeactivateClothModel (clothModel);
	}
}




