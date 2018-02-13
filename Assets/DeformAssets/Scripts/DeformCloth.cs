using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class DeformCloth : DeformBody {

    private const uint MIN_RESOLUTION = 10;
    private const uint MAX_RESOLUTION = 120;

    [Header("General")]
    public Vector2 size = new Vector2(2, 2);
    public uint resolution = 64;
    
    [SerializeField] private Vector2 oldSize;
    [SerializeField] private uint oldResolution;

    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);
    [DllImport("deform_plugin")] private static extern void ShutdownDeformPlugin();
    [DllImport("deform_plugin")] private static extern int CreateDeformableObject(Vector3[] vertices, Vector2[] uvs, uint numVertices,
                                                                                  int[] indices, uint numIndices,
                                                                                  Vector3 location, Vector4 rotation,
                                                                                  float distanceStiffness, float bendingStiffness);

    protected override void Start()
    {
        base.Start();
    }
    
    void OnDestroy()
    {
        RebuildMesh();
        //shouldRegenerateParticles = true;
    }

    void OnValidate()
    {

        
        size.x = Mathf.Max(0.1f, size.x);
        size.y = Mathf.Max(0.1f, size.y);
        
        resolution = Math.Max(Math.Min(resolution, MAX_RESOLUTION), MIN_RESOLUTION);

        //#Malin: for simulating while modeling
        InitMeshComponents();


        if (!size.Equals(oldSize) || resolution != oldResolution) //if we changed the size or resolution
        {
            RebuildMesh();

            //#if UNITY_EDITOR
            //SendClothToVivace();
            //#endif

            shouldRegenerateParticles = true;
        }

        GetComponent<MeshRenderer>().material = material;
    }

    protected override void Reset()
    {
        base.Reset();

        mesh = new Mesh();
        mesh.MarkDynamic();

        RebuildMesh();
        shouldRegenerateParticles = true;
    }

    void RebuildMesh()
    {
        if (!mesh)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
        }

        MeshUtils.CreateClothMesh(size, resolution, mesh);

        Debug.Log(mesh.vertices.Length);
        
        ResetColorBuffers();
        
        meshVertices = mesh.vertices;
        meshNormals = mesh.normals;

        oldSize = size;
        oldResolution = resolution;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

       // UpdateMesh(mesh.vertices, mesh.normals);
    }

    void SendClothToVivace()
    {
        ShutdownDeformPlugin();
        InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");
        id = CreateDeformableObject(meshVertices, mesh.uv, (uint) meshVertices.Length,
                                    mesh.triangles, (uint) mesh.triangles.Length / 3,
                                    transform.position, GetRotation(),
                                    distanceStiffness, bendingStiffness);
    }

    public void ResetColorBuffers()
    {
        uint numVertices, numIndices, resX, resY;

        MeshUtils.GetPatchInfo(size, resolution, out numVertices, out numIndices, 
            out resX, out resY);

        fixedVertices = new bool[numVertices];
        attachedVertices = new bool[numVertices];
    }

    public override Vector4 GetRotation()
    {
        return base.GetRotation();
    }

    //#Malin: so we can set size during run time
    public void SetSize(float x, float y)
    {
        if (!(size == null))
        {
            oldSize = size;
        }
        size.x = x;
        size.y = y;
        this.OnValidate();
    }

    public void SetMaterial(Material material)
    {
        this.material = material;
        this.OnValidate();
    }

    public void UseReset()
    {
        Reset();
    }


}
