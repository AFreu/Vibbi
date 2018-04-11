using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibbiUtils {

	/*public static int pooledLines = 40;
	static List<GameObject> lines;

	public static void InitLinePool(){
		lines = new List<GameObject> ();
		for (int i = 0; i < pooledLines; i++) {

			GameObject line = new GameObject ("Line");
			line.AddComponent<LineRenderer> ().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
			line.AddComponent<Deactivate> ();
			line.SetActive (false);
			lines.Add (line);

		}
	}*/
		

	/*public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.1f){

		if (lines == null) {
			InitLinePool ();
		}

		for (int i = 0; i < lines.Count; i++) {
			
			if (!lines[i].activeInHierarchy) {
				var line = lines [i];
				line.transform.position = start;

				var renderer = line.GetComponent<LineRenderer> ();
				renderer.startColor = color;
				renderer.endColor = color;
				renderer.startWidth = 0.01f;
				renderer.endWidth = 0.01f;
				renderer.SetPosition(0, start);
				renderer.SetPosition(1, end);

				line.GetComponent<Deactivate> ().timeActive = duration;
				line.SetActive (true);
				break;


			}
		}
	}*/

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
