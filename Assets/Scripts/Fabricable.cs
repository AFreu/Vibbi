using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Fabricable : Behaviour {

	public Material defaultMaterial;

	private List<Material> materials;
	private List<Material> simulationMaterials;

	public int materialIndex = 0;

	//Clone used for bidirectionality
	public Fabricable clone;

	void Start(){
		materials = new List<Material> ();
		materials.Add(defaultMaterial);

		materials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Chevron.mat", typeof(Material)));
		materials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Leather.mat", typeof(Material)));
		materials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Purple_Flannel.mat", typeof(Material)));
		materials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Brown_Flannel.mat", typeof(Material)));
		materials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Green_Flannel.mat", typeof(Material)));

		simulationMaterials = new List<Material> ();
		simulationMaterials.Add(defaultMaterial);

		simulationMaterials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Chevron_Two_Sided.mat", typeof(Material)));
		simulationMaterials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Leather_Two_Sided.mat", typeof(Material)));
		simulationMaterials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Purple_Flannel_Two_Sided.mat", typeof(Material)));
		simulationMaterials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Brown_Flannel_Two_Sided.mat", typeof(Material)));
		simulationMaterials.Add((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Fabrics/Green_Flannel_Two_Sided.mat", typeof(Material)));

		UpdateMaterial (materialIndex);
	}

	void OnMouseUp(){
		

		if (Input.GetKey (KeyCode.Alpha1)) {

			UpdateState (1);

		}

		if (Input.GetKey (KeyCode.Alpha2)) {

			UpdateState (2);

		}

		if (Input.GetKey (KeyCode.Alpha3)) {

			UpdateState (3);

		}

		if (Input.GetKey (KeyCode.Alpha4)) {

			UpdateState (4);

		}

		if (Input.GetKey (KeyCode.Alpha5)) {

			UpdateState (5);

		}

		if (Input.GetKey (KeyCode.Alpha0)) {

			UpdateState (0);
		}
	}

	public int GetSimulationMaterialIndex(){
		return materialIndex;
	}

	public Material GetMaterial(){
		return materials [materialIndex];
	}

	public Material GetSimulationMaterial(){
		return simulationMaterials[materialIndex];
	}

	public void SetSimulationMaterial(int index){
		UpdateState (index);
	}

	void UpdateState(int index){
		var newIndex = index != this.materialIndex;
		if (newIndex) {

			UpdateMaterial (index);

			//Update clone if it exist
			if (clone) {
				clone.UpdateMaterial (index);
			}

		}
	}

	void UpdateMaterial(int index){
		//Use the index
		GetComponent<Renderer> ().material = materials[index];

		//Store the index
		materialIndex = index;
	}
}
