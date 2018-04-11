using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamBehaviour : MonoBehaviour {

	public GameObject line1;
	public GameObject line2;

	private List<GameObject> connections = new List<GameObject> ();

    private GameObject notch1;
    private GameObject notch2;
    private Vector3 notchPos1;
    private Vector3 notchPos2;

    private Color color;

	void Update(){
		if (line1 == null || line2 == null)
			return;

		if (line1.GetComponent<Selectable> ().isSelected () || line2.GetComponent<Selectable> ().isSelected ()) {
			if (Input.GetKeyUp (KeyCode.W)) {
				Swap ();
			}
				
		}
        //remove seam
        if (line1.GetComponent<Selectable>().isSelected() || line2.GetComponent<Selectable>().isSelected())
        {
            if (Input.GetKeyUp(KeyCode.B))
            {
                GetComponentInParent<ClothModelHandler>().RemoveSeam(this.gameObject);

            }

        }

       // UpdateColorCoding();
        UpdateNotches();
		UpdateConnections ();

	}
		
	void UpdateConnections(){

		var lineRendererOne = connections[0].GetComponent<LineRenderer> ();
		lineRendererOne.SetPosition (0, line1.GetComponent<BoundaryLineBehaviour>().start.position);
		lineRendererOne.SetPosition (1, line2.GetComponent<BoundaryLineBehaviour>().start.position);

		var lineRendererTwo = connections[1].GetComponent<LineRenderer> ();
		lineRendererTwo.SetPosition (0, line1.GetComponent<BoundaryLineBehaviour>().end.position);
		lineRendererTwo.SetPosition (1, line2.GetComponent<BoundaryLineBehaviour>().end.position);

	}

    void UpdateColorCoding()
    {
        line1.GetComponent<Renderer>().material.SetColor("_Color", this.color);
        line2.GetComponent<Renderer>().material.SetColor("_Color", this.color);
    }

    void UpdateNotches()
    {
        SetNotchPositions();
        notch1.transform.position = notchPos1;        
        notch1.transform.up = line1.GetComponent<BoundaryLineBehaviour>().unitVector;
        //color when selecting/highlighting
        notch1.GetComponent<Renderer>().material.SetColor("_Color",line1.GetComponent<Renderer>().material.color);

        notch2.transform.position = notchPos2;
        notch2.transform.up = line2.GetComponent<BoundaryLineBehaviour>().unitVector;
        //color when selecting/highlighting
        notch2.GetComponent<Renderer>().material.SetColor("_Color", line2.GetComponent<Renderer>().material.color);
    }


    void AddConnection(Transform start, Transform end){

		connections.Add(VibbiUtils.CreateLine (start.position, end.position, Color.blue, transform));

	}

	void OnDestroy(){
        line1.GetComponent<Selectable>().ResetSewingColor(color);
        line2.GetComponent<Selectable>().ResetSewingColor(color);
        foreach (GameObject c in connections) {
			Destroy (c);
		}
	}

	public void Init(GameObject line1, GameObject line2, Color color){ //remove hitpointline1 & 2
		this.line1 = line1;
		this.line2 = line2;

		var firstStart = line1.GetComponent<BoundaryLineBehaviour> ().start;
		var secondStart = line2.GetComponent<BoundaryLineBehaviour> ().start;
		var firstEnd = line1.GetComponent<BoundaryLineBehaviour> ().end;
		var secondEnd = line2.GetComponent<BoundaryLineBehaviour> ().end;

        //color
        //normal color = color
        this.color = color;
        line1.GetComponent<Selectable>().normalColor = color;
        line2.GetComponent<Selectable>().normalColor = color;

        InstantiateNotches();
       
        AddConnection(firstStart, secondStart);
		AddConnection(firstEnd, secondEnd);
	}

    //sets the position of the notches depending on where the start and endpoints of the line are
    private void SetNotchPositions()
    {
        //positions line one
        Vector3 positionFirstLineOne = line1.GetComponent<BoundaryLineBehaviour>().start.position;
        Vector3 positionSecondLineOne = line1.GetComponent<BoundaryLineBehaviour>().end.position;

        //positions line two
        Vector3 positionFirstLineTwo = line2.GetComponent<BoundaryLineBehaviour>().start.position;
        Vector3 positionSecondLineTwo = line2.GetComponent<BoundaryLineBehaviour>().end.position;

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
        notch1 = AddNotch(this.gameObject, line1, notchPos1, color);
        notch2 = AddNotch(this.gameObject, line2, notchPos2, color);
    }

	private GameObject AddNotch(GameObject seam, GameObject line, Vector3 notchPos, Color color)
	{
		GameObject notch = Instantiate(line.GetComponentInParent<BoundaryPointsHandler>().notchPrefab, notchPos, Quaternion.identity, seam.transform) as GameObject;
		notch.transform.up = line.GetComponent<BoundaryLineBehaviour>().unitVector;
		notch.GetComponent<Renderer>().material.SetColor("_Color", color);
		return notch;
	}


    public void Swap(){
		Debug.Log ("Swap not implemented");
		//endPoints.Reverse ();
	}

	public bool isSelected(){
		return line1.GetComponent<Selectable> ().isSelected () || line2.GetComponent<Selectable> ().isSelected ();
	}

	public GameObject GetFirstLine(){
		return line1;
	}

	public GameObject GetSecondLine(){
		return line2;
	}

	public Mesh GetFirstMesh(){
		Debug.Log("GetFirstMesh");
		return line1.GetComponentInParent<BoundaryPointsHandler> ().gameObject.GetComponent<MeshFilter>().mesh;
	}

	public Mesh GetSecondMesh(){
		Debug.Log("GetSecondMesh");
		return line2.GetComponentInParent<BoundaryPointsHandler> ().gameObject.GetComponent<MeshFilter>().mesh;
	}

	public GameObject GetFirstCloth(){
		return line1.GetComponentInParent<BoundaryPointsHandler> ().gameObject;
	}

	public GameObject GetSecondCloth(){
		return line2.GetComponentInParent<BoundaryPointsHandler> ().gameObject;
	}
}
