using UnityEngine;
using System.Collections;

public class Bendable : MonoBehaviour
{
    public enum BendAxis { X, Y, Z };

    public float rotate = 90;
    public float fromPosition = 0.5F; //from 0 to 1
    public BendAxis axis = BendAxis.X;
    public Mesh mesh;
    Vector3[] vertices;

    void Awake()
    {
        
    }

    public void Bend()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        if (axis == BendAxis.X)
        {
            float meshWidth = mesh.bounds.size.y;
            for (var i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].y + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].z * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                vertices[i].z += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].y = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            }
        }

        /*Bending straight
        if (axis == BendAxis.X)
        {

            float meshWidth = mesh.bounds.size.y;
            float bendingY = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
            Debug.Log(bendingY);
            for (var i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].y < bendingY)
                {
                    continue;
                }

                float deltaY = vertices[i].y - bendingY;
                vertices[i].z += deltaY;
                vertices[i].y = bendingY;
                

            }
        }*/


        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
 }