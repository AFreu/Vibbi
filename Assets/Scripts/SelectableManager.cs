using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableManager : Behaviour {

	private static List<Selectable> currentlyHighlighted = new List<Selectable>(); //Can be max one
	private static List<Selectable> currentlySelected = new List<Selectable> (); //Can be plenty

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void AddHighligted(Selectable selectable){
		currentlyHighlighted.Add (selectable);

		//Highlight the first in line
		currentlyHighlighted [0].SetHighlighted (true);
	}

	public void RemoveHighligted(Selectable selectable){
		currentlyHighlighted.Remove (selectable);

		selectable.SetHighlighted (false);
		//Highlight the first in line
		if (currentlyHighlighted.Count > 0) {
			currentlyHighlighted [0].SetHighlighted (true);
		}
	}

	public void AddSelected(Selectable selectable){
		//Add to list
		if (!currentlySelected.Contains (selectable)) {
			currentlySelected.Add (selectable);
		}

		GetComponent<InteractionStateManager> ().Selected (selectable);
	}

	public void RemoveSelected(Selectable selectable){
		//Remove from list
		if (currentlySelected.Contains(selectable)) {
			currentlySelected.Remove (selectable);
		}
	}

	public List<Selectable> GetCurrentlySelected(){
		return currentlySelected;
	}
}
