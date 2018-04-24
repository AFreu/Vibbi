﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentPointsHandler : MonoBehaviour {

	public GameObject attachmentPointPrefab;
	public Camera cam;
	public float distance = 0.1f;

	public List<GameObject> attachmentPoints = new List<GameObject>();
    //private Transform selectedAttachMentPoint;
    private GameObject selectedAttachmentPoint;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		HandleInput ();

        selectedAttachmentPoint = null;
        foreach(GameObject a in attachmentPoints)
        {
            if (a.GetComponent<Selectable>().isSelected())
            {
                selectedAttachmentPoint = a;
            }
        }
	}

	void HandleInput(){
		if (Input.GetKeyUp (KeyCode.A)) {
			int layerMask = LayerMask.GetMask("Body");

			RaycastHit hit;
			if (Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit, 30f, layerMask)) {
				AddAttachMentPoint (hit.collider.gameObject, hit.point + hit.normal * distance);
			}
		}
	}

	void AddAttachMentPoint(GameObject body, Vector3 position){

		GameObject o = Instantiate (attachmentPointPrefab, position, Quaternion.identity, body.transform) as GameObject;
		o.GetComponent<AttachmentPoint> ().body = body;
		attachmentPoints.Add (o);
	}

    public GameObject GetSelectedAttachmentPoint()
    {
        return selectedAttachmentPoint;
    }

	public void ShowAttachmentPoints(bool enabled){
		foreach (GameObject o in attachmentPoints) {
			o.GetComponent<Renderer> ().enabled = enabled;
		}
	}
}
