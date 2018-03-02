using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuiLabs.Undo;
using System;

public class ClothModelHandler : MonoBehaviour {

	public GameObject clothModelPrefab;
    public Material garmentMaterial;
    public DeformManager deformManager;
    public Mesh backgroundPatchMesh;

    private List<GameObject> clothModels = new List<GameObject> ();

	private ActionManager actionManager;


    private IDictionary<int, List<int>> indexToTrianglesDictionary = new Dictionary<int, List<int>>();


    // Use this for initialization
    void Start () {
		actionManager = GetComponentInParent<ActionManager> ();
        this.FindTrianglesInBackgroundPatch();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp (KeyCode.C)){
			AddCloth (new Vector3 (1.0f, 1.0f, 0.0f));

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


    public void Simulate()
    {

        GameObject go = new GameObject("A piece of cloth");
        DeformObject deformObject = go.AddComponent<DeformObject>();

        go.transform.parent = deformManager.transform.parent;
        go.transform.localPosition = new Vector3(0,5,0);
        go.transform.localRotation = Quaternion.AngleAxis(90,new Vector3(1,0,0));

        //clothModels[0].transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);

        //Mesh mesh = clothModels[0].GetComponent<BoundaryPointsHandler>().GetComponent<MeshFilter>().sharedMesh;
        Mesh mesh = CreateSubMesh();

        deformObject.SetMesh(mesh);
        deformObject.SetMaterial(garmentMaterial);



        deformManager.Reset();
        
    }

    public void FindTrianglesInBackgroundPatch()
    {
        //init dictionary
        int end = backgroundPatchMesh.vertices.Length; //speeds things up
        for (int i = 0; i < end; i++)
        {
            indexToTrianglesDictionary.Add(i, new List<int>());
        }
        
        //go through all triangles and put them in the dictionary
        end = backgroundPatchMesh.triangles.Length; //speeds things up a lot
        int[] triangles = backgroundPatchMesh.triangles; //this too
        for (int i = 0; i < end; i++)
        {
            indexToTrianglesDictionary[triangles[i]].Add(i);
        }

    }

    
    public Mesh CreateSubMesh()
    {
        //fix mesh
        List<GameObject> boundaryPoints = clothModels[0].GetComponent<BoundaryPointsHandler>().GetBoundaryPoints(); //get all points
        //get a list with all of the positions
        List<Vector3> positions = new List<Vector3>();
        int count = boundaryPoints.Count;
        for (int i = 0; i < count; i++)
        {
            positions.Add(boundaryPoints[i].transform.position);
        }
        //get ymax, ymin, xmax & xmin to create bounding box of the polygon
        float xmax = -100000.0f;
        float xmin = 100000.0f;
        float ymax = -100000.0f;
        float ymin = 100000.0f;
        foreach (Vector3 pos in positions)
        {
            if (pos.x > xmax)
                xmax = pos.x;
            if (pos.x < xmin)
                xmin = pos.x;
            if (pos.y > ymax)
                ymax = pos.y;
            if(pos.y < ymin)
                ymin = pos.y;
        }
        float epsilon = 0.166f;
        xmax += epsilon;
        xmin -= epsilon;
        ymax += epsilon;
        ymin -= epsilon;
        //background patch mesh
        List<int> subMeshIndices = new List<int>();
        List<Vector3> subMeshVertices = new List<Vector3>();

        float stepSize = 0.33f;
        int startRow =(int)((ymin + 10) / stepSize) -1;
        int endRow = (int)((ymax + 10) / stepSize) +1;
        int startCol = (int)((xmin + 20) / stepSize) - 1;
        int endCol = (int)((xmax + 20) / stepSize) + 1;

        
        for (int i = startRow * 120; i < endRow * 120; i++)
        {
            //if the vertex is outside of the bounding box don't add the vertice to our submesh
            if (! (backgroundPatchMesh.vertices[i].x > xmax
                || backgroundPatchMesh.vertices[i].x < xmin
                || backgroundPatchMesh.vertices[i].z > ymax
                || backgroundPatchMesh.vertices[i].z < ymin))
            {
                Vector3 theVertex = backgroundPatchMesh.vertices[i];
                subMeshVertices.Add(new Vector3(theVertex.x, theVertex.z, 0.0f));
                subMeshIndices.Add(i);
            }

        }


        //#########################
        //       TRIANGLES       //
        //#########################

        //dictionairy time
        

        



        //nu kör vi
        int startElement = (subMeshIndices[0] - 240) * 3 * 2; //lowest index, *3 for three element per triangle, *2 for avg. 2 triangles per vertex/index
        int endElement = (subMeshIndices[subMeshIndices.Count - 1] + 240) * 3 * 2;

        //debug
        Debug.Log("start "+ startElement);
        Debug.Log("end " +endElement);
        Debug.Log("yo "+ (endElement - startElement));
        float startTime = Time.realtimeSinceStartup;
        int numberOfTriangles = 0;
        int loopSteps = 0;
        //debug

        List<Triangle> triangles = new List<Triangle>();
        int[] backgroundPatchTriangles = backgroundPatchMesh.triangles; //speeds things up alot

        for (int i = startElement; i < endElement; i = i + 3)
        {
            int triangleIndex1 = backgroundPatchTriangles[i];
            int triangleIndex2 = backgroundPatchTriangles[i + 1];
            int triangleIndex3 = backgroundPatchTriangles[i + 2];

            for (int j = 0; j < subMeshIndices.Count; j++)
            {
                loopSteps++;
                if (triangleIndex1 == subMeshIndices[j]
                    || triangleIndex2 == subMeshIndices[j]
                    || triangleIndex3 == subMeshIndices[j])
                {
                    Triangle foundTriangle = new Triangle();
                    foundTriangle.points.Add(triangleIndex1);
                    foundTriangle.points.Add(triangleIndex2);
                    foundTriangle.points.Add(triangleIndex3);

                    //add triangle if it isn't already in the list
                    bool alreadyInList = false;
                    foreach (Triangle t in triangles)
                    {
                        if (t == null)
                        {
                            break;
                        }
                        if (foundTriangle.Equals(t))
                        {
                            alreadyInList = true;
                        }
                    }
                    if (!alreadyInList)
                    {
                        triangles.Add(foundTriangle);
                    }
                    numberOfTriangles++;
                    break;
                }
            }
        }
        

        float totalTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("NUM TRIANGLES: " + numberOfTriangles);
        Debug.Log("Time: " + totalTime);
        Debug.Log("Loopidoopi: " + loopSteps);
        Debug.Log("Num of triangles in list: "+ triangles.Count);


        //now we have a list with triangles, but not all of them belong to the polygon since at least one of their vertices are not part of our submesh
        for (int i = (triangles.Count-1); i >= 0; i--)
        {
            Triangle t = triangles[i];

            for (int j = 0; j < 3; j++)
            {
                bool found = false;
                for (int k = 0; k < subMeshIndices.Count; k++)
                {
                    if (t.points[j] == subMeshIndices[k])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    triangles.RemoveAt(i);
                    break;
                }
            }
        }

        
        Debug.Log("Triangle trim list: "+ triangles.Count);

        //translate the triangles to their correct indices for the submesh
        for (int i = 0; i < subMeshIndices.Count; i++)
        {
            foreach (Triangle t in triangles)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (subMeshIndices[i] == t.points[j])
                    {
                        t.points[j] = i;
                    }
                }
            }
        }

        //add all the point in triangles to a list of ints
        List<int> tmpSubMeshTriangles = new List<int>();
        foreach (Triangle t in triangles)
        {
            for (int i = 0; i < 3; i++)
            {
                tmpSubMeshTriangles.Add(t.points[i]);
            }
        }



        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = subMeshVertices.ToArray();
        mesh.triangles = tmpSubMeshTriangles.ToArray();

        return mesh;
    }
    
}

class Triangle
{

    public List<int> points;

    public Triangle()
    {
        points = new List<int>();
    }

    public bool Equals(Triangle otherTriangle)
    {
        for (int i = 0; i < 3; i ++)
        {
            if (!(this.points[i] == otherTriangle.points[i]))
            {
                return false;
            }
        }
        return true;
    }
    
}
