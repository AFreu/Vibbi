using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothModelBehaviour : MonoBehaviour {


	// Use this for initialization
	void Start () {
        
        
        //garmentHandler = GetComponentInParent<GarmentHandler>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
	}

    void HandleInput()
    {
        if (GetComponent<Selectable>().isSelected())
        {
            if (Input.GetKeyUp(KeyCode.L))
            {
                GetComponentInParent<ClothModelHandler>().LoadCloth(gameObject);
            }
            
        }
    }
}
