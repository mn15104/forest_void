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

    private float t;
    private bool FadingIn;
    private Text text;
    private Color initialColor;
    private Color endColor;
    private GameObject ViewCone;


    void Start () {
        amplitude = 0.0007f;
        floatSpeed = 3f;
        text = gameObject.GetComponent<Text>();
        interactableViewCone.transform.localScale = narrativeViewCone.transform.localScale * 0.5f;

        Initialise();
    }
    
    void Initialise()
    {
        switch (textType)
        {
            case TextTypeEnum.INTERACTABLE:
                // white to yellow
                text.color = new Color(1, 1, 1, 1);
                initialColor = text.color;
                endColor = Color.yellow;
                //Cone size
                ViewCone = interactableViewCone;
                break;
            case TextTypeEnum.NARRATIVE:
                // transparent to white
                text.color = new Color(1, 1, 1, 0);
                initialColor = text.color;
                endColor = new Color(1, 1, 1, 1);
                //Cone size
                ViewCone = narrativeViewCone;
                break;
            default:
                text.color = new Color(1, 1, 1, 0);
                ViewCone = narrativeViewCone;
                break;
        }
    }
    
    void Update () {
        var y0 = transform.position.y;
        transform.position = new Vector3(transform.position.x, y0 + amplitude * Mathf.Sin(floatSpeed * Time.time), transform.position.z);
    }

    // Still a bit off when fading in and out lots of times
    IEnumerator FadeIn()
    {
        Debug.Log("Fading");
        FadingIn = true;
        float timeToStart = Time.time;
        while (text.color != endColor)
        {
            text.color = Color.Lerp(text.color, endColor, (Time.time - timeToStart));
            yield return null;
        }
        Debug.Log("Changed Colour");
        FadingIn = false;
    }

    IEnumerator FadeOut()
    {
        while (FadingIn) {
            yield return null;
            Debug.Log("Waiting for fade in to finish");
        }
        float timeToStart = Time.time;
        while (text.color != initialColor)
        {
            text.color = Color.Lerp(text.color, initialColor, (Time.time - timeToStart));
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
        if (other.gameObject == ViewCone)
        {
            Debug.Log("Fading In");
            StartCoroutine("FadeIn");
			//EventManager.ViewText += FadeInDelegate;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == ViewCone)
        {
            StartCoroutine("FadeOut");
            //EventManager.ViewText -= FadeInDelegate;
            //EventManager.ViewText += FadeOutDelegate;
        }
    }
}
