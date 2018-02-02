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
    public State currentState;
    public State debugState;
    private Vector3 appearPosition;
	private Animator anim;
	private GameObject trigger;
    
	private float chaseTimer;
    //
    private float m_WalkSpeed = 1f;
    private float m_ApproachSpeed = 1.2f;
    private float m_MinChaseSpeed = 0.3f;
    private float m_MaxChaseSpeed = 3.5f;
    private float m_CurrentSpeed = 1f;
    // 
    private bool _isLerping = false;
    private float _timeStartedLerping = 0f;
    private float timeTakenDuringLerp = 1f;
    //
    private Vector3 destination;
    private bool collisionAhead = false;


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
		chaseTimer = 10;
        anim.SetBool("Idle", true);
        appearPosition = transform.position;
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
            if (!collisionAhead)
                destination = player.transform.position;
            else
            {
                Vector3 dest = transform.position;
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;
                Vector3 rot = forward + right;
                destination = transform.position + forward + right;
            }
        }
    }
    
    public void NotifyCollisionAhead(bool isCollision)
    {
        collisionAhead = isCollision;
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
                    foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
                        mesh.enabled = true;
                    transform.position = appearPosition;
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", true);
                    StartCoroutine(DelayChase());
                    break;
                case State.APPROACH:
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", true);
                    m_CurrentSpeed = m_MinChaseSpeed;
                    break;
                case State.CHASE:
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);
                    anim.SetFloat("Speed", m_MinChaseSpeed);
                    _isLerping = true;
                    _timeStartedLerping = Time.time;
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
            currentState = state;
        }
    }
	void setPosition(Vector3 position){

	}

	void hidden () {

    }

	void appear () {
	}
    
    void approach()
    {
        var lookPos = destination - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        //Chase
        anim.SetFloat("Speed", m_CurrentSpeed);
        Vector3 playerpos = destination;
        Vector3 planar = new Vector3(1, 0, 1);
        Ray ray = new Ray(transform.position, transform.forward);
        Vector3 dir = (playerpos - transform.position).normalized * m_CurrentSpeed;
        dir.y = 0;
        transform.GetComponent<Rigidbody>().velocity = dir;
    }

    IEnumerator DelayChase()
    {
        setState(State.APPROACH);
        yield return new WaitForSeconds(4f);
        setState(State.CHASE);
    }

    void chase () {
        //Rotate
        var lookPos = destination - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        //Chase
        float distanceToHuman = Mathf.Sqrt(Mathf.Pow(destination.x - transform.position.x, 2) 
                                + Mathf.Pow(destination.y - transform.position.y, 2));
        
		chaseTimer -= Time.deltaTime;
		if (chaseTimer < 0) {
			setState(State.HIDDEN);
        }

		// Game Over
		float distance = (destination - transform.position).magnitude;
		if (distance < 1f) {
            setState(State.GAMEOVER);
        }
	}

    void FixedUpdate()
    {
        if (_isLerping)
        {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = (timeSinceStarted / timeTakenDuringLerp) * 0.5f;
            m_CurrentSpeed = Mathf.Lerp(m_MinChaseSpeed, m_MaxChaseSpeed, percentageComplete);
            if (percentageComplete >= 1.0f)
            {
                _isLerping = false;
            }
        }
    }

    void humanDetected(Transform detectionTrans)
    {
        destination = detectionTrans.position;
        setState(State.APPEAR);
        setPosition(detectionTrans.position);
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