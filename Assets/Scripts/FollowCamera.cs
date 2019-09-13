using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    public Transform m_FollowTarget;
    public Vector3 m_CameraOffset; //offset to position camera at a more comfortable angle

    public float m_VerticalCamSpeed = 1.0f;
    public float m_HorizonatalCamSpeed = 1.0f;

    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float newFollowX = Mathf.SmoothStep(m_Camera.transform.position.x, m_FollowTarget.position.x + m_CameraOffset.x, Time.deltaTime * m_HorizonatalCamSpeed);
        float newFollowY = Mathf.SmoothStep(m_Camera.transform.position.y, m_FollowTarget.position.y + m_CameraOffset.y, Time.deltaTime * m_VerticalCamSpeed);

        m_Camera.transform.position = new Vector3(newFollowX, newFollowY, m_Camera.transform.position.x);
    }

}
