using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using System;

public class ConditionedTriangulator {

	public static bool triangulate(List<Vector2> points, List<List<Vector2>> holes, out List<int> outIndices, out List<Vector3> outVertices, ConstraintOptions options, QualityOptions quality)
	{
		outVertices = new List<Vector3> ();
		outIndices= new List<int> ();
		Polygon poly = new Polygon ();

		// Points and Segments
		for (int i = 0; i < points.Count; i++) {
			poly.Add (new Vertex (points [i].x, points [i].y));

			if (i == points.Count - 1) {
				poly.Add (new Segment (new Vertex (points [i].x, points [i].y), new Vertex (points [0].x, points [0].y)));

			} else {
				poly.Add (new Segment (new Vertex (points [i].x, points [i].y), new Vertex (points [i+1].x, points [i+1].y)));
			}
		}

		// Holes
		for (int i = 0; i < holes.Count; i++) {
			List<Vertex> vertices = new List<Vertex> ();
			for (int j = 0; j < holes [i].Count; j++) {
				vertices.Add (new Vertex(holes[i][j].x, holes[i][j].y));
			}
			poly.Add (new Contour (vertices), true);
		}





		var mesh = poly.Triangulate (options, quality);

		foreach (ITriangle t in mesh.Triangles) 
		{
			for (int j = 2; j >= 0; j--) 
			{
				bool found = false;
				for (int k = 0; k < outVertices.Count; k++) 
				{

                    //double absA = Math.Abs(outVertices[k].x);
                    //double absB = Math.Abs((float)t.GetVertex(j).X);
                    float diffX = Math.Abs(outVertices[k].x - (float)t.GetVertex(j).X);
                    float diffY = Math.Abs(outVertices[k].y - (float)t.GetVertex(j).Y);

                    float epsilon = 0.00001f;


                    if ((diffX <= epsilon) && (diffY <= epsilon))
					{
                        outIndices.Add(k);
						found = true;
						break;
					}
				}

				if(!found){
                    //Debug.Log("Not found:");
                    //Debug.Log("Triangle vertex " + t.GetVertex(j).X);
                    //Debug.Log("Triangle FLOAT vertex " + (float)t.GetVertex(j).X);
                    
                    outVertices.Add (new Vector3 ((float)t.GetVertex (j).X, (float)t.GetVertex (j).Y, 0));
					outIndices.Add (outVertices.Count - 1);
				}
			}
		} 
		return true;
	}
}
