using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Switch lerp to consistent speed and not use position as first param
//TODO Make exit functionality for vein 

public class WaterVein : MonoBehaviour
{
    public Transform[] m_Points;
    public float m_LerpSpeed;

    private BezierCurve m_Curve;
    private Vector3 cachedPoint; //used to save memory rather than instantiating new vector3 each frame

    private Transform m_TransportingObject; //object being moved
    private int m_VeinLocationIndex = 0;
    private bool m_IsObjectInVein;

    private void Start()
    {
        m_Curve = new BezierCurve(m_Points[0].localPosition, m_Points[1].localPosition, m_Points[2].localPosition, m_Points[3].localPosition);
    }

    private void Update()
    {
        //move object through vein
        if (m_IsObjectInVein)
        {
            //get next destination position
            Debug.Log("Index: " + m_VeinLocationIndex);
            //lerp to position
            Vector3 newPos = Vector3.Lerp(m_TransportingObject.position, (Vector3)m_Curve.GetPoint(m_VeinLocationIndex, transform), Time.deltaTime * m_LerpSpeed);
            m_TransportingObject.position = newPos;

            //check distance every other frame
            if (Time.frameCount % 2 == 0)
            {
                //Debug.Log("Distance: " + Vector3.Distance(m_TransportingObject.position, (Vector3)m_Curve.GetPoint(m_VeinLocationIndex) + transform.position));
                if (Vector3.Distance(m_TransportingObject.position, (Vector3)m_Curve.GetPoint(m_VeinLocationIndex, transform)) <= 0.1f)
                {
                    m_VeinLocationIndex++;
                    if (m_VeinLocationIndex >= m_Curve.GetPointArrayLength())
                    {
                        m_IsObjectInVein = false;
                        m_TransportingObject = null;
                        return;
                    }
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

        Debug.Log("Enterin Vein");
        
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
