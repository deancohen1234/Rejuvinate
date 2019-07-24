using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

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
    public float m_MagnitudeThreshold = 10.0f;
    [Range(0.0f, 1.0f)]
    public float m_StickOuterRimRadius = 0.5f;
    public bool m_UseAlternateControlScheme = false;

    [Header("Collider Properties")]
    public LayerMask m_GroundLayerMask;
    public LayerMask m_WallLayerMask;
    public Vector2 m_BottomColliderOffset;
    public Vector2 m_RightColliderOffset;
    public Vector2 m_LeftColliderOffset;
    public float m_SphereRadius = 0.1f;

    private Rigidbody2D m_Rigidbody;
    private PlayerEssenceController m_EssenceController;
    private CameraShake m_CameraShake;

    private float m_WallJumpMomentum;

    private bool m_hasDoubleJump;
    private bool m_IsGrounded;
    private bool m_IsOnWall;
    private bool m_IsWallJumping;
    private bool m_CanMove = true;
    private bool m_IsExpelled;
    private bool m_IsLeaf;

    private bool m_OnRightWall;

    private Vector2 m_StoredGustDirection; //needed to measure delta
    private bool m_HasChargedGust = false;
    private bool m_TriggerPressed = false;

    //vibration public variables
    private bool playerIndexSet = false;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_EssenceController = GetComponent<PlayerEssenceController>();
        m_CameraShake = FindObjectOfType<CameraShake>();

        m_EssenceController.m_OnPlayerDeath += OnPlayerDeath;
    }

    // Update is called once per frame
    void Update()
    {
        //floaty leaf form
        if (Input.GetAxis("RTrigger") >= 0.8f)
        {
            Physics2D.gravity = new Vector2(0.0f, -0.05f);
            m_Rigidbody.drag = .75f;
            m_IsLeaf = true;
        }
        else
        {
            Physics2D.gravity = new Vector2(0.0f, -9.81f);
            m_Rigidbody.drag = 0f;
            m_IsLeaf = false;
        }
        CheckForWindGust();


        if (m_IsLeaf) { return; }

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

    private void ManageVibration()
    {
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        prevState = state;
        state = GamePad.GetState(playerIndex);
    }

    private void CheckForWindGust()
    {
        float rx = Input.GetAxis("RStickHorizontal");
        float ry = Input.GetAxis("RStickVertical");
        
        Vector2 direction = new Vector2(rx, ry);
        /*
        if (direction.sqrMagnitude <= 0.1) { return; }

        Vector2 directionDelta =  (direction * 10f) - (m_StoredGustDirection * 10.0f); //* 10 to make numbers sqr able

        //dot bewteen direction vector and direction to center vector
        //1 -> 0 = pointing in direction of center
        //0 -> -1 = pointing in opposite direction of center
        float dot = Vector2.Dot(directionDelta.normalized, Vector2.zero - direction.normalized);

        Debug.Log("Dot: " + dot);
        //IsStickOnOuterRim(m_StoredGustDirection)
        if (dot >= .4f && directionDelta.sqrMagnitude >= Mathf.Pow(m_MagnitudeThreshold, 2.0f))
        {
            ActivateWindGust(new Vector2(Vector2.zero.x - directionDelta.normalized.x, Vector2.zero.y - directionDelta.normalized.y)); //world x direction and input direction are flipped thats why its negative
        }

        m_StoredGustDirection = direction;*/

        if (m_UseAlternateControlScheme == false)
        {
            if (IsStickOnOuterRim(direction))
            {
                m_HasChargedGust = true;
                m_StoredGustDirection = direction;
            }
            else
            {
                if (m_HasChargedGust)
                {
                    //use gust
                    ActivateWindGust(new Vector2(-m_StoredGustDirection.normalized.x, m_StoredGustDirection.normalized.y));
                    m_HasChargedGust = false;
                }
            }
        }

        else
        {
            if (IsStickOnOuterRim(direction))
            {
                m_HasChargedGust = true;
                m_StoredGustDirection = direction;
                if (Input.GetAxis("LTrigger") >= .8f && !m_TriggerPressed)
                {
                    ActivateWindGust(new Vector2(-m_StoredGustDirection.normalized.x, m_StoredGustDirection.normalized.y));
                    m_TriggerPressed = true;
                }
                else if (Input.GetAxis("LTrigger") <= 0.1f)
                {
                    m_TriggerPressed = false;
                }
            }
            
        }


    }

    public bool IsStickOnOuterRim(Vector2 stickPosition)
    {
        float rimXDist = Mathf.Abs((stickPosition.normalized * m_StickOuterRimRadius).x);
        float rimYDist = Mathf.Abs((stickPosition.normalized * m_StickOuterRimRadius).y);

        if (rimXDist < Mathf.Abs(stickPosition.x) && rimYDist < Mathf.Abs(stickPosition.y))
        {
            //stick is in outer radius
            return true;
        }
        else
        {
            return false;
        }

    }

    private void ActivateWindGust(Vector2 direction)
    {
        StartCoroutine(DisableMovement(2.0f));
        //StartCoroutine(LowerGravity(2f));

        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.velocity = (direction * m_WindPower);

        m_EssenceController.UseEssence();
        m_CameraShake.AddTrauma(0.2f);

        StartCoroutine(ActivateVibration(0.25f, .5f));

        Debug.Log("Direction: " + direction);
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

    private IEnumerator LowerGravity(float time)
    {
        Physics2D.gravity = new Vector2(0.0f, -0.1f);
        yield return new WaitForSeconds(time);
        Physics2D.gravity = new Vector2(0.0f, -9.81f);
    }

    //activate vibration for seconds
    private IEnumerator ActivateVibration(float length, float strength)
    {
        GamePad.SetVibration(playerIndex, strength, strength);

        yield return new WaitForSeconds(length);

        GamePad.SetVibration(playerIndex, 0f, 0f);
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

    private void OnPlayerDeath()
    {
        m_Rigidbody.velocity = Vector2.zero;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 90), "IsGrounded: " + m_IsGrounded + "\nIsOnWall: " + m_IsOnWall + "\nIsWallJumping: " + m_IsWallJumping);
    }

    private void OnDestroy()
    {
        //ensure vibration stops
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }
}
