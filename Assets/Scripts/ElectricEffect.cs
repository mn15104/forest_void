using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine("ElectricDisplay");
	}

    IEnumerator ElectricDisplay()
    {
        while (true)
        {
            foreach (Transform child in transform)
            {
                DisplayParticles(child);
                Debug.Log("Particle show");
                yield return new WaitForSeconds(1);
                HideParticles(child);
            }
        }
        
    }

    void HideParticles(Transform currentParticles)
    {
        currentParticles.gameObject.GetComponent<ParticleSystem>().Stop();
    }

    void DisplayParticles(Transform currentParticles)
    {

        currentParticles.gameObject.GetComponent<ParticleSystem>().Play();
        var main = currentParticles.gameObject.GetComponent<ParticleSystem>().main;
        main.loop = false;

    }
	
	// Update is called once per frame
	void Update () {



	}
}
