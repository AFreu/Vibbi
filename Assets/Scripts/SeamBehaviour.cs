using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamBehaviour : MonoBehaviour {

	public GameObject lineOne;
	public GameObject lineTwo;

	private List<Transform> startPoints = new List<Transform> ();
	private List<Transform> endPoints = new List<Transform> ();

	private List<GameObject> connections = new List<GameObject>();

    private List<GameObject> notches = new List<GameObject>();

    private GameObject notch1;
    private GameObject notch2;
    private Vector3 notchPos1;
    private Vector3 notchPos2;

    private Color color;


	void Start(){
	}

	void Update(){
		if (lineOne == null || lineTwo == null)
			return;

		if (lineOne.GetComponent<Selectable> ().isSelected () || lineTwo.GetComponent<Selectable> ().isSelected ()) {
			if (Input.GetKeyUp (KeyCode.W)) {
				Swap ();
			}
				
		}

        UpdateColorCoding();
        UpdateNotches();
		UpdateConnections ();

	}

	void UpdateConnections(){
		for(int i = 0; i < connections.Count; i++) {
			var lineRenderer = connections[i].GetComponent<LineRenderer> ();
			lineRenderer.SetPosition (0, startPoints[i].position);
			lineRenderer.SetPosition (1, endPoints[i].position);
		}
	}

    void UpdateColorCoding()
    {
        lineOne.GetComponent<Renderer>().material.SetColor("_Color", this.color);
        lineTwo.GetComponent<Renderer>().material.SetColor("_Color", this.color);
    }

    void UpdateNotches()
    {
        SetNotchPositions();
        notch1.transform.position = notchPos1;
        //notch1.transform.position = lineOne.GetComponent<BoundaryLineBehaviour>().transform.position;
        notch1.transform.up = lineOne.GetComponent<BoundaryLineBehaviour>().unitVector;

        notch2.transform.position = notchPos2;
        //notch2.transform.position = lineTwo.GetComponent<BoundaryLineBehaviour>().transform.position;
        notch2.transform.up = lineTwo.GetComponent<BoundaryLineBehaviour>().unitVector;
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

	public void Init(GameObject line1, GameObject line2){ //remove hitpointline1 & 2
		lineOne = line1;
		lineTwo = line2;

		var p1 = lineOne.GetComponent<BoundaryLineBehaviour> ().start;
		var p2 = lineTwo.GetComponent<BoundaryLineBehaviour> ().start;
		var p3 = lineOne.GetComponent<BoundaryLineBehaviour> ().end;
		var p4 = lineTwo.GetComponent<BoundaryLineBehaviour> ().end;


        //color = VibbiUtils.RandomColor();
        Debug.Log("Setting seam color");
        color = lineOne.GetComponent<Selectable>().sewingColor;


        InstantiateNotches();
       
        AddConnection(p1, p2);
		AddConnection (p3, p4);
	}

    //sets the position of the notches depending on where the start and endpoints of the line are
    private void SetNotchPositions()
    {
        //positions line one
        Vector3 positionFirstLineOne = lineOne.GetComponent<BoundaryLineBehaviour>().start.position;
        Vector3 positionSecondLineOne = lineOne.GetComponent<BoundaryLineBehaviour>().end.position;

        //positions line two
        Vector3 positionFirstLineTwo = lineTwo.GetComponent<BoundaryLineBehaviour>().start.position;
        Vector3 positionSecondLineTwo = lineTwo.GetComponent<BoundaryLineBehaviour>().end.position;

        //directions
        Vector3 direction1 = positionSecondLineOne - positionFirstLineOne;
        Vector3 direction2 = positionSecondLineTwo - positionFirstLineTwo;

        //notch positions
        notchPos1 = positionFirstLineOne + direction1 / 4;
        notchPos2 = positionFirstLineTwo + direction2 / 4;
    }

    private void InstantiateNotches()
    {
        SetNotchPositions();
        notch1 = VibbiUtils.AddNotch(this.gameObject, lineOne, notchPos1, color);
        notch2 = VibbiUtils.AddNotch(this.gameObject, lineTwo, notchPos2, color);
    }


    public void Swap(){
		endPoints.Reverse ();
	}

	public bool isSelected(){
		return lineOne.GetComponent<Selectable> ().isSelected () || lineTwo.GetComponent<Selectable> ().isSelected ();
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

	public GameObject GetFirstCloth(){
		return lineOne.GetComponentInParent<BoundaryPointsHandler> ().gameObject;
	}

	public GameObject GetSecondCloth(){
		return lineTwo.GetComponentInParent<BoundaryPointsHandler> ().gameObject;
	}
}
