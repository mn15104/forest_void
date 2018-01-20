using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExclamationMark : MonoBehaviour {

    bool m_Active = false;

    private void OnEnable()
    {
        foreach(MeshRenderer meshRend in GetComponentsInChildren<MeshRenderer>())
        {
            meshRend.enabled = false;
        }
        GetComponent<Light>().intensity = 0f;
        AppleScript_2.OnAlert += StartFlashing;
    }

    private void OnDisable()
    {
        AppleScript_2.OnAlert -= StartFlashing;
    }

    void StartFlashing(GameObject human)
    {
        m_Active = true;
    }

	// Update is called once per frame
	void Update () {
        if (m_Active)
        {
            StartCoroutine(Flash());

            m_Active = false;
        }
	}

    void EnableMeshRenderers(bool t_enabled)
    {
        foreach (MeshRenderer meshRend in GetComponentsInChildren<MeshRenderer>())
        {
            meshRend.enabled = t_enabled;
        }
    }
    IEnumerator Flash()
    {
        MeshRenderer[] rends = GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < 50; i++)
        {
            GetComponent<Light>().intensity = 15f;
            EnableMeshRenderers(true);
            yield return (new WaitForSeconds(0.25f));
            GetComponent<Light>().intensity = 0f;
            EnableMeshRenderers(false);
            yield return (new WaitForSeconds(0.25f));
        }
    }
}
