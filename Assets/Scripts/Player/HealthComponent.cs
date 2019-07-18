using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float m_MaxHealth = 100f;
    public Action m_OnPlayerDeath;

    private float m_CurrentHealth = 100f;
    private bool m_IsDead = false;
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
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
