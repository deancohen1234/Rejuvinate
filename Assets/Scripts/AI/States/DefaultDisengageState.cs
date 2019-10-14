using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefaultDisengageState : BaseState
{
    public override void OnEnter(GameObject enemy)
    {
        Debug.Log("Overriding Disengage Enemy Enter...");
    }

    public override void OnUpdate(GameObject enemy)
    {
        Debug.Log("Overriding <color=blue>Patrol </color>Enemy Update...");

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_SwapStateCallback != null)
            {
                m_SwapStateCallback.DynamicInvoke("Patrol");
            }
        }
        
    }
}
