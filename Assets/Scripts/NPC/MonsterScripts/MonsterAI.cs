using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MonsterAI : MonoBehaviour {

	public enum State {HIDDEN, APPEAR, CHASE};
	public State currentState;
	public int speed;
	public GameObject player;
	public float timer;

	private Renderer rend;
	private GameObject[] waypoints;
	private GameObject trigger;

	//Calculating whether the monster will appear or not
	private float timeFromStart;
	private float endTime;
	private float terrainSize;

	//FreeRoam
	public float roamRadius;
	private UnityEngine.AI.NavMeshAgent agent;
	public float wanderTimer;

	// Use this for initialization
	void Start () {
		//rb = GetComponent<Rigidbody>();
		rend = GetComponent<Renderer>();

		currentState = State.HIDDEN;

		//willAppear variables
		timeFromStart = Time.realtimeSinceStartup;
		endTime = 20f;
		terrainSize = GameObject.Find("ForestTerrain").GetComponent<Terrain>().terrainData.size.magnitude;

		//FreeRoam
		roamRadius = terrainSize;
		agent = GetComponent<NavMeshAgent>();
		timer = wanderTimer;
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

	State hidden () {
		//Change colour - doesnt even matter cause invisible 
		rend.material.SetColor("_Color", Color.green);
		//rend.enabled = false;

		timer += Time.deltaTime;
		if (timer >= wanderTimer) {
			Vector3 newPos = freeRoam (transform.position);
			//Vector3 newPos = new Vector3(250, 0, 270);
			agent.SetDestination (newPos);
			timer = 0;
		}
		return State.HIDDEN;
	}

	State appear () {
		//Make visible and change colour
		rend.enabled = true;
		rend.material.SetColor("_Color", Color.yellow);

		float offset = 5;

		if (willAppear ()) {
			agent.speed = 0;
			transform.position = player.transform.position - (player.transform.forward * offset);
			transform.position = new Vector3 (transform.position.x, 0.50f, transform.position.z);
			return State.CHASE;
		} else
			return State.HIDDEN;
	}
		
	State chase () {
		//Change colour
		//rend.enabled = true;
		rend.material.SetColor("_Color", Color.red);


		RaycastHit hit = new RaycastHit();
		Vector3 direction = player.transform.forward * 5; ///vector that goes out from player

		if(Physics.Raycast(player.transform.position, direction, out hit))
		{	//TODO: make this a range instead of single line maybe?
			if (hit.transform == transform ) {
				rend.material.SetColor("_Color", Color.blue);
			}
		}
		Debug.DrawRay (player.transform.position, direction, Color.black);

		return State.CHASE;
	}

	bool willAppear(){
		//This returns the probabilty of the monster appearing once the trigger point is touched
		//Calculated using the distance and time passed

		float timeProb = timeFromStart / endTime;
		float distanceFromPlayer = (transform.position - player.transform.position).magnitude;
		float distanceProb = distanceFromPlayer / terrainSize;

		float probability = timeProb * distanceProb;

		float randomProb = Random.Range (0.0f, 1.0f);

		if (probability > randomProb) {
			return true;
		} else
			return true;
	}

	Vector3 freeRoam(Vector3 origin)
	{
//		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * roamRadius;
//
//		randomDirection += origin;
//
//		NavMeshHit navHit;
//
//		NavMesh.SamplePosition (randomDirection, out navHit, roamRadius, 1);
//
//		return navHit.position;

		Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		Vector3 finalPosition = Vector3.zero;
		if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1)) {
			finalPosition = hit.position;            
		}
		return finalPosition;

	}

	bool inRange(Vector3 firstVector, Vector3 secondVector, int threshold){
		float range = (firstVector - secondVector).magnitude;
		if (range < threshold) return true;
		else return false; 
	}
}