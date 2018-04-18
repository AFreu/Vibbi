using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentPoint : MonoBehaviour {

	public GameObject body;
    public bool bendPoint { set; get; }

    private Vector3 closestPointOnBody;
    
	// Use this for initialization
	void Start () {
		if (body == null) {
			body = transform.parent.gameObject;
		}

		closestPointOnBody = body.GetComponent<Collider> ().ClosestPoint (transform.position);
        

		Debug.DrawLine (closestPointOnBody, transform.position, Color.blue, 100);

		Vector3 v = transform.position - closestPointOnBody;
		Vector3 normal = v.normalized;

		transform.up = normal;

        if (name.Equals("RAO") || name.Equals("LAO"))
        {
            bendPoint = true;
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
