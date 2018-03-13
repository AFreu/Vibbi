using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamBehaviour : MonoBehaviour {

	public GameObject lineOne;
	public GameObject lineTwo;

	private List<Transform> startPoints = new List<Transform> ();
	private List<Transform> endPoints = new List<Transform> ();

	private List<GameObject> connections = new List<GameObject>();


	void Start(){
	}

	void Update(){
		if (lineOne == null || lineTwo == null)
			return;

		if (lineOne.GetComponent<Selectable> ().isSelected () || lineTwo.GetComponent<Selectable> ().isSelected ()) {
			if (Input.GetKeyUp (KeyCode.W)) {
				Swap ();
			}

			if (Input.GetKeyUp(KeyCode.L))
			{
				GetComponentInParent<ClothModelHandler>().LoadSeam(gameObject);
			}
		}

		UpdateConnections ();

	}

	void UpdateConnections(){
		for(int i = 0; i < connections.Count; i++) {
			var lineRenderer = connections[i].GetComponent<LineRenderer> ();
			lineRenderer.SetPosition (0, startPoints[i].position);
			lineRenderer.SetPosition (1, endPoints[i].position);
		}
	}

	void AddConnection(Transform start, Transform end){

		startPoints.Add (start);
		endPoints.Add (end);
		connections.Add(VibbiUtils.CreateLine (start.position, end.position, Color.blue, transform));

	}

	void OnDestroy(){
		foreach (GameObject c in connections) {
			Destroy (c);
		}
	}

	public void Init(GameObject line1, GameObject line2){
		lineOne = line1;
		lineTwo = line2;

		var p1 = lineOne.GetComponent<BoundaryLineBehaviour> ().first;
		var p2 = lineTwo.GetComponent<BoundaryLineBehaviour> ().first;
		var p3 = lineOne.GetComponent<BoundaryLineBehaviour> ().second;
		var p4 = lineTwo.GetComponent<BoundaryLineBehaviour> ().second;

		AddConnection (p1, p2);
		AddConnection (p3, p4);
	}

	public void Swap(){
		endPoints.Reverse ();
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
