using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent (typeof (PolygonCollider2D))]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshFilter))]
/// <summary>
/// Builds a Mesh for a gameObject using the PolygonCollider2D's path</summary>
public class PolygonEditor : MonoBehaviour {
	protected PolygonCollider2D polygon;
	protected MeshFilter meshFilter;

	protected Collider collider;
	protected BoxCollider boxCollider;

	///<summary>The index of the polygon path to use for the mesh</summary>
	int pathIndex = 0;
	///<summary>The Z position for the generated mesh</summary>
	public float zPosition = 0f;

	void Start() {
		polygon = gameObject.GetComponent<PolygonCollider2D>();
		meshFilter = gameObject.GetComponent<MeshFilter>();

		collider = gameObject.GetComponent<Collider> ();
		boxCollider = gameObject.GetComponent<BoxCollider> ();
	} 

	#if UNITY_EDITOR
	///<summary>
	///(Re)builds the Mesh using the path of the PolygonCollider2D</summary>
	public void OnColliderUpdate() {
		Vector2[] path = polygon.GetPath(pathIndex);

		Mesh msh = new Mesh();

		msh.vertices = path.Select(v => new Vector3(v.x, v.y, zPosition)).ToArray();
		msh.triangles = new NaiveTriangulator(path).Triangulate();

		msh.RecalculateNormals();
		msh.RecalculateBounds();
		meshFilter.mesh = msh;
	}

	void Update() {
		if (!Application.isPlaying) OnColliderUpdate();
	}
	#endif
}
	
