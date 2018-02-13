using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DeformObject : DeformBody {
    [Header("General")]
    public Mesh originalMesh;

    [HideInInspector] public Vector2[] meshUVs;
    [HideInInspector] public int[] meshTriangles;

    [HideInInspector][SerializeField] Mesh oldMesh;

    [DllImport("deform_plugin")] private static extern int CreateDeformableObjectFromFile(string path, Vector3 location, Vector4 rotation,
                                                                                          float distanceStiffness, float bendingStiffness);

    [DllImport("deform_plugin")] private static extern int CreateDeformableObject(Vector3[] vertices, Vector2[] uvs, uint numVertices,
                                                                                  int[] indices, uint numIndices,
                                                                                  Vector3 location, Vector4 rotation,
                                                                                  float distanceStiffness, float bendingStiffness);

    [DllImport("deform_plugin")] private static extern void GetMeshData(string path, IntPtr verts, IntPtr nor, IntPtr uvs, IntPtr tris);
    [DllImport("deform_plugin")] private static extern void GetMeshBufferSizes(string path, out uint numVertices, out uint numTriangles);

    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);
    [DllImport("deform_plugin")] private static extern void ShutdownDeformPlugin();

    protected override void Start()
    {
        base.Start();
    }

    void OnDestroy()
    {
        RebuildMesh();
    }

    void OnValidate()
    {
        if(!originalMesh.Equals(oldMesh))
        {
            RebuildMesh();
            shouldRegenerateParticles = true;
            oldMesh = originalMesh;
        }

        GetComponent<MeshRenderer>().material = material;
    }

    protected override void Reset()
    {
        base.Reset();

        RebuildMesh();
        shouldRegenerateParticles = true;
    }

    public void Build()
    {
        base.Reset();
        //RebuildMesh();
    }
    
    unsafe void RebuildMesh()
    {
        Debug.Log("RebuildMesh");
        if (originalMesh == null) return;
        Debug.Log("StillRebuildingMesh");
        mesh = new Mesh();
        mesh.MarkDynamic();

        mesh.vertices = originalMesh.vertices;
        mesh.normals = originalMesh.normals;
        mesh.uv = originalMesh.uv;
        mesh.triangles = originalMesh.triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshVertices = mesh.vertices;
        meshNormals = mesh.normals;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

        fixedVertices = new bool[mesh.vertices.Length];
        attachedVertices = new bool[mesh.vertices.Length];
            
        oldMesh = mesh;

        ShutdownDeformPlugin();
        InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");
        id = CreateDeformableObject(mesh.vertices, mesh.uv, (uint)mesh.vertices.Length, mesh.triangles, (uint)mesh.triangles.Length / 3,
                                    transform.position, GetRotation(), distanceStiffness, bendingStiffness);
    }

    public override Vector4 GetRotation()
    {
        return base.GetRotation();
    }
}
