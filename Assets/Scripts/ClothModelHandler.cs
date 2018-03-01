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


	// Use this for initialization
	void Start () {
		actionManager = GetComponentInParent<ActionManager> ();
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
            Debug.Log(positions[i]);
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

        int verticesPerRow = 120;
        int numberOfRows = 60;

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

        //figure out which triangles we need to look for : should be around the bounding box
       

        //nu kör vi
        int startElement = (subMeshIndices[0] - 240) * 3 * 2; //lowest index, *3 for three element per triangle, *2 for avg. 2 triangles per vertex/index
        int endElement = (subMeshIndices[subMeshIndices.Count - 1] + 240) * 3 * 2;

        Debug.Log("start "+ startElement);
        Debug.Log("end " +endElement);
        Debug.Log("yo "+ (endElement - startElement));

        float startTime = Time.realtimeSinceStartup;
        int numberOfTriangles = 0;
        int loopSteps = 0;
        List<Triangle> triangles = new List<Triangle>();
        //        Debug.Log("Number of indices " + subMeshIndices.Count);


        for (int i = startElement; i < endElement; i=i+3)
        {
            bool found = false;
            int triangleIndex = backgroundPatchMesh.triangles[i];
            

            for (int j = 0; j < subMeshIndices.Count; j++)
            {
                loopSteps++;
                if (triangleIndex == subMeshIndices[j])
                {
                    numberOfTriangles++;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                continue;
            }

            triangleIndex = backgroundPatchMesh.triangles[i+1];

            for (int j = 0; j < subMeshIndices.Count; j++)
            {
                loopSteps++;
                if (triangleIndex == subMeshIndices[j])
                {
                    numberOfTriangles++;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                continue;
            }

            triangleIndex = backgroundPatchMesh.triangles[i+2];


            for (int j = 0; j < subMeshIndices.Count; j++)
            {
                loopSteps++;
                if (triangleIndex == subMeshIndices[j])
                {
                    numberOfTriangles++;
                    found = true;
                    break;
                }
            }
        }


        float totalTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("NUM TRIANGLES: " + numberOfTriangles);
        Debug.Log("Time: " + totalTime);
        Debug.Log("Loopidoopi: " + loopSteps);

        /*
        for (int i = 0; i < subMeshIndices.Count; i++) //loop through all the indices in our submesh
        {
            indexLoops++;
            int currentTriangles = 0;
            int SMI = subMeshIndices[i];
            
            //for (int j = 0; j < backgroundPatchMesh.triangles.Length; j++)
            //for (int j = 120*2*startRow; j < 120*2*endRow; j++)
            for (int j = startElement; j < endElement; j++)
                //for (int j = 0; j < 3; j++)
                
            {
                loopSteps++;

                if (currentTriangles == 8) //can only be a maximum of eight triangles per index
                {
                    break;
                }
                
                if (SMI == backgroundPatchMesh.triangles[j]) // found a triangle
                {
                    currentTriangles++;
                    numberOfTriangles++;
                    /*
                    currentTriangles++;
                    Triangle triangle = new Triangle();
                    switch ((j+1) % 3) //use position in the list to determine which other indexes belong to the triangle
                    {
                        case 0: //last spot in the triangle
                            triangle.points.Add(backgroundPatchMesh.triangles[j - 2]);
                            triangle.points.Add(backgroundPatchMesh.triangles[j - 1]);
                            triangle.points.Add(backgroundPatchMesh.triangles[j]);
                            break;
                        case 1: //first spot
                            triangle.points.Add( backgroundPatchMesh.triangles[j]);
                            triangle.points.Add( backgroundPatchMesh.triangles[j + 1]);
                            triangle.points.Add(backgroundPatchMesh.triangles[j + 2]);
                            break;
                        case 2: //second spot
                            triangle.points.Add(backgroundPatchMesh.triangles[j - 1]);
                            triangle.points.Add(backgroundPatchMesh.triangles[j]);
                            triangle.points.Add(backgroundPatchMesh.triangles[j + 1]);
                            break;
                    }

                    //add triangle if it isn't already in the list
                    foreach (Triangle t in triangles)
                    {
                        if (!triangle.Equals(t))
                        {
                            triangles.Add(triangle);
                        }
                    }*/
        /*       }
           }
       }*/



        /*
        //now we have a list with triangles, but not all of them belong to the polygon since at least one of their vertices are not part of our submesh
        foreach (Triangle t in triangles)
        {
            int okays = 0;
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < subMeshIndices.Count; i++)
                {
                    if (t.points[i] == subMeshIndices[i])
                    {
                        okays++;
                        break;
                    }
                }
            }
            
            if (okays < 3)
            {
                triangles.Remove(t);
            }

        }

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
        Debug.Log("VERTEX "+ mesh.vertices[0]);
        mesh.triangles = tmpSubMeshTriangles.ToArray();
        Debug.Log("INDEX "+ mesh.triangles[0]);

        return mesh;
        */
        Mesh mesh = new Mesh();
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
       
        for (int i = 0; i < points.Count; i++)
        {
            if (! (this.points[i] == otherTriangle.points[i] 
                || this.points[i] == otherTriangle.points[i+1] 
                || this.points[i] == otherTriangle.points[i+2]))
            {
                return false;
            }
        }

        return true;
    }
    
}
