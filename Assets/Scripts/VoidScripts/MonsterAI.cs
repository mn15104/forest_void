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
    public MonsterState currentState = MonsterState.HIDDEN_IDLE;
    public MonsterState debugState = MonsterState.HIDDEN_IDLE;
    private Animator anim;
	private GameObject trigger;
	private float chaseTimer = 20f;
    //
    private float m_HiddenIdleSpeed = 0f;
    private float m_HiddenMovingSpeed = 1.5f;
    private float m_AppearSpeed = 1f;
    private float m_MinApproachSpeed = 0.3f;
    private float m_MaxApproachSpeed = 3.5f;
    private float m_RunSpeed = 0.5f;
    private float m_CurrentSpeed = 1f;
    //
    private Vector3 destinationPosition;
    private bool collisionRight;
    private bool collisionLeft;
    private AICollisionSide firstCollision = AICollisionSide.NONE;
    // Used while hidden
    public float soundDetectionPercentage = 0;
    public float lightDetectionPercentage = 0;
    private Quaternion destinationRotation;
    private float distanceToHuman;
    private float maxDetectionRange = 165;
    private bool isHidden = false;

    private void OnEnable()
    {
        HumanVRRightHand.OnHumanLightEmission += HumanLightDetected;
        HumanVRAudioController.OnHumanAudioEmission += HumanSoundDetected;
        MonsterTrigger.OnHumanDetected += HumanDetected;
    }

    private void OnDisable()
    {
        HumanVRRightHand.OnHumanLightEmission -= HumanLightDetected;
        HumanVRAudioController.OnHumanAudioEmission -= HumanSoundDetected;
        MonsterTrigger.OnHumanDetected -= HumanDetected;
    }

    void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("Idle", true);
        destinationPosition = player.transform.position;
        Invoke("DebugInvokeInit", 2);
    }

    void DebugInvokeInit()
    {
        SetState(MonsterState.HIDDEN_MOVING);
    }
     
	// Update is called once per frame
	void Update () {
        distanceToHuman = Mathf.Sqrt(Mathf.Pow(destinationPosition.x - transform.position.x, 2)
                                   + Mathf.Pow(destinationPosition.y - transform.position.y, 2));
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

        if (currentState != debugState)
            SetState(debugState);
    }




    public void SetState(MonsterState state)
    {
        if (state != currentState)
        {
            OnMonsterStateChange(state);
            switch (state)
            {
                case MonsterState.HIDDEN_IDLE:
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    anim.SetFloat("Speed", m_HiddenIdleSpeed);
                    if (!isHidden)
                    {
                        isHidden = true;
                        StopAllCoroutines();
                        StartCoroutine(UpdateHiddenDestination());
                    }
                    m_CurrentSpeed = m_HiddenIdleSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.HIDDEN_MOVING, 2f));
                    break;
                case MonsterState.HIDDEN_MOVING:
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", true);
                    anim.SetBool("Idle", false);
                    anim.SetFloat("Speed", m_HiddenMovingSpeed);
                    if (!isHidden)
                    {
                        isHidden = true;
                        StopAllCoroutines();
                        StartCoroutine(UpdateHiddenDestination());
                    }
                    m_CurrentSpeed = m_HiddenMovingSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.HIDDEN_IDLE, 8f));
                    break;
                case MonsterState.FOLLOW:
                    StopAllCoroutines();
                    anim.SetBool("Run", false);
                    anim.SetBool("Idle", true);
                    anim.SetBool("Walk", false);
                    StartCoroutine(UpdateChaseDestination());
                    break;
                case MonsterState.APPEAR:
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    anim.SetFloat("Speed", m_AppearSpeed);
                    if (isHidden)
                    {
                        isHidden = false;
                        StopAllCoroutines();
                        StartCoroutine(UpdateChaseDestination());
                    }
                    m_CurrentSpeed = m_AppearSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.APPROACH, 3f));
                    break;
                case MonsterState.APPROACH:
                    if (isHidden)
                    {
                        isHidden = false;
                        StopAllCoroutines();
                        StartCoroutine(UpdateChaseDestination());
                    }
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", true);
                    anim.SetFloat("Speed", m_MinApproachSpeed);
                    m_CurrentSpeed = m_MinApproachSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.CHASE, 6f));
                    break;
                case MonsterState.CHASE:
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);
                    anim.SetFloat("Speed", m_RunSpeed);
                    if (isHidden)
                    {
                        isHidden = false;
                        StopAllCoroutines();
                        StartCoroutine(UpdateChaseDestination());
                    }
                    m_CurrentSpeed = m_RunSpeed;
                    break;
                case MonsterState.GAMEOVER:
                    StopAllCoroutines();
                    isHidden = false;
                    anim.SetBool("Idle", true);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", false);
                    m_CurrentSpeed = 0;
                    break;
                default:
                    hidden_idle();
                    break;
            }
            debugState = state;
            currentState = state;
        }
    }

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
                rot.y += 100 * NextGaussianFloat() * (1 / detectionRate);
            }
            else
            {
                rot.y += 30 * NextGaussianFloat() * (1.5f / detectionRate);
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

    void follow()
    {
        if (firstCollision != AICollisionSide.NONE)
        {
            Vector3 t_rotation = transform.rotation.eulerAngles;
            if (firstCollision == AICollisionSide.RIGHT)
            {
                t_rotation.y -= Time.deltaTime * 60;
            }
            else
            {
                t_rotation.y += Time.deltaTime * 60;
            }

            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            var lookPos = destinationPosition - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        }
        float distanceToHuman = Mathf.Sqrt(Mathf.Pow(destinationPosition.x - transform.position.x, 2)
                                         + Mathf.Pow(destinationPosition.y - transform.position.y, 2));

        if (distanceToHuman > 7 && !anim.GetBool("Walk"))
        {
            m_CurrentSpeed = m_HiddenMovingSpeed;
            anim.SetBool("Run", false);
            anim.SetBool("Walk", true);
            anim.SetBool("Idle", false);
            anim.SetFloat("Speed", m_CurrentSpeed);
        }
        else if (anim.GetBool("Walk")) { 
            anim.SetBool("Run", false);
            anim.SetBool("Idle", true);
            anim.SetBool("Walk", false);
        }
    }

    void hidden_idle()
    {

    }

    void hidden_walking() {
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
            transform.rotation = Quaternion.Slerp(transform.rotation, destinationRotation, Time.deltaTime * 2);
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
                t_rotation.y -= Time.deltaTime * 60;
            }
            else
            {
                t_rotation.y += Time.deltaTime * 60;
            }
            
            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            var lookPos = destinationPosition - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
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