using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class MonsterAI : MonoBehaviour {

    public delegate void StateChange(State monsterState);
    public static event StateChange OnStateChange;



    //Components
	public enum State {
        HIDDEN,
        APPEAR,
        APPROACH,
        CHASE,
        GAMEOVER
    };

    public GameObject player;
    public State currentState = State.HIDDEN;
    public State debugState = State.HIDDEN;
    private Animator anim;
	private GameObject trigger;
    
	private float chaseTimer = 20f;
    //
    private float m_WalkSpeed = 1f;
    private float m_MinApproachSpeed = 0.3f;
    private float m_MaxApproachSpeed = 3.5f;
    private float m_RunSpeed = 0.5f;
    private float m_CurrentSpeed = 1f;
    //
    private Vector3 destination;
    private bool collisionRight;
    private bool collisionLeft;

    private void OnEnable()
    {
        MonsterTrigger.OnHumanDetected += humanDetected;
    }

    private void OnDisable()
    {
        MonsterTrigger.OnHumanDetected -= humanDetected;
    }

    void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("Idle", true);
        destination = player.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateDestination();
        switch (currentState) 
		{
		case State.HIDDEN:
			hidden ();
			break;
		case State.APPEAR:
			appear();
            break;
        case State.APPROACH:
                approach();
            break;
		case State.CHASE:
            chase ();			
			break;
        case State.GAMEOVER:
            hidden();			
			break;
		default:
            hidden();
            break;
		}

        if (currentState != debugState)
            setState(debugState);
        
    }

    void UpdateDestination()
    {
        if (currentState == State.APPROACH || currentState == State.CHASE || currentState == State.APPEAR)
        {
                destination = player.transform.position;
        }
    }
    
    public void NotifyCollisionAhead(AICollisionDetection.CollisionSide collisionSide, bool isCollision)
    {
        switch (collisionSide)
        {
            case AICollisionDetection.CollisionSide.LEFT:
                collisionLeft = isCollision;
                break;
            case AICollisionDetection.CollisionSide.RIGHT:
                collisionRight = isCollision;
                break;
        }   
    }

	public void setState(State state){
        if (state != currentState)
        {
            OnStateChange(state);
            switch (state)
            {
                case State.HIDDEN:
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
                        mesh.enabled = false;
                    break;
                case State.APPEAR:
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
                        mesh.enabled = true;
                    StartCoroutine(DelayStateChange(State.APPROACH, 3f));
                    break;
                case State.APPROACH:
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", true);
                    m_CurrentSpeed = m_MinApproachSpeed;
                    StartCoroutine(DelayStateChange(State.CHASE, 6f));
                    break;
                case State.CHASE:
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);
                    anim.SetFloat("Speed", m_RunSpeed);
                    break;
                case State.GAMEOVER:
                    anim.SetBool("Idle", true);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", false);
                    break;
                default:
                    hidden();
                    break;
            }
            debugState = state;
            currentState = state;
        }
    }

	void hidden () {

    }

	void appear () {

	}
    
    void approach()
    {
        if (collisionLeft || collisionRight)
        {
            Vector3 t_rotation = transform.rotation.eulerAngles;

            if (collisionRight)
                t_rotation.y -= Time.deltaTime * 60;
            else
                t_rotation.y += Time.deltaTime * 60;

            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            var lookPos = destination - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        }
        //Chase
        m_CurrentSpeed = Mathf.Lerp(m_CurrentSpeed, m_MaxApproachSpeed, Time.deltaTime * 0.1f);
        anim.SetFloat("Speed", m_CurrentSpeed);
    }

    IEnumerator DelayStateChange(State state, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        setState(state);
    }

    void chase () {
        if (collisionLeft || collisionRight)
        {
            Vector3 t_rotation = transform.rotation.eulerAngles;

            if (collisionRight)
                t_rotation.y -= Time.deltaTime * 60;
            else
                t_rotation.y += Time.deltaTime * 60;

            transform.rotation = Quaternion.Euler(t_rotation);
        }
        else
        {
            var lookPos = destination - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        }
        //Chase
        float distanceToHuman = Mathf.Sqrt(Mathf.Pow(destination.x - transform.position.x, 2) 
                                + Mathf.Pow(destination.y - transform.position.y, 2));
        
		chaseTimer -= Time.deltaTime;
		if (chaseTimer < 0) {
			setState(State.HIDDEN);
            chaseTimer = 20f;
        }

		// Game Over
		float distance = (destination - transform.position).magnitude;
		if (distance < 1f) {
            setState(State.GAMEOVER);
        }
	}

    void humanDetected(Transform detectionTrans)
    {
        destination = detectionTrans.position;
        setState(State.APPEAR);
    }

    void GameOver(){
        
	}
   



    //bool willAppear(){
    //	//This returns the probabilty of the monster appearing once the trigger point is touched
    //	//Calculated using the distance and time passed

    //	float timeProb = timeFromStart / endTime;
    //	float distanceFromPlayer = (transform.position - player.transform.position).magnitude;
    //	float distanceProb = distanceFromPlayer / terrainSize;

    //	float probability = timeProb * distanceProb;

    //	float randomProb = Random.Range (0.0f, 1.0f);

    //	if (probability > randomProb) {
    //		return true;
    //	} else
    //		return true;
    //}

    //	Vector3 freeRoam(Vector3 origin)
    //	{
    //		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * roamRadius;
    //
    //		randomDirection += origin;
    //
    //		NavMeshHit navHit;
    //
    //		NavMesh.SamplePosition (randomDirection, out navHit, roamRadius, 1);
    //
    //		return navHit.position;

    //		Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
    //		randomDirection += transform.position;
    //		NavMeshHit hit;
    //		Vector3 finalPosition = Vector3.zero;
    //		if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1)) {
    //			finalPosition = hit.position;            
    //		}
    //		return finalPosition;

    //	}

    //bool inRange(Vector3 firstVector, Vector3 secondVector, int threshold){
    //	float range = (firstVector - secondVector).magnitude;
    //	if (range < threshold) return true;
    //	else return false; 
    //}
}