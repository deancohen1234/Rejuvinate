using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEssenceController : MonoBehaviour
{
    public delegate void PlayerDied();
    public PlayerDied m_OnPlayerDeath;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            //kill player
            m_OnPlayerDeath(); //also could be invoke
        }
    }
}
