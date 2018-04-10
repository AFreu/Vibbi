using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#pragma warning disable 0219
public class Test_DeformManager : MonoBehaviour
{

    public Material garmentMaterial;
    public DeformManager deformManager;

    public GameObject pieceOfCloth;

    public Mesh meshForDeformObject;
    public GameObject cubeCollider;

    private List<GameObject> clothPieces = new List<GameObject>();

    private DeformCloth dc;

    public void MakeAPieceOfCloth()
    {
        //GameObject go = new GameObject("A piece of cloth");
        
        //dc = go.AddComponent<DeformCloth>();
        //dc.SetSize(5, 5);
        //dc.SetMaterial(garmentMaterial);
        //go.GetComponent<MeshRenderer>().material = garmentMaterial;



        StartSimulation();
    }

    private int clicks = 0;
    public void MakeAPieceOfClothFromMesh()
    {
        clicks += 2;
        GameObject clothPiece = Instantiate(pieceOfCloth, deformManager.transform.parent);


        //Place cloth piece above box
        clothPiece.transform.localPosition = new Vector3(0, clicks, 0);
        clothPiece.transform.localRotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));

        //DeformObject deformObject = clothPiece.AddComponent<DeformObject>();

        //Init cloth piece mesh according to the given cloth model mesh
        clothPiece.GetComponent<MeshFilter>().sharedMesh = meshForDeformObject;
        clothPiece.GetComponent<MeshCollider>().sharedMesh = meshForDeformObject;
        if (clicks > 2)
        {
            clothPiece.GetComponent<MeshRenderer>().material = garmentMaterial;
            
        }


        clothPieces.Add(clothPiece);

     
    }

    public void StartSimulation()
    {
        foreach (GameObject o in clothPieces)
        {
            Mesh mesh = o.GetComponent<MeshFilter>().sharedMesh;
            DeformObject deformObject = o.AddComponent<DeformObject>();

            deformObject.originalMesh = mesh;
            deformObject.material = o.GetComponent<MeshRenderer>().material;
            deformObject.AddToSimulation();
        }

        deformManager.Reset();

    }


    //for now, changing the size
    public void UpdateClothMesh()
    {
        //get id 
        int id = clothPieces[0].GetComponent<DeformObject>().GetId();

        int count = meshForDeformObject.vertices.Length / 2;
        //new vertices
        for (int i = 0; i < count; i++)
        {
            meshForDeformObject.vertices[i].x += 20;
        }


        deformManager.MoveTheParticle(id, 0, new Vector3(meshForDeformObject.vertices[0].x + 2, meshForDeformObject.vertices[0].y, meshForDeformObject.vertices[0].z));
    }
}
