﻿using System.Collections;
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

    private List<GameObject> sewingList = new List<GameObject>();

	private ActionManager actionManager;


    // Use this for initialization
    void Start () {
		actionManager = GetComponentInParent<ActionManager> ();
	}

	// Update is called once per frame
	void Update () {
		
		HandleInput ();

	}

	private void HandleInput(){
		if(Input.GetKeyUp (KeyCode.C)){
			AddCloth ();
		}

		if(Input.GetButtonUp("Triangulate")){
			Debug.Log ("Triangulate");
			TriangulateModels ();
		}

		if(Input.GetButtonUp("Sew")){
			Debug.Log ("Sew");
			Sew ();
		}

		if (Input.GetButtonUp ("Load")) {
			Debug.Log ("LoadCloth");
			Load ();
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

	public void AddCloth(){
		AddCloth (new Vector3 (0.0f, 0.0f, 0.0f));
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

	public void Load(){
		foreach (GameObject cloth in clothModels) {
			if (cloth.GetComponent<Selectable> ().isSelected ()) {
				LoadCloth (cloth);
			}
		}

		foreach (GameObject seam in seamModels) {
			if (seam.GetComponent<SeamBehaviour> ().isSelected ()) {
				LoadSeam (seam);
			}
		}
	}

    public void LoadCloth(GameObject gameObject)
    {
        garmentHandler.LoadCloth(gameObject);
    }

	public void LoadSeam(GameObject gameObject)
	{
		garmentHandler.LoadSeam (gameObject);
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
        
        //deformManager.Reset();


        //////////////
        //yolo
        /////////////
        List<int> yobro = VibbiMeshUtils.DefineSeamFromLines(clothModels[1].GetComponent<BoundaryPointsHandler>().boundaryLines[0], clothModels[0].GetComponent<BoundaryPointsHandler>().boundaryLines[1]);

        Debug.Log(yobro.Count);
        for (int i = 0; i < yobro.Count; i+=2)
        {
            Debug.Log(yobro[i] + " " + yobro[i+1]);
        }
    }



    //takes a line and saves it in the sewing list, if two lines are present, SEW
    public void InitSewing(GameObject line)
    {
        Debug.Log("mittbena");
        sewingList.Add(line);
        if (sewingList.Count == 2)
        {
            SewFromList();
            sewingList.Clear();
        }
    }

    public void SewFromList()
    {
        Debug.Log("tjena");
        CreateSeam(sewingList[0], sewingList[1]);
    }

	public void Sew(){
		List<GameObject> selectedLines = new List<GameObject> ();

		foreach (GameObject c in clothModels) {
			var bph = c.GetComponent<BoundaryPointsHandler> ();
			selectedLines.AddRange(bph.GetSelectedLines ());

			if (selectedLines.Count > 2) {
				break;
			}
		}

		if (selectedLines.Count == 2) {
			var firstLine = selectedLines [0];
			var secondLine = selectedLines [1];

			CreateSeam (firstLine, secondLine);

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

	public void RemoveSeam(GameObject seam){
		seamModels.Remove (seam);
		GameObject.Destroy (seam);
	}

}
