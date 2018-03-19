using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VibbiMeshUtils : MonoBehaviour
{
    //submesh
    private IDictionary<int, List<int>> indexToTrianglesDictionary = new Dictionary<int, List<int>>();
    public Mesh backgroundPatchMesh;

    //function variables
    private static float k;
    private static float m;
    private static bool straightUpLine = false;
    private static float straightUpX;

    //take two lines and find their vertices and make an array with all the pairs
    public static List<int> DefineSeamFromLines(GameObject line1, GameObject line2)
    {
        //find vertex indices on the lines
        List<int> indicesLine1 = indicesFromLine(line1);
        List<int> indicesLine2 = indicesFromLine(line2);
        
        //pair them up
        //distribution : how many jumps to make from the last index
        List<int> smallestList = indicesLine1.Count <= indicesLine2.Count ? indicesLine1 : indicesLine2;
        List<int> largestList = indicesLine1.Count >= indicesLine2.Count ? indicesLine1 : indicesLine2;
        
        int distribution = Mathf.FloorToInt(largestList.Count / smallestList.Count);
       
        int isLine1Smallest = indicesLine1.Count <= indicesLine2.Count ? distribution: 1;
        int isLine2Smallest = indicesLine2.Count < indicesLine1.Count ? distribution : 1;
        
        List<int> pairsOfIndices = new List<int>(); //index from line1 is first then index from line2
        for (int i = 0; i < smallestList.Count; i ++)
        {
            if (i == smallestList.Count-1)
            {
                pairsOfIndices.Add(indicesLine1[indicesLine1.Count-1]);
                pairsOfIndices.Add(indicesLine2[indicesLine2.Count-1]);
            }
            else
            {
                pairsOfIndices.Add(indicesLine1[i * (distribution / isLine1Smallest)]);
                pairsOfIndices.Add(indicesLine2[i * (distribution / isLine2Smallest)]);
            }

        }
        
        return pairsOfIndices;
    }

    private static List<int> indicesFromLine(GameObject line)
    {
        straightUpLine = false;
        float epsilon = 0.01f;

        /*get the two points of the line and
        find the function of these two points*/
        DefineFunction( line.GetComponent<BoundaryLineBehaviour>().first.localPosition, 
                        line.GetComponent<BoundaryLineBehaviour>().second.localPosition);

        

        //get all the vertices of the mesh that the line belongs to
        Vector3[] meshVertices = line.GetComponentInParent<BoundaryPointsHandler>().GetComponent<MeshFilter>().mesh.vertices;
        List<int> lineIndices = new List<int>(); //fill this list with the vertices on the line

        //check if vertices are on the line
        for (int i = 0; i < meshVertices.Length; i++)
        {
            if (straightUpLine)
            {
                if (meshVertices[i].x > (straightUpX - epsilon) &&
                    meshVertices[i].x < (straightUpX + epsilon))
                {
                    lineIndices.Add(i); //add the index of the vertex
                }
            }
            else
            {
                if (LineFunction(meshVertices[i].x) > (meshVertices[i].y - epsilon)
                    && LineFunction(meshVertices[i].x) < (meshVertices[i].y + epsilon))
                {
                    lineIndices.Add(i); //add the index of the vertex
                }
            }
        }

        //decide starting point : either first or second, why second????
        Vector3 startPoint = line.GetComponent<BoundaryLineBehaviour>().first.localPosition;


        lineIndices = SortIndexList(lineIndices, startPoint, meshVertices);
        
        return lineIndices;
    }
		

    private static List<int> SortIndexList(List<int> lineIndices, Vector3 startPoint, Vector3[] meshVertices)
    {
        //bubble sort
        bool swapped = true;
        while (swapped)
        {
            swapped = false;
            for (int i = 1; i < lineIndices.Count; i++)
            {
                if ((meshVertices[lineIndices[i - 1]] - startPoint).magnitude > (meshVertices[lineIndices[i]] - startPoint).magnitude)
                {
                    //swap lineInd[i-1] with lineInd[i];
                    int tmp = lineIndices[i - 1];
                    lineIndices[i - 1] = lineIndices[i];
                    lineIndices[i] = tmp;

                    swapped = true;
                }
            }
        }
        return lineIndices;
    }

    //param x => f(x) = kx + m 
    //with k & m defined by DefineFunction(Vector3, Vector3)
    private static float LineFunction(float x)
    {
        return k*x+m;
    } 

    //defines k & m in linear function f(x) = kx + m that goes from firstPointPos to secondPointPos
    private static void DefineFunction(Vector3 firstPointPos, Vector3 secondPointPos)
    {

        float deltaX = (secondPointPos.x - firstPointPos.x); //if = 0 line goes straight up
        float deltaY = (secondPointPos.y - firstPointPos.y);

        if (deltaX == 0)
        {
            straightUpLine = true;
            straightUpX = firstPointPos.x;
        }
        else
        {
            k = deltaY / deltaX;
            m = firstPointPos.y - (k * firstPointPos.x);
        }
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



    public Mesh CreateSubMeshFromSmallPatch(GameObject clothModel)
    {


        List<GameObject> boundaryPoints = clothModel.GetComponent<BoundaryPointsHandler>().GetBoundaryPoints(); //get all points
        List<Vector3> positions = new List<Vector3>(); //get a list with all of the positions
        int count = boundaryPoints.Count;
        for (int i = 0; i < count; i++)
        {
            positions.Add(boundaryPoints[i].transform.position);
        }

        //background patch mesh
        List<int> subMeshIndices = new List<int>();
        List<Vector3> boundingBoxVertices = new List<Vector3>();
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

        Debug.Log("XMAX: " + xmax);
        Debug.Log("XMIN: " + xmin);
        Debug.Log("YMAX: " + ymax);
        Debug.Log("YMIN: " + ymin);
        #endregion
        Debug.Log("BB TIME: " + (Time.realtimeSinceStartup - bbTime));

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

        List<Vector3> leftEdgeVertices = new List<Vector3>();
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
                //find all vertices on the left edge of the bounding box
                if (theVertex.x > (xmin - epsilon) && theVertex.x < (xmin + epsilon))
                {
                    leftEdgeVertices.Add(new Vector3(theVertex.x, theVertex.z, 0.0f));
                }
                boundingBoxVertices.Add(new Vector3(theVertex.x, theVertex.z, 0.0f));
                subMeshIndices.Add(i);
            }

        }
        Debug.Log("LEFT EDGE VERTICES: " + leftEdgeVertices.Count);

        //raycast: is point in polygon?
        int numRows = (int)(erow - srow);
        Debug.Log("Number of rows: " + numRows);
        float widthOfBBplusOffset = (xmax - xmin) + 1.0f;

        Debug.Log("BB WIDTH: " + widthOfBBplusOffset);


        //##############################################
        //           RAYCAST ALGORITHM
        //############################################## 
        RaycastHit[] collisions = new RaycastHit[10];
        for (int i = 0; i < numRows; i++)
        {
            //          Vector3 origin = new Vector3(xmin , ymin + (GRIDWIDTH * i),0);
            Vector3 origin = new Vector3(leftEdgeVertices[i].x - 0.5f, leftEdgeVertices[i].y, 0);
            //find collisions
            collisions = Physics.RaycastAll(origin, Vector3.right, widthOfBBplusOffset, LayerMask.GetMask("BoundaryLine"));

            if (collisions.Length == 2)
            {
                Debug.Log(i);
                float x1 = collisions[0].point.x - epsilon;
                float x2 = collisions[1].point.x + epsilon;

                //add all points inbetween in the list
                for (int j = 0; j < boundingBoxVertices.Count; j++) //TO DO: start & end index; look at first 'row' only, remove that row afterwards
                {
                    if (boundingBoxVertices[j].x > x1 && boundingBoxVertices[j].x < x2
                        && boundingBoxVertices[j].y > collisions[0].point.y - epsilon && boundingBoxVertices[j].y < collisions[0].point.y + epsilon)
                    {
                        subMeshVertices.Add(boundingBoxVertices[j]);
                        Debug.DrawLine(boundingBoxVertices[j], boundingBoxVertices[j] * 0.01f, Color.red, 100.0f);
                    }
                }

            }

            Debug.DrawLine(origin, origin + (Vector3.right * widthOfBBplusOffset), Color.blue, 100.0f);

        }
        Debug.Log(subMeshVertices.Count);


        /* 

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
        /*
        #region Triangles Not Dictionary
        //#########################
        //       TRIANGLES       //
        //#########################
        //nu kör vi
        int startElement = (subMeshIndices[0] - resolution * 2) * 3 * 2; //lowest index, *3 for three element per triangle, *2 for avg. 2 triangles per vertex/index
        int endElement = (subMeshIndices[subMeshIndices.Count - 1] + resolution * 2) * 3 * 2;

        int maxIndex = subMeshIndices[subMeshIndices.Count - 1];
        int minIndex = subMeshIndices[0];


        Debug.Log("Min: " + minIndex);
        Debug.Log("Max: "+maxIndex);

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

        float trimTime = Time.realtimeSinceStartup;
        #region Trim TriangleList


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
        #endregion
        Debug.Log("TRIMTIME: " + (Time.realtimeSinceStartup - trimTime));
        Debug.Log("Triangle trim list: " + triangles.Count);
        

        float transTime = Time.realtimeSinceStartup;
        #region Translate triangles
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
        */
        Mesh mesh = new Mesh();
        return mesh;
    }


    public Mesh CreateSubMesh(GameObject clothModel)
    {
        //fix mesh
        List<GameObject> boundaryPoints = clothModel.GetComponent<BoundaryPointsHandler>().GetBoundaryPoints(); //get all points
		float GRIDWIDTH = boundaryPoints[0].GetComponent<Movable>().gridWidth;
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
            if (pos.y < ymin)
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
        int startRow = (int)((ymin - (height / 2)) / GRIDWIDTH) - 1;
        int endRow = (int)((ymax - (height / 2)) / GRIDWIDTH) + 1;
        int startCol = (int)((xmin - (width / 2)) / GRIDWIDTH) - 1;
        int endCol = (int)((xmax - (width / 2)) / GRIDWIDTH) + 1;


        for (int i = startRow * resolution; i < endRow * resolution; i++)
        {
            Vector3 theVertex = backgroundPatchMesh.vertices[i];
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


        //#########################
        //       TRIANGLES       //
        //#########################

        //dictionairy time


        //nu kör vi
        int startElement = (subMeshIndices[0] - resolution * 2) * 3 * 2; //lowest index, *3 for three element per triangle, *2 for avg. 2 triangles per vertex/index
        int endElement = (subMeshIndices[subMeshIndices.Count - 1] + resolution * 2) * 3 * 2;

        //debug
        Debug.Log("start " + startElement);
        Debug.Log("end " + endElement);
        Debug.Log("yo " + (endElement - startElement));
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
        Debug.Log("Num of triangles in list: " + triangles.Count);


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
        for (int i = 0; i < 3; i++)
        {
            if (!(this.points[i] == otherTriangle.points[i]))
            {
                return false;
            }
        }
        return true;
    }

}
