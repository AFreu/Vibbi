using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothModelHandler : MonoBehaviour {

	public GameObject clothModelPrefab;

	private List<GameObject> clothModels = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp (KeyCode.C)){
			AddClothModel (new Vector3 (1.0f, 1.0f, 0.0f));
		}
	}

	void AddClothModel(Vector3 position){
		GameObject o = Instantiate (clothModelPrefab, transform) as GameObject;
		o.transform.Translate (position);
		o.GetComponent<BoundaryPointsHandler> ().InitQuad ();
		clothModels.Add (o);
	}

	public void AddClothModel(Vector3 position, Points points){
		GameObject o = Instantiate (clothModelPrefab, transform) as GameObject;
		o.transform.Translate (position);
		o.GetComponent<BoundaryPointsHandler> ().InitPolygon (points);
		clothModels.Add (o);	
	}

	public void RemoveClothModel(GameObject clothModel){

		clothModels.Remove (clothModel);
		GameObject.Destroy (clothModel);
		
	}

	public void CopyClothModel(GameObject clothModel, Vector3 position){

		GameObject o = Instantiate (clothModel, transform);
		o.GetComponent<BoundaryPointsHandler> ().InitCopy ();
		o.transform.Translate (position);
		clothModels.Add (o);

	}

	public void TriangulateModels()
	{
		foreach(GameObject o in clothModels){
			o.GetComponent<BoundaryPointsHandler> ().TriangulateModel ();
		}	
	}








}
