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
public partial class MonsterAI : MonoBehaviour {

    public delegate void MonsterStateChange(MonsterState monsterState);
    public event MonsterStateChange OnMonsterStateChange = delegate { };

    //Main Component Variables
    public GameObject player;
    private MonsterState currentState;
    public MonsterState debugState;
    public MonsterAppear currentAppear = MonsterAppear.STAGE1;
    private MonsterAIState m_MonsterStateMachine;
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
    private void Start()
    {
        m_MonsterStateMachine = new MonsterAIState(this);
    }
    void Update () {
        distanceToHuman = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2)
                                   + Mathf.Pow(player.transform.position.z - transform.position.z, 2));
        m_MonsterStateMachine.update_state();
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
    IEnumerator DelayStateChange(MonsterState state, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        m_MonsterStateMachine.SetState(state);
    }

    /* ---------> STAGE 1 <--------- */

    private bool fadingIn = false;
    private bool fadingOut = false;
    public float Stage1_TeleportInterval = 5f;
    public float Stage1_AppearInterval = 2f;
    public float Stage1_AppearDistance = 12.5f;
    IEnumerator UpdateStage1()
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
            TeleportVoidInfrontHuman_NoCollider(Stage1_AppearDistance);

            ////////////////////HIDE//////////////////////////
            inittime = 0f;
            while (e < 10f)
            {
                inittime += Time.deltaTime / 2f;
                yield return null;
                e = Mathf.Lerp(e, 10f, inittime);
                emission.rateOverTime = e;
            }
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            inittime = 0f;
            while (e > 0f)
            {
                inittime += Time.deltaTime / 4f;
                yield return null;
                e = Mathf.Lerp(e, 0f, inittime);
                emission.rateOverTime = e;
            }
            yield return null;
        }
    }


    /* ---------> STAGE 2 <--------- */

    bool stage2_playerTorchOn1 = false;
    bool stage2_playerTorchOff = false;
    bool stage2_playerTorchOn2 = false;
    bool stage2_playerTorchOff2 = false;
    bool stage2_coroutine_finished = false;
    void UpdateStage2()
    {
        if (!stage2_playerTorchOn1)
        {
            if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            {
                player.GetComponentInChildren<Flashlight>().Switch(gameObject);
                RenderSettings.fogMode = FogMode.ExponentialSquared;
                RenderSettings.fogDensity = RenderSettings.fogDensity * 5f;
                stage2_playerTorchOn1 = true;
            }
        }
        else if (!stage2_playerTorchOff)
        {
            if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            {
                TeleportVoidInfrontHuman(3f);
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = RenderSettings.fogDensity / 5f;
                stage2_playerTorchOff = true;
                StartCoroutine(Stage2_ToggleBool(0.4f));
            }
        }
        else if (!stage2_playerTorchOn2 && stage2_coroutine_finished)
        {
            if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            {
                player.GetComponentInChildren<Flashlight>().Switch(gameObject);
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                GetComponentInChildren<MeshRenderer>().enabled = false;
                RenderSettings.fogMode = FogMode.ExponentialSquared;
                RenderSettings.fogDensity = RenderSettings.fogDensity * 5f;
                stage2_playerTorchOn2 = true;
            }
        }
        else if (!stage2_playerTorchOff2 && stage2_playerTorchOn2)
        {
            if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            {
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = RenderSettings.fogDensity / 5f;
                m_MonsterStateMachine.SetState(MonsterState.HIDDEN_IDLE);
                currentAppear = MonsterAppear.STAGE2;
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
                GetComponentInChildren<MeshRenderer>().enabled = true;
                gameObject.SetActive(false);
            }
        }
    }
    IEnumerator Stage2_ToggleBool(float time)
    {
        yield return new WaitForSeconds(time);
        stage2_coroutine_finished = true;
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
        GetComponent<Rigidbody>().isKinematic = true;
        original_y = transform.position.y;
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        StartCoroutine(UpdateStage1());
    }

    float original_y;
    void InitialiseStage2()
    {

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
    void TeleportVoidInfrontHuman(float dist = 10)
    {
        original_y = transform.position.y;
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
        
        Vector3 voidPos = humanPos + dist * humanFacingDir;
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
        m_MonsterStateMachine.SetState(MonsterState.APPEAR);
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


    bool stage1_playerLookingAtMonster = false;
    bool stage2_coroutineCalled = false;
    bool stage3_invokeCalled = false;
    bool stage3_invokeFinished = false;
    void JumpscareBehindHuman()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        TeleportVoidBehindHuman(1f);
        stage3_invokeFinished = true;
    }
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
                    m_MonsterStateMachine.SetState(MonsterState.CHASE);
                }
            }
        }
    }

}