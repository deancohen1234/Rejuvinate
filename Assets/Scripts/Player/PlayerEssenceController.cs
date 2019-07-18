using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEssenceController : MonoBehaviour
{
    public float m_StandardEssenceUseAmount = 25.0f;

    public delegate void PlayerDied();
    public PlayerDied m_OnPlayerDeath;

    public Slider m_EssenceSlider;

    private HealthComponent m_HealthComponent;

    private void Start()
    {
        m_HealthComponent = GetComponent<HealthComponent>();

        //on player death, invoke essence controller's event
        m_HealthComponent.m_OnPlayerDeath += delegate { m_OnPlayerDeath(); };
    }

    public void UseEssence()
    {
        m_HealthComponent.DealDamage(m_StandardEssenceUseAmount);
        UpdateEssenceUI();
    }

    public void RestoreAllEssence()
    {
        m_HealthComponent.Revive();
        UpdateEssenceUI();
    }

    private void UpdateEssenceUI()
    {
        if (m_EssenceSlider == null) { return; }

        float normalizedValue = DeanUtils.Map(m_HealthComponent.GetCurrentHealth(), 0, m_HealthComponent.m_MaxHealth, 0.0f, 1.0f);
        m_EssenceSlider.value = normalizedValue;
    }
}

