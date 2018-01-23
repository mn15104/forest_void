using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

	public CanvasGroup uiElement;

	public void FadeToBlack(){
		StartCoroutine (FadeCanvasGroup (uiElement, uiElement.alpha, 1));
		Debug.Log ("Faded out");
	}

	public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.5f ){

		float timeStartedLerping = Time.time;
		float timeSinceStarted = Time.time - timeStartedLerping;
		float percentageComplete = timeSinceStarted / lerpTime;

		while (true) {
			timeSinceStarted = Time.time - timeStartedLerping;
			percentageComplete = timeSinceStarted / lerpTime;

			float currentValue = Mathf.Lerp (start, end, percentageComplete);

			cg.alpha = currentValue;

			if (percentageComplete >= 1)
				break;

			yield return new WaitForEndOfFrame();
		}
	}
}
