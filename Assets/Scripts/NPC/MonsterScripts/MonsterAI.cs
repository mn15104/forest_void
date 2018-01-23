using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class MonsterAI : MonoBehaviour {

	public enum State {HIDDEN, APPEAR, CHASE};
	public State currentState;
	//public int speed;
	public GameObject player;
	public float timer;
	private Vector3 appearPosition;
	private Animator anim;


	private Renderer rend;
	private GameObject trigger;
	private Collider m_Collider;


	//GAME OVER
	public Text gameOverText;
	public FadeOut fo;

	// NavMesh 
	private UnityEngine.AI.NavMeshAgent agent;
	private float pathTimer;
	//Vector3 destination;

//	//Calculating whether the monster will appear or not
//	private float timeFromStart;
//	private float endTime;
//	private float terrainSize;

//	//FreeRoam
//	public float roamRadius;
//	
//	public float wanderTimer;

//	//Player variables
//	public float playerViewDistance;


	// Use this for initialization
	void Start () {
		currentState = State.HIDDEN;
        anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		gameOverText.text = "";
		pathTimer = 1;
		//destination = new Vector3 (500, transform.position.y, transform.position.z);
		m_Collider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		State nextState;
		switch (currentState) 
		{
		case State.HIDDEN:
			nextState = hidden ();
			break;
		case State.APPEAR:
			nextState = appear ();
			break;
		case State.CHASE:
			nextState = chase ();			
			break;
		default:
			nextState = State.HIDDEN; 
			break;
		}
		currentState = nextState;
	}

	public void setState(State state){
		currentState = state;
	}
	public void setPostion(Vector3 position){
		appearPosition = position;
	}

	State hidden () {
		//Change colour - doesnt even matter cause invisible 
		return State.HIDDEN;
	}

	State appear () {
		transform.position = appearPosition;
		//transform.position = player.transform.position + (player.transform.forward * offset);
		//transform.position = new Vector3 (transform.position.x, 0.50f, transform.position.z);
		return State.CHASE;
	}
		
	State chase () {
        Debug.Log("chasing");

		//Chase player
		pathTimer -= Time.deltaTime;
		if (pathTimer < 0) {
			agent.SetDestination (player.transform.position);
			pathTimer = 1;
			anim.SetTrigger ("StartRunning");
			Debug.Log (agent.destination);
		}

		// GAME OVER
		float distance = (player.transform.position - transform.position).magnitude;
		if (distance <= 5) {
			timer -= Time.deltaTime;
			Debug.Log (timer);
			anim.SetTrigger ("StopRunning");
			//m_CurrentSpeed = 0;
			if (timer < 0) {
				//TODO Reset timer if player is no longer within distance 
				Debug.Log ("Game over");
				GameOver ();
				fo.FadeToBlack (); //FADE IN DOESN'T WORK
			}
		} else {
			timer = 5;
		}
		return State.CHASE;
	}

	void GameOver(){
		gameOverText.text = "GAME OVER";
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