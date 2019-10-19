using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FollowPathPatrol : BaseState
{
    public Transform[] m_PathPoints;
    public float m_PatrolSpeed;

    public int m_NextPointIndex = 0; //array index of next path point

    public override void OnEnter(GameObject enemy)
    {
        Debug.Log("Overriding Enemy Enter...");
    }

    public override void OnUpdate(GameObject enemy)
    {
        Debug.Log("Overriding <color=yellow>Follow Path </color>Enemy Update...");

        if (m_PathPoints.Length == 0)
        {
            Debug.LogError("No Path Points for Path Patrol");
            return;
        }

        //get direction and move enemy towards destination
        Vector3 direction = m_PathPoints[m_NextPointIndex].position - enemy.transform.position;
        Vector3 newPosition = enemy.transform.position + (direction.normalized * m_PatrolSpeed * Time.deltaTime);

        enemy.transform.position = newPosition;

        if (Vector3.Distance(enemy.transform.position, m_PathPoints[m_NextPointIndex].position) < 0.1f)
        {
            m_NextPointIndex = (int)Mathf.Repeat(m_NextPointIndex + 1, m_PathPoints.Length); //repeat is exclusive
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_SwapStateCallback != null)
            {
                m_SwapStateCallback.DynamicInvoke("Engage");
            }
        }
    }
}
