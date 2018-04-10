using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[DisallowMultipleComponent]
public abstract class DeformBody : MonoBehaviour {
    
    /**
     * The Unity material used for rendering.
     **/ 
    public Material material;

    [Header("Behavior")]

    /**
     * Determines the stiffness of the distance constraints.
     **/
    [Range(0, 1)]
    public float distanceStiffness = 1;

    /**
     * Determines the stiffness of the bending constraints.
     **/
    [Range(0, 1)]
    public float bendingStiffness = 0.05f;

    /**
     * The GameObject which this DeformBody will attach to. 
     **/
    public GameObject attachTo;

    /**
     * Whether or not this DeformBody is simulated. 
     **/
    public bool includeInSimulation = true;

    [HideInInspector] public Mesh mesh;
    [HideInInspector] public Vector3[] meshVertices;
    [HideInInspector] public Vector3[] meshNormals;

    /**
     * Determines whether each vertex will be fixed (immobile) or not.
     **/
    [HideInInspector] public bool[] fixedVertices;

    /**
     * Determines whether each vertex will be attached to an object or not. In order for vertices to be attached, the *attachTo* variable must be set.
     **/
    [HideInInspector] public bool[] attachedVertices;

    /**
     * Determines whether each vertex will be attached to an object or not. In order for vertices to be attached, the *attachTo* variable must be set.
     **/
    [HideInInspector] public bool[] friction;

    /**
     * Map for attaching DeformBody objects to Unity meshes. Maps vertices of the DeformBody to vertices of a Unity mesh. 
     **/
    [HideInInspector] public Dictionary<int, int> attachmentMap;
    [HideInInspector] public Dictionary<int, BarycentricCoordinate> attachmentMap2;

    [HideInInspector] public bool shouldRegenerateParticles;

    [HideInInspector] public Transform originalTransform;

    [SerializeField] Vector3 originalPosition;
    [SerializeField] Quaternion originalRotation;
    [SerializeField] Vector3 originalScale;

    /**
     * The unique id of this DeformBody. This assigned by the physics engine.
     **/
    protected int id;
    
    bool transformReset;

    public struct BarycentricCoordinate
    {
        public Vector3 coordinate;
        public int triangleIndex;

        public BarycentricCoordinate(Vector3 coord, int tri_index)
        {
            this.coordinate = coord;
            this.triangleIndex = tri_index;
        }
    }

    protected virtual void Start () {
        Debug.Log("Starting body");
       /* originalTransform = transform;

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        
        transformReset = true;
        */
    }

    void OnDestroy()
    {
        ResetTransform();

        // Reset material
        //MeshRenderer renderer = GetComponent<MeshRenderer>();
        //renderer.material = material;
    }

    protected virtual void Awake()
    {
        //HideMeshComponents();
    }

    protected virtual void Reset()
    {
        //HideMeshComponents();
        originalTransform = transform;
    }

    protected virtual void OnValidate()
    {
        if (originalTransform != transform)
        {
            shouldRegenerateParticles = true;
            RebuildMesh();
        }

        if (!Application.isPlaying) originalTransform = transform;
    }

    public virtual void AddToSimulation()
    {
        Debug.Log("Add to simulation "+id);
        RebuildMesh();
        includeInSimulation = true;
    }

    public void ResetTransform()
    {
        Debug.Log("Resetting transform of "+id);
        if (transformReset)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
            transformReset = false;
        }
    }

    public virtual void RemoveFromSimulation()
    {
        includeInSimulation = false;
    }

    public abstract void RebuildMesh();

    protected void HideMeshComponents()
    {
        //GetComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
        //GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
    }

    public void UpdateMesh(Vector3[] vertices, Vector3[] normals, int[] triangles)
    {
        if (this is DeformObject)
        {
           // mesh.triangles = triangles;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;

        mesh.RecalculateBounds();
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

    public void ClearFriction()
    {
        friction = new bool[mesh.vertexCount];
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
    
    public void SetTransformToZero()
    {
        originalTransform = transform;

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);

        transformReset = true;
    }
}
