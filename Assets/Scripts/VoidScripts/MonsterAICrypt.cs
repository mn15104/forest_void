using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterAICrypt : MonoBehaviour {

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

    private void OnEnable()
    {
       
    }

    private void OnDisable()
    {
       
    }

    void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("Idle", true);
        destinationPosition = player.transform.position;
    }

     
	// Update is called once per frame
	void Update () {
        switch (currentState) 
		{
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
                case MonsterState.APPEAR:
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    anim.SetFloat("Speed", m_AppearSpeed);
                    
                        StopAllCoroutines();
                        StartCoroutine(UpdateChaseDestination());
                    
                    m_CurrentSpeed = m_AppearSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.APPROACH, 3f));
                    break;
                case MonsterState.APPROACH:

                        StopAllCoroutines();
                        StartCoroutine(UpdateChaseDestination());
                    
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
        
                        StopAllCoroutines();
                        StartCoroutine(UpdateChaseDestination());
                    
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
            debugState = state;
            currentState = state;
        }
    }

    IEnumerator DelayStateChange(MonsterState state, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SetState(state);
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
			SetState(MonsterState.HIDDEN_IDLE);////////////////////////////////
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
    
    
    public MonsterState GetMonsterState()
    {
        return currentState;
    }

    public float GetMonsterSpeed()
    {
        return m_CurrentSpeed;
    }
    
}