using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_DeformManager : MonoBehaviour
{

    public Material garmentMaterial;
    public DeformManager deformManager;

    public GameObject pieceOfCloth;

    public Mesh meshForDeformObject;
    public GameObject cubeCollider;


    private DeformCloth dc;


    public void MakePrefabCloth()
    {
        Instantiate(pieceOfCloth, new Vector3(0, 2, 0), Quaternion.identity);
    }

    public void MakeAPieceOfCloth()
    {
        GameObject go = new GameObject("A piece of cloth");

        dc = go.AddComponent<DeformCloth>();
        dc.SetSize(2, 2);
        dc.SetMaterial(garmentMaterial);

        deformManager.Reset();
    }

    public void MakeAPieceOfClothFromMesh()
    {
        GameObject go2 = new GameObject("A weird piece of cloth");

        DeformObject deformObject = go2.AddComponent<DeformObject>();

        deformObject.SetMesh(meshForDeformObject);
        deformObject.SetMaterial(garmentMaterial);


        deformManager.Reset();
    }


    //for now, changing the size
    public void UpdateClothMesh()
    {
        dc.SetSize(2, 3);
        //dc.UseReset();

        //figure out location:: location of cube?
        //dc.transform
        Vector3 location = cubeCollider.transform.position;
        location.y = location.y + 0.5f;

        Vector3 locationDC = dc.transform.position;
        locationDC.y = locationDC.y + 0.5f;

        deformManager.CreateNewDeformableObject(dc, locationDC); //doesnt work very well
    }
}
