using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable))]
public class Copyable : MonoBehaviour {

	private Selectable obj;

	// Use this for initialization
	void Start () {
		obj = GetComponent<Selectable> ();
	}

	
	void Update () {

		//Implementation on works for cloth prefab at the moment!
		if (obj.isSelected () && Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.LeftCommand)) {
			gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().Duplicate();
		} else if (obj.isSelected () && Input.GetKeyUp (KeyCode.D)) {
			gameObject.transform.GetComponentInParent<BoundaryPointsHandler> ().Remove ();
		}
	}
}
