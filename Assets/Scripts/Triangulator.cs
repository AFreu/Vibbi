using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TriangleNet.Meshing;

public class Triangulator : MonoBehaviour {

	public float maximumArea;
	public float minimumAngle;
	//public float maximumAngle;
	public bool conformingDelaunay;
	public bool naive;

	public bool autoTriangulate = false;

	public Material material;
	public float y;

	private GameObject go;
	private Mesh mesh;

	// Use this for initialization
	void Start () {

		go = new GameObject ();
		//go.SetActive (true);
		go.name = "Shirt";
		go.transform.position = new Vector3 (0, 1, 0);
		MeshFilter mf = go.AddComponent<MeshFilter> ();
		go.AddComponent<MeshCollider> ();
		MeshRenderer mr = go.AddComponent<MeshRenderer> ();
		mr.material = material;

		mesh = mf.mesh;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool AutoTriangulate(){
		return autoTriangulate;
	}

	public void Triangulate(Points ps)
	{
		
		List<Vector2> points = ps.points;
		List<List<Vector2>> holes = new List<List<Vector2>>();


		foreach (List<Vector2> hole in ps.holes) {
			holes.Add (hole);
		}

		Triangulate (mesh, points, holes);
	}

	public void Init(GameObject o){
		go = o;

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		mesh = mf.mesh;

		go.AddComponent<MeshCollider> ();

		MeshRenderer mr = go.AddComponent<MeshRenderer> ();
		mr.material = material;


	}

	public void Triangulate(List<Vector2> ps, List<List<Vector2>> hs)
	{
		Triangulate (mesh, ps, hs);
	}

	public void Triangulate(Mesh m, List<Vector2> points, List<List<Vector2>> holes)
	{


		List<int> indices = null;
		List<Vector3> vertices = null;

		var options = new ConstraintOptions() { ConformingDelaunay = conformingDelaunay };
		var quality = new QualityOptions() { MinimumAngle = minimumAngle, MaximumArea = maximumArea};

		ConditionedTriangulator.triangulate(points, holes, out indices, out vertices, options, quality);

		m.Clear();
		m.vertices = vertices.ToArray();
        m.triangles = indices.ToArray();
		m.RecalculateNormals ();

		Vector2[] uvs = new Vector2[m.vertices.Length];
		for (int i = 0; i < uvs.Length; i++) {
			uvs [i] = new Vector2 (m.vertices[i].x, m.vertices[i].y);
		}
		m.uv = uvs;

	}
}
