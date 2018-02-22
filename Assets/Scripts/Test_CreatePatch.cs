using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CreatePatch : MonoBehaviour {

    public Vector2 size = new Vector2(2, 2);
    public uint resolution = 64;
    public Mesh mesh;
    public Material material;

    private const uint MIN_RESOLUTION = 10;
    private const uint MAX_RESOLUTION = 120;

    [HideInInspector] [SerializeField] Vector3 originalPosition;
    [HideInInspector] [SerializeField] Quaternion originalRotation;
    [HideInInspector] [SerializeField] Vector3 originalScale;

    [SerializeField] private Vector2 oldSize;
    [SerializeField] private uint oldResolution;

    bool transformReset;
    // Use this for initialization
    void Start () {
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        
    }
    void OnValidate()
    {
        size.x = Mathf.Max(0.1f, size.x);
        size.y = Mathf.Max(0.1f, size.y);

        resolution = Math.Max(Math.Min(resolution, MAX_RESOLUTION), MIN_RESOLUTION);

        //#Malin: for simulating while modeling
        //InitMeshComponents();


        if (!size.Equals(oldSize) || resolution != oldResolution) //if we changed the size or resolution
        {
            RebuildMesh();

            //#if UNITY_EDITOR
            //SendClothToVivace();
            //#endif

            //shouldRegenerateParticles = true;
        }

        GetComponent<MeshRenderer>().material = material;
    }

    void RebuildMesh()
    {
        if (!mesh)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
        }

        MeshUtils.CreateClothMesh(size, resolution, mesh);


        //ResetColorBuffers();

        // meshVertices = mesh.vertices;
        //meshNormals = mesh.normals;

        //oldSize = size;
        //oldResolution = resolution;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

        // UpdateMesh(mesh.vertices, mesh.normals);
    }


 
    // Update is called once per frame
    void Update () {
		
	}
    
}
