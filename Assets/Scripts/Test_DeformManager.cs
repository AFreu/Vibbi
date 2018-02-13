﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_DeformManager : MonoBehaviour
{

    public Material garmentMaterial;
    public DeformManager deformManager;

    public GameObject pieceOfCloth;

    public Mesh meshForDeformObject;

    public void MakePrefabCloth()
    {
        Instantiate(pieceOfCloth, new Vector3(0, 2, 0), Quaternion.identity);
    }

    public void MakeAPieceOfCloth()
    {
        GameObject go = new GameObject("A piece of cloth");

        DeformCloth dc = go.AddComponent<DeformCloth>();
        dc.SetSize(2, 3);
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
}