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

public enum MonsterAppear
{
    NONE = -1,
    STAGE1 = 1,
    STAGE2 = 2,
    STAGE3 = 3,
    STAGE4 = 4,
    STAGE5 = 5
}

public class MonsterAI : MonoBehaviour {

    public delegate void MonsterStateChange(MonsterState monsterState);
    public event MonsterStateChange OnMonsterStateChange = delegate { };

    //Main Component Variables
    public GameObject player;
    private MonsterState currentState;
    public MonsterState debugState;
    public MonsterAppear currentAppear = MonsterAppear.STAGE1;
    private Animator anim;
	private GameObject trigger;
	private float chaseTimer = 10f;
    //Speed Variables
    private const float m_HiddenIdleSpeed = 0f;
    private const float m_HiddenMovingSpeed = 1.0f;
    private const float m_FollowSpeed = 1.0f;
    private const float m_AppearSpeed = 1.0f;
    private const float m_MinApproachSpeed = 0.3f;
    private const float m_MaxApproachSpeed = 3.0f;
    private const float m_RunSpeed = 0.35f;
    private float m_CurrentSpeed = 1f;
    //Destination Variables
    private Quaternion destinationRotation;
    private Vector3 destinationPosition;
    private bool collisionRight;
    private bool collisionLeft;
    private AICollisionSide firstCollision;
    //Human Detection Variables
    public float soundDetectionPercentage = 0.2f;
    public float lightDetectionPercentage = 0.2f;
    private float distanceToHuman;
    private float distanceToHuman_FollowTrigger = 12;
    private float distanceToHuman_AppearTrigger = 6;
    private const float maxDetectionRange = 165;
    //State Utility Variables
    private bool follow_finished;
    private bool humanTorchOn;

    /*------------------ STANDARD Functions --------------------*/

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
        if (currentAppear == MonsterAppear.STAGE1)
        {
            distanceToHuman_FollowTrigger = 12;
            distanceToHuman_AppearTrigger = 6;
        }
        else if (currentAppear == MonsterAppear.STAGE2)
        {
            distanceToHuman_FollowTrigger = 15;
            distanceToHuman_AppearTrigger = 12;
        }
        else if (currentAppear == MonsterAppear.STAGE3)
        {
            distanceToHuman_FollowTrigger = 15;
            distanceToHuman_AppearTrigger = 12;
        }
    }

    private void OnDisable()
    {
        HumanVRRightHand.OnHumanLightEmission -= HumanLightDetected;
        HumanVRAudioController.OnHumanAudioEmission -= HumanSoundDetected;
        MonsterTrigger.OnHumanDetected -= HumanDetected;
        m_CurrentSpeed = m_HiddenIdleSpeed;
        soundDetectionPercentage = 0.2f;
        lightDetectionPercentage = 0.2f;
        chaseTimer = 16f;
        distanceToHuman = Mathf.Infinity;
        collisionRight = false;
        collisionLeft = false;
    }
    
    void Update () {

        distanceToHuman = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2)
                                   + Mathf.Pow(player.transform.position.z - transform.position.z, 2));
        if (currentState != debugState)
            SetState(debugState);
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

    /*------------------ SET Functions --------------------*/

    public void SetState(MonsterState state)
    {
        if (state != currentState)
        {
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
                    InitialiseCurrentAppearBehaviour(currentAppear);           // CALL APPEAR BEHAVIOUR TYPE
                    follow_finished = false;                                   // Reset follow bool
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Idle", true);
                    anim.SetFloat("Speed", m_AppearSpeed);
                    m_CurrentSpeed = m_AppearSpeed;
                    break;
                case MonsterState.APPROACH:
                    StopAllCoroutines();
                    StartCoroutine(UpdateChaseDestination());
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", true);
                    anim.SetBool("Run", false);
                    anim.SetFloat("Speed", m_MinApproachSpeed);
                    m_CurrentSpeed = m_MinApproachSpeed;
                    StartCoroutine(DelayStateChange(MonsterState.CHASE, 4f));
                    break;
                case MonsterState.CHASE:
                    StopAllCoroutines();
                    StartCoroutine(UpdateChaseDestination());
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);
                    anim.SetFloat("Speed", m_RunSpeed);
                    m_CurrentSpeed = m_RunSpeed;
                    NextAppearStage();
                    break;
                case MonsterState.GAMEOVER:
                    GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                    anim.SetBool("Idle", true);
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", false);
                    break;
                default:
                    hidden_idle();
                    break;
            }
            stage1_playerLookingAtMonster = false;
            stage2_coroutineCalled = false;
            stage3_invokeCalled = false;
            debugState = state;
            currentState = state;
            OnMonsterStateChange(state);
        }
    }

    IEnumerator DelayStateChange(MonsterState state, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SetState(state);
    }

    /*------------------ UPDATE DESTINATION Functions --------------------*/

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

    /*------------------ STATE UPDATE Functions --------------------*/

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

        if (!follow_finished && (currentAppear == MonsterAppear.STAGE1 || currentAppear == MonsterAppear.NONE))
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
        if (!follow_finished && (currentAppear == MonsterAppear.STAGE2))
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
                StartCoroutine(DelayStateChange(MonsterState.APPEAR, 0f));
                follow_finished = true;
            }
        }
    }
    
    void appear () {
        if(currentAppear == MonsterAppear.STAGE1)
        {
            UpdateStage1();
        }
        else if (currentAppear == MonsterAppear.STAGE2)
        {
            ///////////////
        }
    }
    bool stage1_playerTorchOn1 = false;
    bool stage1_playerTorchOff = false;
    bool stage1_coroutinefinished = false;
    bool stage1_playerTorchOn2 = false;
    void UpdateStage1()
    {
        if (!stage1_playerTorchOn1)
        {
            if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            {
                stage1_playerTorchOn1 = true;
            }
        }
        else if (!stage1_playerTorchOff)
        {
            if (stage1_coroutinefinished)
            {
                stage1_playerTorchOff = true;
            }
        }
        else if (!stage1_playerTorchOn2)
        {
            if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            {
                SetState(MonsterState.HIDDEN_IDLE);
                currentAppear = MonsterAppear.STAGE2;
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = RenderSettings.fogDensity / 5f;
                stage1_playerTorchOn2 = true;
                gameObject.SetActive(false);
            }
        }
    }

    private bool fadingIn = false;
    private bool fadingOut = false;
    public float Stage2_TeleportInterval = 5f;
    public float Stage2_AppearInterval = 2f;
    public float Stage2_AppearDistance = 12.5f;
    IEnumerator UpdateStage2()
    {
        float e = 0f;
        while (true)
        {
            ////////////////////APPEAR//////////////////////////
            float inittime = 0f;
            ParticleSystem.EmissionModule emission
                = GetComponentInChildren<ParticleSystem>().emission;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            GetComponentInChildren<MeshRenderer>().enabled = true;
            TeleportVoidInfrontHuman_NoCollider(Stage2_AppearDistance);

            ////////////////////HIDE//////////////////////////
            inittime = 0f;
            while (e < 10f)
            {
                inittime += Time.deltaTime / 2f;
                yield return null;
                e = Mathf.Lerp(e,
                                        10f, inittime);
                emission.rateOverTime = e;
            }
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            inittime = 0f;
            while (e > 0f)
            {
                inittime += Time.deltaTime / 4f;
                yield return null;
                e = Mathf.Lerp(e,
                                        0f, inittime);
                emission.rateOverTime = e;
            }
            yield return null;
        }
    }
    
    IEnumerator Stage1Appear()
    {
        TeleportVoidBehindHuman(-10f);
        StartCoroutine(DelayStateChange(MonsterState.APPROACH, 5f));
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = RenderSettings.fogDensity / 5f;
        yield return new WaitForSeconds(0.4f);
        if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
        {
            player.GetComponentInChildren<Flashlight>().Switch(gameObject);
        }
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = RenderSettings.fogDensity * 5f;
        stage1_coroutinefinished = true;
    }
    bool stage1_playerLookingAtMonster = false;
    bool stage2_coroutineCalled = false;
    bool stage3_invokeCalled = false;
    bool stage3_invokeFinished = false;
   
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

    /*------------------ UTILITY Functions --------------------*/

    void InitialiseCurrentAppearBehaviour(MonsterAppear appear)
    {
        switch (appear)
        {
            case MonsterAppear.NONE:
                break;
            case MonsterAppear.STAGE1:
                InitialiseStage1();
                break;
            case MonsterAppear.STAGE2:
                InitialiseStage2();
                break;
            case MonsterAppear.STAGE3:
                InitialiseStage3();
                break;
        }
    }

    void NextAppearStage()
    {
        switch (currentAppear)
        {
            case MonsterAppear.NONE:
                break;
            case MonsterAppear.STAGE1:
                currentAppear = MonsterAppear.STAGE2;
                distanceToHuman_FollowTrigger = 20f;
                distanceToHuman_AppearTrigger = 15f;
                break;
            case MonsterAppear.STAGE2:
                currentAppear = MonsterAppear.STAGE3;
                distanceToHuman_FollowTrigger = 15f;
                distanceToHuman_AppearTrigger = 10f;
                break;
            case MonsterAppear.STAGE3:
                currentAppear = MonsterAppear.STAGE1;
                distanceToHuman_FollowTrigger = 12f;
                distanceToHuman_AppearTrigger = 6f;
                break;
        }
    }

    void InitialiseStage1()
    {
        if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            player.GetComponentInChildren<Flashlight>().Switch(gameObject);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = RenderSettings.fogDensity * 5f;
    }

    float original_y;
    void InitialiseStage2()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        original_y = transform.position.y;
        foreach(Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        StartCoroutine(UpdateStage2());
    }

    void InitialiseStage3()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        TeleportVoidBehindHuman(1f);
    }

    public float Stage1_MinAngle = 1f;
    public float Stage1_MaxAngle = 3f;
    void TeleportVoidInfrontHuman_NoCollider(float dist = 10)
    {
        Vector3 humanPos = player.transform.position;
        Vector3 humanFacingDir = player.transform.forward;
        Vector3 humanRightDir = player.transform.right;
        foreach (Camera cam in player.GetComponentsInChildren<Camera>())
        {
            if (cam.isActiveAndEnabled)
            {
                humanFacingDir = cam.transform.forward;
            }
        }

        float isAppearLeft = (Random.value >= 0.5) ? 1f : -1f;
        float sidewaysAppearOffset = Random.Range(Stage1_MinAngle, Stage1_MaxAngle);

        Vector3 voidPos = humanPos + dist * humanFacingDir + isAppearLeft * sidewaysAppearOffset * humanRightDir;
        voidPos.y = original_y;
        transform.position = voidPos;
        Quaternion rotat = Quaternion.LookRotation(-player.transform.forward);
        rotat.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, rotat.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = rotat;
    }
    void TeleportVoidBehindHuman(float dist = 10)
    {
        float orig_y = transform.position.y;
        Vector3 humanPos = player.transform.position;
        Vector3 humanFacingDir = player.transform.forward;
        foreach (Camera cam in player.GetComponentsInChildren<Camera>())
        {
            if (cam.isActiveAndEnabled)
            {
                humanFacingDir = cam.transform.forward;
            }
        }
        Vector3 voidPos = humanPos - dist * humanFacingDir;
        voidPos.y = orig_y;
        int mask = LayerMask.GetMask("Trees", "Rocks", "Structures");
        int giveUpCounter = 0;
        var checkResult = Physics.OverlapSphere(voidPos, 0.5f, mask);
        Vector3 voidPosAttempt = voidPos;
        while (checkResult.Length == 0 && giveUpCounter < 20)
        {
            voidPosAttempt = voidPos;
            voidPosAttempt.x += Random.Range(-1f, 2f);
            voidPosAttempt.z += Random.Range(-1f, 2f);
            giveUpCounter++;
            checkResult = Physics.OverlapSphere(voidPosAttempt, 0.5f, mask);
        }
        if (giveUpCounter >= 20)
        {
            Vector3 lastAttempt = player.transform.position - dist * player.transform.forward;
            lastAttempt.y = orig_y;
            transform.position = lastAttempt;
            Quaternion rotat = Quaternion.LookRotation(player.transform.forward);
            rotat.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, rotat.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation = rotat;
            return;
        }
        transform.position = voidPosAttempt;
        Quaternion rot = Quaternion.LookRotation(player.transform.forward);
        rot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, rot.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = rot;
    }
    void JumpscareBehindHuman()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        TeleportVoidBehindHuman(1f);
        stage3_invokeFinished = true;
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

    /*------------------ Detection Functions --------------------*/

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
        Debug.Log(soundHeard);
        Vector3 xyz_distance = player.transform.position - transform.position;
        Vector2 xz_distance = new Vector2(xyz_distance.x, xyz_distance.z);
        float distance = xz_distance.magnitude;

        soundDetectionPercentage = soundHeard * (distance / maxDetectionRange);
    }

    void HumanLightDetected(bool on)
    {
        Debug.Log(on);
        Vector3 xyz_distance = player.transform.position - transform.position;
        Vector2 xz_distance = new Vector2(xyz_distance.x, xyz_distance.z);
        float distance = xz_distance.magnitude;
        if (on)
        {
            humanTorchOn = true;
            lightDetectionPercentage = (distance / maxDetectionRange);
        }
        else
        {
            humanTorchOn = false;
            lightDetectionPercentage = 0f;
        }
    }

    /*------------------ GET Functions --------------------*/
    public MonsterState GetMonsterState()
    {
        return currentState;
    }
    public float GetMonsterSpeed()
    {
        return m_CurrentSpeed;
    }
    
    
    /*------------------ DEPRECATED Functions --------------------*/
    void old_appear()
    {
        
        if (currentAppear == MonsterAppear.STAGE1 && !stage1_playerLookingAtMonster)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(player.GetComponentInChildren<Camera>());
            bool monsterInFrustum = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
            if (monsterInFrustum)
            {
                Vector3 dirToMonster = (transform.position - player.transform.position).normalized;
                float angleBetween = Vector3.Angle(dirToMonster, player.transform.forward);
                if (angleBetween < player.GetComponentInChildren<Camera>().fieldOfView / 1.35f)
                {
                    stage1_playerLookingAtMonster = true;
                }
            }
            if (stage1_playerLookingAtMonster)
            {
                StartCoroutine(DelayStateChange(MonsterState.APPROACH, 5f));
            }
            else if ((player.transform.position - transform.position).magnitude > 5f)
            {
                TeleportVoidBehindHuman(4);
            }
        }
        else if (currentAppear == MonsterAppear.STAGE2 && !stage2_coroutineCalled)
        {
            StartCoroutine(DelayStateChange(MonsterState.APPROACH, 2f));
            stage2_coroutineCalled = true;
        }
        else if (currentAppear == MonsterAppear.STAGE3 && !stage3_invokeCalled)
        {
            Invoke("JumpscareBehindHuman", 8f);
            stage3_invokeCalled = true;
        }
        if (stage3_invokeFinished)
        {
            Debug.Log("Here");
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(player.GetComponentInChildren<Camera>());
            bool monsterInFrustum = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
            if (monsterInFrustum)
            {
                Vector3 dirToMonster = (transform.position - player.transform.position).normalized;
                Debug.Log(dirToMonster);
                float angleBetween = Vector3.Angle(dirToMonster, player.transform.forward);
                if (angleBetween < player.GetComponentInChildren<Camera>().fieldOfView / 1.35f)
                {
                    SetState(MonsterState.CHASE);
                }
            }
        }
    }

}