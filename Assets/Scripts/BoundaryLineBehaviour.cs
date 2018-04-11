using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoundaryLineBehaviour : SimpleLineBehaviour{

	public Transform start;
	public Transform end;
    public bool isFirstStart = true;
    
	void OnMouseUp(){

		//Get mouse position on screen
		Vector3 mousePos = Input.mousePosition;

		//Adjust mouse position 
		mousePos.z = transform.position.z - Camera.main.transform.position.z;

		//Get a world position for the mouse
		var hit =  Camera.main.ScreenToWorldPoint(mousePos);

        if (Input.GetKey(KeyCode.A) || interactionStateManager.currentState == InteractionStateManager.InteractionState.ADDPOINT) {
            GetComponentInParent<BoundaryPointsHandler>().AddPoint(gameObject, hit);
            //case where user adds a point on a seamline
            if (isFirstStart) end = second;
            else start = second;

        } else if (Input.GetKey(KeyCode.U) || interactionStateManager.currentState == InteractionStateManager.InteractionState.UNFOLDCLOTH) {
            GetComponentInParent<ClothModelBehaviour>().editedAndNotTriangulated = true; //if the polygon is unfolded, it should be triangulated
            GetComponentInParent<BoundaryPointsHandler>().Unfold(gameObject);

        } else if (Input.GetKey(KeyCode.K) || interactionStateManager.currentState == InteractionStateManager.InteractionState.SEW) {
            //switch first and second depending on hit position
            //shortest way to which point?
            start = first;
            end = second;

            Transform tmp = start;
            if ((first.position - hit).magnitude > (second.position - hit).magnitude) //if the length between first point and clicked point is larger than the second point and clicked point
            {
                start = end;
                end = tmp;
                isFirstStart = false;
            }
            //init sewing
            GetComponentInParent<ClothModelHandler>().InitSewing(this.gameObject);

            Debug.Log("Sewing");

        } 
	}
    
}
