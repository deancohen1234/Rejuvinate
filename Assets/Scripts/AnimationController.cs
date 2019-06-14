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
    }
}
