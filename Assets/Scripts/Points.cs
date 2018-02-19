using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Points : MonoBehaviour {

	public Triangulator triangulator;
	public ClothModelHandler clothModelHandler;

	public float scaleX = 1.0f;
	public float scaleY = 1.0f;

	//private List<Pattern> patterns;

	public List<Vector2> points;
	public List<List<Vector2>> holes;

	// Use this for initialization
	void Start () {
		//patterns = new List<Pattern> ();
		points = new List<Vector2> ();
		holes = new List<List<Vector2>> ();

		InitShirt (scaleX, scaleY);
		//patterns.Add (InitShirt ());
		//patterns.Add (InitSleeve (scaleX,scaleY));

	}

	// Update is called once per frame
	void Update () {
		
	}

	public void ModelSleeve(){
		InitSleeve (scaleX, scaleY);
		clothModelHandler.AddClothModel (Vector3.zero, this);
	}

	public void ModelShirt(){
		InitShirt (scaleX, scaleY);
		clothModelHandler.AddClothModel (Vector3.zero, this);
	}

	public void TriangulateSleeve(){
		InitSleeve (scaleX, scaleY);
		triangulator.Triangulate (this);
	}

	/*private Pattern InitShirt(){
		points.Clear ();
		holes.Clear ();

		points.Add (new Vector2 (0, 0));
		points.Add (new Vector2 (0, 10));
		points.Add (new Vector2 (-3, 8));
		points.Add (new Vector2 (-5, 12));
		points.Add (new Vector2 (0, 15));
		points.Add (new Vector2 (1, 15));
		points.Add (new Vector2 (4, 12));
		points.Add (new Vector2 (7, 15));
		points.Add (new Vector2 (8, 15));
		points.Add (new Vector2 (13, 12));
		points.Add (new Vector2 (11, 8));
		points.Add (new Vector2 (8, 10));
		points.Add (new Vector2 (8, 0));

		return new Pattern (points);

	}*/

	private Pattern InitSleeve(float scaleX,float scaleY){
		points.Clear ();
		holes.Clear ();

		points.Add (new Vector2 (0*scaleX, 0*scaleY));
		points.Add (new Vector2 (18*scaleX, 0*scaleY));
		points.Add (new Vector2 (17.5f*scaleX, 1*scaleY));
		points.Add (new Vector2 (16.5f*scaleX, 2*scaleY));
		points.Add (new Vector2 (15*scaleX, 3*scaleY));
		points.Add (new Vector2 (13.5f*scaleX, 4*scaleY));
		points.Add (new Vector2 (12.5f*scaleX, 5*scaleY));
		points.Add (new Vector2 (12*scaleX, 6*scaleY));
		points.Add (new Vector2 (0*scaleX, 5*scaleY));

		return new Pattern (points);
	}

	private Pattern InitShirt(float scaleX,float scaleY){
		points.Clear ();
		holes.Clear ();

		points.Add (new Vector2 (0*scaleX, 0*scaleY));
		points.Add (new Vector2 (8*scaleX, 0*scaleY));
		points.Add (new Vector2 (10*scaleX, 10*scaleY));
		points.Add (new Vector2 (8.5f*scaleX, 11*scaleY));
		points.Add (new Vector2 (8*scaleX, 12*scaleY));
		points.Add (new Vector2 (8.5f*scaleX, 13*scaleY));
		points.Add (new Vector2 (9*scaleX, 14*scaleY));
		points.Add (new Vector2 (9.5f*scaleX, 15*scaleY));
		points.Add (new Vector2 (10*scaleX, 16*scaleY));
		points.Add (new Vector2 (4*scaleX, 18*scaleY));
		points.Add (new Vector2 (0*scaleX, 14*scaleY));

		return new Pattern (points);
	}

	private enum Patterns {
		SHIRT,
		SKIRT,
		SLEEVE,
		LEG
	}

	private class Pattern {
		public List<Vector2> points;
		public List<List<Vector2>> holes;

		public Pattern(List<Vector2> points, List<List<Vector2>> holes){
			this.points = points;
			this.holes = holes;
		}

		public Pattern(List<Vector2> points) : this(points, new List<List<Vector2>>())
		{
		}

	}
}


