using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DeformCollider : MonoBehaviour {
    public enum ColliderType { Box, Sphere, Capsule, Plane };
    public enum CapsulePointType { Vector, Transform };

    public ColliderType colliderType;

    //Box
    public Vector3 size = new Vector3(1, 1, 1);

    //Capsule
    public CapsulePointType capsulePointType;

    public Vector3 pointA = new Vector3(-0.5f, 0, 0);
    public Vector3 pointB = new Vector3(0.5f, 0, 0);

    public Transform transformA;
    public Transform transformB;

    //Sphere/capsule
    public float radius = 0.5f;
    public float otherRadius = 0.5f;

    //All
    public bool showColliderInEditor = true;
    public bool showColliderInGame = false;
    
    int id;
    
    private GameObject bounds;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    
    void Update()
    {
        if (Selection.activeGameObject == bounds)
        {
            Selection.activeGameObject = gameObject;
        }    

        if (!Application.isPlaying && showColliderInEditor) UpdateBounds(); // Do more checks to hide collider when unchecking showColliderInEditor
        if (Application.isPlaying && showColliderInGame) UpdateBounds();
    }

    public void UpdateBounds()
    {
        if (bounds == null)
        {
            bounds = new GameObject();
            bounds.hideFlags = HideFlags.HideAndDontSave;
            bounds.transform.parent = transform;
        }

        InitMeshComponents();
        meshFilter.sharedMesh = null;

        mesh = new Mesh();
        DrawColliderBounds();
    }

    void InitMeshComponents()
    {
        meshRenderer = bounds.GetComponent<MeshRenderer>();
        meshFilter = bounds.GetComponent<MeshFilter>();

        if (meshRenderer == null)
        {
            meshRenderer = bounds.AddComponent<MeshRenderer>();
            meshRenderer.hideFlags = HideFlags.HideInInspector;
        }

        if (meshFilter == null)
        {
            meshFilter = bounds.AddComponent<MeshFilter>();
            meshFilter.hideFlags = HideFlags.HideInInspector;
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
        bounds.transform.localPosition = Vector3.zero;
        bounds.transform.localRotation = Quaternion.identity;
        bounds.transform.localScale = new Vector3(1, 1, 1);

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
        meshRenderer.sharedMaterial = Resources.Load<Material>("Wireframe/Examples/Materials/Wireframe-TransparentCulled");
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

        if (capsulePointType == CapsulePointType.Transform && transformA && transformB)
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

        bounds.transform.up = aToB;
        bounds.transform.position = middle;
    }

    void DrawPlaneColliderBounds()
    {
        Primitives.CreatePlaneMesh(3, 3, 2, 2, mesh);
        mesh.RecalculateBounds();
    }
}
