using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.PostProcessing;
public class VRCameraScript : MonoBehaviour {
    public Shader fishEyeShader;
    public GlitchEffect m_GlitchEffect;
    
    private PostProcessingProfile m_PostProcessProfile;
    private GrainModel.Settings grain;
    private VignetteModel.Settings vignette;

    private MonsterAI m_Monster;


    public bool effectsOn = false;
    [Range(0.0f, 100f)]
    public float timeLookedAtMonster = 0f;
    [Range(0.0f, 100f)]
    public float distanceToMonster = 0f;

    private const float m_PPP_GrainMaxIntensity = 0.5f;
    private const float m_PPP_GrainMaxSize = 1.7f;
    private const float m_PPP_GrainFixedLuminance = 0.5f;

    private const float m_PPP_VignetteMaxIntensity = 0.55f;
    private const float m_PPP_VignetteFixedSmoothness = 0.215f;
    private const float m_PPP_VignetteFixedRoundness = 1f;
    private const string m_PPP_VignetteFixedColor = "#2F0505";

    private const float m_GlitchMaxIntensity = 1f;
    private const float m_GlitchMaxFlipIntensity = 0.35f;
    private const float m_GlitchFixedColorIntensity = 0f;

    private const float m_FisheyeMax_X = 1f;
    private const float m_FisheyeMax_Y = 1f;

    private const float m_MinDistanceEffectTrigger = 10f;
    private Fisheye m_Fisheye;
    private float rateOfChange = 1f;
    private float upperBound = 0f;

    // Use this for initialization
    void Start () {
        m_Monster = FindObjectOfType<MonsterAI>();
        m_PostProcessProfile = gameObject.GetComponent<PostProcessingBehaviour>().profile;
        m_Fisheye = gameObject.AddComponent<Fisheye>();
        m_Fisheye.fishEyeShader = fishEyeShader;
        m_GlitchEffect = gameObject.GetComponent<GlitchEffect>();
        grain = m_PostProcessProfile.grain.settings;
        grain.intensity = 0f;
        grain.size = 0f;
        grain.luminanceContribution = 0.5f;
        m_PostProcessProfile.grain.settings = grain;
        vignette = m_PostProcessProfile.vignette.settings;
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
	
	// Update is called once per frame
	void Update () {
      /*
        if ((m_Monster.transform.position - transform.position).magnitude < m_MinEffectTrigger)
        {
            //Player looking at mannequin
            if (Vector3.Dot(transform.forward, (m_Monster.transform.position - transform.position).normalized) > 0)
            {
                effectsOn = true;
            }
            //Not looking at mannequin
            else
            {
                effectsOn = false;
            }
        }
        else if (effectsOn)
        {
            effectsOn = false;
        }
        */
        UpdateEffects();
    }

    void UpdateEffects()
    {
        //upperBound    = distanceToMonster;
        //rateOfChange =   m_MinDistanceEffectTrigger / distanceToMonster;
        if (!effectsOn) {
            UpdateVignette(0f);
            UpdateGrain(0f, 0f);
            UpdateFisheye(0f, 0f);
            UpdateGlitch(0f, 0f);
        }
        else{
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
        vignette = m_PostProcessProfile.vignette.settings;
        vignette.intensity = Mathf.Lerp(vignette.intensity, intensityDestination *(rateOfChange), Time.deltaTime * 1f * rateOfChange );
        m_PostProcessProfile.vignette.settings = vignette;
    }

    void UpdateGrain(float intensityDestination, float sizeDestination)
    {
        grain = m_PostProcessProfile.grain.settings;
        grain.intensity = Mathf.Lerp(grain.intensity, intensityDestination * (rateOfChange), Time.deltaTime * 1f * rateOfChange);
        grain.size = Mathf.Lerp(grain.intensity, sizeDestination * (rateOfChange), Time.deltaTime * 1f * rateOfChange);
        m_PostProcessProfile.grain.settings = grain;
    }

    void UpdateFisheye(float strengthXDestination, float strengthYDestination)
    {
        float distortx = m_Fisheye.strengthX;
        float distorty = m_Fisheye.strengthY;

        m_Fisheye.strengthX = Mathf.Lerp(distortx, strengthXDestination * (rateOfChange), Time.deltaTime * 1f * rateOfChange);
        m_Fisheye.strengthY = Mathf.Lerp(distorty, strengthYDestination * (rateOfChange), Time.deltaTime * 1.5f * rateOfChange);
        
        
    }

    void UpdateGlitch(float intensityDestination, float flipIntensityDestination)
    {
        float intensity = m_GlitchEffect.intensity;
        float flipIntensity = m_GlitchEffect.flipIntensity;
        m_GlitchEffect.intensity = Mathf.Lerp(intensity, intensityDestination * (upperBound / m_MinDistanceEffectTrigger), Time.deltaTime * 1f * rateOfChange);
        m_GlitchEffect.flipIntensity = Mathf.Lerp(flipIntensity, flipIntensityDestination * (upperBound / m_MinDistanceEffectTrigger), Time.deltaTime * 1f * rateOfChange);
    }



}
