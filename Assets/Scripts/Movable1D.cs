using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable1D : Movable {

	public Transform start;
	public Transform end;

	public override void Move(Vector3 position){

		var input = position - transform.position;

		var direction = (end.position - start.position).normalized;

		Vector3 translation = Vector3.Dot(input , direction) * direction;

		transform.position += translation;

		//Love linear algebra! 
	}

	public void Init(Transform start, Transform end){
		this.start = start;
		this.end = end;
	}

}
