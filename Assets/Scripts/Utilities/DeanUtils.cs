using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeanUtils
{
    public static float Map(float value, float startMin, float startMax, float endMin, float endMax)
    {
        float diff = (value - startMin) / (startMax - startMin);

        float newValue = (endMin * (1 - diff)) + (endMax * diff);

        return newValue;
    }

    public static float AngleOverAxis(Vector3 from, Vector3 to, Vector3 axis)
    {
        //angle in radians
        float angle = Mathf.Acos(Vector3.Dot(from, to));
        Vector3 rv = Vector3.Cross(from, to).normalized * angle; // todo: zero cross?
        return Vector3.Dot(axis, rv);
    }

    public static bool IsAlmostEqual(float a, float b, float threshold)
    {
        bool check = Mathf.Abs(a - b) <= threshold;
        return check;
    }
}

