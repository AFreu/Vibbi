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

    DeformObject[] objectArr;
    DeformCloth[] clothArr;
    DeformCollider[] colliderArr;
    DeformBody[] deformables;

    Vector3[] vertices, normals;

    Vector3 pickedPos;
    int pickedObjectId;
    int pickedIndex;
    float pickedDistance;

    Vector3 originalGravity;
    Vector3 originalWind;

    bool isDragging;

    MouseOrbit mouseOrbit;

    //GUI
    bool mouseCursorInGUI = false;

    GameObject gravityXField;
    GameObject gravityYField;
    GameObject gravityZField;

    GameObject windXField;
    GameObject windYField;
    GameObject windZField;

    GameObject distanceStiffnessSlider;
    GameObject bendingStiffnessSlider;

    GameObject wireframeToggle;

    //Texture-related
    int textureResolution;

    uint totalNumberOfVertices;
    Texture2D vertexTexture;
    Texture2D normalTexture;

    delegate void LogCallback(string msg);

    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);
    [DllImport("deform_plugin")] private static extern void UpdateDeformPlugin();
    [DllImport("deform_plugin")] private static extern void ShutdownDeformPlugin();
    [DllImport("deform_plugin")] private static extern bool StartSimulation();

    [DllImport("deform_plugin")] private static extern int GetNumVertices();
    [DllImport("deform_plugin")] private static extern int GetNumVerticesOfObject(int object_id);

    [DllImport("deform_plugin")] private static extern void GetVertices(IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetObjectVertices(int object_id, IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetNormals(IntPtr data);
    [DllImport("deform_plugin")] private static extern void GetObjectNormals(int object_id, IntPtr data);

    [DllImport("deform_plugin")] private static extern void GetParticleNeighbors(int object_id, int particleIndex, IntPtr neighbors, int count);

    [DllImport("deform_plugin")] private static extern int CreateDeformableObjectFromFile(string path, Vector3 location, Vector4 rotation,
                                                                                          float distanceStiffness, float bendingStiffness);

    [DllImport("deform_plugin")] private static extern int CreateDeformableObject(Vector3[] vertices, Vector2[] uvs, uint numVertices,
                                                                                  int[] indices, uint numIndices,
                                                                                  Vector3 location, Vector4 rotation,
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
    [DllImport("deform_plugin")] private static extern int FixParticle(int object_id, int particleIndex);

    [DllImport("deform_plugin")] private static extern bool PickParticle(Vector3 rayBegin, Vector3 rayEnd, float range,
                                                                         out int objectId, out int particleIndex, out Vector3 position,
                                                                         out float distanceToRayStart);

    [DllImport("deform_plugin")] private static extern bool MoveParticle(int objectId, int particleIndex, Vector3 position);
    [DllImport("deform_plugin")] private static extern void ReleaseParticle(int objectId, int particleIndex);

    [DllImport("deform_plugin")] private static extern void SetLogCallback(LogCallback callback);

#if UNITY_EDITOR
    [InitializeOnLoad]
    class PluginInitializer
    {
        static PluginInitializer()
        {
            SetLogCallback(CustomLogger);
            InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");
            Debug.Log("Initialized the Deform plugin");
        }
    }
#endif

    void Start () {
        InitCameraComponents();

        originalGravity = gravity;
        originalWind = wind;

        ResetSimulation();
        //SetGUIParameters(clothArr[0].distanceStiffness, clothArr[0].bendingStiffness);
        SetGUIParameters(0, 0);

        Debug.Log("malinlog:: DeformManager start done");
    }

    unsafe void Update()
    {
        UpdateColliders();

		if (deformables == null)
			return;

        if (deformables.Length > 0)
        {
            UpdateDeformPlugin();

            foreach(DeformBody body in deformables)
            {
                int id = body.GetId();
                int numVertices = GetNumVerticesOfObject(id);

                vertices = new Vector3[numVertices];
                normals = new Vector3[numVertices];

                fixed (Vector3* v = vertices, n = normals)
                {
                    GetObjectVertices(id, (IntPtr)v);
                    GetObjectNormals(id, (IntPtr)n);
                }

                body.UpdateMesh(vertices, normals);
            }

            //vertices = new Vector3[GetNumVertices()];
            //normals = new Vector3[GetNumVertices()];

            //fixed (Vector3* v = vertices, n = normals)
            //{
            //    GetVertices((IntPtr) v);
            //    GetNormals((IntPtr) n);
            //}

            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    float v_r = vertices[i].x;
            //    float v_g = vertices[i].y;
            //    float v_b = vertices[i].z;

            //    float n_r = normals[i].x;
            //    float n_g = normals[i].y;
            //    float n_b = normals[i].z;

            //    int x = i / textureResolution;
            //    int y = i % textureResolution;

            //    vertexTexture.SetPixel(x, y, new Color(v_r, v_g, v_b, 1));
            //    normalTexture.SetPixel(x, y, new Color(n_r, n_g, n_b, 1));
            //}

            //vertexTexture.Apply();
            //normalTexture.Apply();

            UpdateAttachedParticles();
        }

        HandleInput();
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

    void ResetSimulation()
    {

        Debug.Log("Log:: Reset Simulation");
        InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");

        SetGlobalParameters();
        InitDeformObjects();

        if (deformables.Length == 0) return;


        Debug.Log("Log:: Deformables > 0");

        StartSimulation();

        //Fixed particles
        for (int i = 0; i < deformables.Length; i++)
        {
            for (int j = 0; j < deformables[i].fixedVertices.Length; j++)
            {
                if (deformables[i].fixedVertices[j])
                {
                    FixParticle(deformables[i].GetId(), j);
                }
            }
        }

        // Attached vertices
        for (int i = 0; i < deformables.Length; i++)
        {
            deformables[i].attachmentMap = new Dictionary<int, int>();
            Vector3[] attachmentPoints = new Vector3[0];

            if (deformables[i].attachTo)
            {
                if (deformables[i].attachTo.GetComponent<SkinnedMeshRenderer>())
                {
                    if (deformables[i] is DeformCloth) //Temporary fix, investigate this issue
                    {
                        Mesh baked = new Mesh();
                        deformables[i].attachTo.GetComponent<SkinnedMeshRenderer>().BakeMesh(baked);
                        attachmentPoints = baked.vertices;
                    }
                    else
                    {
                        attachmentPoints = deformables[i].attachTo.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
                    }
                }
                else if (deformables[i].attachTo.GetComponent<MeshFilter>())
                {
                    attachmentPoints = deformables[i].attachTo.GetComponent<MeshFilter>().mesh.vertices;
                }
            }
            else
            {
                continue;
            }

            AttachVertices(deformables[i], attachmentPoints);
        }
    }

    void AttachVertices(DeformBody body, Vector3[] attachmentPoints)
    {
        for (int j = 0; j < body.attachedVertices.Length; j++)
        {
            if (body.attachedVertices[j] && attachmentPoints.Length > 0)
            {
                FixParticle(body.GetId(), j);

                float closest = Mathf.Infinity;
                int closestIdx = -1;

                for (int k = 0; k < attachmentPoints.Length; k++)
                {
                    Vector3 clothPos = body.transform.TransformPoint(body.meshVertices[j]);
                    Vector3 attachPos = body.attachTo.transform.TransformPoint(attachmentPoints[k]);

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

    void SetGUIParameters(float distance, float bending)
    {
        gravityXField = GameObject.Find("GravityXInput");
        gravityYField = GameObject.Find("GravityYInput");
        gravityZField = GameObject.Find("GravityZInput");

        windXField = GameObject.Find("WindXInput");
        windYField = GameObject.Find("WindYInput");
        windZField = GameObject.Find("WindZInput");

        distanceStiffnessSlider = GameObject.Find("DistanceStiffnessSlider");
        bendingStiffnessSlider = GameObject.Find("BendingStiffnessSlider");

        wireframeToggle = GameObject.Find("WireframeToggle");

        if (gravityXField && gravityYField && gravityZField && windXField && windYField && windZField &&
           distanceStiffnessSlider && bendingStiffnessSlider)
        {
            gravityXField.GetComponent<InputField>().text = gravity.x.ToString();
            gravityYField.GetComponent<InputField>().text = gravity.y.ToString();
            gravityZField.GetComponent<InputField>().text = gravity.z.ToString();

            windXField.GetComponent<InputField>().text = wind.x.ToString();
            windYField.GetComponent<InputField>().text = wind.y.ToString();
            windZField.GetComponent<InputField>().text = wind.z.ToString();

            distanceStiffnessSlider.GetComponent<Slider>().value = distance;
            bendingStiffnessSlider.GetComponent<Slider>().value = bending;
        }
    }

    void InitDeformObjects()
    {

        Debug.Log("Log:: Init Deform Objects");
        objectArr = FindObjectsOfType<DeformObject>();
        clothArr = FindObjectsOfType<DeformCloth>();
        colliderArr = FindObjectsOfType(typeof(DeformCollider)) as DeformCollider[];

        deformables = FindObjectsOfType<DeformBody>();

        foreach(DeformBody body in deformables)
        {

            int id = CreateDeformableObject(body.meshVertices, body.mesh.uv, (uint)body.mesh.vertices.Length, body.mesh.triangles,
                                            (uint)body.mesh.triangles.Length / 3, body.transform.position, body.GetRotation(),
                                            body.distanceStiffness, body.bendingStiffness);

            body.SetId(id);

            totalNumberOfVertices += (uint) body.mesh.vertexCount;
        }
        
        foreach (DeformCollider collider in colliderArr)
        {
            CreateCollider(collider);
        }

        //if (sdfPath != null && objPath != null && sdfPath.Length > 0 && objPath.Length > 0)
        //{
        //    CreateSDFCollider(Application.dataPath + "/" + sdfPath, Application.dataPath + "/" + objPath);
        //}

        //SetupDeformTexture(totalNumberOfVertices);
    }

    void SetupDeformTexture(uint numVertices)
    {
        textureResolution = 8;

        while (textureResolution * textureResolution < numVertices)
        {
            textureResolution *= 2;
        }

        vertexTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBAFloat, false);
        vertexTexture.wrapMode = TextureWrapMode.Clamp;
        vertexTexture.filterMode = FilterMode.Point;

        normalTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBAFloat, false);
        normalTexture.wrapMode = TextureWrapMode.Clamp;
        normalTexture.filterMode = FilterMode.Point;

        for (int i = 0; i < textureResolution; i++)
        {
            for(int j = 0; j < textureResolution; j++)
            {
                vertexTexture.SetPixel(i, j, new Color(1, 1, 1, 1));
                normalTexture.SetPixel(i, j, new Color(1, 1, 1, 1));
            }
        }

        vertexTexture.Apply();
        normalTexture.Apply();

        int texIndex = 0;

        foreach (DeformBody body in deformables)
        {
            Vector2[] deformTextureUVs = new Vector2[body.mesh.vertexCount];

            for (int j = 0; j < body.mesh.vertexCount; j++)
            {
                float u = (float) (texIndex / textureResolution) / (float) (textureResolution);
                float v = (float) (texIndex % textureResolution) / (float) (textureResolution);

                deformTextureUVs[j] = new Vector2(u, v);

                texIndex++;
            }

            body.SetDeformTextureUVs(deformTextureUVs, vertexTexture, normalTexture);
        }

        GameObject debugSurface = GameObject.FindGameObjectWithTag("DebugSurface");
        if(debugSurface)
        {
            MeshRenderer renderer = debugSurface.GetComponent<MeshRenderer>();
            renderer.material.SetTexture("_MainTex", normalTexture);
        }
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

                foreach (KeyValuePair<int, int> entry in body.attachmentMap)
                {
                    MoveParticle(body.GetId(), entry.Key, baked.vertices[entry.Value]);
                }
            }
            else if (body.attachTo.GetComponent<MeshFilter>())
            {
                Mesh mesh = body.attachTo.GetComponent<MeshFilter>().mesh;

                foreach (KeyValuePair<int, int> entry in body.attachmentMap)
                {
                    MoveParticle(body.GetId(), entry.Key, body.attachTo.transform.TransformPoint(mesh.vertices[entry.Value]));
                }
            }
        }
    }

    void UpdateColliders()
    {
        colliderArr = FindObjectsOfType(typeof(DeformCollider)) as DeformCollider[];
        foreach (DeformCollider collider in colliderArr)
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
        if (Input.GetKeyDown(KeyCode.R)) { Reset(); }
        if (Input.GetKeyDown(KeyCode.Escape)) { Quit(); }
        if (Input.GetKeyDown(KeyCode.W)) { ToggleWireframe(); }

        if (mouseCursorInGUI) return;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayBegin = ray.origin;
            Vector3 rayEnd = ray.origin + (4096 * ray.direction);

            if (PickParticle(rayBegin, rayEnd, 0.05f, out pickedObjectId, out pickedIndex, out pickedPos, out pickedDistance))
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

        Debug.Log("Log:: Resetting");
        ShutdownDeformPlugin();
        ResetSimulation();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private static void CustomLogger(string msg)
    {
        Debug.LogError("<color=#820b40>[C++]: " + msg + "</color>");
    }

    // GUI handling

    public void OnGravityEdited()
    {
        float x, y, z;
        gravity.x = float.TryParse(gravityXField.GetComponent<InputField>().text, out x) ? x : gravity.x;
        gravity.y = float.TryParse(gravityYField.GetComponent<InputField>().text, out y) ? y : gravity.x;
        gravity.z = float.TryParse(gravityZField.GetComponent<InputField>().text, out z) ? z : gravity.x;
        SetGravity(gravity);
    }

    public void OnWindEdited()
    {
        float x, y, z;
        wind.x = float.TryParse(windXField.GetComponent<InputField>().text, out x) ? x : wind.x;
        wind.y = float.TryParse(windYField.GetComponent<InputField>().text, out y) ? y : wind.y;
        wind.z = float.TryParse(windZField.GetComponent<InputField>().text, out z) ? z : wind.z;
        SetWind(wind);
    }

    public void OnDistanceStiffnessEdited(Slider slider)
    {
        for (int i = 0; i < objectArr.Length; i++)
        {
            SetDistanceStiffness(objectArr[i].GetId(), slider.value);
        }

        for (int i = 0; i < clothArr.Length; i++)
        {
            SetDistanceStiffness(clothArr[i].GetId(), slider.value);
        }
    }

    public void OnBendingStiffnessEdited(Slider slider)
    {
        for (int i = 0; i < objectArr.Length; i++)
        {
            SetBendingStiffness(objectArr[i].GetId(), slider.value);
        }

        for (int i = 0; i < clothArr.Length; i++)
        {
            SetBendingStiffness(clothArr[i].GetId(), slider.value);
        }
    }

    public void ToggleWireframe()
    {
        wireframeToggle.GetComponent<Toggle>().isOn = !wireframeToggle.GetComponent<Toggle>().isOn;
    }

    public void OnMouseEnterGUI()
    {
        mouseCursorInGUI = true;
        mouseOrbit.SetActive(false);
    }

    public void OnMouseExitGUI()
    {
        mouseCursorInGUI = false;
        mouseOrbit.SetActive(true);
    }


    //######################################
    // MALIN PRÖVAR SAKER
    //######################################s
    public void CreateNewDeformableObject(DeformBody body, Vector3 location)
    {
        int id = CreateDeformableObject(body.meshVertices, body.mesh.uv, (uint)body.mesh.vertices.Length, body.mesh.triangles,
                                            (uint)body.mesh.triangles.Length / 3, location, body.GetRotation(),
                                            body.distanceStiffness, body.bendingStiffness);
        body.SetId(id);

        StartSimulation();
    }
}
