using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentHandler : MonoBehaviour {

	public GameObject clothPiecePrefab;

    public Material garmentMaterial;
    public DeformManager deformManager;

    public AttachmentPointsHandler attachMentPointsHandler;

    public List<GameObject> clothPieces = new List<GameObject>();
	public List<GameObject> garmentSeams = new List<GameObject>();
    
	public Color seamColor = Color.red;
	public float seamWidth = 0.01f;

	
	// Update is called once per frame
	void Update () {
        HandleInput();

	}

    void HandleInput()
    {
        if (Input.GetButtonUp("Start Simulation"))
        {
            Debug.Log("Start Simulation");
            StartSimulation();
        }

        if (Input.GetButtonUp("Stop Simulation"))
        {
            Debug.Log("Stop Simulation");
            StopSimulation();
        }
    }

    public void LoadCloth(GameObject clothModel)
    {

		//Create a cloth piece
		GameObject clothPiece = Instantiate (clothPiecePrefab, deformManager.transform.parent);

        Transform selectedAttachmentPoint = attachMentPointsHandler.getSelectedAttachmentPoint();
		if(selectedAttachmentPoint != null)
        {
			//Place cloth piece on the selected attachment point
			AttachCloth(clothPiece, selectedAttachmentPoint);
        }
        else
        {
			//Place cloth piece above avatar
			clothPiece.transform.localPosition = new Vector3(0, 11, 0);
			clothPiece.transform.localRotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
        }
        
		//Init cloth piece mesh according to the given cloth model mesh
		var clothModelMesh = clothModel.GetComponent<MeshFilter>().sharedMesh;
		clothPiece.GetComponent<MeshFilter> ().sharedMesh = clothModelMesh;
		clothPiece.GetComponent<MeshCollider>().sharedMesh = clothModelMesh;

		//Set garment material accordingly
		clothPiece.GetComponent<MeshRenderer> ().material = garmentMaterial;

		//Keep eventual scaling
		clothPiece.transform.localScale = clothModel.transform.localScale;

		clothPieces.Add(clothPiece);
    }

	public void UnloadCloth(GameObject clothPiece){

		List<GameObject> connectedSeams = new List<GameObject> ();

		foreach (GameObject seam in garmentSeams) {
			if (seam.GetComponent<GarmentSeamBehaviour> ().firstClothPiece == clothPiece ||
			   seam.GetComponent<GarmentSeamBehaviour> ().secondClothPiece == clothPiece) {
				connectedSeams.Add (seam);
			}
		}

		foreach (GameObject seam in connectedSeams) {
			UnloadSeam (seam);
		}

		clothPieces.Remove (clothPiece);
		Destroy (clothPiece);
	}

	public void UnloadSeam(GameObject garmentSeam){
		garmentSeams.Remove (garmentSeam);
		Destroy (garmentSeam);
	}

	public void UnloadAll(){

		//First unload all seams
		foreach (GameObject seam in garmentSeams) {
			Destroy (seam);
		}

		garmentSeams.Clear ();

		//Then unload all cloth pieces
		foreach (GameObject cloth in clothPieces) {
			Destroy (cloth);
		}

		clothPieces.Clear ();
	}

    public void AttachCloth(GameObject go, Transform t)
    {
        go.transform.position = t.position;
        go.transform.forward = -t.up;
    }

	public bool ClothIsLoaded(GameObject cloth){
		var clothModelMesh = cloth.GetComponent<MeshFilter> ().sharedMesh;

		for (int index = 0; index < clothPieces.Count; index++) {
			if (clothPieces[index].GetComponent<MeshFilter> ().sharedMesh.Equals (clothModelMesh)) {
				return true;
			}
		}
		return false;
	}

	public void LoadSeam(GameObject seam){
		Debug.Log ("Load Seam");
		var sb = seam.GetComponent<SeamBehaviour> ();
		int firstLineMeshIndex = -1;
		int secondLineMeshIndex = -1;
		bool firstMeshFound = false;
		bool secondMeshFound = false;



		for (int index = 0; index < clothPieces.Count; index++) {

			//Check if first cloth is loaded
			if (clothPieces[index].GetComponent<MeshFilter> ().sharedMesh.Equals (sb.GetFirstMesh ())) {
				Debug.Log ("Mesh 1 is previously loaded");
				firstLineMeshIndex = index;
				firstMeshFound = true;
			}

			//Check if second cloth is loaded
			if (clothPieces[index].GetComponent<MeshFilter> ().sharedMesh.Equals (sb.GetSecondMesh ())) {
				Debug.Log ("Mesh 2 is previously loaded");
				secondLineMeshIndex = index;
				secondMeshFound = true;
			}
		
		}


		if (firstMeshFound && secondMeshFound) {
			List<int> LineVerticeIndices = VibbiMeshUtils.DefineSeamFromLines (sb.GetFirstLine (), sb.GetSecondLine()); 

			if (LineVerticeIndices.Count <= 0 ) {
				Debug.Log ("Seam edge contains 0 vertices, aborting!");
				return;
			}

			CreateGarmentSeam (firstLineMeshIndex, secondLineMeshIndex, LineVerticeIndices, seam);
		}
	}

	private void CreateGarmentSeam(int firstClothPieceIndex, int secondClothPieceIndex, List<int> lineVerticeIndices, GameObject seam){

		GameObject garmentSeam = new GameObject ("GarmentSeam");
		garmentSeam.transform.parent = transform;

		LineRenderer renderer = garmentSeam.AddComponent<LineRenderer> ();
		renderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		renderer.startColor = seamColor;
		renderer.endColor = seamColor;
		renderer.startWidth = seamWidth;
		renderer.endWidth = seamWidth;

		GarmentSeamBehaviour garmentSeamBehaviour = garmentSeam.AddComponent<GarmentSeamBehaviour> ();
		garmentSeamBehaviour.Init (firstClothPieceIndex, secondClothPieceIndex, lineVerticeIndices, clothPieces[firstClothPieceIndex], clothPieces[secondClothPieceIndex], seam);

		garmentSeams.Add(garmentSeam);

	}

    private IDictionary<int, int> idToPositonInList = new Dictionary<int, int>();
    private int totalNumberOfVertices = 0;

    public void StartSimulation()
    {
        foreach(GameObject o in clothPieces)
         {
            Mesh mesh = o.GetComponent<MeshFilter>().sharedMesh;
            DeformObject deformObject = o.AddComponent<DeformObject>();

            deformObject.originalMesh = mesh;
            deformObject.material = garmentMaterial;
            deformObject.AddToSimulation();
        }
        
        deformManager.Reset();

    }

    public void StopSimulation()
    {
        deformManager.ShutDownDeformPlugin();
    }

    public void InitSeams()
    {
        foreach (GameObject seam in garmentSeams)
        {
            GarmentSeamBehaviour gsb = seam.GetComponent<GarmentSeamBehaviour>();
            int id1 = gsb.firstClothPiece.GetComponent<DeformObject>().GetId();
            int id2 = gsb.secondClothPiece.GetComponent<DeformObject>().GetId();

            uint[] vertices = new uint[gsb.lineVerticeIndices.Count];
            for (int i = 0; i < gsb.lineVerticeIndices.Count; i = i + 2)
            {
                vertices[i] = (uint)(gsb.lineVerticeIndices[i] + idToPositonInList[id1]);
                vertices[i + 1] = (uint)(gsb.lineVerticeIndices[i] + idToPositonInList[id2]);
            }

            deformManager.Sew(id1, id2, vertices, vertices.Length);
        }
    }

    public void setIDs()
    {
        //gå baklänges
        for (int i = clothPieces.Count-1; i > -1; i--)
        {
            idToPositonInList.Add(clothPieces[i].GetComponent<DeformObject>().GetId(), totalNumberOfVertices); //so that we can get global index when sewing
            totalNumberOfVertices += clothPieces[i].GetComponent<MeshFilter>().sharedMesh.vertexCount;  
        }
    }

	public void UpdateGarmentSeams(){
		foreach (GameObject o in garmentSeams) {
			o.GetComponent<GarmentSeamBehaviour> ().UpdateIndices ();
		}
	}
		
}
