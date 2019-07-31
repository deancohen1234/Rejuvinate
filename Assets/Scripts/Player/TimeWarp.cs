using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//currently has extra calls for settimescale
//maybe work to add extra checks to stop this in the future
public class TimeWarp : MonoBehaviour
{
    public AnimationCurve m_TransitionCurve;
    public float m_TimeScalar = 5.0f;

    private float m_StartingTimeScale;

    private float m_PreviousTimeScale;
    private float m_DesiredTimeScale;

    private float m_Time;

    private bool m_IsWarping;

    private void Awake()
    {
        m_StartingTimeScale = Time.timeScale;
    }

    void Update()
    {
        if (m_IsWarping)
        {
            Debug.Log("IsWarping");
            float curveTime = m_TransitionCurve.Evaluate(m_Time);
            float warpTimeScale = DeanUtils.Map(curveTime, 0, 1, m_PreviousTimeScale, m_DesiredTimeScale);

            Time.timeScale = warpTimeScale;

            m_Time += Time.deltaTime * m_TimeScalar;

            if (m_Time >= 1.0f)
            {
                //warp complete
                m_Time = 0;
                m_IsWarping = false;
                Time.timeScale = m_DesiredTimeScale;
            }
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            SetTimeWarp(0.2f);
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            SetTimeWarp(1.0f);
        }
    }

    public void SetTimeWarp(float timeScale)
    {
        if (DeanUtils.IsAlmostEqual(timeScale, Time.timeScale, 0.0001f))
        {
            //don't set time warp if we are already there
            return;
        }

        m_Time = 0.0f;

        m_PreviousTimeScale = Time.timeScale;
        m_DesiredTimeScale = timeScale;
        m_IsWarping = true;
    }

    public bool IsWarping()
    {
        return m_IsWarping;
    }

    private void OnDestroy()
    {
        Time.timeScale = m_StartingTimeScale;
    }
}
