using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;
using System;

public class ClothModelHandler : Behaviour {

	public GameObject clothModelPrefab;
	public GameObject seamModelPrefab;
	public GameObject hemlineModelPrefab;

    public Material garmentMaterial;
    public DeformManager deformManager;
    public GarmentHandler garmentHandler;
    
    private List<GameObject> clothModels = new List<GameObject> ();
	private List<GameObject> seamModels = new List<GameObject> ();
	private List<GameObject> hemlineModels = new List<GameObject> ();

    //sewing
    private List<GameObject> sewingList = new List<GameObject>();

	private ActionManager actionManager;

	private Color color;


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
            Debug.Log("Add Cloth");
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
			//Debug.Log ("Redo");
			//actionManager.Redo ();
		}

		if(Input.GetKeyUp(KeyCode.H)){
			Debug.Log ("Hemline");

			List<GameObject> selectedLines = new List<GameObject> ();
			foreach (GameObject cloth in clothModels) {
				selectedLines.AddRange (cloth.GetComponent<BoundaryPointsHandler> ().GetSelectedLines ());
			}

			CreateHemline (selectedLines);
		}

		if(Input.GetKeyUp(KeyCode.I)){
			Debug.Log("Load Hemlines");
			LoadHemlines();
		}


	}

	//Records an add cloth model action at the given position
	public void AddCloth(Vector3 position = new Vector3()){
        Debug.Log("Adding cloth");
        actionManager.RecordAction (new AddClothAction (this, position));

	}
		
	//Records an remove cloth model action of the given cloth model
	public void RemoveCloth(GameObject clothModel){

		actionManager.RecordAction (new RemoveClothAction (this, clothModel));

	}

	//Records an copy cloth model action at the given position
	public void CopyCloth(GameObject clothModel, Vector3 position){

		actionManager.RecordAction (new CopyClothAction (this, clothModel, position));

	}
		

	//Adds a cloth model at the given position, #predefinedCloth is optional
	public GameObject AddClothModel(Vector3 position, PredefinedCloth predefinedCloth = null){

		GameObject cloth = Instantiate (clothModelPrefab, transform) as GameObject;
		cloth.transform.Translate (position);
		cloth.GetComponent<BoundaryPointsHandler> ().Init (predefinedCloth);
		cloth.GetComponent<Fabricable> ().materialIndex = interactionStateManager.GetValue ("Selected Material");
		clothModels.Add (cloth);
		return cloth;

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

		GameObject newClothModel = Instantiate (clothModel, transform);
		newClothModel.GetComponent<BoundaryPointsHandler> ().InitCopy ();
		newClothModel.transform.Translate (position);
		clothModels.Add (newClothModel);

		return newClothModel;
	}


	public void TriangulateModels()
	{
		foreach(GameObject cloth in clothModels){
            Debug.Log("Triangulating cloth");
			cloth.GetComponent<Triangulatable> ().Triangulate ();
		}

		//When triangulating, the seam indices need to be updated
		garmentHandler.UpdateGarmentSeams ();
	}

	public void Load(){
		foreach (GameObject cloth in clothModels) {
			if (cloth.GetComponent<Selectable> ().isSelected ()) {
				LoadClothWithSeams (cloth);
			}
		}
	}

	public void LoadAll(){

		LoadAllCloths ();
		LoadAllSeams ();

	}

	public void LoadAllCloths(){
		foreach (GameObject cloth in clothModels) {
			LoadCloth (cloth);
		}
	}

	public void LoadAllSeams(){
		foreach (GameObject seam in seamModels) {
			LoadSeam (seam);
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

	public void LoadClothWithSeams(GameObject gameObject){
		Debug.Log("LoadClothWithSeams");
		LoadCloth (gameObject);

		foreach (GameObject seam in seamModels) {
			var first = seam.GetComponent<SeamBehaviour> ().GetFirstCloth ();
			var second = seam.GetComponent<SeamBehaviour> ().GetSecondCloth ();

			//If cloth has a seam, try to load it. It won't be loaded if the other cloth isn't
			if (first.Equals (gameObject) || second.Equals (gameObject)) {
				LoadSeam (seam);
			}
		}
	}




    public void Simulate()
    {

        GameObject go = new GameObject("A piece of cloth");
        DeformObject deformObject = go.AddComponent<DeformObject>();

        go.transform.parent = deformManager.transform.parent;
        go.transform.localPosition = new Vector3(0,1,0);
        go.transform.localRotation = Quaternion.AngleAxis(90,new Vector3(1,0,0));

        Mesh mesh = clothModels[0].GetComponent<BoundaryPointsHandler>().GetComponent<MeshFilter>().sharedMesh;

        //deformObject.SetMesh(mesh);
        //deformObject.SetMaterial(garmentMaterial);
        deformObject.mesh = mesh;
        deformObject.material = garmentMaterial;


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
        if (sewingList.Contains(line))
        {
            return;
        }

        if (sewingList.Count == 0)
        {
            //set selected material color
            color = VibbiUtils.RandomColor();
        }
        

        sewingList.Add(line);
        
        if (sewingList.Count == 2)
        {
            SewFromList();
            sewingList.Clear();
        }
        line.GetComponent<Selectable>().SetSewingColor(color);
    }

    public void SewFromList()
    {
        //write over the 'old' seam
        foreach (GameObject s in seamModels)
        {
            var seamBehaviour = s.GetComponent<SeamBehaviour>();
            if (seamBehaviour.GetFirstLine().Equals(sewingList[0]) 
                || seamBehaviour.GetFirstLine().Equals(sewingList[1])
                || seamBehaviour.GetSecondLine().Equals(sewingList[0])
                || seamBehaviour.GetSecondLine().Equals(sewingList[1]))
            {
                RemoveSeam(s);
                break; //only one seam possible to find
            }
        }
        
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
		GameObject seam = new GameObject ("Seam");
		seam.transform.parent = transform;
		var seamBehaviour = seam.AddComponent<SeamBehaviour> ();
		seamBehaviour.Init (firstLine, secondLine, color);
		seamModels.Add (seam);
		return seam;
	}

	public void RemoveSeam(GameObject seam){
		seamModels.Remove (seam);
		GameObject.Destroy (seam);
	}

	public GameObject CreateHemline(List<GameObject> lines){
		GameObject hemline = Instantiate (hemlineModelPrefab, transform);
		var hemlineBehaviour = hemline.GetComponent<HemlineBehaviour> ();
		hemlineBehaviour.Init (lines);
		hemlineModels.Add (hemline);

		return hemline;
	
	}

	public void LoadHemlines(){
		foreach (GameObject hemline in hemlineModels) {
			garmentHandler.LoadHemline (hemline);
		}
	}

}
