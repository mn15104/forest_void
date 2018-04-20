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
    ATTACK,
    GAMEOVER,
    STAGE_COMPLETE,
    DISABLED,
    HUMAN_IN_STRUCT
};

public enum BlendMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent
}

public partial class MonsterAI : MonoBehaviour
{

    public delegate void MonsterStateChange(MonsterState monsterState);
    public event MonsterStateChange OnMonsterStateChange = delegate { };

    //Main Component Variables
    private MonsterState currentState;
    public MonsterState debugState;
    public EventManager.Stage currentStage = EventManager.Stage.Intro;
    public GameObject player;
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
    private const float m_RunSpeed = 0.42f;
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
    private float distanceToHuman_FollowTrigger = 20;
    private float distanceToHuman_AppearTrigger = 10;
    private const float maxDetectionRange = 165;
    //State Utility Variables
    private float m_OriginalFogDensity;
    private bool follow_finished = false;
    private bool humanTorchOn = false;

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
        if (currentStage == EventManager.Stage.Intro)
        {
            debugState = MonsterState.DISABLED;
            currentState = MonsterState.DISABLED;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            anim.SetBool("Idle", true);
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
        }
        else
        {
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
        m_OriginalFogDensity = RenderSettings.fogDensity;
        m_MonsterStateMachine = new MonsterAIState(this);
    }

    void Update()
    {
        Debug.Log(currentStage);
        Debug.Log(currentState);
        distanceToHuman = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2)
                                    + Mathf.Pow(player.transform.position.z - transform.position.z, 2));
        m_MonsterStateMachine.update_state();
    }

    IEnumerator FadeInMaterial(Material mat, float fadeSpeed, bool isSkinned)
    {
        Color col = mat.color;
        float alpha = col.a;
        if (isSkinned)
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        else
            GetComponentInChildren<MeshRenderer>().enabled = true;

        while (alpha < 0.6f)
        {
            alpha += fadeSpeed * Time.deltaTime;
            mat.color = new Color(col.r, col.g, col.b, alpha);
            yield return null;
        }
    }

    IEnumerator FadeOutMaterial(Material mat, float fadeSpeed, bool isSkinned)
    {
        Color col = mat.color;
        float alpha = col.a;
        while (alpha > 0.0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            mat.color = new Color(col.r, col.g, col.b, alpha);
            yield return null;
        }
        if(isSkinned)
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        else
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }

    }

    void ChangeRenderMode(Material mat, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                mat.SetFloat("_Mode", 0);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = -1;
                break;
            case BlendMode.Fade:
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                break;
        }
    }
    /*------------------ UPDATE DESTINATION Functions --------------------*/

    public IEnumerator UpdateHiddenDestination()
    {
        while (true)
        {
            float detectionRate = (soundDetectionPercentage + lightDetectionPercentage) / 2;
            Vector3 directionToPlayer = player.transform.position - transform.position;
            Vector3 rot = Quaternion.LookRotation(directionToPlayer).eulerAngles;
            if (detectionRate == 0)
            {
                rot.y += (0.1f) / (2f * NextGaussianFloat());
            }
            else
            {
                rot.y += (detectionRate) / (2f * NextGaussianFloat());
            }

            destinationRotation = Quaternion.Euler(rot);
            yield return new WaitForSeconds(3);
        }
    }
    public IEnumerator UpdateChaseDestination()
    {
        while (true)
        {
            destinationPosition = player.transform.position;
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator DelayStateChange(MonsterState state, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        m_MonsterStateMachine.SetState(state);
    }
    public void SetState(MonsterState state_)
    {
        m_MonsterStateMachine.SetState(state_);
    }
    public void SetStage(EventManager.Stage stage)
    {
        if (stage != currentStage)
        {
            StopAllCoroutines();
           
            if (currentState == MonsterState.APPEAR)
            {
                if (currentStage == EventManager.Stage.Stage1)
                {
                    foreach (Material mat in GetComponentInChildren<SkinnedMeshRenderer>().materials)
                    {
                        ChangeRenderMode(mat, BlendMode.Opaque);
                    }
                    foreach (Material mat in GetComponentInChildren<MeshRenderer>().materials)
                    {
                        ChangeRenderMode(mat, BlendMode.Opaque);
                    }
                }
                else if (currentStage == EventManager.Stage.Stage2)
                {
                    GetComponent<Rigidbody>().isKinematic = false;
                    RenderSettings.fogMode = FogMode.Exponential;
                    RenderSettings.fogDensity = m_OriginalFogDensity;
                    foreach (Collider collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                }
                else if (currentStage == EventManager.Stage.Stage3)
                {

                }
            }
            currentStage = stage;
        }
    }
    /////////////// STAGE 1 ///////////////

    public float Stage1_TeleportInterval = 3f;
    public float Stage1_AppearInterval = 2f;
    public float Stage1_AppearDistance = 17f;
    public float Stage1_FadeOutTime = 0.1f;
    private int  Stage1_appearCount = 0;
    public const int Stage1_maxAppears = 10;
    public IEnumerator UpdateStage1()
    {
        //Set both mesh renders to fade and set alpha as zero before looping
        foreach (Material mat in GetComponentInChildren<SkinnedMeshRenderer>().materials)
            ChangeRenderMode(mat, BlendMode.Fade);
        foreach (Material mat in GetComponentInChildren<MeshRenderer>().materials)
            ChangeRenderMode(mat, BlendMode.Fade);
        foreach (Material mat in GetComponentInChildren<SkinnedMeshRenderer>().materials)
            StartCoroutine(FadeOutMaterial(mat, 2f, true));
        foreach (Material mat in GetComponentInChildren<MeshRenderer>().materials)
            StartCoroutine(FadeOutMaterial(mat, 2f, false));
        yield return new WaitForSeconds(1f);

        while (Stage1_appearCount < Stage1_maxAppears)
        {
            // Teleport and fade in
            TeleportVoidInfrontHuman_NoCollider(Stage1_AppearDistance);
            foreach (Material mat in GetComponentInChildren<SkinnedMeshRenderer>().materials)
                StartCoroutine(FadeInMaterial(mat, .5f, true));
            foreach (Material mat in GetComponentInChildren<MeshRenderer>().materials)
                StartCoroutine(FadeInMaterial(mat, .5f, false));
            yield return new WaitForSeconds(Mathf.Max(Stage1_AppearInterval, 2f));

            // Fade out
            foreach (Material mat in GetComponentInChildren<SkinnedMeshRenderer>().materials)
                StartCoroutine(FadeOutMaterial(mat, .5f, true));
            foreach (Material mat in GetComponentInChildren<MeshRenderer>().materials)
                StartCoroutine(FadeOutMaterial(mat, .5f, false));
            yield return new WaitForSeconds(Stage1_FadeOutTime);
            
            yield return new WaitForSeconds(Stage1_TeleportInterval);
            yield return null;
            Stage1_appearCount += 1;
        }

        m_MonsterStateMachine.SetState(MonsterState.STAGE_COMPLETE);
        /////// Need to reset render mode at some point 
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        GetComponentInChildren<MeshRenderer>().enabled = true;
        foreach (Material mat in GetComponentInChildren<SkinnedMeshRenderer>().materials)
            ChangeRenderMode(mat, BlendMode.Opaque);
        foreach (Material mat in GetComponentInChildren<MeshRenderer>().materials)
            ChangeRenderMode(mat, BlendMode.Opaque);
    }


    /////////////// STAGE 2 ///////////////

    public bool stage2_playerTorchOn1  = false;
    public bool stage2_playerTorchOff  = false;
    public bool stage2_playerTorchOn2  = false;
    public bool stage2_playerTorchOff2  = false;
    public bool stage2_coroutine0_finished = false;
    public bool stage2_coroutine1_finished  = false;
    public bool stage2_coroutine2_finished = false;
    public void UpdateStage2()
    {
        Flashlight flashlight = player.GetComponentInChildren<Flashlight>();
        if (!stage2_playerTorchOn1)
        {
            if (player.GetComponentInChildren<Flashlight>().m_FlashlightActive)
            {
                flashlight.SetDisableFlashlight(true);
                flashlight.ForceSwitchFlashlight(false);
                RenderSettings.fogMode = FogMode.ExponentialSquared;
                RenderSettings.fogDensity = RenderSettings.fogDensity * 5f;
                stage2_playerTorchOn1 = true;
                StartCoroutine(Stage2_ToggleCoroutine0(1.5f));
            }
        }
        else if (!stage2_playerTorchOff && stage2_coroutine0_finished)
        {
            TeleportVoidInfrontHuman(3f);
            flashlight.ForceSwitchFlashlight(true);
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = RenderSettings.fogDensity / 5f;
            stage2_playerTorchOff = true;
            StartCoroutine(Stage2_ToggleCoroutine1(0.2f));
        }
        else if (!stage2_playerTorchOn2 && stage2_coroutine1_finished)
        {
            flashlight.ForceSwitchFlashlight(false);
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = RenderSettings.fogDensity * 5f;
            stage2_playerTorchOn2 = true;
            StartCoroutine(Stage2_ToggleCoroutine2(5f));
        }
        else if (!stage2_playerTorchOff2 && stage2_playerTorchOn2 && stage2_coroutine2_finished)
        {
            m_MonsterStateMachine.SetState(MonsterState.STAGE_COMPLETE);
            flashlight.ForceSwitchFlashlight(true);
            player.GetComponentInChildren<Flashlight>().SetDisableFlashlight(false);
            player.GetComponentInChildren<Flashlight>().SetDisableFlicker(false);
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = RenderSettings.fogDensity / 5f;
            stage2_playerTorchOff2 = true;
        }
    }
    IEnumerator Stage2_ToggleCoroutine0(float time)
    {
        yield return new WaitForSeconds(time);
        stage2_coroutine0_finished = true;
    }
    IEnumerator Stage2_ToggleCoroutine1(float time)
    {
        yield return new WaitForSeconds(time);
        stage2_coroutine1_finished = true;
    }
    IEnumerator Stage2_ToggleCoroutine2(float time)
    {
        yield return new WaitForSeconds(time);
        stage2_coroutine2_finished = true;
    }


    float timer_Stage3Active = 0f;
    public bool Stage3_Appeared = false;
    public bool Stage3_playerWalking = false;
    /////////////// STAGE 3 ///////////////
    public void UpdateStage3()
    {
        if (!Stage3_Appeared) { 
            if (distanceToHuman > 3f)
            {
                TeleportVoidBehindHuman(2f);
                Stage3_playerWalking = true;
            }
        
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(player.GetComponentInChildren<Camera>());
            bool monsterInFrustum = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
            if (monsterInFrustum)
            {
                Vector3 dirToMonster = (transform.position - player.transform.position).normalized;
                float angleBetween = Vector3.Angle(dirToMonster, player.transform.forward);
                if (angleBetween < player.GetComponentInChildren<Camera>().fieldOfView / 1.35f)
                {
                    Stage3_Appeared = true;
                    StartCoroutine(DelayStateChange(MonsterState.APPROACH, 4f));
                }
            }
        }
    }
    
    /*------------------ UTILITY Functions --------------------*/

    public void InitialiseCurrentAppearBehaviour(EventManager.Stage t_stage)
    {
        switch (t_stage)
        {
            case EventManager.Stage.Stage1:
                InitialiseStage1();
                break;
            case EventManager.Stage.Stage2:
                InitialiseStage2();
                break;
            case EventManager.Stage.Stage3:
                InitialiseStage3();
                break;
        }
    }


    float original_y;
    public void InitialiseStage1()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        original_y = transform.position.y;
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        StartCoroutine(UpdateStage1());
    }

    public void InitialiseStage2()
    {
        player.GetComponentInChildren<Flashlight>().SetDisableFlicker(true);
    }

    public void InitialiseStage3()
    {
        TeleportVoidBehindHuman(2f);
    }

    public float Stage1_MinAngle = 1f;
    public float Stage1_MaxAngle = 3f;
    public void TeleportVoidInfrontHuman_NoCollider(float dist = 10)
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
    public void TeleportVoidInfrontHuman(float dist = 10)
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
    public void TeleportVoidBehindHuman(float dist = 10)
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

        transform.position = voidPos;
        Quaternion rot = Quaternion.LookRotation(player.transform.forward);
        rot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, rot.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = rot;
    }

    public float NextGaussianFloat()
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

    public void HumanSoundDetected(float soundHeard)
    {
        Debug.Log(soundHeard);
        Vector3 xyz_distance = player.transform.position - transform.position;
        Vector2 xz_distance = new Vector2(xyz_distance.x, xyz_distance.z);
        float distance = xz_distance.magnitude;

        soundDetectionPercentage = soundHeard * (distance / maxDetectionRange);
    }

    public void HumanLightDetected(bool on)
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
    public EventManager.Stage GetMonsterStage()
    {
        return currentStage;
    }
    public float GetMonsterSpeed()
    {
        return m_CurrentSpeed;
    }
    
    /*------------------ DEPRECATED Functions --------------------*/

    void OnCollisionEnter(Collision col)
    {
       
        if (currentState == MonsterState.ATTACK)
        {
            if (col.gameObject.GetComponent<HumanVRController>() ||
                col.gameObject.GetComponent<HumanController>())
            {
                SetState(MonsterState.GAMEOVER);
            }
        }
    }

    void ResetStageVariables()
    {
        GetComponentInChildren<MonsterAudioController>().Reset();
        Stage1_appearCount = 0;
        stage2_playerTorchOn1 = false;
        stage2_playerTorchOff = false;
        stage2_playerTorchOn2 = false;
        stage2_playerTorchOff2 = false;
        stage2_coroutine0_finished = false;
        stage2_coroutine1_finished = false;
        stage2_coroutine2_finished = false;
        Stage3_Appeared = false;
        follow_finished = false;
        player.GetComponentInChildren<Flashlight>().SetDisableFlashlight(false);
        player.GetComponentInChildren<Flashlight>().SetDisableFlicker(false);
        timer_Stage3Active = 0f;
}

}
