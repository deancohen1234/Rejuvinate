using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    public float m_Accuracy = 0.05f;

    private Vector2[] m_CurvePoints;

    public BezierCurve()
    {
        //default contrustor
    }

    public BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        CreateCurve(p0, p1, p2, p3);
    }

    public void CreateCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        if (m_Accuracy == 0)
        {
            Debug.LogError("Accuracy is Zero for Bezier Creation. Can't divide by 0");
            return;
        }
        //don't know how to properly deallocate memory for array so we just create a new array to destroy previous
        m_CurvePoints = new Vector2[Mathf.FloorToInt(1.0f / m_Accuracy)];

        for (float f = 0.0f; f <= 1.0f; f += m_Accuracy)
        {
            Vector2 point = CalculatePoint(f, p0, p1, p2, p3);
            int index = Mathf.FloorToInt(f * (float)m_CurvePoints.Length);

            m_CurvePoints[index] = point;
        }
    }

    public Vector2 GetPoint(int arrayIndex, Transform transformOffset)
    {
        Vector3 positionOffset = Vector3.zero;

        if (transformOffset != null)
        {
            positionOffset = transformOffset.position;
        }

        return (Vector3)m_CurvePoints[arrayIndex] + positionOffset;
    }

    public int GetPointArrayLength()
    {
        return m_CurvePoints.Length;
    }

    private Vector2 CalculatePoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Vector2 partOne = Mathf.Pow((1 - t), 3.0f) * p0;
        Vector2 partTwo = 3.0f * (Mathf.Pow((1 - t), 2.0f)) * t * p1;
        Vector2 partThree = 3.0f * (1 - t) * Mathf.Pow(t, 2.0f) * Mathf.Pow(t, 2.0f) * p2;
        Vector2 partFour = Mathf.Pow(t, 3.0f) * p3;

        Vector2 finalPoint = partOne + partTwo + partThree + partFour;

        return finalPoint;
    }
}
