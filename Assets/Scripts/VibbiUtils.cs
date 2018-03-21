using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibbiUtils : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.1f)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.01f;
		lr.endWidth = 0.01f;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}

	public static GameObject CreateLine(Vector3 start, Vector3 end, Color color, Transform parent){
		GameObject myLine = new GameObject("Connection");
		myLine.transform.position = start;
		myLine.transform.parent = parent;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.01f;
		lr.endWidth = 0.01f;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		return myLine;
	}

    public static Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1);
    }

}
