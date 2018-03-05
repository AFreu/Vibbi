using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable1D : Movable {

	public Vector3 origin;

	public override void Move(Vector3 position){

		var start = transform.parent.position;
		var end = transform.position;

		var a = position - end;

		var unitVector = (end - start).normalized;

		Vector3 a2 = Vector3.Dot(a , unitVector) * unitVector;

		transform.position += a2;

		//Love linear algebra! 
	}

}
