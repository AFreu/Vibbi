using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothModelHandler : MonoBehaviour {

	public GameObject clothModelPrefab;

	// Use this for initialization
	void Start () {

		GameObject o1 = Instantiate (clothModelPrefab) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp (KeyCode.C)){
			AddClothModel (new Vector3 (1.0f, 1.0f, 0.0f));
		}
	}

	void AddClothModel(Vector3 position){
		GameObject o = Instantiate (clothModelPrefab) as GameObject;
		o.transform.Translate (position);
	}
}
