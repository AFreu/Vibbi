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

    bool rebuilding;

    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);
    [DllImport("deform_plugin")] private static extern void ShutdownDeformPlugin();
    [DllImport("deform_plugin")] private static extern int CreateDeformableObject(Vector3[] vertices, Vector2[] uvs, uint numVertices,
                                                                                  int[] indices, uint numIndices,
                                                                                  Vector3 location, Vector4 rotation, Vector3 scale,
                                                                                  float distanceStiffness, float bendingStiffness);
    protected override void Reset()
    {
        base.Reset();

        RebuildMesh();
        shouldRegenerateParticles = true;
    }

    void OnDestroy()
    {
        RebuildMesh();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        
        size.x = Mathf.Max(0.1f, size.x);
        size.y = Mathf.Max(0.1f, size.y);

        resolution = Math.Max(Math.Min(resolution, MAX_RESOLUTION), MIN_RESOLUTION);

        if (!size.Equals(oldSize) || resolution != oldResolution)
        {
            RebuildMesh();

            InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");
            id = CreateDeformableObject(mesh.vertices, mesh.uv, (uint)mesh.vertices.Length,
                                        mesh.triangles, (uint)mesh.triangles.Length / 3,
                                        transform.position, GetRotation(), transform.lossyScale,
                                        distanceStiffness, bendingStiffness);

            shouldRegenerateParticles = true;
        }

        GetComponent<MeshRenderer>().material = material;
    }

    public override void RebuildMesh()
    {
        if (!mesh)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
        }

        MeshUtils.CreateClothMesh(size, resolution, mesh);

        meshVertices = mesh.vertices;
        meshNormals = mesh.normals;

        ResetColorBuffers();

        oldSize = size;
        oldResolution = resolution;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }

    public void ResetColorBuffers()
    {
        uint numVertices, numIndices, resX, resY;

        MeshUtils.GetPatchInfo(size, resolution, out numVertices, out numIndices, 
            out resX, out resY);

        fixedVertices = new bool[numVertices];
        attachedVertices = new bool[numVertices];
        friction = new bool[numVertices];
    }
}
