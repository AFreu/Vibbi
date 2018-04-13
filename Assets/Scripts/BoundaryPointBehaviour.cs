using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPointBehaviour : Behaviour{

	public bool revertScaling = true; //Used to revert scaling done on parent, this object should never change in size

	[HideInInspector]
	public GameObject line;

	Transform scalableParent;
	Vector3 desiredScale;

	void Start(){
		
		scalableParent = GetComponentInParent<Scalable> ().transform;
		desiredScale = transform.localScale;

	}

	void Update(){
		if(revertScaling)
			RevertScaling ();
	}

	void OnMouseUp(){
		
		if (Input.GetKey (KeyCode.D) || interactionStateManager.currentState == InteractionStateManager.InteractionState.REMOVEPOINT) {

			GetComponentInParent<ClothModelBehaviour>().editedAndNotTriangulated = true;
			GetComponentInParent<BoundaryPointsHandler> ().RemovePoint(gameObject);

		}
	}

	void RevertScaling(){
		var X = scalableParent.localScale.x;
		var Y = scalableParent.localScale.y;
		var Z = scalableParent.localScale.z;

		if (X == 0 || Y == 0 || Z == 0)
			return;

		Vector3 n = new Vector3 (desiredScale.x / X, desiredScale.y / Y, desiredScale.z / Z);

		transform.localScale = n;
	}


}
