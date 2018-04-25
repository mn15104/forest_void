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
    public float finalClip;
    public float killTime;
    


    public Text mytext;
    
    // Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (Time.time > finalClip)
        {
            mytext.text = "Log no. 259 \nMessage received: 21:53 04.06.2015 \nReported: Missing persons \n Assigned: 13:53 05.06.2015 \nSerial Code: IGNM462HGH ";
        }

        else if (Time.time > tenth2Clip)
        {
            mytext.text = "";
        }

        else if (Time.time > tenthClip)
        {
            mytext.text = "I can't -";
        }

        else if (Time.time > ninthClip)
        {
            mytext.text = "It's coming back";
        }
        else if (Time.time > eighthClip)
        {
            mytext.text = "You have to help me";
        }
        else if (Time.time > seventh2Clip)
        {
            mytext.text = "";
        }
        else if (Time.time > seventhClip)
        {
            mytext.text = "I can't get out";
        }
        else if (Time.time > sixthClip)
        {
            mytext.text = "It's dark";
        }

        else if (Time.time > fifthClip)
        {
            mytext.text = "Something took me";
        }

        else if (Time.time > fourthClip)
        {
            mytext.text = "There was this house";
        }

        else if (Time.time > thirdClip)
        {
            mytext.text = "Please, I don't know where I am";
        }
        else if (Time.time > secondClip)
        {
            mytext.text = "... Hello?";
        }
        else if (Time.time > firstClip)
        {
            mytext.text = "Hello?";
        }

    }
}
