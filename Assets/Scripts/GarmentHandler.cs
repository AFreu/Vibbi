using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentHandler : MonoBehaviour {

    public Material garmentMaterial;
    public DeformManager deformManager;

    public AttachmentPointsHandler attachMentPointsHandler;

    public List<GameObject> clothPieces = new List<GameObject>();
	private List<GarmentSeam> seams = new List<GarmentSeam>();
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.S))
        {
            StartSimulation();
        }


		UpdateGarmentSeams ();

	}

	private void UpdateGarmentSeams(){
		foreach (GarmentSeam s in seams) {
			var firstClothPiece = clothPieces [s.firstMesh];
			var secondClothPiece = clothPieces [s.secondMesh];
			var firstMeshVertices = firstClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;
			var secondMeshVertices = secondClothPiece.GetComponent<MeshFilter> ().sharedMesh.vertices;



			for (int i = 0; i < s.firstMeshVertices.Count; i++) {

				int j = i;
				if (j >= s.secondMeshVertices.Count) {
					j = s.secondMeshVertices.Count - 1;
				}

				var start = firstClothPiece.transform.TransformPoint (firstMeshVertices [s.firstMeshVertices [i]]);
				var end = secondClothPiece.transform.TransformPoint (secondMeshVertices [s.secondMeshVertices [j]]);

				VibbiUtils.DrawLine (start, end, Color.red, 0.1f);
			}
		}
	}



    public void LoadCloth(GameObject cloth)
    {

        GameObject go = new GameObject("A piece of cloth");
        
        go.transform.parent = deformManager.transform.parent;

        Transform t = attachMentPointsHandler.getSelectedAttachmentPoint();
        if(t != null)
        {
            AttachCloth(go, t);
        }
        else
        {
            go.transform.localPosition = new Vector3(0, 5, 0);
            go.transform.localRotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
        }
        
        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.sharedMesh = cloth.GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        renderer.material = garmentMaterial;

        clothPieces.Add(go);
    }

    public void AttachCloth(GameObject go, Transform t)
    {
        go.transform.localPosition = t.localPosition;
        go.transform.forward = t.up;
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
			List<int> firstLineVerticeIndices = VibbiMeshUtils.VerticesFromLine (sb.GetFirstLine ()); 
			List<int> secondLineVerticeIndices = VibbiMeshUtils.VerticesFromLine (sb.GetSecondLine());
			if (firstLineVerticeIndices.Count <= 0 || secondLineVerticeIndices.Count <= 0) {
				Debug.Log ("Seam edge contains 0 vertices, aborting!");
				return;
			}
			//CreateSeam (firstLineMeshIndex, secondLineMeshIndex, firstLineVerticeIndices, secondLineVerticeIndices);
		}
	}

	/*private void CreateSeam(int firstClothPieceIndex, int secondClothPieceIndex, List<int> firstVerticeIndices, List<int> secondVerticeIndices){
		GameObject s = new GameObject ("GarmentSeam");
		s.transform.parent = transform;
		var seam = s.AddComponent<GarmentSeamBehaviour> ();
		seam.Init (firstLine, secondLine);
		seamModels.Add (s);
		return s;

		//TODO Create seam lines

		seams.Add (new GarmentSeam (firstClothPieceIndex, secondClothPieceIndex, firstVerticeIndices, secondVerticeIndices));
	}*/


    public void StartSimulation()
    {
        foreach(GameObject o in clothPieces)
         {
            Mesh mesh = o.GetComponent<MeshFilter>().sharedMesh;
            DeformObject deformObject = o.AddComponent<DeformObject>();
            deformObject.SetMesh(mesh);
            deformObject.SetMaterial(garmentMaterial);
        }
        
        deformManager.Reset();

    }

	 private struct GarmentSeam {
		public int firstMesh { set; get;}
		public int secondMesh{ set; get;}

		public List<int> firstMeshVertices { set; get;}
		public List<int> secondMeshVertices { set; get;}

		public GarmentSeam(int firstMesh, int secondMesh, List<int> firstMeshVertices, List<int> secondMeshVertices){
			this.firstMesh = firstMesh;
			this.secondMesh = secondMesh;
			this.firstMeshVertices = firstMeshVertices;
			this.secondMeshVertices = secondMeshVertices;
		}
	}
}
