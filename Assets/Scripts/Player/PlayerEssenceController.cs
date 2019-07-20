using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEssenceController : MonoBehaviour
{
    public float m_StandardEssenceUseAmount = 25.0f;
    public float m_HealthLerpSpeed = 10.0f; //speed at which health lerps down/up

    public delegate void PlayerDied();
    public PlayerDied m_OnPlayerDeath;

    public Slider m_EssenceSlider;

    private HealthComponent m_HealthComponent;

    private bool m_IsLerpingHealthUI;


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

        m_IsLerpingHealthUI = true;

        float normalizedValue = DeanUtils.Map(m_HealthComponent.GetCurrentHealth(), 0, m_HealthComponent.m_MaxHealth, 0.0f, 1.0f);
        //m_EssenceSlider.value = normalizedValue;
    }

    private void Update()
    {
        if (m_IsLerpingHealthUI)
        {
            float mappedHealthValue = DeanUtils.Map(m_HealthComponent.GetCurrentHealth(), 0, m_HealthComponent.m_MaxHealth, 0.0f, 1.0f);

            float lerpVal = Mathf.Lerp(m_EssenceSlider.value, mappedHealthValue, Time.deltaTime * m_HealthLerpSpeed);
            m_EssenceSlider.value = lerpVal;

            if (DeanUtils.IsAlmostEqual(lerpVal, m_HealthComponent.GetCurrentHealth(), 0.01f))
            {
                m_IsLerpingHealthUI = false;
                m_EssenceSlider.value = mappedHealthValue;
            }
        }
    }
}

