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

    [DllImport("deform_plugin")] private static extern int CreateDeformableObject(Vector3[] vertices, Vector2[] uvs, uint numVertices,
                                                                                  int[] indices, uint numIndices,
                                                                                  Vector3 location, Vector4 rotation, Vector3 scale,
                                                                                  float distanceStiffness, float bendingStiffness);

    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);
    [DllImport("deform_plugin")] private static extern void ShutdownDeformPlugin();

    void OnDestroy()
    {
        RebuildMesh();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        

        if(!originalMesh.Equals(oldMesh))
        {
            RebuildMesh();
            shouldRegenerateParticles = true;
            oldMesh = originalMesh;
        }

        GetComponent<MeshRenderer>().material = material;
    }
    
    public override void RebuildMesh()
    {
        if (originalMesh == null) return;
        
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
        meshUVs = mesh.uv;
        meshTriangles = mesh.triangles;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

        fixedVertices = new bool[mesh.vertexCount];
        attachedVertices = new bool[mesh.vertexCount];

        friction = new bool[mesh.vertexCount];
        for(int i = 0; i < mesh.vertexCount; i++)
        {
            friction[i] = true;
        }
            
        oldMesh = mesh;

        InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");
        id = CreateDeformableObject(meshVertices, mesh.uv, (uint)mesh.vertices.Length, mesh.triangles, (uint)mesh.triangles.Length / 3,
                                    transform.position, GetRotation(), transform.lossyScale, distanceStiffness, bendingStiffness);

    }


}
