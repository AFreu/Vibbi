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
        //Mesh mesh = CreateSubMesh();
        Mesh mesh = CreateSubMeshFromSmallPatch();

        deformObject.SetMesh(mesh);
        deformObject.SetMaterial(garmentMaterial);
        
        deformManager.Reset();
        
    }

    public void FindTrianglesInBackgroundPatch()
    {
        //init the dictionary
        int end = backgroundPatchMesh.vertices.Length; //speeds things up
        for (int i = 0; i < end; i++)
        {
            indexToTrianglesDictionary.Add(i, new List<int>()); //key and empty list
        }
        
        //go through all triangles and put them in the dictionary
        end = backgroundPatchMesh.triangles.Length; //speeds things up a lot
        int[] triangles = backgroundPatchMesh.triangles; //this too
        for (int i = 0; i < end; i++)
        {
            indexToTrianglesDictionary[triangles[i]].Add(i);
        }

    }



    public Mesh CreateSubMeshFromSmallPatch()
    {

       
        List<GameObject> boundaryPoints = clothModels[0].GetComponent<BoundaryPointsHandler>().GetBoundaryPoints(); //get all points
        List<Vector3> positions = new List<Vector3>(); //get a list with all of the positions
        int count = boundaryPoints.Count;
        for (int i = 0; i < count; i++)
        {
            positions.Add(boundaryPoints[i].transform.position);
        }

        //background patch mesh
        List<int> subMeshIndices = new List<int>();
        List<Vector3> subMeshVertices = new List<Vector3>();
        //info about background mesh
        int resolution = 120;
        float width = 5;
        float height = 5;
//        float GRIDWIDTH = boundaryPoints[0].GetComponent<Movable>().GRIDWIDTH;
        float GRIDWIDTH = height > width ? (height / resolution) : (width / resolution); //i've done the math
        float epsilon = GRIDWIDTH / 2; //ensures that we get the points on the boundary as well


        float bbTime = Time.realtimeSinceStartup;
        #region Bounding Box
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
            if (pos.y < ymin)
                ymin = pos.y;
        }
        xmax += epsilon;
        xmin -= epsilon;
        ymax += epsilon;
        ymin -= epsilon;

        Debug.Log("XMAX: "+ xmax);
        Debug.Log("XMIN: " + xmin);
        Debug.Log("YMAX: " + ymax);
        Debug.Log("YMIN: " + ymin);
        #endregion
        Debug.Log("BB TIME: "+ (Time.realtimeSinceStartup - bbTime));

        float PIPTime = Time.realtimeSinceStartup;
        #region PIP
        //######################################
        // position = -2.5 + (stepsize * rader)
        // => rader = (position + 2.5) / stepsize
        //########################################
        float srow = (ymin + (height / 2)) / GRIDWIDTH;
        float erow = (ymax + (height / 2)) / GRIDWIDTH;

        int startIndex = (int)srow * resolution;
        int endIndex = (int)erow * resolution;
       
        //int startCol = (int)((xmin - (width / 2)) / GRIDWIDTH) - 1;
        //int endCol = (int)((xmax - (width / 2)) / GRIDWIDTH) + 1;

        
        Vector3[] bPMVertices = backgroundPatchMesh.vertices;
        for (int i = startIndex; i < endIndex; i++)
        {
            Vector3 theVertex = bPMVertices[i];
           //if the vertex is outside of the bounding box don't add the vertice to our submesh
            if (!(theVertex.x > xmax
                || theVertex.x < xmin
                || theVertex.z > ymax
                || theVertex.z < ymin))
            {
                subMeshVertices.Add(new Vector3(theVertex.x, theVertex.z, 0.0f));
                subMeshIndices.Add(i);
            }

        }
        #endregion
        Debug.Log("PIP TIME: " + (Time.realtimeSinceStartup - PIPTime));
        
        #region Triangles Dictionary
        //triangles, dictionary
        /*List<Triangle> subMeshTriangles = new List<Triangle>();
        int[] BPMtriangles = backgroundPatchMesh.triangles;
        for (int i = 0; i < subMeshIndices.Count; i++)
        {
            //for every index get the list of triangles
            List<int> triangleList = indexToTrianglesDictionary[subMeshIndices[i]];

            //go through list and add all triangles
            for (int j = 0; j < triangleList.Count; j++)
            {
                int[] tPoints = new int[3];

                switch ((triangleList[j] + 1) % 3)
                {
                    case 0: //delbart med 3
                        tPoints[0] = BPMtriangles[triangleList[j] - 2];
                        tPoints[1] = BPMtriangles[triangleList[j] - 1];
                        tPoints[2] = subMeshIndices[i];
                        break;
                    case 1://andraplatsen
                        tPoints[0] = BPMtriangles[triangleList[j] - 1];
                        tPoints[1] = subMeshIndices[i];
                        tPoints[2] = BPMtriangles[triangleList[j] + 1];
                        break;
                    case 2: //förstaplats!
                        tPoints[0] = subMeshIndices[i];
                        tPoints[1] = BPMtriangles[triangleList[j] + 1];
                        tPoints[2] = BPMtriangles[triangleList[j] + 2];
                        break;
                }


                //are all points part of submeshindices? i.e, is the triangle part of the polygon
                int okays = 0;
                bool addTriangle = false;
                for (int k = 0; k < subMeshIndices.Count; k++)
                {
                    if (tPoints[0] == subMeshIndices[k]
                        || tPoints[1] == subMeshIndices[k]
                        || tPoints[2] == subMeshIndices[k])
                    {
                        okays++;
                    }
                    if (okays == 3)
                    {
                        addTriangle = true;
                        break;
                    }
                }

                
                if (addTriangle)
                {
                    Triangle newTriangle = new Triangle();
                    newTriangle.points = tPoints;
                    foreach (Triangle t in subMeshTriangles)
                    {
                        if (newTriangle.Equals(t))
                        {
                            addTriangle = false;
                            break;
                        }
                    }

                    if (addTriangle)
                    {
                        subMeshTriangles.Add(newTriangle);
                    }
                }

            }

        }
        Debug.Log(subMeshTriangles.Count);*/
        #endregion

        #region Triangles Not Dictionary
        //#########################
        //       TRIANGLES       //
        //#########################
        //nu kör vi
        int startElement = (subMeshIndices[0] - resolution * 2) * 3 * 2; //lowest index, *3 for three element per triangle, *2 for avg. 2 triangles per vertex/index
        int endElement = (subMeshIndices[subMeshIndices.Count - 1] + resolution * 2) * 3 * 2;

        //debug
        float startTime = Time.realtimeSinceStartup;
        int numberOfTriangles = 0;
        int loopSteps = 0;
        int foundT = 0;
        //debug

        List<Triangle> triangles = new List<Triangle>();
        int[] backgroundPatchTriangles = backgroundPatchMesh.triangles; //speeds things up alot
    
        //for (int i = startElement; i < endElement; i = i + 3)
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
                    foundTriangle.points[0] = (triangleIndex1);
                    foundTriangle.points[1] = (triangleIndex2);
                    foundTriangle.points[2] = (triangleIndex3);

                    
                    triangles.Add(foundTriangle);
                   
                    numberOfTriangles++;
                    break;
                }
            }
        }


        float totalTime = Time.realtimeSinceStartup - startTime;

        Debug.Log("NUM TRIANGLES: " + numberOfTriangles);
        Debug.Log("Time: " + totalTime);
        Debug.Log("Loopidoopi: " + loopSteps);
        Debug.Log("Num of triangles in list: " + triangles.Count);
        Debug.Log("Number found double: "+ foundT);

        #endregion

        #region Trim TriangleList
        
        float trimTime = Time.realtimeSinceStartup;
        //now we have a list with triangles, but not all of them belong to the polygon since at least one of their vertices are not part of our submesh
        for (int i = (triangles.Count - 1); i >= 0; i--)
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


        Debug.Log("Triangle trim list: " + triangles.Count);
        Debug.Log("TRIMTIME: "+ (Time.realtimeSinceStartup - trimTime));
        

        float transTime = Time.realtimeSinceStartup;
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
        #endregion

        Debug.Log("TRansTIME: " + (Time.realtimeSinceStartup - transTime));

        float listIntTime = Time.realtimeSinceStartup;
        //add all the point in triangles to a list of ints
        List<int> tmpSubMeshTriangles = new List<int>();
        foreach (Triangle t in triangles)
        {
            for (int i = 0; i < 3; i++)
            {
                tmpSubMeshTriangles.Add(t.points[i]);
            }
        }
        Debug.Log("LIST INT TIME: "+ (Time.realtimeSinceStartup - listIntTime));

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = subMeshVertices.ToArray();
        mesh.triangles = tmpSubMeshTriangles.ToArray();
        
        return mesh;
    }



    
    public Mesh CreateSubMesh()
    {
        //fix mesh
        List<GameObject> boundaryPoints = clothModels[0].GetComponent<BoundaryPointsHandler>().GetBoundaryPoints(); //get all points
        float GRIDWIDTH = boundaryPoints[0].GetComponent<Movable>().GRIDWIDTH;
        float epsilon = GRIDWIDTH / 2;
        //info about background mesh
        int resolution = 120;
        int width = 5;
        int height = 5;


        //get a list with all of the positions
        List<Vector3> positions = new List<Vector3>();
        int count = boundaryPoints.Count;
        for (int i = 0; i < count; i++)
        {
            positions.Add(boundaryPoints[i].transform.position);
        }

        #region BoundingBox
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
        xmax += epsilon;
        xmin -= epsilon;
        ymax += epsilon;
        ymin -= epsilon;
        #endregion

        //background patch mesh
        List<int> subMeshIndices = new List<int>();
        List<Vector3> subMeshVertices = new List<Vector3>();
        
        //######################################
        // position = -2.5 + (stepsize * rader)
        // => rader = (position + -2.5) / stepsize
        //########################################
        int startRow =(int)((ymin - (height/2)) / GRIDWIDTH) -1;
        int endRow = (int)((ymax - (height/2)) / GRIDWIDTH) +1;
        int startCol = (int)((xmin - (width/2)) / GRIDWIDTH) - 1;
        int endCol = (int)((xmax - (width/2)) / GRIDWIDTH) + 1;

        
        for (int i = startRow * resolution; i < endRow * resolution; i++)
        {
            Vector3 theVertex = backgroundPatchMesh.vertices[i];
            //if the vertex is outside of the bounding box don't add the vertice to our submesh
            if (! (theVertex.x > xmax
                || theVertex.x < xmin
                || theVertex.z > ymax
                || theVertex.z < ymin))
            {
                subMeshVertices.Add(new Vector3(theVertex.x, theVertex.z, 0.0f));
                subMeshIndices.Add(i);
            }

        }


        //#########################
        //       TRIANGLES       //
        //#########################

        //dictionairy time
        

        //nu kör vi
        int startElement = (subMeshIndices[0] - resolution*2) * 3 * 2; //lowest index, *3 for three element per triangle, *2 for avg. 2 triangles per vertex/index
        int endElement = (subMeshIndices[subMeshIndices.Count - 1] + resolution*2) * 3 * 2;

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
                   // foundTriangle.points.Add(triangleIndex1);
                   // foundTriangle.points.Add(triangleIndex2);
                   // foundTriangle.points.Add(triangleIndex3);

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

    public int[] points;

    public Triangle()
    {
        points = new int[3];
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
