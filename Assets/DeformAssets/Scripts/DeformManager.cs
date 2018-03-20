using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DeformManager : MonoBehaviour {

    [Header("Physics parameters")]
    public Vector3 gravity = new Vector3(0, -9.82f, 0);
    public Vector3 wind;

    [Range(0, 1)]
    public float airFriction = 0.9995f;

    public bool intercollision;

    //[Header("SDF collision")]
    //public string sdfPath;
    //public string objPath;

    DeformCollider[] colliders;
    DeformBody[] deformables;

    int activeObjects = 0;

    Vector3[] vertices, normals;
    int[] triangles;

    int pickedObjectId;
    int pickedIndex;
    float pickedDistance;

    Vector3 originalGravity;
    Vector3 originalWind;

    bool isDragging;

    MouseOrbit mouseOrbit;
    
    delegate void LogCallback(string msg);

    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);
    [DllImport("deform_plugin")] private static extern void UpdateDeformPlugin();
    [DllImport("deform_plugin")] private static extern void ShutdownDeformPlugin();
    [DllImport("deform_plugin")] private static extern bool StartSimulation();

    [DllImport("deform_plugin")] private static extern bool SetObjectVertexBuffer(int object_id, IntPtr vertexBuffer, int vertexCount);

    [DllImport("deform_plugin")] private static extern int GetNumVertices();
    [DllImport("deform_plugin")] private static extern int GetNumVerticesOfObject(int object_id);
    [DllImport("deform_plugin")] private static extern int GetNumIndices();
    [DllImport("deform_plugin")] private static extern int GetNumIndicesOfObject(int object_id);

    [DllImport("deform_plugin")] private static extern void GetVertices(IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetObjectVertices(int object_id, IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetNormals(IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetObjectNormals(int object_id, IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetIndices(IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetObjectIndices(int object_id, IntPtr data);

    [DllImport("deform_plugin")] private static extern void GetParticleNeighbors(int object_id, int particleIndex, IntPtr neighbors, int count);

    [DllImport("deform_plugin")] private static extern int CreateDeformableObjectFromFile(string path, Vector3 location, Vector4 rotation,
                                                                                          float distanceStiffness, float bendingStiffness);

    [DllImport("deform_plugin")] private static extern int CreateDeformableObject(Vector3[] vertices, Vector2[] uvs, uint numVertices,
                                                                                  int[] indices, uint numIndices,
                                                                                  Vector3 location, Vector4 rotation, Vector3 scale,
                                                                                  float distanceStiffness, float bendingStiffness);

    [DllImport("deform_plugin")] private static extern void SetGravity(Vector3 gravity);
    [DllImport("deform_plugin")] private static extern void SetWind(Vector3 wind);
    [DllImport("deform_plugin")] private static extern void SetAirFriction(float airFriction);
    [DllImport("deform_plugin")] private static extern void SetSelfCollision(bool selfCollision);

    [DllImport("deform_plugin")] private static extern void SetDistanceStiffness(int object_id, float stiffness);
    [DllImport("deform_plugin")] private static extern void SetBendingStiffness(int object_id, float stiffness);

    [DllImport("deform_plugin")] private static extern int CreateBoxCollider(Vector3 min, Vector3 max, Vector3 location);
    [DllImport("deform_plugin")] private static extern int CreateSphereCollider(float radius, Vector3 location);
    [DllImport("deform_plugin")] private static extern int CreatePlaneCollider(Vector3 normal, Vector3 location);
    [DllImport("deform_plugin")] private static extern int CreateCapsuleCollider(float radiusA, float radiusB, Vector3 pointA, Vector3 pointB);
    [DllImport("deform_plugin")] private static extern int CreateSDFCollider(string sdfPath, string objPath);

    [DllImport("deform_plugin")] private static extern void UpdateCapsuleCollider(int object_id, float radiusA, float radiusB,
                                                                                  Vector3 a, Vector3 b);

    [DllImport("deform_plugin")] private static extern int SetParticleInvmass(int object_id, int particleIndex, float amount);
    [DllImport("deform_plugin")] private static extern int SetParticleFriction(int object_id, int particleIndex, float amount);
    [DllImport("deform_plugin")] private static extern int FixParticle(int object_id, int particleIndex);

    [DllImport("deform_plugin")] private static extern bool PickParticle(Vector3 rayBegin, Vector3 rayEnd, float range,
                                                                         out int objectId, out int particleIndex, 
                                                                         out float distanceToRayStart);

    [DllImport("deform_plugin")] private static extern bool RayCast(Vector3 rayBegin, Vector3 rayEnd, int[] tris, int numTris, Vector3[] verts,
                                                                    out float u, out float v, out float w, out float t, out int tri,
                                                                    out Vector3 intersectionPoint);

    [DllImport("deform_plugin")] private static extern bool MoveParticle(int objectId, int particleIndex, Vector3 position);
    [DllImport("deform_plugin")] private static extern void ReleaseParticle(int objectId, int particleIndex);

    [DllImport("deform_plugin")] private static extern void SewObjects(int object_id_0, int object_id_1, uint[] vertices, int numVertices);

    [DllImport("deform_plugin")] private static extern void StartFBXCapture(int object_id, Vector3 position);
    [DllImport("deform_plugin")] private static extern bool StopFBXCapture(string path);
    [DllImport("deform_plugin")] private static extern int SnapshotFBX(int object_id, Vector3 position, string path);

    [DllImport("deform_plugin")] private static extern void SetLogCallback(LogCallback callback);

#if UNITY_EDITOR
    [InitializeOnLoad]
    class PluginInitializer
    {
        static PluginInitializer()
        {
            SetLogCallback(CustomLogger);
            InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");
        }
    }
#endif

    void Start()
    {
        //Time.timeScale = 0.66f;
        
        originalGravity = gravity;
        originalWind = wind;

        ResetSimulation();
    }

    unsafe void Update()
    {
        HandleInput();
        UpdateColliders();

        if (activeObjects == 0) return;
        UpdateAttachedParticles();

        UpdateDeformPlugin();

        foreach (DeformBody body in deformables)
        {
            int id = body.GetId();
            int numVertices = GetNumVerticesOfObject(id);
            int numIndices = GetNumIndicesOfObject(id);

            vertices = new Vector3[numVertices];
            normals =  new Vector3[numVertices];
            triangles = new int[numIndices];

            fixed (Vector3* v = vertices, n = normals)
            {
                GetObjectVertices(id, (IntPtr)v);
                GetObjectNormals(id, (IntPtr)n);
            }

            if (body is DeformObject)
            {
                fixed (int* t = triangles)
                {
                    GetObjectIndices(id, (IntPtr)t);
                }
            }

            body.UpdateMesh(vertices, normals, triangles);
        }
    }

    void InitCameraComponents()
    {
        mouseOrbit = Camera.main.GetComponent<MouseOrbit>();

        if (mouseOrbit == null)
        {
            mouseOrbit = Camera.main.gameObject.AddComponent<MouseOrbit>();
        }

        mouseOrbit.SetActive(true);
    }

    void OnValidate()
    {
        SetGravity(gravity);
    }

    /**
     *  Resets the simulation 
     */
    unsafe public void ResetSimulation()
    {
        activeObjects = 0;

        InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");

        SetGlobalParameters();
        InitDeformObjects();

       /* if (activeObjects == 0) return;

        uint[] vertices = new uint[256];
        for (uint i = 0; i < 64; i++)
        {
            vertices[i * 2] = i;
            vertices[i * 2 + 1] = i + (64 * 64);
            vertices[i * 2 + 128] = i + 63 * 64;
            vertices[i * 2 + 1 + 128] = i + 63 * 64 + (64 * 64);
        }*/

       /* if (deformables.Length >= 2)
        {
            SewObjects(deformables[0].GetId(), deformables[1].GetId(), vertices, vertices.Length);
        }*/

        StartSimulation();

        //Fixed particles and friction
        for (int i = 0; i < deformables.Length; i++)
        {
            for (int j = 0; j < deformables[i].fixedVertices.Length; j++)
            {
                if (deformables[i].fixedVertices[j])
                {
                    FixParticle(deformables[i].GetId(), j);
                }

                if (deformables[i].friction[j])
                {
                    SetParticleFriction(deformables[i].GetId(), j, 1.0f);
                }
            }
        }

        // Attached vertices
        for (int i = 0; i < deformables.Length; i++)
        {
            deformables[i].attachmentMap = new Dictionary<int, int>();
            //deformables[i].attachmentMap2 = new Dictionary<int, DeformBody.BarycentricCoordinate>();

            Vector3[] attachmentPoints = new Vector3[0];
            int[] attachmentTriangles = new int[0];

            if (deformables[i].attachTo)
            {
                if (deformables[i].attachTo.GetComponent<SkinnedMeshRenderer>())
                {
                    Mesh baked = new Mesh();
                    deformables[i].attachTo.GetComponent<SkinnedMeshRenderer>().BakeMesh(baked);
                    attachmentPoints = baked.vertices;
                    attachmentTriangles = baked.triangles;
                }
                else if (deformables[i].attachTo.GetComponent<MeshFilter>())
                {
                    attachmentPoints = deformables[i].attachTo.GetComponent<MeshFilter>().mesh.vertices;
                    attachmentTriangles = deformables[i].attachTo.GetComponent<MeshFilter>().mesh.triangles;
                }
            }
            else
            {
                continue;
            }

            AttachParticles(deformables[i], attachmentPoints);
            //AttachParticles2(deformables[i], attachmentPoints, attachmentTriangles);
        }
    }

    void AttachParticles(DeformBody body, Vector3[] attachmentPoints)
    {
        if (attachmentPoints.Length == 0) return;
        
        for (int j = 0; j < body.attachedVertices.Length; j++)
        {
            if (body.attachedVertices[j])
            {
                FixParticle(body.GetId(), j);

                float closest = Mathf.Infinity;
                int closestIdx = -1;

                for (int k = 0; k < attachmentPoints.Length; k++)
                {
                    Vector3 clothPos = body.originalTransform.TransformPoint(body.mesh.vertices[j]);
                    Vector3 attachPos = attachmentPoints[k];

                    // Divide by lossyScale since baked vertices are scaled up
                    attachPos.x /= body.attachTo.transform.lossyScale.x;
                    attachPos.y /= body.attachTo.transform.lossyScale.y;
                    attachPos.z /= body.attachTo.transform.lossyScale.z;

                    attachPos = body.attachTo.transform.TransformPoint(attachPos);

                    if ((clothPos - attachPos).magnitude < closest)
                    {
                        closest = (clothPos - attachPos).magnitude;
                        closestIdx = k;
                    }
                }

                if (closestIdx >= 0) body.attachmentMap.Add(j, closestIdx);
            }
        }
    }

    unsafe void AttachParticles2(DeformBody body, Vector3[] attachmentVerts, int[] attachmentTriangles)
    {
        if (attachmentVerts.Length == 0) return;

        for (int j = 0; j < body.attachedVertices.Length; j++)
        {
            if (body.attachedVertices[j])
            {
                FixParticle(body.GetId(), j);

                Vector3 start = body.originalTransform.TransformPoint(body.mesh.vertices[j]);
                Vector3 end_a = start + body.mesh.normals[j];
                Vector3 end_b = start - body.mesh.normals[j];

                float u, v, w;
                float t = Mathf.Infinity;
                int tri = -1;
                Vector3 inter;

                float u2, v2, w2;
                float t2 = Mathf.Infinity;
                int tri2 = -1;
                Vector3 inter2;

                Debug.DrawLine(start, end_a, Color.cyan, 100.0f);
                RayCast(start, end_a, attachmentTriangles, attachmentTriangles.Length, attachmentVerts,
                            out u, out v, out w, out t, out tri, out inter);

                Debug.DrawLine(start, end_b, Color.magenta, 100.0f);
                RayCast(start, end_b, attachmentTriangles, attachmentTriangles.Length, attachmentVerts,
                            out u2, out v2, out w2, out t2, out tri2, out inter2);

                if (tri2 != -1 && (inter2 - start).magnitude < (inter - start).magnitude)
                {
                    u = u2;
                    v = v2;
                    w = w2;
                    t = t2;
                    tri = tri2;
                }

                if (tri != -1)
                {
                    body.attachmentMap2.Add(j, new DeformBody.BarycentricCoordinate(new Vector3(u, v, w), tri));

                    // Draw intersected triangle
                    Debug.DrawLine(attachmentVerts[attachmentTriangles[tri * 3]],
                                   attachmentVerts[attachmentTriangles[tri * 3 + 1]], Color.red, 100.0f);

                    Debug.DrawLine(attachmentVerts[attachmentTriangles[tri * 3 + 1]],
                                   attachmentVerts[attachmentTriangles[tri * 3 + 2]], Color.red, 100.0f);

                    Debug.DrawLine(attachmentVerts[attachmentTriangles[tri * 3 + 2]],
                                   attachmentVerts[attachmentTriangles[tri * 3]], Color.red, 100.0f);
                }
            }
        }
    }

    void SetGlobalParameters()
    {
        gravity = originalGravity;
        wind = originalWind;

        SetGravity(gravity);
        SetWind(wind);
        SetAirFriction(airFriction);

        if(intercollision)
        {
            SetSelfCollision(true);
        }
    }

    void InitDeformObjects()
    {
        deformables = FindObjectsOfType<DeformBody>();
        
        foreach (DeformBody body in deformables)
        {
            body.ResetTransform();

            if (!body.includeInSimulation || body.mesh == null) continue;

            int id = CreateDeformableObject(body.meshVertices, body.mesh.uv, (uint)body.mesh.vertexCount,
                                            body.mesh.triangles, (uint)body.mesh.triangles.Length / 3,
                                            body.transform.position, body.GetRotation(), body.transform.lossyScale,
                                            body.distanceStiffness, body.bendingStiffness);

            body.SetId(id);
            activeObjects++;
        }

        colliders = FindObjectsOfType<DeformCollider>();

        foreach (DeformCollider collider in colliders)
        {
            CreateCollider(collider);
        }

        //if (sdfPath != null && objPath != null && sdfPath.Length > 0 && objPath.Length > 0)
        //{
        //    CreateSDFCollider(Application.dataPath + "/" + sdfPath, Application.dataPath + "/" + objPath);
        //}
    }

    void CreateCollider(DeformCollider collider)
    {
        int id;

        switch (collider.colliderType)
        {
            case DeformCollider.ColliderType.Box:
                Vector3 min = new Vector3(-collider.size.x / 2, -collider.size.y / 2, -collider.size.z / 2);
                Vector3 max = new Vector3(collider.size.x / 2, collider.size.y / 2, collider.size.z / 2);
                id = CreateBoxCollider(min, max, collider.transform.position);
                break;

            case DeformCollider.ColliderType.Sphere:
                id = CreateSphereCollider(collider.radius, collider.transform.position);
                break;

            case DeformCollider.ColliderType.Capsule:
                if (collider.capsulePointType == DeformCollider.CapsulePointType.Transform)
                {
                    id = CreateCapsuleCollider(collider.radius, collider.otherRadius, collider.transformA.position, collider.transformB.position);
                }
                else
                {
                    id = CreateCapsuleCollider(collider.radius, collider.otherRadius, collider.pointA, collider.pointB);
                }
                break;

            case DeformCollider.ColliderType.Plane:
                id = CreatePlaneCollider(collider.transform.up, collider.transform.position);
                break;

            default:
                id = -1;
                break;
        }

        collider.SetId(id);
    }

    void UpdateAttachedParticles()
    {
        foreach(DeformBody body in deformables)
        {
            if (body.attachmentMap.Count == 0) continue;

            if (body.attachTo.GetComponent<SkinnedMeshRenderer>())
            {
                Mesh baked = new Mesh();
                body.attachTo.GetComponent<SkinnedMeshRenderer>().BakeMesh(baked);

                Transform t = body.attachTo.transform;

                foreach (KeyValuePair<int, int> entry in body.attachmentMap)
                {
                    Vector3 v = baked.vertices[entry.Value];

                    // Divide by lossyScale since baked vertices are scaled up
                    v.x /= t.lossyScale.x;
                    v.y /= t.lossyScale.y;
                    v.z /= t.lossyScale.z;

                    v = t.TransformPoint(v);

                    MoveParticle(body.GetId(), entry.Key, v);
                }
            }
            else if (body.attachTo.GetComponent<MeshFilter>())
            {
                Mesh mesh = body.attachTo.GetComponent<MeshFilter>().mesh;

                foreach (KeyValuePair<int, int> entry in body.attachmentMap)
                {
                    MoveParticle(body.GetId(), entry.Key, body.attachTo.transform.parent.transform.TransformPoint(mesh.vertices[entry.Value]));
                }
            }
        }
    }

    void UpdateAttachedParticles2()
    {
        foreach (DeformBody body in deformables)
        {
            if (body.attachmentMap2.Count == 0) continue;

            if (body.attachTo.GetComponent<SkinnedMeshRenderer>())
            {
                Mesh baked = new Mesh();
                body.attachTo.GetComponent<SkinnedMeshRenderer>().BakeMesh(baked);

                Transform t = body.attachTo.transform;

                foreach (KeyValuePair<int, DeformBody.BarycentricCoordinate> entry in body.attachmentMap2)
                {
                    DeformBody.BarycentricCoordinate bc = entry.Value;
                    Vector3 a = baked.vertices[baked.triangles[bc.triangleIndex * 3]];
                    //Vector3 b = baked.vertices[baked.triangles[bc.triangleIndex * 3 + 1]];
                    //Vector3 c = baked.vertices[baked.triangles[bc.triangleIndex * 3 + 2]];

                    // U                  // V                  // W
                    Vector3 pos = a;// bc.coordinate.x * a + bc.coordinate.y * b + bc.coordinate.z * c;
                    
                    // Divide by lossyScale since baked vertices are scaled up
                    pos.x /= t.lossyScale.x;
                    pos.y /= t.lossyScale.y;
                    pos.z /= t.lossyScale.z;

                    pos = t.TransformPoint(pos);

                    MoveParticle(body.GetId(), entry.Key, pos);
                }
            }
            //else if (body.attachTo.GetComponent<MeshFilter>())
            //{
            //    Mesh mesh = body.attachTo.GetComponent<MeshFilter>().mesh;

            //    foreach (KeyValuePair<int, int> entry in body.attachmentMap)
            //    {
            //        MoveParticle(body.GetId(), entry.Key, body.attachTo.transform.parent.transform.TransformPoint(mesh.vertices[entry.Value]));
            //    }
            //}
        }
    }

    void UpdateColliders()
    {
        colliders = FindObjectsOfType(typeof(DeformCollider)) as DeformCollider[];
        foreach (DeformCollider collider in colliders)
        {
            if (collider.colliderType == DeformCollider.ColliderType.Capsule)
            {
                if (collider.capsulePointType == DeformCollider.CapsulePointType.Vector)
                {
                    UpdateCapsuleCollider(collider.GetId(), collider.radius, collider.otherRadius, collider.pointA, collider.pointB);
                }
                else
                {
                    UpdateCapsuleCollider(collider.GetId(), collider.radius, collider.otherRadius, collider.transformA.position, collider.transformB.position);
                }
            }
        }
    }

    unsafe void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reset(); 
        if (Input.GetKeyDown(KeyCode.Escape)) Quit(); 

       /* if (Input.GetKeyDown(KeyCode.Return)) StartFBXCapture(0, deformables[0].transform.position);
        if (Input.GetKeyDown(KeyCode.Backspace)) StopFBXCapture(Application.dataPath + "/ExportedFBX/mesh.fbx");
        if (Input.GetKeyDown(KeyCode.S)) SnapshotFBX(0, deformables[0].transform.position, Application.dataPath + "/ExportedFBX/mesh.fbx");
        */
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayBegin = ray.origin;
            Vector3 rayEnd = ray.origin + (4096 * ray.direction);

            if (PickParticle(rayBegin, rayEnd, 0.05f, out pickedObjectId, out pickedIndex, out pickedDistance))
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(1) && isDragging)
        {
            ReleaseParticle(pickedObjectId, pickedIndex);
            isDragging = false;
        }

        if (isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 dragPos = ray.origin + (ray.direction * pickedDistance);
            isDragging = MoveParticle(pickedObjectId, pickedIndex, dragPos);
        }
    }

    void OnDestroy()
    {
        ShutdownDeformPlugin();
    }
    
    public void Reset()
    {
        InitCameraComponents();
        ShutdownDeformPlugin();
        ResetSimulation();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private static void CustomLogger(string msg)
    {
        Debug.Log("<color=#820b40>[C++]: " + msg + "</color>");
    }
}
