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
    public float m_TopBounds = 0.5f; //distance from the center view before camera will pan vertically (Potentially split this into top and bottom buffer)
    public float m_BottomBounds = 0.5f;
    public float m_VerticalCamSpeedCap = 1.0f;

    private Camera m_Camera;
    private Vector3 m_CameraDistance;
    private float m_FixedCamYPosition;

    private bool m_JustEnteredBounds = false;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_CameraDistance = (m_FollowTarget.position + m_CameraOffset) - m_Camera.transform.position;

        m_FixedCamYPosition = m_FollowTarget.position.y + m_CameraOffset.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 lerpPos = Vector3.Lerp(m_Camera.transform.position, m_FollowTarget.position + m_CameraOffset, Time.deltaTime * m_CamSpeed);
        lerpPos.z = m_Camera.transform.position.z;
        
        if (!IsWithinRange(m_FollowTarget.position.y, m_Camera.transform.position.y - m_BottomBounds, m_Camera.transform.position.y + m_TopBounds))
        {
            Vector3 dir = m_FollowTarget.transform.position + m_CameraOffset - m_Camera.transform.position;
            Vector3 moveVector = (dir.normalized * m_VerticalCamSpeed * Time.deltaTime) + m_Camera.transform.position;

            lerpPos.y = moveVector.y;
            m_JustEnteredBounds = false;
        }
        else
        {
            //Debug.Log("Just Entered Bounds: " + m_JustEnteredBounds + "\nDist: " + (Mathf.Abs(m_FollowTarget.transform.position.y - lerpPos.y) - m_CameraOffset.y));
            if (m_JustEnteredBounds == false)
            {
                Vector3 dir = m_FollowTarget.transform.position + m_CameraOffset - m_Camera.transform.position;
                Vector3 moveVector = (dir.normalized * m_VerticalCamSpeed * Time.deltaTime) + m_Camera.transform.position;

                lerpPos.y = moveVector.y;

                if (IsAlmostEqual(m_Camera.transform.position.y, (m_FollowTarget.position.y + m_CameraOffset.y), 0.1f))
                {
                    Debug.Log("Setting Fixed");
                    m_FixedCamYPosition = m_FollowTarget.transform.position.y + m_CameraOffset.y;
                    m_JustEnteredBounds = true;
                    //lerpPos.y = m_FixedCamYPosition;
                }
            }

            else
            {
                lerpPos.y = m_FixedCamYPosition;
            }
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Vector3 topPoint = transform.position + new Vector3(0, m_TopBounds, 0);
        Vector3 bottomPoint = transform.position + new Vector3(0, -m_BottomBounds, 0);

        Gizmos.DrawWireCube((topPoint + bottomPoint) / 2, new Vector3(5.0f, m_BottomBounds + m_TopBounds, 5.0f));
    }
}
