using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour : MonoBehaviour {

	protected InteractionStateManager interactionStateManager;

	void Awake(){
		interactionStateManager = Component.FindObjectOfType<InteractionStateManager> ();
	}

}
