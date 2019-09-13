using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float m_MaxHealth = 100f;
    public float m_RespawnDelay = 1.5f; //in seconds, time until player respawns after death

    public ParticleSystem m_DeathSystem;

    public Action m_OnPlayerDeath;
    public Action m_OnPlayerRespawn; //time after player has died and animation/particles are finished

    private float m_CurrentHealth = 100f;
    private bool m_IsDead = false;

    private float m_TimeOfDeath;
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    private void Update()
    {
        if (m_IsDead)
        {
            //wait to respawn
            if (Time.time - m_TimeOfDeath >= m_RespawnDelay)
            {
                //revive player
                Revive();
            }
        }
    }

    public void DealDamage(float damage)
    {
        if (m_IsDead)
        {
            Debug.Log("Can't deal damage, character is dead");
            return;
        }

        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0)
        {
            //kill player
            m_IsDead = true;
            m_OnPlayerDeath.Invoke();

            m_DeathSystem.Emit(75);

            m_TimeOfDeath = Time.time;
        }
    }

    public void HealPlayer(float healAmount)
    {
        if (healAmount < 0.0f)
        {
            Debug.LogWarning("Cannot Heal a negative number: " + healAmount);
            return;
        }

        m_CurrentHealth += Mathf.Clamp(m_CurrentHealth + healAmount, 0, m_MaxHealth);
    }

    public void Revive()
    {
        m_IsDead = false;
        m_CurrentHealth = m_MaxHealth;

        if (m_OnPlayerRespawn != null)
        {
            m_OnPlayerRespawn.Invoke();
        }
    }

    public float GetCurrentHealth()
    {
        return m_CurrentHealth;
    }

    public bool IsDead()
    {
        return m_IsDead;
    }
}
