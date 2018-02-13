using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_DeformManager : MonoBehaviour {

    public Material garmentMaterial;
    public DeformManager deformManager;

    public GameObject pieceOfCloth;

    public void MakePrefabCloth()
    {
        Instantiate(pieceOfCloth, new Vector3(0, 2, 0), Quaternion.identity);
    }

    public void MakeAPieceOfCloth()
    {
        GameObject go = new GameObject("A piece of cloth");

        DeformCloth testDeformCloth = new DeformCloth();
        testDeformCloth.size = new Vector2(2, 3);
        testDeformCloth.material = garmentMaterial;



        //go.AddComponent<DeformCloth>().testDeformCloth as DeformCloth;
        

        DeformCloth dc = go.AddComponent<DeformCloth>();
        dc.SetSize(2, 3);
        dc.SetMaterial(garmentMaterial);
//        go.GetComponent<DeformCloth>().SetSize(2,3);



        //dc.size = new Vector2(3,3);
        //dc.material = garmentMaterial;
        
    
        //go.transform.parent = transform;
        //go.transform.position = transform.position;

        //Instantiate(go, new Vector3(0, 2, 0), Quaternion.identity);

//        GameObject.Destroy(go);
  //      go = null;

        deformManager.Reset();


    }
}
