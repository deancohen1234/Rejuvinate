using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Move Properties")]
    public float m_MoveSpeed = 5.0f;
    public float m_JumpAmount = 10.0f;
    public float m_FallMultiplier = 2.5f;
    public float m_LowJumpMultiplier = 2.0f;

    [Header("Wall Jump Properties")]
    public float m_WallSlideSpeed = 0.5f;
    public float m_WallJumpHorizonatlStrength = 3.0f;
    public float m_WallJumpVerticalStrength = 3.0f;
    public float m_WallJumpDrag = 0.95f; //x momentum drag when wall jumping
    public float m_WallJumpLerpTime = 1.0f;

    [Header("Wind Power")]
    public float m_WindPower = 10.0f;

    [Header("Collider Properties")]
    public LayerMask m_GroundLayerMask;
    public LayerMask m_WallLayerMask;
    public Vector2 m_BottomColliderOffset;
    public Vector2 m_RightColliderOffset;
    public Vector2 m_LeftColliderOffset;
    public float m_SphereRadius = 0.1f;

    private Rigidbody2D m_Rigidbody;

    private float m_WallJumpMomentum;

    private bool m_hasDoubleJump;
    private bool m_IsGrounded;
    private bool m_IsOnWall;
    private bool m_IsWallJumping;
    private bool m_CanMove = true;
    private bool m_IsExpelled;

    private bool m_OnRightWall;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateCollisions();
        CalculateFallingSpeed();

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x, y);

        if (m_CanMove)
        {
            Move(direction);
        }

        //Jumping
        if (Input.GetButtonDown("Fire1"))
        {
            if (m_IsGrounded || m_hasDoubleJump)
            {
                Jump();
            }
        }

        if (Input.GetButtonDown("Fire3"))
        {
            ActivateWindGust();
        }

        if (m_IsOnWall)
        {
            m_hasDoubleJump = true;
        }

        //Wall sliding
        if (m_IsOnWall && m_CanMove)
        {
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, -m_WallSlideSpeed);
        }

        //Wall jumping
        if (m_IsOnWall && Input.GetButtonDown("Fire1"))
        {
            WallJump();
        }

        if (m_IsGrounded)
        {
            m_IsWallJumping = false;
            m_hasDoubleJump = true;
            m_IsExpelled = false;
        }
    }

    private void ActivateWindGust()
    {
        StartCoroutine(DisableMovement(0.3f));

        Debug.Log("Activating Gust");

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x, y);

        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.velocity = (direction * m_WindPower);
    }

    private void Move(Vector2 moveDirection)
    {
        //caluclate player velocity after a wall jump
        if (m_IsWallJumping || m_IsExpelled)
        {
            //doesn't need to know if on right wall or left wall because momentum was multiplied by it earilier
            m_Rigidbody.velocity = Vector2.Lerp(m_Rigidbody.velocity, new Vector2(moveDirection.x * m_MoveSpeed, m_Rigidbody.velocity.y), Time.deltaTime * m_WallJumpLerpTime);
        }

        else
        {
            m_Rigidbody.velocity = new Vector2(moveDirection.x * m_MoveSpeed, m_Rigidbody.velocity.y);
        }
    }

    private void CalculateCollisions()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle((Vector2)transform.position + m_BottomColliderOffset, m_SphereRadius, m_GroundLayerMask);
        Collider2D leftWallCollider = Physics2D.OverlapCircle((Vector2)transform.position + m_LeftColliderOffset, m_SphereRadius, m_WallLayerMask);
        Collider2D rightWallCollider = Physics2D.OverlapCircle((Vector2)transform.position + m_RightColliderOffset, m_SphereRadius, m_WallLayerMask);

        if (groundCollider == null) { m_IsGrounded = false; }
        else { m_IsGrounded = true; }

        if (leftWallCollider != null)
        {
            m_IsOnWall = true;
            m_OnRightWall = false;
        }
        else if (rightWallCollider != null)
        {
            m_IsOnWall = true;
            m_OnRightWall = true;
        }
        else
        {
            m_IsOnWall = false;
        }
    }

    private void CalculateFallingSpeed()
    {
        //if falling
        if (m_Rigidbody.velocity.y < 0)
        {
            //- 1 to account for physics already applying 1 force of gravity
            m_Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (m_FallMultiplier - 1) * Time.deltaTime;
        }
        //if we are rising
        //needs to be done because once player lets go of jump button harder gravity needs to be applied
        else if (m_Rigidbody.velocity.y > 0 && !Input.GetButton("Fire1"))
        {
            m_Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (m_LowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Jump()
    {
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, m_JumpAmount);

        //player is already in air when jumping
        if (m_IsGrounded == false)
        {
            //use double jump
            m_hasDoubleJump = false;
        }
    }

    private void WallJump()
    {
        //if on right wall then make velocity negative so you go left
        int onRightWall = (m_OnRightWall) ? -1 : 1;
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x + (m_WallJumpHorizonatlStrength * onRightWall), m_Rigidbody.velocity.y + m_WallJumpVerticalStrength);

        StopCoroutine(DisableMovement(0.1f));
        StartCoroutine(DisableMovement(0.1f));

        m_IsWallJumping = true;
        m_IsOnWall = false;
    }

    private IEnumerator DisableMovement(float time)
    {
        m_CanMove = false;
        yield return new WaitForSeconds(time);
        m_CanMove = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere((Vector2)transform.position + m_BottomColliderOffset, m_SphereRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + m_RightColliderOffset, m_SphereRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + m_LeftColliderOffset, m_SphereRadius);
    }

    //used when player is expelled from vein, functions similarly to isWallJumping
    public void SetIsExpelled(bool b)
    {
        m_IsExpelled = b;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 90), "IsGrounded: " + m_IsGrounded + "\nIsOnWall: " + m_IsOnWall + "\nIsWallJumping: " + m_IsWallJumping);
    }
}
