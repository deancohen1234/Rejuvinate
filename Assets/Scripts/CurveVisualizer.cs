using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveVisualizer : MonoBehaviour
{
    public Transform[] m_Points;
    public bool m_IsEnabled;

    private BezierCurve m_Curve;
    private Vector3 cachedPoint; //used to save memory rather than instantiating new vector3 each frame
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (m_IsEnabled == false) { return; }

        if (m_Points.Length == 4)
        {
            Vector2 p0 = new Vector2(m_Points[0].localPosition.x, m_Points[0].localPosition.y);
            Vector2 p1 = new Vector2(m_Points[1].localPosition.x, m_Points[1].localPosition.y);
            Vector2 p2 = new Vector2(m_Points[2].localPosition.x, m_Points[2].localPosition.y);
            Vector2 p3 = new Vector2(m_Points[3].localPosition.x, m_Points[3].localPosition.y);

            m_Curve = new BezierCurve(p0, p1, p2, p3);

            for (int i = 0; i < m_Curve.GetPointArrayLength(); i++)
            {
                Vector2 point = m_Curve.GetPoint(i, transform);

                cachedPoint = new Vector3(point.x, point.y, 0);
                Gizmos.DrawSphere(cachedPoint, 0.2f);
            }
        }
        
    }
}
