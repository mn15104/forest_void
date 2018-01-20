using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {

    //Intro Fade In

    public Texture fadeIntro;
    private Canvas m_Canvas;
    private float alpha = 0f;
    private float fadeDir = 1;
    private float fadeSpeed = 0.2f;
    private float skyboxRotationRate = 4f;
    public Material daylightSkybox;
    public Material eveningSkybox;
    public Transform targetRotation;
    public AudioSource daylightAudio;
    public AudioSource glitchAudio;
    public AudioSource eveningAudio;
    private Quaternion lookrotation;
    Camera m_Camera;
    Light areaLight;
    bool playing = false;
    bool beginRotate = false;
    bool rotationDone = false;
    bool setFadeOut = false;
    int countRotations = 0;


    private void OnGUI()
    {
        if (setFadeOut)
        {
            if (alpha < 1f)
            {
                alpha += fadeDir * fadeSpeed * Time.deltaTime;
                alpha = Mathf.Clamp01(alpha);
            }
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = -1000;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeIntro);
             
        }
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyboxRotationRate);
    }

    // Use this for initialization
    void Start () {
        m_Canvas = FindObjectOfType<Canvas>();
        areaLight = GetComponentInChildren<Light>();
        m_Camera = GetComponentInChildren<Camera>();
        daylightAudio.enabled = true;
        daylightAudio.loop = true;
        eveningAudio.enabled = false;
        glitchAudio.enabled = false;
        
    }
	
	// Update is called once per frame
	void Update () {
        if (playing && !rotationDone)
        {
            if (areaLight.intensity > 0){
                areaLight.intensity -= Time.deltaTime;
            }
        }

        if (playing && beginRotate && !rotationDone)
        {
            Vector3 _direction = (targetRotation.position - m_Camera.transform.position).normalized;

            if (countRotations < 20 && Mathf.Abs((lookrotation.eulerAngles - m_Camera.transform.eulerAngles).magnitude) < 5f)
            {
                countRotations++;
                m_Camera.gameObject.GetComponent<GlitchEffect>().colorIntensity -= 0.02f;
                lookrotation = Random.rotation;
            }
            else if (countRotations >= 20)
                lookrotation = Quaternion.LookRotation(_direction);

            m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.transform.rotation, lookrotation, Time.deltaTime*10);

            if (Mathf.Abs((lookrotation.eulerAngles - m_Camera.transform.eulerAngles).magnitude) < 1f && countRotations >= 20)
            {
                Debug.Log((lookrotation.eulerAngles - m_Camera.transform.eulerAngles).magnitude);
                   rotationDone = true;
            }
            if (rotationDone)
            {
                StartCoroutine(StopGlitch());
            }
        }
        if (playing && eveningAudio.isActiveAndEnabled && eveningAudio.volume < 0.5f)
        {
            eveningAudio.volume += 0.001f;
        }
    }

    IEnumerator BeginRotate()
    {
        yield return new WaitForSeconds(1.5f);
        beginRotate = true;
        eveningAudio.enabled = true;
        eveningAudio.volume = 0f;

    }
    IEnumerator StopGlitch()
    {
        yield return new WaitForSeconds(1.5f);
        m_Camera.gameObject.GetComponent<GlitchEffect>().enabled = false;
        yield return new WaitForSeconds(2.5f);
        setFadeOut = true;
    }

    public void PlayGame()
    {
        lookrotation = Random.rotation;
        daylightAudio.enabled = false;
        glitchAudio.enabled = true;
        glitchAudio.loop = false;
        m_Camera.gameObject.GetComponent<GlitchEffect>().enabled = true;
        playing = true;
        RenderSettings.skybox = eveningSkybox;
        StartCoroutine(BeginRotate());
    }
}
