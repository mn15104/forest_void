using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinTrigger : MonoBehaviour {
    Animator m_animator;
    Rigidbody m_rigidbody;
    CapsuleCollider m_capsule;
    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        StartCoroutine(RootAnimationState());
        m_rigidbody = GetComponent<Rigidbody>();
        m_capsule = GetComponent<CapsuleCollider>();
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    IEnumerator RootAnimationState()
    {
        yield return new WaitForSeconds(0.5f);
        m_animator.GetComponent<Animation>().enabled = false;
    }
    // Update is called once per frame
    void Update () {
        

    }
}
