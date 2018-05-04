using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitlesScript : MonoBehaviour {

    public float firstClip;
    public float secondClip;
    public float thirdClip;
    public float fourthClip;
    public float fifthClip;
    public float sixthClip;
    public float seventhClip;
    public float seventh2Clip;
    public float eighthClip;
    public float ninthClip;
    public float tenthClip;
    public float tenth2Clip;
    private bool startclip = false;
    public float finalClip;
    public float killTime;
    private AudioSource distressCall;
    private float subtitleTimer = 0f;

    public Text mytext;
    
    // Use this for initialization
	void Start () {
        distressCall = GetComponentInChildren<AudioSource>();
        StartCoroutine(distressSubtitles());
    }
	
    IEnumerator distressSubtitles()
    {
        yield return new WaitForSeconds(2.5f);
        distressCall.Play();
        yield return new WaitForSeconds(3f);
        startclip = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (startclip)
        {
            subtitleTimer += Time.deltaTime;
            if (subtitleTimer > finalClip)
            {
                mytext.text = "Log no. 259 \nMessage received: 21:53 04.06.2015 \nReported: Missing persons \n Assigned: 13:53 05.06.2015 \nSerial Code: IGNM462HGH ";
            }

            else if (subtitleTimer > tenth2Clip)
            {
                mytext.text = "";
            }

            else if (subtitleTimer > tenthClip)
            {
                mytext.text = "I can't -";
            }

            else if (subtitleTimer > ninthClip)
            {
                mytext.text = "It's coming back";
            }
            else if (subtitleTimer > eighthClip)
            {
                mytext.text = "You have to help me";
            }
            else if (subtitleTimer > seventh2Clip)
            {
                mytext.text = "";
            }
            else if (subtitleTimer > seventhClip)
            {
                mytext.text = "I can't get out";
            }
            else if (subtitleTimer > sixthClip)
            {
                mytext.text = "It's dark";
            }

            else if (subtitleTimer > fifthClip)
            {
                mytext.text = "Something took me";
            }

            else if (subtitleTimer > fourthClip)
            {
                mytext.text = "There was this house";
            }

            else if (subtitleTimer > thirdClip)
            {
                mytext.text = "Please, I don't know where I am";
            }
            else if (subtitleTimer > secondClip)
            {
                mytext.text = "... Hello?";
            }
            else if (subtitleTimer > firstClip)
            {
                mytext.text = "Hello?";
            }

        }
    }
}
