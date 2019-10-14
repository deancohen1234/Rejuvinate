using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefaultPatrolState : BaseState
{
    public float m_PatrolSpeed;

    public override void OnEnter(GameObject enemy)
    {
        Debug.Log("Overriding Enemy Enter...");
    }

    public override void OnUpdate(GameObject enemy)
    {
        Debug.Log("Overriding <color=yellow>Patrol </color>Enemy Update...");

        enemy.transform.position += new Vector3(Mathf.Sin(m_PatrolSpeed * Time.time), 0, 0) * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_SwapStateCallback != null)
            {
                m_SwapStateCallback.DynamicInvoke("Engage");
            }
        }
    }
}
