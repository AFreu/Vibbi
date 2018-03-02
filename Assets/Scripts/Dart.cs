using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart {
	public Vector2 p1{ get; }
	public Vector2 p2{ get; }
	public Vector2 p3{ get; }
	public Vector2 p4{ get; }

	public Dart(Vector2 start, Vector2 end){
		p1 = start;
		p2 = end;

		var v = p2 - p1;
		var dv = v.normalized;
		var m = v.magnitude;

		var p = p1 + v / 2; 

		var n = new Vector2 (v.y, -v.x);

		p3 = p + n/3;
		p4 = p - n/3;


	}

	public Dart(Vector2 start, Vector2 hit, BoundaryLineBehaviour bl){
		p1 = start;
		p2 = hit;

		var p = hit;

		var first = bl.first.position;
		var second = bl.second.position;

		Vector2 n = second - first;

		var v = hit - start;

		p3 = p + n.normalized * v.magnitude / 2;
		p4 = p - n.normalized * v.magnitude / 2;

	}

	public List<Vector2> getPoints(){
		List<Vector2> temp = new List<Vector2> ();
		temp.Add (p1);
		temp.Add (p3);
		temp.Add (p2);
		temp.Add (p4);
		return temp;
	}

}
