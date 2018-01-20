using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour {
    
    Animator m_animator;
    Renderer m_renderer;
    Rigidbody m_rigidbody;
    BoxCollider m_houseTrigger;
    Camera m_targetCamera;
    CapsuleCollider m_capsule;
    Vector3 m_direction;
    bool m_active = false;
	// Use this for initialization
	void Start () {
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponentInChildren<Renderer>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_capsule = GetComponent<CapsuleCollider>();
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
        StartCoroutine(RootAnimationState());
        HumanController[] pcplayers = FindObjectsOfType<HumanController>();
        HumanVRController[] vrplayers = FindObjectsOfType<HumanVRController>();
  
    }
	
    IEnumerator RootAnimationState()
    {
        yield return new WaitForSeconds(2);
        m_animator.speed = 0;
    }

    public void AlertMannequin(Camera playercam)
    {
        if (!m_active)
        {
            m_active = true;
            m_targetCamera = playercam;
        }
    }

    public void DisableMannequin(Camera playercam)
    {
        if (playercam == m_targetCamera)
        {
            m_active = false;
        }
    }

   

    void Update () {

        //Player is in house
        if (m_active)
        {
            //Player looking at mannequin
            if (Vector3.Dot(m_targetCamera.transform.forward, (transform.position - m_targetCamera.transform.position).normalized) > 0)
            {
                m_animator.speed = 0;
                m_rigidbody.velocity = new Vector3(0,0,0);
            }
            //Not looking at mannequin
            else
            {
                RotateMannequin();
                MoveMannequin();
                UpdateMannequinAnimator(m_direction);
            }
        }
    }

    void RotateMannequin()
    {
        var lookPos = m_targetCamera.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1);
    }

    void MoveMannequin()
    {
        Vector3 playerpos = m_targetCamera.GetComponent<Transform>().position;
        Vector3 planar = new Vector3(1, 0, 1);
        Ray ray = new Ray(transform.position, transform.forward);
        Vector3 dir = (playerpos - transform.position).normalized * 0.3f;
        dir.y = 0;
        m_rigidbody.velocity = dir;
        /////////////
        m_direction = dir;
    }



    void UpdateMannequinAnimator(Vector3 move)
    {
        m_animator.SetFloat("Speed", 0.1f, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", 0.1f, 0.1f, Time.deltaTime);

        float runCycle =
            Mathf.Repeat(
                m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.1f, 1);
        float jumpLeg = (runCycle < 0.5f ? 1 : -1) * 0.2f;

        m_animator.SetFloat("JumpLeg", jumpLeg);
        m_animator.speed = move.magnitude > 0 ? 1.5f : m_animator.speed = 1;
    }
}
