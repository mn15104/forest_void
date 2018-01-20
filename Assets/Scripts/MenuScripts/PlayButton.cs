using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour {

    MenuScript menu;
    public Color mouseOverColor;
    string hexMouseOverColor;
    public Color defaultColor;
    string hexDefaultColor;
    Renderer renderer;
    GUIStyle style;
    // Use this for initialization

    void Start () {
        
        hexMouseOverColor = "#" + ColorUtility.ToHtmlStringRGBA(mouseOverColor);
        hexDefaultColor = "#" + ColorUtility.ToHtmlStringRGBA(defaultColor);
        style = new GUIStyle();
        style.richText = true;
        TextMesh tm = GetComponent<TextMesh>();
        tm.richText = true;

        menu = GetComponentInParent<MenuScript>();
        renderer = GetComponent<Renderer>();
        renderer.material.color = defaultColor;
    }
	
	// Update is called once per frame
	void Update () {
	}

    private void OnMouseUpAsButton()
    {
        menu.PlayGame();
    }
    //Not functional yet
    IEnumerator BlipColors()
    {
        TextMesh tm = GetComponent<TextMesh>();
        string originalString = tm.text;
        string currentStringStart = "";
        string currentStringEnd = tm.text;
        foreach (char c in originalString)
        {

            string newstring = currentStringStart + "<size=140>" + c + "</size>" + currentStringEnd;
            tm.text = newstring;
            yield return new WaitForSeconds(0.05f);
            currentStringStart += c;
            currentStringEnd = currentStringEnd.Substring(1);
        }
    }

    private void OnMouseEnter()
    {

        renderer.material.color = mouseOverColor;
    }

    private void OnMouseExit()
    {
        renderer.material.color = defaultColor;
    }
}
