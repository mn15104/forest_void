using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class MonsterAI : MonoBehaviour {

    public delegate void StateChange(State monsterState);
    public static event StateChange OnStateChange;

    //Components
    public AudioSource jumpScareSound;
    public AudioSource chaseSound;
	public enum State {HIDDEN, APPEAR, CHASE};
	public State currentState;
	public GameObject player;
	private Vector3 appearPosition;
	private Animator anim;
	private GameObject trigger;

	//GAME OVER
	public Text gameOverText;
	private bool monsterApproach;
	private bool caughtByMonster;


	// NavMesh 
	private UnityEngine.AI.NavMeshAgent agent;
	private float pathTimer;
	private float chaseTimer;

//	//Calculating whether the monster will appear or not
//	private float timeFromStart;
//	private float endTime;
//	private float terrainSize;

//	//FreeRoam
//	public float roamRadius;
//	public float wanderTimer;


	// Use this for initialization

	void Start () {
	
        anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		gameOverText.text = "";
		pathTimer = 1;
		chaseTimer = 10;
		monsterApproach = false;
		caughtByMonster = false;
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
        OnStateChange(state);
        currentState = state;
	}
	public void setPostion(Vector3 position){
		appearPosition = position;
	}

	State hidden () {
		transform.Find ("meshes").gameObject.SetActive(false);
		return State.HIDDEN;
	}

	State appear () {
		transform.Find ("meshes").gameObject.SetActive(true);
		transform.position = appearPosition;
        jumpScareSound.enabled = true;
        jumpScareSound.Play();
		return State.CHASE;
	}
		
	State chase () {
        Debug.Log("chasing");
		monsterApproach = true; //Screen starts flashing red
		anim.SetTrigger ("StartRunning");
        StartCoroutine(ChaseMusic());
       
		//Chase player
		pathTimer -= Time.deltaTime;
		chaseTimer -= Time.deltaTime;
		if (pathTimer < 0) {
			agent.SetDestination (player.transform.position);
			pathTimer = 1;
	

			//Debug.Log (chaseTimer);
			if (chaseTimer < 0) {
				anim.SetTrigger ("StopRunning");
				agent.isStopped = true;
				monsterApproach = false;
				return State.HIDDEN;
			}
		}

		// GAME OVER
		float distance = (player.transform.position - transform.position).magnitude;
		Debug.Log (distance);
		if (distance < 1f) {
			monsterApproach = false;
			caughtByMonster = true;
			agent.speed = 0;
			anim.SetTrigger ("StopRunning");

			Debug.Log ("Game over");
			GameOver ();
		}
		return State.CHASE;
	}



    void GameOver(){
		gameOverText.text = "GAME OVER";
	}

	void OnGUI() { 
		Texture2D t;
		Color currentBlendColor;
		Color toColor;

		if (monsterApproach == true) {
			t = new Texture2D (1, 1);
			t.SetPixel (0, 0, Color.white);
			currentBlendColor = new Color (1, 0, 0, 0.25f); 
			//Color fromColor = new Color( 1, 0, 0, 1 ); 
			toColor = new Color (0, 0, 0, 0);

			// Now each GUI draw the texture and blend it in. 
			currentBlendColor = Color.Lerp (currentBlendColor, toColor, Mathf.PingPong (Time.time, 0.75f)); 
			// Set GUI color 
			GUI.color = currentBlendColor;
			// Draw fade 
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), t, ScaleMode.StretchToFill);
		}
		if (caughtByMonster == true) {
			t = new Texture2D (1, 1);
			t.SetPixel (0, 0, Color.white);
			currentBlendColor = new Color (0, 0, 0, 0); 
			//Color fromColor = new Color( 1, 0, 0, 1 ); 
			toColor = new Color (0, 0, 0, 1);

			// Now each GUI draw the texture and blend it in. 
			currentBlendColor = Color.Lerp (currentBlendColor, toColor, 1); 
			// Set GUI color 
			GUI.color = currentBlendColor;
			// Draw fade 
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), t, ScaleMode.StretchToFill);
		}
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