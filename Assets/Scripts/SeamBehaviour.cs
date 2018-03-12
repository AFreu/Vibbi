using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamBehaviour : MonoBehaviour {

	public GameObject lineOne;
	public GameObject lineTwo;


	void Start(){
	}

	void Update(){
	}

	public void Init(GameObject line1, GameObject line2){
		lineOne = line1;
		lineTwo = line2;
	}

	public GameObject GetFirstLine(){
		return lineOne;
	}

	public GameObject GetSecondLine(){
		return lineTwo;
	}

	public Mesh GetFirstMesh(){
		return lineOne.GetComponentInParent<BoundaryPointsHandler> ().gameObject.GetComponent<MeshFilter>().mesh;
	}

	public Mesh GetSecondMesh(){
		return lineTwo.GetComponentInParent<BoundaryPointsHandler> ().gameObject.GetComponent<MeshFilter>().mesh;
	}
}
