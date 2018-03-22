using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointBehaviour : Behaviour{

	public GameObject line;

	Transform scalableParent;
	Vector3 wantedScale;

	public bool revertScaling = true;

	void Start(){
		scalableParent = transform.GetComponentInParent<BoundaryPointsHandler> ().transform;
		wantedScale = transform.localScale;

	}

	void Update(){
		if(revertScaling)
			RevertScaling ();
	}

	void OnMouseUp(){
		
		if (Input.GetKey (KeyCode.D) || interactionStateManager.currentState == InteractionStateManager.InteractionState.REMOVEPOINT) {
			transform.GetComponentInParent<BoundaryPointsHandler> ().RemovePoint(gameObject);
		}
	}

	void RevertScaling(){
		var X = scalableParent.localScale.x;
		var Y = scalableParent.localScale.y;
		var Z = scalableParent.localScale.z;

		if (X == 0 || Y == 0 || Z == 0)
			return;

		Vector3 n = new Vector3 (wantedScale.x / X, wantedScale.y / Y, wantedScale.z / Z);

		transform.localScale = n;
	}


}
