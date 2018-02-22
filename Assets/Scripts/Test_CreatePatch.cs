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

    [SerializeField] private Vector2 oldSize;
    [SerializeField] private uint oldResolution;

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
        
        if (!size.Equals(oldSize) || resolution != oldResolution) //if we changed the size or resolution
        {
            RebuildMesh();
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
        
        oldSize = size;
        oldResolution = resolution;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

    }


 
    // Update is called once per frame
    void Update () {
		
	}
    
}
