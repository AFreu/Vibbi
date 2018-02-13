using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_DeformManager : MonoBehaviour
{

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

        DeformCloth dc = go.AddComponent<DeformCloth>();
        dc.SetSize(2, 3);
        dc.SetMaterial(garmentMaterial);

        deformManager.Reset();


    }
}
