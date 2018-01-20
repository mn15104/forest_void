using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Bridge audio component
public class BridgeAudio : MonoBehaviour {

    AudioSource m_BridgeSound;
    float m_AudioNormalizer = 31f;

	// Use this for initialization
	void Start () {
        m_BridgeSound = GetComponent<AudioSource>();
        m_BridgeSound.Play();
        m_BridgeSound.loop = true;
    }
	
	// Update is called once per frame
	void Update () {
        float totalVelocity = 0f;
		foreach(Rigidbody trans in GetComponentsInChildren<Rigidbody>())
        {
            totalVelocity += trans.velocity.sqrMagnitude;
        }

        m_AudioNormalizer = totalVelocity > m_AudioNormalizer ? totalVelocity : m_AudioNormalizer;
        m_BridgeSound.volume = totalVelocity / 5;
    }
}
