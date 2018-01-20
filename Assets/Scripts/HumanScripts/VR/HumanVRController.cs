using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class HumanVRController : MonoBehaviour
{
    public float m_MovingTurnSpeed = 360;
    public float m_StationaryTurnSpeed = 180;
    public float m_GravityMultiplier = 2f;
    public float m_RunCycleLegOffset = 0.2f;
    public float m_MoveSpeedMultiplier = 1f;
    public float m_AnimSpeedMultiplier = 1f;

    Rigidbody m_Rigidbody;
    Animator m_Animator;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_Speed;
    Vector3 m_GroundNormal;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    CapsuleCollider m_Capsule;
    private InventoryScript m_Inventory;

    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        m_Inventory = new InventoryScript();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        //get horizontal and vertical movement
        //float horizontalMove = Input.GetAxis("Horizontal");
        //float verticalMove = Input.GetAxis("Vertical");

        //float upMove = Input.GetAxis("Mouse X");
        //float downMove = Input.GetAxis("Mouse Y");
        

        //takes input and makes movement vector

        //Vector3 movement = verticalMove * transform.forward + horizontalMove * transform.right;
        //Vector3 rotation = new Vector3(upMove, downMove, 0);
        //Move(movement, rotation);
    }

    public void Move(Vector3 move, Vector3 rotation)
    {
        
        //Rotate about y axis
        m_TurnAmount = rotation.x;
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_Speed);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        //Movement 
        m_Rigidbody.velocity = move * m_MoveSpeedMultiplier;
        //Animator Update
        m_Speed = m_Rigidbody.velocity.magnitude;
        UpdateAnimator(move);
    }

    void UpdateAnimator(Vector3 move)
    {
        m_Animator.SetFloat("Speed", m_Speed, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);

        float runCycle =
            Mathf.Repeat(
                m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_Speed;

        m_Animator.SetFloat("JumpLeg", jumpLeg);
        m_Animator.speed = move.magnitude > 0 ? m_AnimSpeedMultiplier : m_Animator.speed = 1; 
    }
}