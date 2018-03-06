using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum MonsterState
{
    HIDDEN_IDLE,
    HIDDEN_MOVING,
    FOLLOW,
    APPEAR,
    APPROACH,
    CHASE,
    GAMEOVER
};

public class MonsterAI : MonoBehaviour {

    public delegate void MonsterStateChange(MonsterState monsterState);
    public event MonsterStateChange OnMonsterStateChange = delegate { };

    //public delegate void Detec(State monsterState);
    //public static event MonsterStateChange OnMonsterStateChange;

    //Components
    public GameObject player;
    private MonsterState currentState;
    public MonsterState debugState;
    private Animator anim;
	private GameObject trigger;
	private float chaseTimer = 10f;
    //
    private const float m_HiddenIdleSpeed = 0f;
    private const float m_HiddenMovingSpeed = 1.0f;
    private const float m_FollowSpeed = 1.0f;
    private const float m_AppearSpeed = 1.0f;
    private const float m_MinApproachSpeed = 0.3f;
    private const float m_MaxApproachSpeed = 3.0f;
    private const float m_RunSpeed = 0.35f;
    private float m_CurrentSpeed = 1f;
    //
    private Vector3 destinationPosition;
    private bool collisionRight;
    private bool collisionLeft;
    private AICollisionSide firstCollision;
    // 
    public float soundDetectionPercentage = 0.2f;
    public float lightDetectionPercentage = 0.2f;
    private Quaternion destinationRotation;
    private float distanceToHuman;
    private const float distanceToHuman_FollowTrigger = 12;
    private const float distanceToHuman_AppearTrigger = 6;
    private const float maxDetectionRange = 165;
    private bool follow_finished;

    private void OnEnable()
    {
        //*Add Events*//
        HumanVRRightHand.OnHumanLightEmission += HumanLightDetected;
        HumanVRAudioController.OnHumanAudioEmission += HumanSoundDetected;
        MonsterTrigger.OnHumanDetected += HumanDetected;
        //*Add Events*//
        anim = GetComponent<Animator>();
        destinationPosition = player.transform.position;
        follow_finished = false;
        //*Set Initial State As Moving*//
        debugState = MonsterState.HIDDEN_MOVING;
        currentState = MonsterState.HIDDEN_MOVING;
        StartCoroutine(UpdateHiddenDestination());
        anim.SetBool("Run", false);
        anim.SetBool("Walk", true);
        anim.SetBool("Idle", false);
        anim.SetFloat("Speed", m_HiddenMovingSpeed);
        m_CurrentSpeed = m_HiddenMovingSpeed;
        StartCoroutine(DelayStateChange(MonsterState.HIDDEN_IDLE, 8f));
    }

    private void OnDisable()
    {
        HumanVRRightHand.OnHumanLightEmission -= HumanLightDetected;
        HumanVRAudioController.OnHumanAudioEmission -= HumanSoundDetected;
        MonsterTrigger.OnHumanDetected -= HumanDetected;
        m_CurrentSpeed = m_HiddenIdleSpeed;
        soundDetectionPercentage = 0.2f;
        lightDetectionPercentage = 0.2f;
        chaseTimer = 10f;
        distanceToHuman = Mathf.Infinity;
        collisionRight = false;
        collisionLeft = false;
    }

    void Start () {
        
    }
    


	void Update () {
        /*UPDATE STATE VARIABLES*/
        distanceToHuman = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2)
                                   + Mathf.Pow(player.transform.position.z - transform.position.z, 2));
        if (currentState != debugState)
            SetState(debugState);
        /*EXECUTE STATE ACTION*/
        switch (currentState) 
		{
        case MonsterState.HIDDEN_IDLE:
            hidden_idle();
            break;
        case MonsterState.HIDDEN_MOVING:
            hidden_walking();
			break;
        case MonsterState.FOLLOW:
            follow();
            break;
        case MonsterState.APPEAR:
			appear();
            break;
        case MonsterState.APPROACH:
            approach();
            break;
		case MonsterState.CHASE:
            chase ();			
			break;
        case MonsterState.GAMEOVER:
            hidden_idle();			
			break;
		default:
            hidden_idle();
            break;
		}
    }
    
    public void SetState(MonsterState state)
    {
        if (state != currentState)
        {
            debugState = state;
            currentState = state;
            OnMonsterStateChange(state);
            switch (state)
            {
                case MonsterState.HIDDEN_IDLE:
                    StopAllCoroutines();
                    StartCoroutine(UpdateHiddenDestination());
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    anim.SetFloat("Speed", m_HiddenIdleSpeed);
                    m_CurrentSpeed = m_HiddenIdleSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.HIDDEN_MOVING, 2f));
                    break;
                case MonsterState.HIDDEN_MOVING:
                    StopAllCoroutines();
                    StartCoroutine(UpdateHiddenDestination());
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", true);
                    anim.SetBool("Idle", false);
                    anim.SetFloat("Speed", m_HiddenMovingSpeed);
                    m_CurrentSpeed = m_HiddenMovingSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.HIDDEN_IDLE, 8f));
                    break;
                case MonsterState.FOLLOW:
                    StopAllCoroutines();
                    StartCoroutine(UpdateChaseDestination());
                    anim.SetBool("Run", false);
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", true);
                    anim.SetFloat("Speed", m_FollowSpeed);
                    m_CurrentSpeed = m_FollowSpeed;
                    break;
                case MonsterState.APPEAR:
                    StopAllCoroutines();
                    StartCoroutine(UpdateChaseDestination());
                    follow_finished = false;                  // Reset follow bool
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    anim.SetFloat("Speed", m_AppearSpeed);
                    m_CurrentSpeed = m_AppearSpeed;
                    /*INVOKE A MECHANIC UPON APPEARING*/
                    Invoke("TeleportVoidBehindHuman", 2f);
                    StartCoroutine(DelayStateChange(MonsterState.APPROACH, 5f));
                    /*--------------------------------*/
                    break;
                case MonsterState.APPROACH:
                    StopAllCoroutines();
                    StartCoroutine(UpdateChaseDestination());
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", true);
                    anim.SetFloat("Speed", m_MinApproachSpeed);
                    m_CurrentSpeed = m_MinApproachSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.CHASE, 4f));
                    break;
                case MonsterState.CHASE:
                    StopAllCoroutines();
                    StartCoroutine(UpdateChaseDestination());
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);
                    anim.SetFloat("Speed", m_RunSpeed);
                    m_CurrentSpeed = m_RunSpeed;
                    break;
                case MonsterState.GAMEOVER:
                    StopAllCoroutines();
                    anim.SetBool("Idle", true);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", false);
                    m_CurrentSpeed = 0;
                    break;
                default:
                    hidden_idle();
                    break;
            }
           
        }
    }
  
    /* DIFFERENT VOID MECHANICS UPON THE VOID SEEING HUMAN/HUMAN SEEING IT */
    void TeleportVoidBehindHuman()
    {
        Vector3 humanPos = player.transform.position;
        Vector3 humanFacingDir = player.transform.forward;
        foreach(Camera cam in player.GetComponentsInChildren<Camera>())
        {
            if (cam.isActiveAndEnabled)
            {
                humanFacingDir = cam.transform.forward;
            }
        }
        Vector3 voidPos = humanPos - 10 * humanFacingDir;

        int mask = LayerMask.GetMask("Trees", "Rocks", "Structures");
        int giveUpCounter = 0;
        var checkResult = Physics.OverlapSphere(voidPos, 0.5f, mask);
        Vector3 voidPosAttempt = voidPos;
        while (checkResult.Length == 0 && giveUpCounter < 5){
            voidPosAttempt = voidPos;
            voidPosAttempt.x += Random.Range(-1f, 2f);
            voidPosAttempt.z += Random.Range(-1f, 2f);
            giveUpCounter++;
            checkResult = Physics.OverlapSphere(voidPosAttempt, 0.5f, mask);
        }
        if(giveUpCounter >= 5)
        {
            return;
        }
        
        transform.position = voidPosAttempt;
        Vector3 directionToPlayer = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = rot;
    }
    /*------------------------------------------------------------------------*/

    IEnumerator DelayStateChange(MonsterState state, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SetState(state);
    }

    IEnumerator UpdateHiddenDestination()
    {
        while (true)
        {
            float detectionRate = (soundDetectionPercentage + lightDetectionPercentage)/2;
            Vector3 directionToPlayer = player.transform.position - transform.position;
            Vector3 rot = Quaternion.LookRotation(directionToPlayer).eulerAngles;
            if (detectionRate == 0)
            {
                rot.y += (0.1f) / (2f * NextGaussianFloat());
            }
            else
            {
                rot.y += (detectionRate) /(2f * NextGaussianFloat()) ;
            }
          
            destinationRotation = Quaternion.Euler(rot);
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator UpdateChaseDestination()
    {
        while (true)
        {
            destinationPosition = player.transform.position;
            yield return new WaitForEndOfFrame();
        }
    }

    void hidden_idle()
    {
        if ((currentState == MonsterState.HIDDEN_IDLE || currentState == MonsterState.HIDDEN_MOVING)
                && distanceToHuman < distanceToHuman_FollowTrigger)
        {
            SetState(MonsterState.FOLLOW);
        }
    }

    void hidden_walking()
    {
        if ((currentState == MonsterState.HIDDEN_IDLE || currentState == MonsterState.HIDDEN_MOVING)
                && distanceToHuman < distanceToHuman_FollowTrigger)
        {
            SetState(MonsterState.FOLLOW);
            return;
        }
        if (firstCollision != AICollisionSide.NONE)
        {
            Vector3 t_rotation = transform.rotation.eulerAngles;
            if (firstCollision == AICollisionSide.RIGHT)
            {
                t_rotation.y -= Time.deltaTime * 100;
            }
            else
            {
                t_rotation.y += Time.deltaTime * 100;
            }
            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, destinationRotation, Time.deltaTime * 1.2f);
        }
    }

    void follow()
    {
        if (firstCollision != AICollisionSide.NONE)
        {
            Vector3 t_rotation = transform.rotation.eulerAngles;
            if (firstCollision == AICollisionSide.RIGHT)
            {
                t_rotation.y -= Time.deltaTime * 100;
            }
            else
            {
                t_rotation.y += Time.deltaTime * 100;
            }

            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            var lookPos = destinationPosition - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1.2f);
        }
        if (!follow_finished)
        {
            if (distanceToHuman > distanceToHuman_AppearTrigger && !anim.GetBool("Walk"))
            {
                m_CurrentSpeed = m_HiddenMovingSpeed;
                anim.SetBool("Run", false);
                anim.SetBool("Walk", true);
                anim.SetBool("Idle", false);
                anim.SetFloat("Speed", m_CurrentSpeed);
            }
            else if (distanceToHuman <= distanceToHuman_AppearTrigger)
            {
                anim.SetBool("Run", false);
                anim.SetBool("Idle", true);
                anim.SetBool("Walk", false);
                StartCoroutine(DelayStateChange(MonsterState.APPEAR, 3f));
                follow_finished = true;
            }
        }
    }
    
	void appear () {

	}
    
    void approach()
    {
        if (firstCollision != AICollisionSide.NONE)
        {
            Vector3 t_rotation = transform.rotation.eulerAngles;
            if (firstCollision == AICollisionSide.RIGHT)
            {
                t_rotation.y -= Time.deltaTime * 100;
            }
            else
            {
                t_rotation.y += Time.deltaTime * 100;
            }
            
            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            var lookPos = destinationPosition - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1.2f);
        }
      
        m_CurrentSpeed = Mathf.Lerp(m_CurrentSpeed, m_MaxApproachSpeed, Time.deltaTime * 0.1f);
        anim.SetFloat("Speed", m_CurrentSpeed);
    }

    void chase () {
        if (firstCollision != AICollisionSide.NONE)
        {
            Vector3 t_rotation = transform.rotation.eulerAngles;
            if (firstCollision == AICollisionSide.RIGHT)
            {
                t_rotation.y -= Time.deltaTime * 100;
            }
            else
            {
                t_rotation.y += Time.deltaTime * 100;
            }

            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            var lookPos = destinationPosition - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1.5f);
        }
        //Chase
        float distanceToHuman = Mathf.Sqrt(Mathf.Pow(destinationPosition.x - transform.position.x, 2) 
                                + Mathf.Pow(destinationPosition.y - transform.position.y, 2));
        
		chaseTimer -= Time.deltaTime;
		if (chaseTimer < 0) {
			SetState(MonsterState.HIDDEN_IDLE);
            chaseTimer = 20f;
        }

		// Game Over
		float distance = (destinationPosition - transform.position).magnitude;
		if (distance < 1f) {
            SetState(MonsterState.GAMEOVER);
        }
	}


    void GameOver()
    {

    }

    public void NotifyCollisionAhead(AICollisionSide collisionSide, bool isCollision)
    {
        switch (collisionSide)
        {
            case AICollisionSide.LEFT:
                if (isCollision)
                {
                    collisionLeft = true;
                    if (!collisionRight)
                        firstCollision = AICollisionSide.LEFT;
                }
                else
                {
                    collisionLeft = false;
                    if (!collisionRight)
                        firstCollision = AICollisionSide.NONE;
                    else
                        firstCollision = AICollisionSide.RIGHT;
                }
                break;
            case AICollisionSide.RIGHT:
                if (isCollision)
                {
                    collisionRight = true;
                    if (!collisionLeft)
                        firstCollision = AICollisionSide.RIGHT;
                }
                else
                {
                    collisionRight = false;
                    if (!collisionLeft)
                        firstCollision = AICollisionSide.NONE;
                    else
                        firstCollision = AICollisionSide.LEFT;
                }
                break;
        }
    }
    
    void HumanDetected(Transform detectionTrans)
    {
        destinationPosition = detectionTrans.position;
        SetState(MonsterState.APPEAR);
    }

    void HumanSoundDetected(float soundHeard)
    {
        Vector3 xyz_distance = player.transform.position - transform.position;
        Vector2 xz_distance = new Vector2(xyz_distance.x, xyz_distance.z);
        float distance = xz_distance.magnitude;

        soundDetectionPercentage = soundHeard * (distance / maxDetectionRange);
    }

    void HumanLightDetected(bool on)
    {
        Vector3 xyz_distance = player.transform.position - transform.position;
        Vector2 xz_distance = new Vector2(xyz_distance.x, xyz_distance.z);
        float distance = xz_distance.magnitude;
        if (on)
        {
            lightDetectionPercentage = (distance / maxDetectionRange);
        }
        else
        {
            lightDetectionPercentage = 0f;
        }
    }
    
    public MonsterState GetMonsterState()
    {
        return currentState;
    }
    public float GetMonsterSpeed()
    {
        return m_CurrentSpeed;
    }
    
    float NextGaussianFloat()
    {
        float u, v, S;

        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        float fac = Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
        return u * fac;
    }


}