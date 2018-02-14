using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Points : MonoBehaviour {

	//public Text outPut;

	public List<Vector2> points;
	public List<List<Vector2>> holes;

	// Use this for initialization
	void Start () {
		points = new List<Vector2> ();
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

		holes = new List<List<Vector2>> ();


		//outPut.text = "Hej hallå!!!! ";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
