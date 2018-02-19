using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MeshUtils : MonoBehaviour {

    private const int MAX_VERTICES_PER_MESH = 65000;
    private const int MAX_CUBES_PER_MESH = 2708; // 65000 / 24
    private const int VERTICES_PER_CUBE = 24;
    private const int INDICES_PER_CUBE = 36;

    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);
    [DllImport("deform_plugin")] private static extern void CreatePatch(Vector2 size, uint resolution, IntPtr v, IntPtr t);

    public static void GetPatchInfo(Vector2 size, uint res, out uint numVertices, out uint numIndices, out uint xRes, out uint yRes)
    {
        xRes = Mathf.Max(size.x, size.y) == size.x ? res : (uint)(res * (size.x / size.y));
        yRes = Mathf.Max(size.x, size.y) == size.y ? res : (uint)(res * (size.y / size.x));

        numVertices = xRes * yRes;
        numIndices = (xRes - 1) * (yRes - 1) * 6;
    }

    public static uint NumVerticesOfPatch(Vector2 size, uint res)
    {
        uint numVertices, numIndices, xRes, yRes;
        GetPatchInfo(size, res, out numVertices, out numIndices, out xRes, out yRes);
        return numVertices;
    }

    public unsafe static void CreateClothMesh(Vector2 size, uint resolution, Mesh mesh)
    {
        uint numVertices, numIndices, xRes, yRes;

        mesh.Clear();
        Debug.Log("MeshUtils:: mesh is cleared");
        Debug.Log("Size is "+ size);
        GetPatchInfo(size, resolution, out numVertices, out numIndices, out xRes, out yRes);

        Vector3[] vertices = new Vector3[numVertices];
        Vector3[] normals  = new Vector3[numVertices];
        Vector2[] uvs      = new Vector2[numVertices];
        int[] triangles    = new int[numIndices];

        fixed (Vector3* v = vertices)
        {
            fixed (int* t = triangles)
            {
                CreatePatch(size / (float) 2, resolution, (IntPtr) v, (IntPtr) t);
            }
        }

        int texCoordColumn = -1;
        int texCoordRow = -1;

        for (int i = 0; i < numVertices; i++)
        {
            normals[i] = new Vector3(0, 1, 0);

            if (i % xRes == 0)
            {
                texCoordColumn = 0;
                texCoordRow++;
            }

            float uvX = (float)texCoordColumn++ / (float)(xRes - 1);
            float uvY = (float)texCoordRow / (float)(yRes - 1);

            uvs[i] = new Vector2(uvX, uvY);
        }
        
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        //mesh.RecalculateTangents();
        //mesh.RecalculateBounds();
    }

    public static void CreateParticles(DeformBody body, int layer, out Mesh[] meshes)
    {
        if (body.meshVertices == null) { 
            meshes = null;
            return;
        }

        int numVertices = body.meshVertices.Length;
        int numMeshes = ((numVertices * VERTICES_PER_CUBE) / MAX_VERTICES_PER_MESH) + 1;

        meshes = new Mesh[numMeshes];

        float size = 0.005f;

        for(int i = 0; i < numMeshes; i++)
        {
            meshes[i] = new Mesh();
            int verticesLeft = Math.Min(numVertices - i * MAX_CUBES_PER_MESH, MAX_CUBES_PER_MESH);

            Vector3[] vertices = new Vector3[verticesLeft * VERTICES_PER_CUBE];
            Vector3[] normals  = new Vector3[verticesLeft * VERTICES_PER_CUBE];
            Vector2[] uvs      = new Vector2[verticesLeft * VERTICES_PER_CUBE];
            Color[] colors     = new Color[verticesLeft * VERTICES_PER_CUBE];
            int[] triangles    = new int[verticesLeft * INDICES_PER_CUBE];

            int vertexOffset = 0;
            int triangleOffset = 0;

            for (int j = i * MAX_CUBES_PER_MESH; j < i * MAX_CUBES_PER_MESH + verticesLeft; j++)
            {
                #region Vertices
                Vector3 inputVertex = body.meshVertices[j];

                Vector3 p0 = inputVertex + new Vector3(-size * .5f, -size * .5f, size * .5f);
                Vector3 p1 = inputVertex + new Vector3(size * .5f, -size * .5f, size * .5f);
                Vector3 p2 = inputVertex + new Vector3(size * .5f, -size * .5f, -size * .5f);
                Vector3 p3 = inputVertex + new Vector3(-size * .5f, -size * .5f, -size * .5f);

                Vector3 p4 = inputVertex + new Vector3(-size * .5f, size * .5f, size * .5f);
                Vector3 p5 = inputVertex + new Vector3(size * .5f, size * .5f, size * .5f);
                Vector3 p6 = inputVertex + new Vector3(size * .5f, size * .5f, -size * .5f);
                Vector3 p7 = inputVertex + new Vector3(-size * .5f, size * .5f, -size * .5f);

                Vector3[] v = new Vector3[]
                {
	                // Bottom
	                p0, p1, p2, p3,
 
	                // Left
	                p7, p4, p0, p3,
 
	                // Front
	                p4, p5, p1, p0,
 
	                // Back
	                p6, p7, p3, p2,
 
	                // Right
	                p5, p6, p2, p1,
 
	                // Top
	                p7, p6, p5, p4
                };

                Array.Copy(v, 0, vertices, vertexOffset, v.Length);
                #endregion

                #region Normals
                Vector3 up = Vector3.up;
                Vector3 down = Vector3.down;
                Vector3 front = Vector3.forward;
                Vector3 back = Vector3.back;
                Vector3 left = Vector3.left;
                Vector3 right = Vector3.right;

                Vector3[] n = new Vector3[]
                {
	                // Bottom
	                down, down, down, down,
 
	                // Left
	                left, left, left, left,
 
	                // Front
	                front, front, front, front,
 
	                // Back
	                back, back, back, back,
 
	                // Right
	                right, right, right, right,
 
	                // Top
	                up, up, up, up
                };

                Array.Copy(n, 0, normals, vertexOffset, n.Length);
                #endregion

                #region UVs
                Vector2 _00 = new Vector2(0f, 0f);
                Vector2 _10 = new Vector2(1f, 0f);
                Vector2 _01 = new Vector2(0f, 1f);
                Vector2 _11 = new Vector2(1f, 1f);

                Vector2[] u = new Vector2[]
                {
	                // Bottom
	                _11, _01, _00, _10,
 
	                // Left
	                _11, _01, _00, _10,
 
	                // Front
	                _11, _01, _00, _10,
 
	                // Back
	                _11, _01, _00, _10,
 
	                // Right
	                _11, _01, _00, _10,
 
	                // Top
	                _11, _01, _00, _10
                };

                Array.Copy(u, 0, uvs, vertexOffset, u.Length);
                #endregion

                #region Colors
                float paintedValue = 0;

                bool[] paintedColors = layer == 0 ? body.fixedVertices : body.attachedVertices;

                if (paintedColors != null) {
                    paintedValue = paintedColors[j] ? 1 : 0;
                }

                Color color = new Color(1, 1, 1, 1);

                switch (layer) {
                    case 0: color = new Color(0.75f, 1 - paintedValue, 1 - paintedValue, 1); break;
                    case 1: color = new Color(1 - paintedValue, 0.75f, 1 - paintedValue, 1); break;
                }

                Color[] c = { color, color, color, color, color, color, color, color,
                              color, color, color, color, color, color, color, color,
                              color, color, color, color, color, color, color, color};

                Array.Copy(c, 0, colors, vertexOffset, v.Length);
                #endregion

                #region Triangles

                int[] t = new int[]
                {
	                // Bottom
	                vertexOffset + 3,         vertexOffset + 1,         vertexOffset + 0,
                    vertexOffset + 3,         vertexOffset + 2,         vertexOffset + 1,			
 
	                // Left
	                vertexOffset + 3 + 4 * 1, vertexOffset + 1 + 4 * 1, vertexOffset + 0 + 4 * 1,
                    vertexOffset + 3 + 4 * 1, vertexOffset + 2 + 4 * 1, vertexOffset + 1 + 4 * 1,
 
	                // Front
	                vertexOffset + 3 + 4 * 2, vertexOffset + 1 + 4 * 2, vertexOffset + 0 + 4 * 2,
                    vertexOffset + 3 + 4 * 2, vertexOffset + 2 + 4 * 2, vertexOffset + 1 + 4 * 2,
 
	                // Back
	                vertexOffset + 3 + 4 * 3, vertexOffset + 1 + 4 * 3, vertexOffset + 0 + 4 * 3,
                    vertexOffset + 3 + 4 * 3, vertexOffset + 2 + 4 * 3, vertexOffset + 1 + 4 * 3,
 
	                // Right
	                vertexOffset + 3 + 4 * 4, vertexOffset + 1 + 4 * 4, vertexOffset + 0 + 4 * 4,
                    vertexOffset + 3 + 4 * 4, vertexOffset + 2 + 4 * 4, vertexOffset + 1 + 4 * 4,
 
	                // Top
	                vertexOffset + 3 + 4 * 5, vertexOffset + 1 + 4 * 5, vertexOffset + 0 + 4 * 5,
                    vertexOffset + 3 + 4 * 5, vertexOffset + 2 + 4 * 5, vertexOffset + 1 + 4 * 5
                };

                Array.Copy(t, 0, triangles, triangleOffset, t.Length);
                #endregion

                vertexOffset += VERTICES_PER_CUBE;
                triangleOffset += INDICES_PER_CUBE;
            }

            meshes[i].vertices = vertices;
            meshes[i].normals = normals;
            meshes[i].uv = uvs;
            meshes[i].colors = colors;
            meshes[i].triangles = triangles;
            meshes[i].RecalculateBounds();
        }
    }

    public static void UpdateColors(DeformBody body, List<int> paintedVertices, Mesh[] meshes, int layer)
    {
        Color[][] colors = new Color[meshes.Length][];

        if (!body.shouldRegenerateParticles)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                colors[i] = meshes[i].colors;
            }
        }

        for (int i = 0; i < paintedVertices.Count; i++)
        {
            int vertex         = paintedVertices[i];
            int mesh           = (vertex * VERTICES_PER_CUBE) / (MAX_CUBES_PER_MESH * VERTICES_PER_CUBE);
            int meshColorIndex = (vertex * VERTICES_PER_CUBE) % (MAX_CUBES_PER_MESH * VERTICES_PER_CUBE);

            for(int j = meshColorIndex; j < meshColorIndex + VERTICES_PER_CUBE; j++)
            {
                float paintedValue = 0;

                if (layer == 0)
                {
                    paintedValue = body.fixedVertices[vertex] ? 1 : 0;
                    colors[mesh][j] = new Color(1, 1 - paintedValue, 1 - paintedValue, 1);
                }
                else
                {
                    paintedValue = body.attachedVertices[vertex] ? 1 : 0;
                    colors[mesh][j] = new Color(1 - paintedValue, 1, 1 - paintedValue, 1);
                } 
            }
        }

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].colors = colors[i];
        }
    }
}
