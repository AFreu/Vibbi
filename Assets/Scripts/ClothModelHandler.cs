using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;
using System;

public class ClothModelHandler : MonoBehaviour {

	public GameObject clothModelPrefab;
	public GameObject seamModelPrefab;
    public Material garmentMaterial;
    public DeformManager deformManager;
    public GarmentHandler garmentHandler;
    
    private List<GameObject> clothModels = new List<GameObject> ();
	private List<GameObject> seamModels = new List<GameObject> ();

	private ActionManager actionManager;


    // Use this for initialization
    void Start () {
		actionManager = GetComponentInParent<ActionManager> ();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp (KeyCode.C)){
			AddCloth (new Vector3 (1.0f, 1.0f, 0.0f));

		}

		if(Input.GetButtonUp("Sew")){
			Debug.Log ("Sew");
			Sew ();
		}

		if (Input.GetKeyUp (KeyCode.Q)) {
			Debug.Log ("Undo");
			actionManager.Undo ();
		}

		if (Input.GetKeyUp (KeyCode.R)) {
			Debug.Log ("Redo");
			actionManager.Redo ();
		}
	}

	public void AddCloth(Vector3 position){
		AddClothAction aca = new AddClothAction (this, position);
		actionManager.RecordAction (aca);
	}

	public GameObject AddClothModel(Vector3 position){
		GameObject o = Instantiate (clothModelPrefab, transform) as GameObject;
		o.transform.Translate (position);
		o.GetComponent<BoundaryPointsHandler> ().InitQuad ();
		clothModels.Add (o);
		return o;
	}

	public GameObject AddClothModel(Vector3 position, Points points){
		GameObject o = Instantiate (clothModelPrefab, transform) as GameObject;
		o.transform.Translate (position);
		o.GetComponent<BoundaryPointsHandler> ().InitPolygon (points);
		clothModels.Add (o);
		return o;
	}

	public void RemoveCloth(GameObject clothModel){
		RemoveClothAction rca = new RemoveClothAction (this, clothModel);
		actionManager.RecordAction (rca);
	}

	public void RemoveClothModel(GameObject clothModel){
		clothModels.Remove (clothModel);
		GameObject.Destroy (clothModel);
		
	}

	public void ActivateClothModel(GameObject clothModel){
		
		clothModels.Add (clothModel);

		clothModel.transform.parent = transform;
		clothModel.SetActive (true);

	}

	public void DeactivateClothModel(GameObject clothModel){
		clothModels.Remove (clothModel);

		clothModel.transform.parent = transform.parent;
		clothModel.SetActive (false);
	}

	public GameObject CopyClothModel(GameObject clothModel, Vector3 position){

		GameObject o = Instantiate (clothModel, transform);
		o.GetComponent<BoundaryPointsHandler> ().InitCopy ();
		o.transform.Translate (position);
		clothModels.Add (o);

		return o;
	}

	public void CopyModel(GameObject clothModel, Vector3 position){
		CopyClothAction cca = new CopyClothAction (this, clothModel, position);
		actionManager.RecordAction (cca);
	}



	public void TriangulateModels()
	{
		foreach(GameObject o in clothModels){
			o.GetComponent<BoundaryPointsHandler> ().TriangulateModel ();
		}	
	}

    public void LoadCloth(GameObject gameObject)
    {
        garmentHandler.LoadCloth(gameObject);
    }


    public void Simulate()
    {

        GameObject go = new GameObject("A piece of cloth");
        DeformObject deformObject = go.AddComponent<DeformObject>();

        go.transform.parent = deformManager.transform.parent;
        go.transform.localPosition = new Vector3(0,1,0);
        go.transform.localRotation = Quaternion.AngleAxis(90,new Vector3(1,0,0));

        Mesh mesh = clothModels[0].GetComponent<BoundaryPointsHandler>().GetComponent<MeshFilter>().sharedMesh;

        deformObject.SetMesh(mesh);
        deformObject.SetMaterial(garmentMaterial);
        
        deformManager.Reset();
        
    }

	public void Sew(){
		List<GameObject> selectedLines = new List<GameObject> ();
		List<GameObject> selectedModels = new List<GameObject> ();
		foreach (GameObject c in clothModels) {
			var bph = c.GetComponent<BoundaryPointsHandler> ();
			List<GameObject> temp = bph.GetSelectedLines ();
			if (temp.Count != 0) {
				selectedModels.Add (c);
				selectedLines.AddRange(temp);
			}

			if (selectedLines.Count > 2) {
				break;
			}

		}

		Debug.Log ("#selectedLines: " + selectedLines.Count);

		if (selectedLines.Count == 2) {
			var firstLine = selectedLines [0];
			var secondLine = selectedLines [1];
			var firstModel = selectedModels [0];
			var secondModel = selectedModels [0];

			if (selectedModels.Count == 2) {
				secondModel = selectedModels [1];
			}


			CreateSeam (firstLine, secondLine);



			//TODO: sew first and second line together.


		} else if (selectedLines.Count < 2) {
			Debug.Log ("Too few edges selected for sewing, select two");
		}else if (selectedLines.Count > 2) {
			Debug.Log ("Too many edges selected for sewing, select only two");
		}
	}

	public GameObject CreateSeam(GameObject firstLine, GameObject secondLine){
		GameObject s = new GameObject ("Seam");
		s.transform.parent = transform;
		var seam = s.AddComponent<SeamBehaviour> ();
		seam.Init (firstLine, secondLine);
		seamModels.Add (s);
		return s;
	}

}
