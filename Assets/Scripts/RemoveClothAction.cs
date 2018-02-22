using System;
using GuiLabs.Undo;
using UnityEngine;


public class RemoveClothAction : AbstractAction
{
	public RemoveClothAction(ClothModelHandler handler, GameObject cloth){
		Debug.Log("Creating CopyClothAction");
		clothModel = cloth;
		clothModelHandler = handler;

	}

	public GameObject clothModel { get; set; }
	public ClothModelHandler clothModelHandler { get; set;}

	protected override void ExecuteCore()
	{
		Debug.Log ("CC: ExecuteCore");
		clothModelHandler.DeactivateClothModel (clothModel); 


	}

	protected override void UnExecuteCore()
	{
		clothModelHandler.ActivateClothModel (clothModel);
	}
}




