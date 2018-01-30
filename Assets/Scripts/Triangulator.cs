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

	public void Triangulate(Points ps)
	{
		
		List<Vector2> points = ps.points;
		List<List<Vector2>> holes = new List<List<Vector2>>();
		List<int> indices = null;
		List<Vector3> vertices = null;

		foreach (List<Vector2> hole in ps.holes) {
			holes.Add (hole);
		}
		var options = new ConstraintOptions() { ConformingDelaunay = conformingDelaunay };
		var quality = new QualityOptions() { MinimumAngle = minimumAngle, MaximumArea = maximumArea};

		ConditionedTriangulator.triangulate(points, holes, out indices, out vertices, options, quality);


		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		//MeshUtility.Optimize(mesh);

		//mesh.Optimize ();
		mesh.RecalculateNormals ();

		go.GetComponent<MeshCollider> ().sharedMesh = mesh; 

		Vector2[] uvs = new Vector2[mesh.vertices.Length];
		for (int i = 0; i < uvs.Length; i++) {
			uvs [i] = new Vector2 (mesh.vertices[i].x, mesh.vertices[i].y);
		}
		mesh.uv = uvs;


		//mr.enabled = false;
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

		List<Vector2> points = ps;
		List<List<Vector2>> holes = hs;

		List<int> indices = null;
		List<Vector3> vertices = null;

		var options = new ConstraintOptions() { ConformingDelaunay = conformingDelaunay };
		var quality = new QualityOptions() { MinimumAngle = minimumAngle, MaximumArea = maximumArea};

		ConditionedTriangulator.triangulate(points, holes, out indices, out vertices, options, quality);

		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		//MeshUtility.Optimize(mesh);

		//mesh.Optimize ();
		mesh.RecalculateNormals ();

		go.GetComponent<MeshCollider> ().sharedMesh = mesh; 

		Vector2[] uvs = new Vector2[mesh.vertices.Length];
		for (int i = 0; i < uvs.Length; i++) {
			uvs [i] = new Vector2 (mesh.vertices[i].x, mesh.vertices[i].y);
		}
		mesh.uv = uvs;
	}

	public void Triangulate(Mesh m, List<Vector2> ps, List<List<Vector2>> hs)
	{

		List<Vector2> points = ps;
		List<List<Vector2>> holes = hs;

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
