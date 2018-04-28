using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.PostProcessing;
public class VRCameraScript : MonoBehaviour
{

    public Shader fishEyeShader;
    private PostProcessingProfile m_PostProcessProfile;
    private GlitchEffect m_GlitchEffect;
    public GameObject m_Monster;
    private Camera m_Camera;

    public bool effectsOn = false;
    public bool isLookingAtMonster = false;
    [Range(0.0f, 5f)]
    public float timeLookedAtMonster = 0f;
    [Range(0.0f, 10f)]
    public float distanceToMonster = 10f;

    private const float m_PPP_GrainMaxIntensity = 0.3f;
    private const float m_PPP_GrainMaxSize = 1.5f;
    private const float m_PPP_GrainFixedLuminance = 0.35f;

    private const float m_PPP_VignetteMaxIntensity = 0.45f;
    private const float m_PPP_VignetteFixedSmoothness = 0.215f;
    private const float m_PPP_VignetteFixedRoundness = 1f;
    private const string m_PPP_VignetteFixedColor = "#2F0505";

    private const float m_GlitchMaxIntensity = 0.8f;
    private const float m_GlitchMaxFlipIntensity = 0.35f;
    private const float m_GlitchFixedColorIntensity = 0f;

    private const float m_FisheyeMax_X = 0.8f;
    private const float m_FisheyeMax_Y = 0.8f;

    private const float m_MinDistanceEffectTrigger = 15f;
    private const float m_MinDistanceLookingEffectTrigger = 10f;

    private Fisheye m_Fisheye;
    private float rateOfChange = 0f;
    Plane[] cameraPlanes;

    private GrainModel.Settings originalGrainSettings;
    private VignetteModel.Settings originalVignetteSettings;
    private ColorGradingModel.Settings originalColorGradSettings;
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_Monster = FindObjectOfType<MonsterAI>().gameObject;
        m_PostProcessProfile = gameObject.GetComponent<PostProcessingBehaviour>().profile;
        originalGrainSettings = m_PostProcessProfile.grain.settings;
        originalVignetteSettings = m_PostProcessProfile.vignette.settings;
        originalColorGradSettings = m_PostProcessProfile.colorGrading.settings;

        m_Fisheye = gameObject.AddComponent<Fisheye>();
        m_Fisheye.fishEyeShader = fishEyeShader;
        m_GlitchEffect = gameObject.GetComponent<GlitchEffect>();
        GrainModel.Settings grain = m_PostProcessProfile.grain.settings;
        grain.intensity = 0f;
        grain.size = 0f;
        grain.luminanceContribution = 0.5f;
        m_PostProcessProfile.grain.settings = grain;
        VignetteModel.Settings vignette = m_PostProcessProfile.vignette.settings;
        vignette.intensity = 0f;
        vignette.smoothness = 0.215f;
        vignette.roundness = 1f;
        m_PostProcessProfile.vignette.settings = vignette;
        m_Fisheye.strengthX = 0f;
        m_Fisheye.strengthY = 0f;
        m_GlitchEffect.intensity = 0f;
        m_GlitchEffect.flipIntensity = 0f;
        m_GlitchEffect.colorIntensity = 0f;
    }
    private void OnDestroy()
    {
        m_PostProcessProfile.grain.settings = originalGrainSettings;
        m_PostProcessProfile.vignette.settings = originalVignetteSettings;
        m_PostProcessProfile.colorGrading.settings = originalColorGradSettings;
    }

    // Update is called once per frame
    void Update()
    {
        cameraPlanes = GeometryUtility.CalculateFrustumPlanes(m_Camera);
        EventManager.Stage monsterstage = m_Monster.GetComponent<MonsterAI>().GetMonsterStage();
        MonsterState monsterstate = m_Monster.GetComponent<MonsterAI>().GetMonsterState();

        // Ignore effects
        if (monsterstage == EventManager.Stage.Intro
            || monsterstage == EventManager.Stage.Stage1
            || monsterstate == MonsterState.DISABLED
            || monsterstate == MonsterState.STAGE_COMPLETE
            || monsterstate == MonsterState.HUMAN_IN_STRUCT)
        {
            effectsOn = false;
            return;
        }
        // Fade screen to black
        else if (monsterstage == EventManager.Stage.GameOverStage)
        {
            GetComponent<Camera>().backgroundColor = Color.black; 
            GetComponent<Camera>().farClipPlane = GetComponent<Camera>().nearClipPlane;
        }
        // Process effects as usual
        else
        {
            distanceToMonster = (m_Monster.transform.position - transform.position).magnitude;
            if (distanceToMonster < m_MinDistanceEffectTrigger)
            {
                if (!effectsOn) effectsOn = true;
                if (distanceToMonster < m_MinDistanceLookingEffectTrigger &&
                    GeometryUtility.TestPlanesAABB(cameraPlanes, m_Monster.GetComponent<Collider>().bounds))
                {
                    if (!isLookingAtMonster) isLookingAtMonster = true;
                    timeLookedAtMonster = Mathf.Min(2.5f, timeLookedAtMonster + Time.deltaTime);
                }
                else
                {
                    if (isLookingAtMonster) isLookingAtMonster = false;
                    timeLookedAtMonster = Mathf.Max(0, timeLookedAtMonster - Time.deltaTime);
                }
            }
            else if (effectsOn)
            {
                effectsOn = false;
            }
            UpdateEffects();
        }
    }

    void UpdateEffects()
    {

        rateOfChange = Mathf.Clamp(timeLookedAtMonster, 1, 5);
        if (!effectsOn)
        {
            UpdateVignette(0f);
            UpdateGrain(0f, 0f);
            UpdateFisheye(0f, 0f);
            UpdateGlitch(0f, 0f);
        }
        else
        {
            ////////////////////////VIGNETTE//////////////////////////
            UpdateVignette(m_PPP_VignetteMaxIntensity);

            //////////////////////////GRAIN///////////////////////////
            UpdateGrain(m_PPP_GrainMaxIntensity, m_PPP_GrainMaxSize);

            /////////////////////////FISHEYE//////////////////////////
            UpdateFisheye(m_FisheyeMax_X, m_FisheyeMax_Y);

            /////////////////////////GLITCH///////////////////////////
            UpdateGlitch(m_GlitchMaxIntensity, m_GlitchMaxFlipIntensity);
        }

    }

    void UpdateVignette(float intensityDestination)
    {
        float upperBoundVig = Mathf.Min(intensityDestination * (m_MinDistanceEffectTrigger / distanceToMonster), m_PPP_VignetteMaxIntensity);
        float maskedRateOfChangeVig = Time.deltaTime * 0.4f * rateOfChange;
        if (!isLookingAtMonster)
        {
            maskedRateOfChangeVig = maskedRateOfChangeVig * 0.6f + Time.deltaTime * 1f;
            upperBoundVig *= 0.7f + Time.deltaTime * 4f;
        }
        VignetteModel.Settings vignette = m_PostProcessProfile.vignette.settings;
        vignette.intensity = Mathf.Lerp(vignette.intensity, upperBoundVig, maskedRateOfChangeVig);
        m_PostProcessProfile.vignette.settings = vignette;
    }
    void UpdateGlitch(float intensityDestination, float flipIntensityDestination)
    {
        float upperBoundIntensity = Mathf.Min(intensityDestination * (m_MinDistanceEffectTrigger / distanceToMonster), m_GlitchMaxIntensity);
        float upperBoundFlip = Mathf.Min(flipIntensityDestination * (m_MinDistanceEffectTrigger / distanceToMonster), m_GlitchMaxFlipIntensity);
        float maskedRateOfChangeIntensity = Time.deltaTime * 4f * rateOfChange;
        float maskedRateOfChangeFlip = Time.deltaTime * 2f * rateOfChange;
        if (!isLookingAtMonster)
        {
            maskedRateOfChangeIntensity = maskedRateOfChangeIntensity * 0.8f + Time.deltaTime * 8f;
            maskedRateOfChangeFlip = maskedRateOfChangeFlip * 0.8f + Time.deltaTime * 4f;
            upperBoundIntensity *= 0.8f;
            upperBoundFlip *= 0.8f;
        }
        float intensity = m_GlitchEffect.intensity;
        float flipIntensity = m_GlitchEffect.flipIntensity;

        m_GlitchEffect.intensity = Mathf.Lerp(0, upperBoundIntensity, maskedRateOfChangeIntensity);
        m_GlitchEffect.flipIntensity = Mathf.Lerp(0, upperBoundFlip, maskedRateOfChangeFlip);
    }

    void UpdateGrain(float intensityDestination, float sizeDestination)
    {
        float upperBoundIntensity = Mathf.Min(intensityDestination * (m_MinDistanceEffectTrigger / distanceToMonster), m_PPP_GrainMaxIntensity);
        float upperBoundSize = Mathf.Min(sizeDestination * (m_MinDistanceEffectTrigger / distanceToMonster), m_PPP_GrainMaxSize);
        float maskedRateOfChangeIntensity = Time.deltaTime * 10f * rateOfChange;
        float maskedRateOfChangeSize = Time.deltaTime * 10f * rateOfChange;
        if (!isLookingAtMonster)
        {
            maskedRateOfChangeIntensity = maskedRateOfChangeIntensity * 0.8f + Time.deltaTime * 20f;
            maskedRateOfChangeSize = maskedRateOfChangeSize * 0.8f + Time.deltaTime * 20f;
            upperBoundIntensity *= 0.75f;
            upperBoundSize *= 0.75f;
        }

        GrainModel.Settings grain = m_PostProcessProfile.grain.settings;
        grain.intensity = Mathf.Lerp(0, upperBoundIntensity, maskedRateOfChangeIntensity);
        grain.size = Mathf.Lerp(0, upperBoundSize, maskedRateOfChangeSize);
        m_PostProcessProfile.grain.settings = grain;
    }

    void UpdateFisheye(float strengthXDestination, float strengthYDestination)
    {
        float upperBoundX = Mathf.Min(strengthXDestination * (m_MinDistanceEffectTrigger / distanceToMonster), m_FisheyeMax_X);
        float upperBoundY = Mathf.Min(strengthYDestination * (m_MinDistanceEffectTrigger / distanceToMonster), m_FisheyeMax_Y);
        float maskedRateOfChangeX = Time.deltaTime * 1f * rateOfChange;
        float maskedRateOfChangeY = Time.deltaTime * 1.5f * rateOfChange;
        if (!isLookingAtMonster)
        {
            maskedRateOfChangeX = maskedRateOfChangeX * 0.8f + Time.deltaTime * 2f;
            maskedRateOfChangeY = maskedRateOfChangeY * 0.8f + Time.deltaTime * 3f;
            upperBoundX *= 0.6f;
            upperBoundY *= 0.6f;
        }
        float distortx = m_Fisheye.strengthX;
        float distorty = m_Fisheye.strengthY;

        m_Fisheye.strengthX = Mathf.Lerp(0, upperBoundX, maskedRateOfChangeX);
        m_Fisheye.strengthY = Mathf.Lerp(0, upperBoundY, maskedRateOfChangeY);


    }




}
