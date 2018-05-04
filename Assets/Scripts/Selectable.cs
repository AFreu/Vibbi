using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

    public Color selectedColor = Color.red;
    public Color highlightColor = Color.white;
    public Color normalColor = Color.black;

	private bool highlighted = false;
	private bool selected = false;
    
    //private Color previousColor;
    private bool switchedColor;
    
	private SelectableManager manager;

	void Awake(){
		manager = Component.FindObjectOfType<SelectableManager> ();
	}

	
	// Update is called once per frame
	void Update () {

		bool leftMouseButtonUp = Input.GetMouseButtonUp (0);
		bool leftShift = Input.GetKey (KeyCode.LeftShift);
		bool mouseOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();


        if (highlighted && leftMouseButtonUp)
        {
            Select(true);
        }

        if (!highlighted && leftMouseButtonUp && !leftShift && !mouseOverUI)
        {
            Select(false);
        }



        if (!selected && switchedColor)
        {
            switchedColor = false;
            ResetSelectedColor();
        }

		if (selected) {
			GetComponent<Renderer> ().material.color = selectedColor;
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

	void OnMouseUp(){
		
	}
		

	void Highlight(bool highlight){
		if (highlight) {
			manager.AddHighligted (this);
		} else {
			manager.RemoveHighligted (this);
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

	public void Select(bool on){
		if (on) {
			GetComponent<Renderer> ().material.color = selectedColor;
			manager.AddSelected (this);

		} else {
			GetComponent<Renderer> ().material.color = normalColor;
			manager.RemoveSelected (this);

		}
		selected = on;
	}

	public bool isHighlighted(){
		return highlighted;
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

    private void ResetNormalColor()
    {
        normalColor = Color.green;
    }

    public void ResetSewingColor(Color color)
    {
        if (color.Equals(normalColor))
        {
            ResetNormalColor();
        }
    }
		
}
