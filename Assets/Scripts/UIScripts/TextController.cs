using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextController : MonoBehaviour {

    public enum TextTypeEnum { INTERACTABLE, NARRATIVE };
    public TextTypeEnum textType;
    public float amplitude;
    public float floatSpeed;

    // View cones - might not need both actually...
    public GameObject interactableViewCone;
    public GameObject narrativeViewCone;
    public GameObject player;

    private float t;
    private bool FadingIn;
    private bool ChangingColor;
    private Text text;
    private Color fadeOutColor;
    private Color fadeInColor;
    private Color interactColor;


    void Start () {
        //narrativeViewCone = player.transform.Find("NarrativeConeCollider").gameObject;
        //interactableViewCone = player.transform.Find("InteractableConeCollider").gameObject;
        interactableViewCone = GameObject.FindWithTag("InteractableConeCollider");
        narrativeViewCone = GameObject.FindWithTag("NarrativeConeCollider");

        amplitude = 0.0007f;
        floatSpeed = 3f;
        text = gameObject.GetComponent<Text>();
        interactableViewCone.transform.localScale = narrativeViewCone.transform.localScale * 0.5f;
        //player = GameObject.FindGameObjectWithTag("Player");
      
        Initialise();
    }
    
    void Initialise()
    {
        text.color = new Color(1, 1, 1, 0);
        fadeOutColor = text.color;
        fadeInColor = new Color(1, 1, 1, 1);
        if(textType == TextTypeEnum.INTERACTABLE)
        {
            interactColor = new Color(1, 0.92f, 0.016f);
        }
    }
    
    void Update () {
        var y0 = transform.position.y;
        transform.position = new Vector3(transform.position.x, y0 + amplitude * Mathf.Sin(floatSpeed * Time.time), transform.position.z);
    }

    IEnumerator ChangeColorIn()
    {
        Debug.Log("Fading");
        ChangingColor = true;
        float timeToStart = Time.time;
        while (text.color != interactColor)
        {
            text.color = Color.Lerp(text.color, interactColor , (Time.time - timeToStart));
            yield return null;
        }
        Debug.Log("Changed Colour");
        ChangingColor = false;
    }


    IEnumerator ChangeColorOut()
    {
        while (ChangingColor)
        {
            yield return null;
            Debug.Log("Waiting for fade in to finish");
        }
        float timeToStart = Time.time;
        while (text.color != fadeInColor)
        {
            text.color = Color.Lerp(text.color, fadeInColor, (Time.time - timeToStart));
            yield return null;
        }
        Debug.Log("Back to transparent");
    }


    // Still a bit off when fading in and out lots of times
    IEnumerator FadeIn()
    {
        Debug.Log("Fading");
        FadingIn = true;
        float timeToStart = Time.time;
        while (text.color != fadeInColor)
        {
            text.color = Color.Lerp(text.color, fadeInColor, (Time.time - timeToStart));
            yield return null;
        }
        Debug.Log(" Faded In");
        FadingIn = false;
    }


    IEnumerator FadeOut()
    {
        while (FadingIn) {
            yield return null;
            Debug.Log("Waiting for fade in to finish");
        }
        float timeToStart = Time.time;
        while (text.color != fadeOutColor)
        {
            text.color = Color.Lerp(text.color, fadeOutColor, (Time.time - timeToStart));
            yield return null;
        }
        Debug.Log("Back to transparent");
    }

	//void FadeInDelegate()
	//{
	//	Debug.Log("Fading");
	//	FadingIn = true;
	//	float timeToStart = Time.time;
	//	while (text.color != new Color(1, 1, 1, 1))
	//	{
	//		text.color = Color.Lerp(text.color, new Color(1, 1, 1, 1), (Time.time - timeToStart) * 0.005f);
	//	}
	//	Debug.Log("Changed Colour");
	//	FadingIn = false;
	//}

	//void FadeOutDelegate(){
	//	while (FadingIn) {
	//		Debug.Log("Waiting for fade in to finish");
	//	}
	//	float timeToStart = Time.time;
	//	while (text.color != new Color(1, 1, 1, 0))
	//	{
	//		text.color = Color.Lerp(text.color, new Color(1, 1, 1, 0), (Time.time - timeToStart) * 0.005f);
	//	}
	//	Debug.Log("Back to transparent");
	//}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == narrativeViewCone)
        {
            Debug.Log("Fading In");
            StartCoroutine("FadeIn");
			//EventManager.ViewText += FadeInDelegate;
        }
        if (textType == TextTypeEnum.INTERACTABLE && other.gameObject == interactableViewCone)
        {
            Debug.Log("This is a thing");
            StopCoroutine("FadeIn");
            StartCoroutine("ChangeColorIn");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (textType == TextTypeEnum.INTERACTABLE && other.gameObject == interactableViewCone)
        {
            StartCoroutine("ChangeColorOut");
        }
        if (other.gameObject == narrativeViewCone)
        {
            StopCoroutine("ChangeColorOut");
            StartCoroutine("FadeOut");
            //EventManager.ViewText -= FadeInDelegate;
            //EventManager.ViewText += FadeOutDelegate;
        }
    }
}
