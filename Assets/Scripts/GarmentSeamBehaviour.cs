using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarmentSeamBehaviour : MonoBehaviour {

	public int firstMesh { set; get;}
	public int secondMesh{ set; get;}

	public GameObject firstClothPiece { set; get;}
	public GameObject secondClothPiece{ set; get;}

	public List<int> lineVerticeIndices { set; get;}
    public bool isSimulationRunning { set; get; }

	private GameObject seam;


	// Update is called once per frame
	void Update () {
        //if seam has been removed in 2D, remove it in 3D 
        if (seam == null)
        {
            Destroy(this.gameObject);
        }
        UpdateLineRenderer ();
	}


	//Called when triangulating
	public void UpdateIndices(){
        
		lineVerticeIndices = VibbiMeshUtils.DefineSeamFromLines (seam.GetComponent<SeamBehaviour>().GetFirstLine (), seam.GetComponent<SeamBehaviour>().GetSecondLine()); 

	}


	private void UpdateLineRenderer(){
		var renderer = GetComponent<LineRenderer> ();

        if (isSimulationRunning)
        {
            renderer.enabled = false;
            return;
        }

		var fMeshVertices = firstClothPiece.GetComponent<MeshCollider> ().sharedMesh.vertices;
		var sMeshVertices = secondClothPiece.GetComponent<MeshCollider> ().sharedMesh.vertices;

		var count = lineVerticeIndices.Count;
        //count = 4;

		renderer.positionCount = count;

		Vector3 [] positions = new Vector3[count];
		bool alt = true;
		for (int i = 0; i < count; i = i+2) {
            
			var start = firstClothPiece.transform.TransformPoint (fMeshVertices [lineVerticeIndices [i]]);
            var end = secondClothPiece.transform.TransformPoint(sMeshVertices[lineVerticeIndices[i+1]]);
           

            if (alt) {
				alt = false;
				positions [i] = start;
				positions [i + 1] = end;
			} else {
				alt = true;
				positions [i] = end;
				positions [i + 1] = start;
			}
		}
        //Debug.Log("positions: " + positions[0] + " " + positions[1] + " " + positions[2] + " " + positions[3]);
        //Debug.Log("indicesstart: " + lineVerticeIndices[0] + " " + lineVerticeIndices[1] + " " + lineVerticeIndices[2] + " " + lineVerticeIndices[3] + " " + lineVerticeIndices[4] + " " + lineVerticeIndices[5] + " " + lineVerticeIndices[6] + " " + lineVerticeIndices[7]);
        //Debug.Log("indicesend: " + lineVerticeIndices[count - 1] + " " + lineVerticeIndices[count - 2] + " " + lineVerticeIndices[count - 3] + " " + lineVerticeIndices[count - 4] + " " + lineVerticeIndices[count - 5] + " " + lineVerticeIndices[count - 6] + " " + lineVerticeIndices[count-7]);
        renderer.SetPositions (positions);
	}

	public void Init(int firstMesh, int secondMesh, List<int> lineVerticeIndices, GameObject firstClothPiece, GameObject secondClothPiece, GameObject seam){
		this.firstMesh = firstMesh;
		this.secondMesh = secondMesh;
		this.lineVerticeIndices = lineVerticeIndices;
		this.firstClothPiece = firstClothPiece;
		this.secondClothPiece = secondClothPiece;

		this.seam = seam;
        isSimulationRunning = false;

    }


}
