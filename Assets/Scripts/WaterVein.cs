using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Switch lerp to consistent speed and not use position as first param
//TODO Make exit functionality for vein 

public class WaterVein : MonoBehaviour
{
    public Action m_VeinMovementCompleted;

    public Transform[] m_Points;
    public float m_LerpSpeed;

    private BezierCurve m_Curve;
    private Vector3 cachedPoint; //used to save memory rather than instantiating new vector3 each frame

    private Transform m_TransportingObject; //object being moved
    private int m_VeinLocationIndex = 0;
    private bool m_IsObjectInVein;

    private float m_LerpTime;

    private void Start()
    {
        m_Curve = new BezierCurve(m_Points[0].localPosition, m_Points[1].localPosition, m_Points[2].localPosition, m_Points[3].localPosition);

        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        
        if (lineRenderer == null) { Debug.LogError("No Line Renderer attached to object!"); return; }

        lineRenderer.positionCount = m_Curve.GetPointArrayLength();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, m_Curve.GetPoint(i, transform));
        }
    }

    private void Update()
    {
        //move object through vein
        if (m_IsObjectInVein)
        {
            //lerp to position
            m_LerpTime += Time.deltaTime * m_LerpSpeed;

            Vector3 newPos = Vector3.Lerp((Vector3)m_Curve.GetPoint(m_VeinLocationIndex, transform), (Vector3)m_Curve.GetPoint(m_VeinLocationIndex + 1, transform), m_LerpTime);
            m_TransportingObject.position = newPos;

            //lerp has finished
            if (m_LerpTime >= 1.0f)
            {
                m_VeinLocationIndex++;
                m_LerpTime = 0.0f;

                //object at end of vein
                if (m_VeinLocationIndex + 1 >= m_Curve.GetPointArrayLength())
                {
                    m_IsObjectInVein = false;
                    m_TransportingObject = null;

                    if (m_VeinMovementCompleted != null)
                    {
                        m_VeinLocationIndex = 0;
                        m_VeinMovementCompleted.Invoke();
                    }

                    return;
                }
            }
            
        }
    }
    //called from other object's trigger enter
    //sends in Transform that is moved through vein
    public void EnterVein(Transform t)
    {
        m_TransportingObject = t;
        m_IsObjectInVein = true;

        Debug.Log("Entering Vein");
        
    }

    public BezierCurve GetCurve()
    {
        return m_Curve;
    }

    private void OnDrawGizmos()
    {
        if (m_Curve == null)
        {
            //m_Curve = new BezierCurve();
        }
        else
        {
            if (m_Points.Length == 4)
            {
                Vector2 p0 = new Vector2(m_Points[0].localPosition.x, m_Points[0].localPosition.y);
                Vector2 p1 = new Vector2(m_Points[1].localPosition.x, m_Points[1].localPosition.y);
                Vector2 p2 = new Vector2(m_Points[2].localPosition.x, m_Points[2].localPosition.y);
                Vector2 p3 = new Vector2(m_Points[3].localPosition.x, m_Points[3].localPosition.y);

                m_Curve.CreateCurve(p0, p1, p2, p3);

                for (int i = 0; i < m_Curve.GetPointArrayLength(); i++)
                {
                    Vector2 point = m_Curve.GetPoint(i, transform);

                    cachedPoint = new Vector3(point.x, point.y, 0);
                    Gizmos.DrawSphere(cachedPoint + transform.position, 0.2f);
                }
            }
        }
    }
}
