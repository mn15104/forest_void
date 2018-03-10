using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextController : MonoBehaviour {

    public float amplitude;
    public float speed;
    public GameObject ViewCone;
    private float t;
    private bool FadingIn;
    private Text text;

	// Use this for initialization
	void Start () {
        amplitude = 0.0007f;
        speed = 3f;
        text = gameObject.GetComponent<Text>();
        text.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update () {
        var y0 = transform.position.y;
        transform.position = new Vector3(transform.position.x, y0 + amplitude * Mathf.Sin(speed * Time.time), transform.position.z);
        //transform.position.Set(transform.position.x, y0 + amplitude * Mathf.Sin(speed * Time.time), transform.position.z);
 
    }

    IEnumerator FadeIn()
    {
        Debug.Log("Fading");
        FadingIn = true;
        float timeToStart = Time.time;
        while (text.color != new Color(1, 1, 1, 1))
        {
            text.color = Color.Lerp(text.color, new Color(1, 1, 1, 1), (Time.time - timeToStart) * 0.05f);
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
        while (text.color != new Color(1, 1, 1, 0))
        {
            text.color = Color.Lerp(text.color, new Color(1, 1, 1, 0), (Time.time - timeToStart) * 0.05f);
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
