using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScripts : MonoBehaviour {
    TextMeshPro textMeshPro;
    TMP_CharacterInfo[] charInfo;
    Shader titleShader;
    Color32 white;
    public Material mat;

    Color32 black;
    // Use this for initialization
    void Start () {
        textMeshPro = GetComponent<TextMeshPro>();
        charInfo = textMeshPro.textInfo.characterInfo;
         white = new Color32(0, 0, 0, 255);
         black = new Color32(255, 255, 255, 255);
    }
    // Update is called once per frame
    void Update () {
	}
}
