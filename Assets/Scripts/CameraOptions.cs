using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOptions : MonoBehaviour {

    public bool useEditorValues;
    public Vector3 position;
    public Vector3 rotation;

    private bool cameraSet;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (cameraSet)
        {
            gameObject.transform.position = new Vector3(2.227537f, 7.813982f, -5.627961f);
            gameObject.transform.eulerAngles = new Vector3(19.68f, -16.56f, 0.0f);
        }
	}

    public void SetCameraTransform()
    {
        if (useEditorValues)
        {
            gameObject.transform.position = position;
            gameObject.transform.eulerAngles = rotation;

        }
        else
        {
            gameObject.transform.position = new Vector3(2.227537f, 7.813982f, -5.627961f);
            gameObject.transform.eulerAngles = new Vector3(19.68f, -16.56f, 0.0f);

            cameraSet = true;
        }

     
    }
}
