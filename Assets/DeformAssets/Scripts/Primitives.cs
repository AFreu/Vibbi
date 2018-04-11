using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Primitives {

    public static void CreateBoxMesh(Vector3 size, Mesh mesh)
    {
        float length = size.x;
        float width =  size.y;
        float height = size.z;

        #region Vertices
        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

        Vector3[] vertices = new Vector3[]
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
        #endregion

        #region Normals
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normals = new Vector3[]
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
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
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
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	        // Bottom
	        3, 1, 0,
            3, 2, 1,			
 
	        // Left
	        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	        // Front
	        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	        // Back
	        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	        // Right
	        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	        // Top
	        3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5
        };
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    public static void CreateSphereMesh(float radius, Mesh mesh)
    {
        float r = radius;
        int nbLong = 24;
        int nbLat = 16;

        #region Vertices
        Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        vertices[0] = Vector3.up * r;
        for (int lat = 0; lat < nbLat; lat++)
        {
            float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++)
            {
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * r;
            }
        }
        vertices[vertices.Length - 1] = Vector3.up * -r;
        #endregion

        #region Normals		
        Vector3[] normals = new Vector3[vertices.Length];
        for (int n = 0; n < vertices.Length; n++)
            normals[n] = vertices[n].normalized;
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for (int lat = 0; lat < nbLat; lat++)
            for (int lon = 0; lon <= nbLong; lon++)
                uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
        #endregion

        #region Triangles
        int nbFaces = vertices.Length;
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap
        int i = 0;
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;
        }

        //Middle
        for (int lat = 0; lat < nbLat - 1; lat++)
        {
            for (int lon = 0; lon < nbLong; lon++)
            {
                int current = lon + lat * (nbLong + 1) + 1;
                int next = current + nbLong + 1;

                triangles[i++] = current;
                triangles[i++] = current + 1;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = next;
            }
        }

        //Bottom Cap
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = vertices.Length - 1;
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    public static void CreateCapsuleMesh(float radiusA, float radiusB, float height, Mesh mesh)
    {
        int segments = 24;

        // make segments an even number
        if (segments % 2 != 0)
            segments++;

        // extra vertex on the seam
        int points = segments + 1;

        // calculate points around a circle
        float[] pX = new float[points];
        float[] pZ = new float[points];
        float[] pY = new float[points];
        float[] pR = new float[points];

        float calcH = 0f;
        float calcV = 0f;

        for (int i = 0; i < points; i++)
        {
            pX[i] = Mathf.Sin(calcH * Mathf.Deg2Rad);
            pZ[i] = Mathf.Cos(calcH * Mathf.Deg2Rad);
            pY[i] = Mathf.Cos(calcV * Mathf.Deg2Rad);
            pR[i] = Mathf.Sin(calcV * Mathf.Deg2Rad);

            calcH += 360f / (float)segments;
            calcV += 180f / (float)segments;
        }

        // - Vertices and UVs -

        Vector3[] vertices = new Vector3[points * (points + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        int ind = 0;

        float radiusAvg = (radiusA + radiusB) / 2.0f;

        // Y-offset is half the height minus the diameter
        float yOff = (height - (radiusAvg * 2f)) * 0.5f;
        if (yOff < 0)
            yOff = 0;

        // uv calculations
        float stepX = 1f / ((float)(points - 1));
        float uvX, uvY;

        // Top Hemisphere
        int top = Mathf.CeilToInt((float)points * 0.5f);

        for (int y = 0; y < top; y++)
        {
            for (int x = 0; x < points; x++)
            {
                vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radiusA;
                vertices[ind].y = yOff + vertices[ind].y;

                uvX = 1f - (stepX * (float)x);
                uvY = (vertices[ind].y + (height * 0.5f)) / height;
                uvs[ind] = new Vector2(uvX, uvY);

                ind++;
            }
        }

        // Bottom Hemisphere
        int btm = Mathf.FloorToInt((float)points * 0.5f);

        for (int y = btm; y < points; y++)
        {
            for (int x = 0; x < points; x++)
            {
                vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radiusB;
                vertices[ind].y = -yOff + vertices[ind].y;

                uvX = 1f - (stepX * (float)x);
                uvY = (vertices[ind].y + (height * 0.5f)) / height;
                uvs[ind] = new Vector2(uvX, uvY);

                ind++;
            }
        }
        
        // - Triangles -

        int[] triangles = new int[(segments * (segments + 1) * 2 * 3)];

        for (int y = 0, t = 0; y < segments + 1; y++)
        {
            for (int x = 0; x < segments; x++, t += 6)
            {
                triangles[t + 0] = ((y + 0) * (segments + 1)) + x + 0;
                triangles[t + 1] = ((y + 1) * (segments + 1)) + x + 0;
                triangles[t + 2] = ((y + 1) * (segments + 1)) + x + 1;

                triangles[t + 3] = ((y + 0) * (segments + 1)) + x + 1;
                triangles[t + 4] = ((y + 0) * (segments + 1)) + x + 0;
                triangles[t + 5] = ((y + 1) * (segments + 1)) + x + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public static void CreatePlaneMesh(float length, float width, int resX, int resZ, Mesh mesh)
    {
        #region Vertices		
        Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
            }
        }
        #endregion

        #region Normals
        Vector3[] normals = new Vector3[vertices.Length];
        for (int n = 0; n < normals.Length; n++)
            normals[n] = Vector3.up;
        #endregion

        #region UVs		
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Triangles
        int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int face = 0; face < nbFaces; face++)
        {
            // Retrieve lower left corner from face ind
            int i = face % (resX - 1) + (face / (resZ - 1) * resX);

            triangles[t++] = i + resX;
            triangles[t++] = i + 1;
            triangles[t++] = i;

            triangles[t++] = i + resX;
            triangles[t++] = i + resX + 1;
            triangles[t++] = i + 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}
