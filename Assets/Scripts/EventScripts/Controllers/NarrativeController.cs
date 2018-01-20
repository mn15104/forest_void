using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NarrativeController : MonoBehaviour {
    

    public delegate void NarrativeEvent(GameObject human);
    public static event NarrativeEvent OnInteract;
    //Subtitles
    private Queue<string> subtitleQueue = new Queue<string>();
    private Text subtitles;
    private Coroutine latestTextCoroutine = null;
    private bool SubtitlePlaying = false;
   



    // Use this for initialization
    void Start () {
        subtitles = FindObjectOfType<Text>();
    }

    // Update is called once per frame
    void Update () {
        if (subtitleQueue.Count != 0 && SubtitlePlaying)
        {
            StopCoroutine(latestTextCoroutine);
            SubtitlePlaying = false;
        }
        if (!SubtitlePlaying && subtitleQueue.Count != 0)
        {
            SubtitlePlaying = true;
            string nextMessage = subtitleQueue.Dequeue();
            latestTextCoroutine = StartCoroutine(WriteSubtitleChunked(nextMessage, latestTextCoroutine));
        }
        if (subtitles.text == "" && latestTextCoroutine == null)
        {
            SubtitlePlaying = false;

        }
    }

    void SetPlayingFalse(Coroutine SubCoroutine)
    {
        if (SubtitlePlaying && latestTextCoroutine == SubCoroutine)
        {
            SubtitlePlaying = false;
        }
    }

    public void QueueText(string message)
    {
        subtitleQueue.Enqueue(message);
    }


    public void SceneChange()
    {

    }


    IEnumerator WriteSubtitleChunked(string message, Coroutine k)
    {
        subtitles.text = "";
        foreach (char c in message)
        {
            subtitles.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(6.0f);
        if (SubtitlePlaying)
        {
            SubtitlePlaying = false;
            subtitles.text = "";
        }
    }
}
