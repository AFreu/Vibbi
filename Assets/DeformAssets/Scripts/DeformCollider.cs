using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DeformCollider : MonoBehaviour {
    public enum ColliderType { Box, Sphere, Capsule, Plane };
    public enum CapsulePointType { Transform, Vector };

    public ColliderType colliderType;

    //Box
    public Vector3 size;

    //Capsule
    public CapsulePointType capsulePointType;
    public Vector3 pointA;
    public Vector3 pointB;
    public Transform transformA;
    public Transform transformB;

    //Sphere/capsule
    public float radius;
    public float otherRadius;

    //All
    public bool showColliderInEditor = true;
    public bool showColliderInGame = false;
    
    int id;

    [SerializeField] GameObject collisionBounds;

    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    void Awake()
    {
        if (!Application.isPlaying || showColliderInGame)
        {
            collisionBounds = new GameObject();
            collisionBounds.hideFlags = HideFlags.HideAndDontSave;
            collisionBounds.transform.position = transform.position;
            collisionBounds.transform.rotation = transform.rotation;
            collisionBounds.transform.parent = transform;

            InitMeshComponents();
            meshFilter.mesh = null;

            mesh = new Mesh();
            DrawColliderBounds();
        }
    }

    void InitMeshComponents()
    {
        meshRenderer = collisionBounds.GetComponent<MeshRenderer>();
        meshFilter = collisionBounds.GetComponent<MeshFilter>();

        if (meshRenderer == null) meshRenderer = collisionBounds.AddComponent<MeshRenderer>();
        if (meshFilter == null) meshFilter = collisionBounds.AddComponent<MeshFilter>();
    }

    private void Update()
    {
        if (!Application.isPlaying || showColliderInGame)
        {
            DrawColliderBounds();
        }
    }

    public int GetId()
    {
        return id;
    }

    public void SetId(int colliderId)
    {
        id = colliderId;
    }

    void DrawColliderBounds()
    {
        mesh.Clear();

        if (showColliderInEditor)
        {
            switch (colliderType)
            {
                case ColliderType.Box:      DrawBoxColliderBounds();        break;
                case ColliderType.Sphere:   DrawSphereColliderBounds();     break;
                case ColliderType.Capsule:  DrawCapsuleColliderBounds();    break;                   
                case ColliderType.Plane:    DrawPlaneColliderBounds();      break;
            }
        }

        meshFilter.sharedMesh = mesh;
        meshRenderer.material = Resources.Load<Material>("Wireframe/Examples/Materials/Wireframe-TransparentCulled");
    }

    void DrawBoxColliderBounds()
    {
        Primitives.CreateBoxMesh(size, mesh);
        mesh.RecalculateBounds();
    }

    void DrawSphereColliderBounds()
    {
        Primitives.CreateSphereMesh(radius, mesh);
        mesh.RecalculateBounds();
    }

    void DrawCapsuleColliderBounds()
    {
        Vector3 aToB;
        Vector3 middle;

        if (capsulePointType == CapsulePointType.Transform)
        {
            aToB = transformA.position - transformB.position;
            middle = transformB.position + aToB / 2;
        }
        else
        {
            aToB = pointA - pointB;
            middle = pointB + aToB / 2;
        }

        Primitives.CreateCapsuleMesh(radius, otherRadius, aToB.magnitude + (radius * 2), mesh);
        mesh.RecalculateBounds();

        collisionBounds.transform.up = aToB;
        collisionBounds.transform.position = middle;
    }

    void DrawPlaneColliderBounds()
    {
        Primitives.CreatePlaneMesh(3, 3, 2, 2, mesh);
        mesh.RecalculateBounds();
    }
}
