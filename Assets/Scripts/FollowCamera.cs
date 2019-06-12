using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    public Transform m_FollowTarget;
    public Vector3 m_CameraOffset; //offset to position camera at a more comfortable angle

    public float m_CamSpeed;

    private Camera m_Camera;
    private Vector3 m_CameraDistance;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_CameraDistance = m_FollowTarget.position - m_Camera.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 lerpPos = Vector3.Lerp(m_Camera.transform.position, m_FollowTarget.position + m_CameraOffset, Time.deltaTime * m_CamSpeed);
        lerpPos.z = m_Camera.transform.position.z;

        m_Camera.transform.position = lerpPos;
    }
}
