
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
//using UnityEditor;

[RequireComponent(typeof(Rigidbody))]

public class HumanVRController : MonoBehaviour
{

    public SixenseHand rhand = null;
    public SixenseHand lhand = null;
    private Vector3 m_Forward;
    private Vector3 m_Right;
    //Input
    float moveHorizontal;
    float moveVertical;
    float rotateHorizontal;
    float rotateVertical;
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
    public const float m_WalkSpeedMultiplier = 2f;
    public const float m_MoveSpeedMultiplier = 2.75f;
    private float m_Speed;

    // Max rotate & rotate multiplier 
    private float m_rotateSpeed = 1.5f;
    [Range(-1, 1)]
    private float maxRotateUp = 0.7f, maxRotateDown = -0.7f;
    private float m_TurnSpeedLerpMax = 180;
    private float m_TurnSpeedLerpMin = 180;
    private float m_TurnSpeed = 0.3f;
    private float m_TurnAmount;

    // Movement State Variables
    private Terrain m_CurrentTerrain;
    private PlayerMoveState m_playerMoveState;
    private Quaternion OriginalRotation;
    private float maxRunTime = 8f;
    private float timeRunning = 0f;


    private bool monsterAttacking = false;

    private void OnEnable()
    {
        FindObjectOfType<MonsterAI>().OnMonsterStateChange += ReactMonsterState;        
    }
    private void ReactMonsterState(MonsterState st)
    {
        if(st == MonsterState.ATTACK)
        {
            monsterAttacking = true;
        }
    }
    void Start()
    {
        m_CurrentTerrain = Terrain.activeTerrain;
        m_playerMoveState = PlayerMoveState.WALKING;
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
        if (!monsterAttacking)
        {
            moveHorizontal = lhand.m_controller.JoystickX;
            moveVertical = lhand.m_controller.JoystickY;
            rotateHorizontal = rhand.m_controller.JoystickX;
            //rotateVertical = rhand.m_controller.JoystickY;

            m_Forward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            m_Right = Vector3.Scale(transform.right, new Vector3(1, 0, 1)).normalized;

            //Get horizontal and vertical movement
            Transform m_trans = GetComponent<Transform>();

            //Create movement & rotation vectors
            Vector3 movement = moveVertical * m_Forward + moveHorizontal * m_Right;
            Vector3 rotation = new Vector3(rotateHorizontal * m_rotateSpeed, 0, 0);

            Move(movement, rotation);
        }
    }

    private void Update()
    {
        if (!monsterAttacking)
        {
            CheckRunState();
        }
    }
  
    void CheckRunState()
    {
        
        if (SixenseInput.Controllers[0].GetButton(SixenseButtons.TRIGGER))
        {
            if (m_playerMoveState != PlayerMoveState.RUNNING && (timeRunning < (maxRunTime) / 2))
            {
                m_Animator.SetBool("Running", true);
                m_playerMoveState = PlayerMoveState.RUNNING;
            }
        }
        else if(m_playerMoveState == PlayerMoveState.RUNNING)
        {
            m_Animator.SetBool("Running", false);
            m_playerMoveState = PlayerMoveState.WALKING;
        }

        if (m_playerMoveState == PlayerMoveState.RUNNING)
        {
            if (timeRunning < maxRunTime)
                timeRunning += Time.deltaTime;
            else
            {
                m_Animator.SetBool("Running", false);
                m_playerMoveState = PlayerMoveState.WALKING;
            }
        }
        else if (m_playerMoveState != PlayerMoveState.RUNNING && timeRunning > 0f)
        {
            timeRunning -= Time.deltaTime;
        }
        
    }
    

    public void Move(Vector3 move, Vector3 rotation)
    {
        //Get original velocities
        float x_frame = m_Rigidbody.velocity.x;
        float y_frame = m_Rigidbody.velocity.y;
        float z_frame = m_Rigidbody.velocity.z;


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
        
        UpdateAnimator(move);
    }


    void UpdateAnimator(Vector3 move)
    {
        if (m_playerMoveState == PlayerMoveState.CLIMBING)
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
    
    
    public PlayerMoveState GetPlayerMoveState()
    {
        return m_playerMoveState;
    }
    public float GetTimeSpentRunning()
    {
        return timeRunning;
    }
    public float GetMaxRunTime()
    {
        return maxRunTime;
    }
}