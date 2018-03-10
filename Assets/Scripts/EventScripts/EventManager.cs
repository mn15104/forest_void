using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* THIS CLASS IS NOT USED ANYWHERE. CAN'T GET DELEGATES WORKING WITH COROUTINES. ALSO THE CLASS NAME IS NOT VERY CLEAR */ 

public class EventManager : MonoBehaviour {

	public delegate void TextAction();
	public static event TextAction ViewText;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (ViewText != null) {
			Debug.Log("View text is not null");
			Debug.Log (ViewText);
			ViewText();
		}
	}
}
