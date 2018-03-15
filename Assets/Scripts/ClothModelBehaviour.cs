using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothModelBehaviour : Behaviour {

	private ClothModelHandler clothModelHandler;

	void Start(){
		clothModelHandler = GetComponentInParent<ClothModelHandler> ();
	}
	// Update is called once per frame
	void Update () {
        HandleInput();
	}

    void HandleInput()
	{
		if (GetComponent<Selectable> () == null) {
			Debug.Log ("Has no Selectable");
			return;
		}

		if (GetComponent<Selectable> ().isSelected ()) {
			if (Input.GetKeyUp (KeyCode.L)) {
				clothModelHandler.LoadCloth (gameObject);
			}
		}
	
	}

	void OnMouseUp(){
		if(Input.GetKey(KeyCode.D) && Input.GetKey (KeyCode.LeftControl) || interactionStateManager.currentState == InteractionStateManager.InteractionState.DUPLICATECLOTH){
			clothModelHandler.CopyModel (gameObject, new Vector3 (1.0f, 1.0f, 0.0f));
		}else if(Input.GetKey (KeyCode.D) || interactionStateManager.currentState == InteractionStateManager.InteractionState.REMOVECLOTH){
			clothModelHandler.RemoveCloth (gameObject);
		}
	}
}
