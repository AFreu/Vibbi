using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointBehaviour : Behaviour{

	public GameObject line;

	void OnMouseUp(){
		
		if (Input.GetKey (KeyCode.D) || interactionStateManager.currentState == InteractionStateManager.InteractionState.REMOVEPOINT) {
			gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().RemovePoint(gameObject);
		}

	}
}
