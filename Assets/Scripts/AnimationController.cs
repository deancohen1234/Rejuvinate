using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AnimationController : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody;

    private bool m_IsFacingLeft;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput < 0f)
        {
            m_IsFacingLeft = true;
        }
        else if(horizontalInput > 0f)
        {
            m_IsFacingLeft = false;
        }

        m_Animator.SetBool("IsLeftMoving", m_IsFacingLeft);


        //check if idling
        if (IsAlmostEqual(horizontalInput, 0.0f, 0.01f))
        {
            m_Animator.SetBool("IsIdle", true);
        }
        else
        {
            m_Animator.SetBool("IsIdle", false);
        }
    }

    private bool IsAlmostEqual(float a, float b, float threshold)
    {
        bool check = Mathf.Abs(a - b) <= threshold;
        return check;
    }
}
