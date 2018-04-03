using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

	public Material selectedMaterial;
	public Material highlightMaterial;
	public Material normalMaterial;

    public Color selectedColor = Color.red;
    public Color highlightColor = Color.white;
    public Color normalColor = Color.green;

	private bool highlighted = false;
	private bool selected = false;
    
    private Color previousColor;
    private bool switchedColor;
    
	private static List<Selectable> currentlyHighlighted;

	// Use this for initialization
	void Start () {
        //previousColor = selectedMaterial.color;
        previousColor = Color.red;
		currentlyHighlighted = new List<Selectable>();

	}
	
	// Update is called once per frame
	void Update () {

		bool leftMouseButtonUp = Input.GetMouseButtonUp (0);
		bool leftShift = Input.GetKey (KeyCode.LeftShift);

        if (highlighted && leftMouseButtonUp)
        {
            Select(true);
        }

        if (!highlighted && leftMouseButtonUp && !leftShift)
        {
            Select(false);
        }

        if (!selected && switchedColor)
        {
            switchedColor = false;
            ResetSelectedColor();
        }

	}
		

	void OnMouseEnter(){
		Highlight (true);
	}

	void OnMouseExit(){
		Highlight (false);
	}

	void OnDisable(){
		Highlight (false);
	}
		

	void Highlight(bool highlight){
		if (highlight) {
			currentlyHighlighted.Add (this);
			currentlyHighlighted [0].SetHighlighted (true);
		} else {
			currentlyHighlighted.Remove (this);
			SetHighlighted (false);

			if (currentlyHighlighted.Count > 0) {
				currentlyHighlighted [0].SetHighlighted (true);
			}
		}

	}

	public void SetHighlighted(bool on){
		highlighted = on;
		if (selected)
			return;
		if (on) {
			GetComponent<Renderer> ().material.color = highlightColor;
		} else {
			GetComponent<Renderer> ().material.color = normalColor;
		}
	}

	void Select(bool on){
		selected = on;
		if (on) {
			GetComponent<Renderer> ().material.color = selectedColor;
		} else {
			GetComponent<Renderer> ().material.color = normalColor;
		}
	}

	public bool isSelected(){
		return selected;
	}

    public void SetSewingColor(Color color)
    {
        switchedColor = true;

        selectedColor = color;
        normalColor = color;
    }

    public void ResetSelectedColor()
    {
        selectedColor = Color.red;
    }

    public void ResetNormalColor()
    {
        normalColor = Color.green;
    }
		
}
