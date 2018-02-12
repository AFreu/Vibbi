using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformBody : MonoBehaviour {
    public Material material;

    [Header("Behavior")]
    [Range(0, 1)]
    public float distanceStiffness = 1;

    [Range(0, 1)]
    public float bendingStiffness = 0.05f;

    public GameObject attachTo;

    [HideInInspector] public int id;
    [HideInInspector] public Mesh mesh;

    [HideInInspector] public Vector3[] meshVertices;
    [HideInInspector] public Vector3[] meshNormals;

    [HideInInspector] public bool[] fixedVertices;
    [HideInInspector] public bool[] attachedVertices;

    [HideInInspector] public Dictionary<int, int> attachmentMap;

    [HideInInspector] public bool shouldRegenerateParticles;

    [HideInInspector][SerializeField] Vector3 originalPosition;
    [HideInInspector][SerializeField] Quaternion originalRotation;
    [HideInInspector][SerializeField] Vector3 originalScale;

    //private Texture2D vertexTexture;
    //private Texture2D normalTexture;

    bool transformReset;
    
    protected virtual void Start () {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);

        transformReset = true;

        // Set material to DeformMaterial in MeshRenderer
        //MeshRenderer renderer = GetComponent<MeshRenderer>();

        //Material m1 = renderer.material;
        //Material m2 = Resources.Load<Material>("DeformMaterial");

        //Color color = m1.GetColor("_Color");
        //Texture main = m1.GetTexture("_MainTex");
        //Texture bump = m1.GetTexture("_BumpMap");
        //float metallic = m1.GetFloat("_Metallic");
        //float glossy = m1.GetFloat("_Glossiness");
        
        //m2.SetColor("_Color", color);
        //m2.SetTexture("_MainTex", main);
        //m2.SetTexture("_BumpMap", bump);
        //m2.SetFloat("_Metallic", metallic);
        //m2.SetFloat("_Glossiness", glossy);

        //m2.SetTexture("_VertexTex", vertexTexture);
        //m2.SetTexture("_NormalTex", normalTexture);

        //renderer.material = m2;
    }

    void OnDestroy()
    {
        if (transformReset)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
            transformReset = false;
        }

        // Reset material
        //MeshRenderer renderer = GetComponent<MeshRenderer>();
        //renderer.material = material;
    }

    protected virtual void Reset()
    {
        InitMeshComponents();
    }

    public void SetDeformTextureUVs(Vector2[] uvs, Texture2D vertices, Texture2D normals)
    {
        mesh.uv2 = uvs;
        //vertexTexture = vertices;
        //normalTexture = normals;
    }

    public void UpdateMesh(Vector3[] vertices, Vector3[] normals)
    {
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.RecalculateBounds();
    }

    void Update()
    {
        #if UNITY_EDITOR

        //Vector3[] verts = new Vector3[mesh.vertexCount];
        //Vector2[] uvs = mesh.uv2;

        //int width = vertexTexture.width;
        //Color c;

        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //    c = vertexTexture.GetPixel((int)(uvs[i].x * width), (int)(uvs[i].y * width));
        //    verts[i] = new Vector3(c.r, c.g, c.b);
        //}

        //mesh.vertices = verts;
        
        #endif
    }

    void InitMeshComponents()
    {
        if(!gameObject.GetComponent<MeshFilter>())
        {
            gameObject.AddComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
        }

        if(!gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
        }
    }

    public void ClearAttachedVertices()
    {
        attachedVertices = new bool[mesh.vertexCount];
        shouldRegenerateParticles = true;
    }

    public void ClearFixedVertices()
    {
        fixedVertices = new bool[mesh.vertexCount];
        shouldRegenerateParticles = true;
    }

    public virtual Vector4 GetRotation()
    {
        Vector3 axis;
        float angle;
        transform.rotation.ToAngleAxis(out angle, out axis);
        return new Vector4(axis.x, axis.y, axis.z, -Mathf.Deg2Rad * angle);
    }

    public int GetId()
    {
        return id;
    }

    public void SetId(int bodyId)
    {
        id = bodyId;
    }
}
