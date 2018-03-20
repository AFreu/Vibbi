using UnityEngine;
using System.Collections;

public class MouseOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 30.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -80f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private Rigidbody m_rigidbody;

    float x = 0.0f;
    float y = 0.0f;

    bool active = true;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        m_rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (m_rigidbody != null)
        {
            m_rigidbody.freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SetActive(!active);
        }

        if (target && active)
        {
            // Rotate
            if (Input.GetMouseButton(0))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
            // Pan
            else if (Input.GetMouseButton(2))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                Vector3 forwardVector = GetComponent<Camera>().transform.forward;

                float mx = Input.GetAxis("Mouse X");
                float my = Input.GetAxis("Mouse Y");

                Vector3 panDelta;

                if(Input.GetKey(KeyCode.LeftShift)) // Depth pan
                {
                    panDelta = new Vector3(forwardVector.x * my, forwardVector.y * my, forwardVector.z * my);
                }
                else // Normal pan
                {
                    panDelta = new Vector3(forwardVector.z * mx, my * 0.5f, -forwardVector.x * mx);
                }

                target.position += panDelta;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void SetActive(bool a)
    {
        active = a;
    }
}