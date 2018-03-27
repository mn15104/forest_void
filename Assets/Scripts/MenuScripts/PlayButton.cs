using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System;
public class PlayButton : MonoBehaviour {

    MenuScript menu;
    GUIStyle style;
    public Material m_Mat;
    public GameObject m_VoidPlayButton;
    public GameObject m_VoidTitle;
    public GameObject m_Title;
    private Color defaultColor = Color.black;
    private Color glowColor;
    void Start () {
        menu = GetComponentInParent<MenuScript>();
        string glowCol = "#DB7B00FF";
        ColorUtility.TryParseHtmlString(glowCol, out glowColor);
        m_Mat.SetVector("_OutlineColor", new Vector4(defaultColor.r, defaultColor.b, defaultColor.g, defaultColor.a));

        m_VoidPlayButton.SetActive(false);
        m_VoidTitle.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void OnMouseUpAsButton()
    {
        menu.PlayGame();
        m_VoidPlayButton.SetActive(true);
        m_VoidTitle.SetActive(true);
        m_Title.SetActive(false);
        gameObject.SetActive(false);
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
        m_Mat.SetVector("_OutlineColor", new Vector4(glowColor.r, glowColor.b, glowColor.g, glowColor.a));
    }

    private void OnMouseExit()
    {
        m_Mat.SetVector("_OutlineColor", new Vector4(defaultColor.r, defaultColor.b, defaultColor.g, defaultColor.a));
    }
}
