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
    

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.S))
        {
            StartSimulation();
        }

	}

    public void LoadCloth(GameObject cloth)
    {

		GameObject go = Instantiate (clothPiecePrefab, deformManager.transform.parent);


        Transform t = attachMentPointsHandler.getSelectedAttachmentPoint();
        if(t != null)
        {
            AttachCloth(go, t);
        }
        else
        {
            go.transform.localPosition = new Vector3(0, 11, 0);
            go.transform.localRotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
        }
        
		go.GetComponent<MeshFilter>().sharedMesh = cloth.GetComponent<MeshFilter>().sharedMesh;
		go.GetComponent<MeshCollider>().sharedMesh = cloth.GetComponent<MeshFilter>().sharedMesh;
		go.GetComponent<MeshRenderer> ().material = garmentMaterial;

		go.transform.localScale = cloth.transform.localScale;

        clothPieces.Add(go);
    }

    public void AttachCloth(GameObject go, Transform t)
    {
        go.transform.position = t.position;
        go.transform.forward = -t.up;
    }

	public void LoadSeam(GameObject seam){
		Debug.Log ("Load Seam");
		var sb = seam.GetComponent<SeamBehaviour> ();
		int firstLineMeshIndex = -1;
		int secondLineMeshIndex = -1;
		bool firstMeshFound = false;
		bool secondMeshFound = false;
		for (int index = 0; index < clothPieces.Count; index++) {
			if (clothPieces[index].GetComponent<MeshFilter> ().sharedMesh.Equals (sb.GetFirstMesh ())) {
				Debug.Log ("Mesh 1 is previously loaded");
				firstLineMeshIndex = index;
				firstMeshFound = true;
			}

			if (clothPieces[index].GetComponent<MeshFilter> ().sharedMesh.Equals (sb.GetSecondMesh ())) {
				Debug.Log ("Mesh 2 is previously loaded");
				secondLineMeshIndex = index;
				secondMeshFound = true;
			}
		
		}

		if (firstMeshFound && secondMeshFound) {
			List<int> LineVerticeIndices = VibbiMeshUtils.DefineSeamFromLines (sb.GetFirstLine (), sb.GetSecondLine()); 
			//List<int> secondLineVerticeIndices = VibbiMeshUtils.VerticesFromLine (sb.GetSecondLine());
			if (LineVerticeIndices.Count <= 0 ) {
				Debug.Log ("Seam edge contains 0 vertices, aborting!");
				return;
			}
			CreateSeam (firstLineMeshIndex, secondLineMeshIndex, LineVerticeIndices);
		}
	}

	private void CreateSeam(int firstClothPieceIndex, int secondClothPieceIndex, List<int> lineVerticeIndices){
		GameObject garmentSeam = new GameObject ("GarmentSeam");
		garmentSeam.transform.parent = transform;
		var seam = garmentSeam.AddComponent<GarmentSeamBehaviour> ();
		seam.Init (firstClothPieceIndex, secondClothPieceIndex, lineVerticeIndices, clothPieces[firstClothPieceIndex], clothPieces[secondClothPieceIndex]);
	
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
		
}
