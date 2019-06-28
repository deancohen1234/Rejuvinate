using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    public Transform m_FollowTarget;
    public Vector3 m_CameraOffset; //offset to position camera at a more comfortable angle
    public float m_VerticalCamSpeed = 1.0f;

    public float m_CamSpeed;
    public float m_VerticalPanBuffer = 0.5f; //distance from the center view before camera will pan vertically (Potentially split this into top and bottom buffer)

    private Camera m_Camera;
    private Vector3 m_CameraDistance;
    private float m_FixedCamYPosition;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_CameraDistance = (m_FollowTarget.position + m_CameraOffset) - m_Camera.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 lerpPos = Vector3.Lerp(m_Camera.transform.position, m_FollowTarget.position + m_CameraOffset, Time.deltaTime * m_CamSpeed);
        lerpPos.z = m_Camera.transform.position.z;

        if (!IsWithinRange(m_FollowTarget.position.y, m_Camera.transform.position.y - m_VerticalPanBuffer, m_Camera.transform.position.y + m_VerticalPanBuffer))
        {
            lerpPos.y = Mathf.Lerp(m_Camera.transform.position.y, m_FollowTarget.position.y + m_CameraOffset.y, Time.deltaTime * m_VerticalCamSpeed);
        }
        else
        {
            lerpPos.y = Mathf.Lerp(m_Camera.transform.position.y, m_FixedCamYPosition, Time.deltaTime * m_VerticalCamSpeed);
        }

        if ((Mathf.Abs(m_FollowTarget.transform.position.y - m_Camera.transform.position.y) - m_CameraOffset.y) <= 0.001f)
        {
            Debug.Log("Setting Fixed");
            m_FixedCamYPosition = m_FollowTarget.position.y + m_CameraOffset.y;
            //lerpPos.y = m_Camera.transform.position.y;
        }

        m_Camera.transform.position = lerpPos;
    }

    private bool IsWithinRange(float value, float min, float max)
    {
        bool check = false;

        if (value > min && value < max)
        {
            check = true;
        }

        return check;
    }

    private bool IsAlmostEqual(float a, float b, float threshold)
    {
        bool check = Mathf.Abs(a - b) <= threshold;
        return check;
    }
}
