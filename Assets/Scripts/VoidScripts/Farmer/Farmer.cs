using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour {
    
    public List<Transform> destinations;
    public AudioSource m_Shouting;
    public AudioSource m_Moving;
    public AudioClip m_Walking;
    public AudioClip m_Approaching;
    public AudioClip m_Running;

    private NPCState m_npcState = NPCState.Idle;
    private Animator m_animator;
    private Transform targetDestination;
    private float m_WalkSpeed = 1f;
    private float m_ApproachSpeed = 1.2f;
    private float m_MinChaseSpeed = 0.3f;
    private float m_MaxChaseSpeed = 0.7f;
    private float m_CurrentSpeed = 1f;
    private int currentDestinationIndex = 0;
    private float stateChangeTimer = 0f;
    bool collisionInFront = false;
    private GameObject human;
    public float timeTakenDuringLerp = 1f;

    public float distanceToMove = 10;
    private bool _isLerping;
    private float _timeStartedLerping;
    
    void FixedUpdate()
    {
        if (_isLerping)
        {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = (timeSinceStarted / timeTakenDuringLerp) * 0.5f;
            m_CurrentSpeed = Mathf.Lerp(m_MinChaseSpeed, m_MaxChaseSpeed, percentageComplete);
            if (percentageComplete >= 1.0f)
            {
                _isLerping = false;
            }
        }
    }

    enum NPCState
    {
        Idle,
        Walk,
        Approach,
        Chase
    }

    void OnEnable()
    {
        m_CurrentSpeed = m_WalkSpeed;
       // AppleScript_2.OnAlert += InitialiseChase;
        m_Shouting.loop = false;
        m_Shouting.enabled = false;
    }

    IEnumerator Shout()
    {
        yield return new WaitForSeconds(2);
        m_Shouting.enabled = true;
        m_Shouting.volume = 0.8f;
        m_Shouting.Play();
    }
    
    void OnDisable()
    {
        //AppleScript_2.OnAlert -= InitialiseChase;
    }

    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    void InitialiseChase(GameObject t_human)
    {
        human = t_human;
        targetDestination = human.transform;
        m_npcState = NPCState.Approach;
        m_animator.SetBool("Idle", false);
        m_animator.SetBool("Walk", true);
        m_CurrentSpeed = m_MinChaseSpeed;
        StartCoroutine(Shout());
        StartCoroutine(DelayChase());
    }

    IEnumerator DelayChase()
    {
        yield return new WaitForSeconds(6f);
        m_npcState = NPCState.Chase;
        m_animator.SetBool("Walk", false);
        m_animator.SetBool("Run", true);
        m_animator.SetFloat("Speed", m_MinChaseSpeed);

        _isLerping = true;
        _timeStartedLerping = Time.time;
    }

    void Chase()
    {
        //Rotate
        targetDestination = human.transform;
        var lookPos = targetDestination.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        //Chase
        float distanceToHuman = Mathf.Sqrt(Mathf.Pow(human.transform.position.x - transform.position.x, 2) + Mathf.Pow(human.transform.position.y - transform.position.y, 2));
        if(distanceToHuman < 10f)
        {
            m_CurrentSpeed = m_MaxChaseSpeed * distanceToHuman * 0.1f;
        }
        else
        {
            m_CurrentSpeed = m_MaxChaseSpeed;
        }
        m_animator.SetFloat("Speed", m_CurrentSpeed);
    }

    void Idle()
    {
        if (stateChangeTimer > 3f)
        {
            m_npcState = NPCState.Walk;
            m_animator.SetBool("Walk", true);
            m_animator.SetBool("Idle", false);
            targetDestination = currentDestinationIndex < destinations.Count - 1 ? destinations[currentDestinationIndex++] : destinations[currentDestinationIndex = 0];
            stateChangeTimer = 0f;
        }
        else
            stateChangeTimer += Time.deltaTime;
    }

    void Walk()
    {
        if (!((transform.position - targetDestination.position).magnitude < 2f))
        {
            var lookPos = targetDestination.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1);


            Vector3 playerpos = targetDestination.position;
            Vector3 planar = new Vector3(1, 0, 1);
            Ray ray = new Ray(transform.position, transform.forward);

            Vector3 dir = (playerpos - transform.position).normalized * m_CurrentSpeed;
            dir.y = 0;
            transform.GetComponent<Rigidbody>().velocity = dir;
            m_CurrentSpeed = m_WalkSpeed;
            m_animator.SetFloat("Speed", m_CurrentSpeed);
        }
        else
        {
            m_npcState = NPCState.Idle;
            m_animator.SetBool("Idle", true);
            m_animator.SetBool("Walk", false);
        }
    }

    void Approach()
    {
        targetDestination = human.transform;
        var lookPos = targetDestination.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        //Chase
        m_animator.SetFloat("Speed", m_CurrentSpeed);
        Vector3 playerpos = targetDestination.position;
        Vector3 planar = new Vector3(1, 0, 1);
        Ray ray = new Ray(transform.position, transform.forward);
        Vector3 dir = (playerpos - transform.position).normalized * m_CurrentSpeed;
        dir.y = 0;
        transform.GetComponent<Rigidbody>().velocity = dir;
    }

    void Update()
    {
        if(m_npcState == NPCState.Approach)
        {
            Approach();
            if (!m_Moving.isPlaying)
            {
                m_Moving.Play();
                m_Moving.loop = false;
            }
            if (m_Moving.clip != m_Approaching)
            {
                m_Moving.clip = m_Approaching;
            }
        }
        if (m_npcState == NPCState.Chase)
        {
            Chase();
            if (!m_Moving.isPlaying)
            {
                m_Moving.Play();
                m_Moving.loop = true;
            }
            if (m_Moving.clip != m_Running)
            {
                m_Moving.clip = m_Running;
            }
        }
        else if (m_npcState == NPCState.Idle)
        {
            m_Moving.Stop();
            Idle();
        }
        else if (m_npcState == NPCState.Walk)
        {
            Walk();
            if (!m_Moving.isPlaying)
            {
                m_Moving.Play();
                m_Moving.loop = true;
            }
            if (m_Moving.clip != m_Walking)
            {
                m_Moving.clip = m_Walking;
            }
        }
    }
}
