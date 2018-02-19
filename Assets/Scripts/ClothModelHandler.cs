using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothModelHandler : MonoBehaviour {

	public GameObject clothModelPrefab;
    public Material garmentMaterial;
    public DeformManager deformManager;

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


    public void Simulate()
    {
        GameObject go = new GameObject("A piece of cloth");
        go.transform.parent = deformManager.transform.parent;
        go.transform.localPosition = new Vector3(0,5,0);
        go.transform.localRotation = Quaternion.AngleAxis(90,new Vector3(1,0,0));
        DeformObject deformObject = go.AddComponent<DeformObject>();


        Mesh mesh = clothModels[0].GetComponent<BoundaryPointsHandler>().GetComponent<MeshFilter>().sharedMesh;

        for (int i = 0; i<mesh.GetIndices(0).Length; i++)
        {
            Debug.Log(mesh.GetIndices(0)[i]);

//            Debug.Log(mesh.GetIndices(0)[i]+" " + mesh.GetIndices(0)[i+1] +" "+ mesh.GetIndices(0)[i+2]);
        }



        deformObject.SetMesh(mesh);
        deformObject.SetMaterial(garmentMaterial);


        //deformManager.Reset();
    }








}
