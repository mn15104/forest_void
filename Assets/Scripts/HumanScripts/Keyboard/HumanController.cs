
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;


//Enum with *layer number* as description - used to pass to HumanAudioController.
public enum CurrentGroundCollision
{
    [Description("11")]
    TERRAIN,
    [Description("3")]
    WOOD,
    [Description("0")]
    AIR
}
//Helper get-function for ground collision value 
public static class TerrainLayerNumber
{
    public static int Value(CurrentGroundCollision play)
    {
        switch (play)
        {
            case CurrentGroundCollision.TERRAIN:
                return 8;
            case CurrentGroundCollision.WOOD:
                return 9;
            case CurrentGroundCollision.AIR:
                return 0;
            default:
                return 0;
        }
    }
}

//Enum with current state with *speed* as description - used to determine velocity.
public enum PlayerMoveState
{
    [Description("1.5f")]
    WALKING,
    [Description("2.25f")]
    RUNNING,
    [Description("1.0f")]
    CROUCHING,
    [Description("1.0f")]
    JUMPING,
    [Description("0.0f")]
    CLIMBING
}
//Helper get-function for speed value 
public static class MoveToFloat
{
    public static float Value(PlayerMoveState play)
    {
        switch (play)
        {
            case PlayerMoveState.WALKING:
                return 1.5f;
            case PlayerMoveState.RUNNING:
                return 2.25f;
            case PlayerMoveState.CROUCHING:
                return 1.0f;
            case PlayerMoveState.JUMPING:
                return 0.0f;
            default:
                return 0f;
        }
    }
}
[RequireComponent(typeof(Rigidbody))]

public class HumanController : MonoBehaviour
{
    //Components
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private float m_CapsuleHeight;
    private Vector3 m_CapsuleCenter;
    private CapsuleCollider m_Capsule;
    private OVRCameraRig cameraRig;

    //Animator Variables
    public float m_RunCycleLegOffset = 0.2f;
    public float m_AnimSpeedMultiplier = 1f;

    // Max speed & speed multiplier
    public const float m_WalkSpeedMultiplier = 1.25f;
    public const float m_MoveSpeedMultiplier = 2.75f;
    private float m_Speed;

    // Max rotate & rotate multiplier 
    private float m_rotateSpeed = 2f;
    [Range(-1, 1)]
    private float maxRotateUp = 0.7f, maxRotateDown = -0.7f;
    private float m_TurnSpeedLerpMax = 180;
    private float m_TurnSpeedLerpMin = 180;
    private float m_TurnSpeed = 0.3f;
    private float m_TurnAmount;

    // Movement State Variables
    public Terrain m_CurrentTerrain;
    private PlayerMoveState m_playerMoveState;
    private CurrentGroundCollision m_CurrentGroundCollision;
    private GameObject m_CurrentGroundCollider;
    private bool m_GetBackUp = false;
    private bool m_FallingOver = false;
    private bool m_Ladder = false;
    private GameObject ladder;
    private Quaternion OriginalRotation;

    void Start()
    {
        m_playerMoveState = PlayerMoveState.RUNNING;
        m_CurrentGroundCollision = CurrentGroundCollision.AIR;
        OriginalRotation = transform.rotation;
        m_Animator = GetComponentInChildren<Animator>();
        cameraRig = GetComponentInChildren<OVRCameraRig>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
    }

    private void FixedUpdate()
    {
        //Get horizontal and vertical movement
        Transform m_trans = GetComponent<Transform>();
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        float upMove = Input.GetAxis("Mouse X");
        float downMove = Input.GetAxis("Mouse Y");

        //Create movement & rotation vectors
        Vector3 movement = verticalMove * transform.forward + horizontalMove * transform.right;
        Vector3 rotation = new Vector3(upMove, downMove, 0);

        //If jumping, require raycast detection for ground collision event
        if (m_playerMoveState == PlayerMoveState.JUMPING)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + new Vector3(0,1,0), transform.TransformDirection(Vector3.down), out hitInfo, 2f))
            {
                m_Animator.SetBool("Jumping", false);
                m_playerMoveState = PlayerMoveState.WALKING;
               
            }
        }
  
        
        Move(movement, rotation);
    }

    public void FallOver()
    {
        m_Rigidbody.constraints = RigidbodyConstraints.None; 
        m_Animator.SetBool("Fallover", true);
        m_Rigidbody.AddForceAtPosition(transform.TransformDirection(Vector3.back) * 20, transform.position + new Vector3(0,0.5f,0));
        m_Rigidbody.AddForceAtPosition(transform.TransformDirection(Vector3.forward) * 20, transform.position - new Vector3(0, 0.5f, 0));
        m_FallingOver = true;
    }

    public void GetBackUp()
    {
        m_Animator.SetBool("Fallover", false);
        m_FallingOver = false;
        m_GetBackUp = true;
    }

    public void JumpForce()
    {
        m_Rigidbody.AddForce(0, 2000, 0);
    }

    private void Update()
    {
        if (!m_FallingOver && !m_GetBackUp)
        {
            CheckJumpState();
            CheckClimbState();
            CheckCrouchState();
            CheckRunState();
        }
    }

    void CheckJumpState()
    {
        if (Input.GetKey(KeyCode.Space) && 
            m_playerMoveState != PlayerMoveState.JUMPING && 
            m_playerMoveState != PlayerMoveState.CLIMBING)
        {
            m_Animator.SetBool("Crouch", false);
            m_Animator.SetBool("Jumping", true);
            m_playerMoveState = PlayerMoveState.JUMPING;
        }
    }

    void CheckRunState()
    {
        if (m_playerMoveState != PlayerMoveState.CROUCHING && 
            m_playerMoveState != PlayerMoveState.JUMPING &&
            m_playerMoveState != PlayerMoveState.CLIMBING)
        {
            if (Input.GetKey(KeyCode.R))
            {
                if (m_playerMoveState != PlayerMoveState.RUNNING)
                {
                    m_Animator.SetBool("Running", true);
                    m_playerMoveState = PlayerMoveState.RUNNING;
                }
            }
            else 
            {
                m_Animator.SetBool("Running", false);
                m_playerMoveState = PlayerMoveState.WALKING;
            }
        }
    }

    void CheckClimbState()
    {
        if (m_Ladder && m_playerMoveState != PlayerMoveState.CLIMBING) 
        {
            if (Input.GetKey(KeyCode.W) && Vector3.Dot(transform.forward, (ladder.transform.position - transform.position).normalized) > 0)
            {
                OriginalRotation = transform.rotation;
                m_Rigidbody.useGravity = false;
                m_playerMoveState = PlayerMoveState.CLIMBING;
                m_Animator.SetBool("Climbing", true);
                m_Animator.SetBool("Running", false);
                var lookPos = ladder.transform.position - transform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = rotation;
            }
        }
        if (m_playerMoveState == PlayerMoveState.CLIMBING && !m_Ladder)
        {
            m_Rigidbody.useGravity = true;
            m_playerMoveState = PlayerMoveState.RUNNING;
            m_Animator.SetBool("Climbing", false);
            transform.rotation = OriginalRotation;
        }
    }

    void CheckCrouchState()
    {
        if ((m_playerMoveState != PlayerMoveState.JUMPING &&
             m_playerMoveState != PlayerMoveState.CLIMBING))
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
                        m_playerMoveState != PlayerMoveState.CROUCHING)
            {
                m_Animator.SetBool("Running", false);
                m_Animator.SetBool("Crouch", true);
                m_playerMoveState = PlayerMoveState.CROUCHING;
            }
            else if (m_playerMoveState == PlayerMoveState.CROUCHING &&
                    !Input.GetKey(KeyCode.LeftControl))
            {
                m_Animator.SetBool("Crouch", false);
                m_playerMoveState = PlayerMoveState.WALKING;
            }
        }
    }

    IEnumerator DelayGetBackUp()
    {
        yield return new WaitForSeconds(1.4f);
        m_GetBackUp = false;
    }




    public void Move(Vector3 move, Vector3 rotation)
    {
        //Get original velocities
        float x_frame = m_Rigidbody.velocity.x;
        float y_frame = m_Rigidbody.velocity.y;
        float z_frame = m_Rigidbody.velocity.z;

        if (m_FallingOver)
        {
            // Do nothing
        }
        else if (m_GetBackUp)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                  OriginalRotation,
                                                  350 * Time.deltaTime);
            if(transform.rotation == OriginalRotation)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                StartCoroutine(DelayGetBackUp());
            }
        }
        // Enables up & down climbing movement along y axis
        else if (m_playerMoveState == PlayerMoveState.CLIMBING && m_Ladder)
        {
            {
                m_Rigidbody.velocity = new Vector3(0, 0, 0);
                Vector3 up = ladder.transform.up;
                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate(new Vector3(0, 0.025f, 0), ladder.transform);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate(new Vector3(0, -0.025f, 0), ladder.transform);
                }
                //Camera Rotate
                if (rotation.y > 0 && cameraRig.transform.forward.y < maxRotateUp ||
                    rotation.y < 0 && cameraRig.transform.forward.y > maxRotateDown)
                {
                    cameraRig.transform.Rotate(Vector3.right, -1 * rotation.y * m_rotateSpeed);
                }
            }
        }
        // Enables walking, running, or crouch-walking
        else if (m_playerMoveState != PlayerMoveState.JUMPING)
        {
            //Rotate
            m_TurnAmount = rotation.x;
            float turnSpeed = m_TurnSpeed * Mathf.Lerp(m_TurnSpeedLerpMin, m_TurnSpeedLerpMax, 0.001f);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
            //Camera Rotate
            if (rotation.y > 0 && cameraRig.transform.forward.y < maxRotateUp || 
                rotation.y < 0 && cameraRig.transform.forward.y > maxRotateDown)
            {
                cameraRig.transform.Rotate(Vector3.right, -1 * rotation.y * m_rotateSpeed);
            }
            //Move
            float moveSpeedMultiplier = MoveToFloat.Value(m_playerMoveState);
            Vector3 vel = move * moveSpeedMultiplier;
            vel.y = y_frame;
            m_Rigidbody.velocity = vel;
            Vector2 xz = new Vector2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z);
            m_Speed = xz.magnitude;
            move = vel;
        }
        UpdateAnimator(move);
    }

    
    void UpdateAnimator(Vector3 move)
    {
        if (m_playerMoveState == PlayerMoveState.CROUCHING)
        {
            float moveSpeed = MoveToFloat.Value(m_playerMoveState) / MoveToFloat.Value(PlayerMoveState.RUNNING);
            m_Animator.speed = move.magnitude > 0 ? moveSpeed : 1;
        }
        else if (m_playerMoveState == PlayerMoveState.JUMPING)
        {
            m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
        }
        else if(m_playerMoveState == PlayerMoveState.CLIMBING)
        {
            // LEAVE THIS HERE until an animation is put in
        }
        else
        {
            m_Animator.SetFloat("Speed", m_Speed, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);

            float runCycle =
                Mathf.Repeat(
                    m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < 0.5f ? 1 : -1) * m_Speed * 0.8f;

            m_Animator.SetFloat("JumpLeg", jumpLeg);

            float moveSpeed = MoveToFloat.Value(m_playerMoveState) / MoveToFloat.Value(PlayerMoveState.RUNNING);
            m_Animator.speed = move.magnitude > 0 ? moveSpeed : 1;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Detect if current terrain has changed
        if (collision.gameObject.GetComponent<Terrain>())
        {
            if(collision.gameObject.GetComponent<Terrain>() != m_CurrentTerrain)
            {
                m_CurrentTerrain = collision.gameObject.GetComponent<Terrain>();
            }
        }   

    }

    private void OnTriggerExit(Collider collider)
    {
        //Detect ladder
        if (m_Ladder && collider.gameObject.tag == "Ladder")
        {
            m_Ladder = false;
            ladder = null;
        }
        //Detect if airborne
        if (collider.gameObject == m_CurrentGroundCollider)
        {
            m_CurrentGroundCollider = null;
            m_CurrentGroundCollision = CurrentGroundCollision.AIR;
        }

    }
    private void OnTriggerStay(Collider collider)
    {
        //Detect proximity of ladder
        if(collider.gameObject.tag == "Ladder" && !m_Ladder)
        {
            m_Ladder = true;
            ladder = collider.gameObject;
        }
        if (collider.gameObject.layer == 9 && m_CurrentGroundCollision != CurrentGroundCollision.WOOD)
        {
            m_CurrentGroundCollision = CurrentGroundCollision.WOOD;
            m_CurrentGroundCollider = collider.gameObject;
        }
        else if (collider.gameObject.layer == 8 && m_CurrentGroundCollision != CurrentGroundCollision.TERRAIN)
        {
            m_CurrentGroundCollision = CurrentGroundCollision.TERRAIN;
            m_CurrentGroundCollider = collider.gameObject;
        }
    }

    public PlayerMoveState GetPlayerMoveState()
    {
        return m_playerMoveState;
    }
    public CurrentGroundCollision GetCurrentGroundCollision()
    {
        return m_CurrentGroundCollision;
    }
    public void SetCurrentTerrain(Terrain terrain)
    {
        m_CurrentTerrain = terrain;
    }
    public Terrain GetCurrentTerrain()
    {
         return m_CurrentTerrain; 
    }
}