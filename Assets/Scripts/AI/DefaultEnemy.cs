using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//contains enemy gameobject and uses its gameObject to send to states and move/interact for enemy
public class DefaultEnemy : MonoBehaviour
{
    public Transform m_Player;

    public FollowPathPatrol m_PatrolState;
    public DefaultEngageState m_EngageState;
    public DefaultDisengageState m_DisengageState;

    private BaseState m_CurrentState;

    private Action<string> m_SwapStateCallback;

    private void OnEnable()
    {
        m_SwapStateCallback += SwapState;
    }

    private void OnDisable()
    {
        m_SwapStateCallback -= SwapState;
    }

    private void Start()
    {
        m_CurrentState = m_PatrolState;

        m_PatrolState.Initialize(m_SwapStateCallback);
        m_EngageState.Initialize(m_SwapStateCallback);
        m_DisengageState.Initialize(m_SwapStateCallback);
    }

    //called from AIController
    private void Update()
    {
        m_CurrentState.OnUpdate(gameObject);
    }

    //called from callbacks
    private void SwapState(string stateName)
    {
        m_CurrentState.OnExit(gameObject);

        BaseState newState = GetState(stateName);

        m_CurrentState = newState;
        m_CurrentState.OnEnter(gameObject);
    }

    private BaseState GetState(string name)
    {
        if (m_PatrolState.m_Name == name)
        {
            return m_PatrolState;
        }

        else if (m_EngageState.m_Name == name)
        {
            return m_EngageState;
        }
        
        else if (m_DisengageState.m_Name == name)
        {
            return m_DisengageState;
        }
        
        else
        {
            Debug.LogError("Get State Name: " + name + " is not the same as any states");
            return null;
        }
    }
}
