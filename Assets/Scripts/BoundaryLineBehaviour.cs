﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryLineBehaviour : SimpleLineBehaviour{

	void OnMouseOver(){
		if (Input.GetKeyUp (KeyCode.A)) {
			int layerMask = 1 << 8;
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 30f, layerMask)) {
				gameObject.transform.parent.GetComponentInParent<BoundaryPointsHandler> ().AddPoint(gameObject, hit.point);
			}

		}else if(Input.GetKeyUp(KeyCode.U)){
			gameObject.transform.parent.GetComponentInParent<BoundaryPointsHandler> ().Unfold(gameObject);
		}
	}
}
