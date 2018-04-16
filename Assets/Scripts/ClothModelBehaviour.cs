using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothModelBehaviour : Behaviour {

	private ClothModelHandler clothModelHandler;


    //for triangulation
    public bool editedAndNotTriangulated = true;
    
    public int id { set; get; }

    void Start(){
		clothModelHandler = GetComponentInParent<ClothModelHandler> ();
        int r = Random.Range(1,13);
        int h = (int)Time.realtimeSinceStartup;
        id = r * h;
	}

	void OnMouseUp(){
		if((Input.GetKey(KeyCode.D) && Input.GetKey (KeyCode.LeftControl)) || interactionStateManager.currentState == InteractionStateManager.InteractionState.DUPLICATECLOTH){
			interactionStateManager.DefaultInteractionState ();
			clothModelHandler.CopyCloth (gameObject, new Vector3 (1.0f, 1.0f, 0.0f));
		}else if(Input.GetKey (KeyCode.D) || interactionStateManager.currentState == InteractionStateManager.InteractionState.REMOVECLOTH){
			clothModelHandler.RemoveCloth (gameObject);
		}


	}
}
