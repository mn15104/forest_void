using UnityEngine;
using UnityEditor;

public partial class MonsterAI
{
    public class MonsterAIState : ScriptableObject
    {
        MonsterAI m_MonsterAI;
        MonsterState stalledState;
        public static MonsterAIState newInstance(MonsterAI monsterAI)
        {
            var data = ScriptableObject.CreateInstance<MonsterAIState>();
            data.init(monsterAI);
            return data;
        }
        public void init(MonsterAI monsterAI)
        {
            this.m_MonsterAI = monsterAI;
        }
        public void SetState(MonsterState state)
        {
            if (m_MonsterAI.player.GetComponentInChildren<Flashlight>().GetDisableFlashlight())
            {
                m_MonsterAI.player.GetComponentInChildren<Flashlight>().SetDisableFlashlight(false);
            }
            // Reset opaque
            if (m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().material.GetFloat("_Mode") != 0)
            {
                foreach (Material mat in m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().materials)
                    m_MonsterAI.ChangeRenderMode(mat, BlendMode.Opaque);
                foreach (Material mat in m_MonsterAI.GetComponentInChildren<MeshRenderer>().materials)
                    m_MonsterAI.ChangeRenderMode(mat, BlendMode.Opaque);
            }

            // If monster is stalled due to human in structure, set to previous set regardless of parameter
            if (m_MonsterAI.currentState == MonsterState.HUMAN_IN_STRUCT)
            {
                state = stalledState;
            }

            // Reset collider disabling
            foreach (Collider collider in m_MonsterAI.GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
            m_MonsterAI.GetComponent<Rigidbody>().isKinematic = false;
            // Reset mesh renderer disabling
            m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            m_MonsterAI.GetComponentInChildren<MeshRenderer>().enabled = true;
            m_MonsterAI.follow_finished = false;
            if (state != m_MonsterAI.currentState)
            {

                switch (state)
                {
                    case MonsterState.HIDDEN_IDLE:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.StartCoroutine(m_MonsterAI.UpdateHiddenDestination());
                        m_MonsterAI.anim.SetBool("Run", false);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Idle", true);
                        m_MonsterAI.anim.SetFloat("Speed", m_HiddenIdleSpeed);
                        m_MonsterAI.m_CurrentSpeed = m_HiddenIdleSpeed;
                        m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.HIDDEN_MOVING, 2f));
                        break;
                    case MonsterState.HIDDEN_MOVING:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.StartCoroutine(m_MonsterAI.UpdateHiddenDestination());
                        m_MonsterAI.anim.SetBool("Run", false);
                        m_MonsterAI.anim.SetBool("Walk", true);
                        m_MonsterAI.anim.SetBool("Idle", false);
                        m_MonsterAI.anim.SetFloat("Speed", m_HiddenMovingSpeed);
                        m_MonsterAI.m_CurrentSpeed = m_HiddenMovingSpeed;
                        m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.HIDDEN_IDLE, 8f));
                        break;
                    case MonsterState.FOLLOW:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.StartCoroutine(m_MonsterAI.UpdateChaseDestination());
                        m_MonsterAI.anim.SetBool("Run", false);
                        m_MonsterAI.anim.SetBool("Idle", false);
                        m_MonsterAI.anim.SetBool("Walk", true);
                        m_MonsterAI.anim.SetFloat("Speed", m_FollowSpeed);
                        m_MonsterAI.m_CurrentSpeed = m_FollowSpeed;
                        break;
                    case MonsterState.APPEAR:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.StartCoroutine(m_MonsterAI.UpdateChaseDestination());
                        m_MonsterAI.InitialiseCurrentAppearBehaviour(m_MonsterAI.currentStage);           // CALL APPEAR BEHAVIOUR TYPE
                        m_MonsterAI.follow_finished = false;                                   // Reset follow bool
                        m_MonsterAI.anim.SetBool("Run", false);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Idle", true);
                        m_MonsterAI.anim.SetFloat("Speed", m_AppearSpeed);
                        m_MonsterAI.m_CurrentSpeed = m_AppearSpeed;
                        break;
                    case MonsterState.APPROACH:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.StartCoroutine(m_MonsterAI.UpdateChaseDestination());
                        m_MonsterAI.anim.SetBool("Idle", false);
                        m_MonsterAI.anim.SetBool("Walk", true);
                        m_MonsterAI.anim.SetBool("Run", false);
                        m_MonsterAI.anim.SetFloat("Speed", m_MinApproachSpeed);
                        m_MonsterAI.m_CurrentSpeed = m_MinApproachSpeed;
                        m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.CHASE, 4f));
                        break;
                    case MonsterState.CHASE:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.StartCoroutine(m_MonsterAI.UpdateChaseDestination());
                        m_MonsterAI.anim.SetBool("Idle", false);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Run", true);
                        m_MonsterAI.anim.SetFloat("Speed", m_RunSpeed);
                        m_MonsterAI.m_CurrentSpeed = m_RunSpeed;
                        break;
                    case MonsterState.ATTACK:
                        m_MonsterAI.StopAllCoroutines();
                        orig_y = m_MonsterAI.transform.position.y;
                        foreach (Collider collider in m_MonsterAI.GetComponentsInChildren<Collider>())
                        {
                            collider.enabled = false;
                        }
                        m_MonsterAI.StartCoroutine(m_MonsterAI.UpdateChaseDestination());
                        m_MonsterAI.anim.SetBool("Idle", false);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Run", true);
                        m_MonsterAI.anim.SetFloat("Speed", m_RunSpeed);
                        m_MonsterAI.m_CurrentSpeed = m_RunSpeed;
                        m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.GAMEOVER, 2f));
                        break;
                    case MonsterState.GAMEOVER:
                        m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                        m_MonsterAI.GetComponentInChildren<MeshRenderer>().enabled = false;
                        m_MonsterAI.anim.SetBool("Idle", true);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Run", false);
                        break;
                    case MonsterState.STAGE_COMPLETE:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                        m_MonsterAI.GetComponentInChildren<MeshRenderer>().enabled = false;
                        m_MonsterAI.anim.SetBool("Idle", true);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Run", false);
                        break;
                    case MonsterState.DISABLED:
                        m_MonsterAI.StopAllCoroutines();
                        m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                        m_MonsterAI.GetComponentInChildren<MeshRenderer>().enabled = false;
                        m_MonsterAI.anim.SetBool("Idle", true);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Run", false);
                        m_MonsterAI.ResetStageVariables();
                        break;
                    case MonsterState.HUMAN_IN_STRUCT:
                        m_MonsterAI.StopAllCoroutines();
                        stalledState = m_MonsterAI.currentState;
                        m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                        m_MonsterAI.GetComponentInChildren<MeshRenderer>().enabled = false;
                        m_MonsterAI.anim.SetBool("Idle", true);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Run", false);
                        m_MonsterAI.ResetStageVariables();
                        break;
                    default:
                        m_MonsterAI.m_MonsterStateMachine.hidden_idle();
                        break;
                }
                //Check for correct state switching
                m_MonsterAI.debugState = state;
                m_MonsterAI.currentState = state;
                m_MonsterAI.OnMonsterStateChange(state);
            }
        }


        public void update_state()
        {
            // Use debugState if changed in inspector during testing
            if (m_MonsterAI.currentState != m_MonsterAI.debugState)
                SetState(m_MonsterAI.debugState);

            switch (m_MonsterAI.currentState)
            {
                case MonsterState.HIDDEN_IDLE:
                    hidden_idle();
                    break;
                case MonsterState.HIDDEN_MOVING:
                    hidden_walking();
                    break;
                case MonsterState.FOLLOW:
                    follow();
                    break;
                case MonsterState.APPEAR:
                    appear();
                    break;
                case MonsterState.APPROACH:
                    approach();
                    break;
                case MonsterState.CHASE:
                    chase();
                    break;
                case MonsterState.ATTACK:
                    attack();
                    break;
                case MonsterState.GAMEOVER:
                    gameover();
                    break;
                case MonsterState.DISABLED:
                    gameover();
                    break;
                case MonsterState.STAGE_COMPLETE:
                    gameover();
                    break;
                case MonsterState.HUMAN_IN_STRUCT:
                    gameover();
                    break;
                default:
                    hidden_idle();
                    break;
            }
        }

        public void hidden_idle()
        {
            if ((m_MonsterAI.currentState == MonsterState.HIDDEN_IDLE || m_MonsterAI.currentState == MonsterState.HIDDEN_MOVING)
                 && m_MonsterAI.distanceToHuman < m_MonsterAI.distanceToHuman_FollowTrigger)
            {
                SetState(MonsterState.FOLLOW);
            }
        }

        public void hidden_walking()
        {
            if ((m_MonsterAI.currentState == MonsterState.HIDDEN_IDLE || m_MonsterAI.currentState == MonsterState.HIDDEN_MOVING)
                    && m_MonsterAI.distanceToHuman < m_MonsterAI.distanceToHuman_FollowTrigger)
            {
                SetState(MonsterState.FOLLOW);
                return;
            }
            if (m_MonsterAI.firstCollision != AICollisionSide.NONE)
            {
                Vector3 t_rotation = m_MonsterAI.transform.rotation.eulerAngles;
                if (m_MonsterAI.firstCollision == AICollisionSide.RIGHT)
                {
                    t_rotation.y -= Time.deltaTime * 100;
                }
                else
                {
                    t_rotation.y += Time.deltaTime * 100;
                }
                m_MonsterAI.transform.rotation = Quaternion.Euler(t_rotation);
            }
            else
            {
                m_MonsterAI.transform.rotation = Quaternion.Slerp(m_MonsterAI.transform.rotation, m_MonsterAI.destinationRotation, Time.deltaTime * 1.2f);
            }
        }

        public void follow()
        {
            if (m_MonsterAI.firstCollision != AICollisionSide.NONE)
            {
                Vector3 t_rotation = m_MonsterAI.transform.rotation.eulerAngles;
                if (m_MonsterAI.firstCollision == AICollisionSide.RIGHT)
                {
                    t_rotation.y -= Time.deltaTime * 100;
                }
                else
                {
                    t_rotation.y += Time.deltaTime * 100;
                }

                m_MonsterAI.transform.rotation = Quaternion.Euler(t_rotation);
            }
            else
            {
                var lookPos = m_MonsterAI.destinationPosition - m_MonsterAI.transform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                m_MonsterAI.transform.rotation = Quaternion.Slerp(m_MonsterAI.transform.rotation, rotation, Time.deltaTime * 1.2f);
            }

            if (!m_MonsterAI.follow_finished && (m_MonsterAI.currentStage == EventManager.Stage.Stage1))
            {
                if (m_MonsterAI.distanceToHuman > m_MonsterAI.distanceToHuman_AppearTrigger && !m_MonsterAI.anim.GetBool("Walk"))
                {
                    m_MonsterAI.m_CurrentSpeed = m_HiddenMovingSpeed;
                    m_MonsterAI.anim.SetBool("Run", false);
                    m_MonsterAI.anim.SetBool("Walk", true);
                    m_MonsterAI.anim.SetBool("Idle", false);
                    m_MonsterAI.anim.SetFloat("Speed", m_MonsterAI.m_CurrentSpeed);
                }
                else if (m_MonsterAI.distanceToHuman <= m_MonsterAI.distanceToHuman_AppearTrigger)
                {
                    m_MonsterAI.anim.SetBool("Run", false);
                    m_MonsterAI.anim.SetBool("Idle", true);
                    m_MonsterAI.anim.SetBool("Walk", false);
                    m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.APPEAR, 2f));
                    m_MonsterAI.follow_finished = true;
                }
            }
            if (!m_MonsterAI.follow_finished && (m_MonsterAI.currentStage == EventManager.Stage.Stage2))
            {
                if (m_MonsterAI.distanceToHuman > m_MonsterAI.distanceToHuman_AppearTrigger && !m_MonsterAI.anim.GetBool("Walk"))
                {
                    m_MonsterAI.m_CurrentSpeed = m_HiddenMovingSpeed;
                    m_MonsterAI.anim.SetBool("Run", false);
                    m_MonsterAI.anim.SetBool("Walk", true);
                    m_MonsterAI.anim.SetBool("Idle", false);
                    m_MonsterAI.anim.SetFloat("Speed", m_MonsterAI.m_CurrentSpeed);
                }
                else if (m_MonsterAI.distanceToHuman <= m_MonsterAI.distanceToHuman_AppearTrigger)
                {
                    m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.APPEAR, 0f));
                    m_MonsterAI.follow_finished = true;
                }
            }
            if (!m_MonsterAI.follow_finished && (m_MonsterAI.currentStage == EventManager.Stage.Stage3))
            {
                if (m_MonsterAI.distanceToHuman > m_MonsterAI.distanceToHuman_AppearTrigger && !m_MonsterAI.anim.GetBool("Walk"))
                {
                    m_MonsterAI.m_CurrentSpeed = m_HiddenMovingSpeed;
                    m_MonsterAI.anim.SetBool("Run", false);
                    m_MonsterAI.anim.SetBool("Walk", true);
                    m_MonsterAI.anim.SetBool("Idle", false);
                    m_MonsterAI.anim.SetFloat("Speed", m_MonsterAI.m_CurrentSpeed);
                }
                else if (m_MonsterAI.distanceToHuman <= m_MonsterAI.distanceToHuman_AppearTrigger)
                {
                    m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.APPEAR, 0f));
                    m_MonsterAI.follow_finished = true;
                }
            }
        }

        public void appear()
        {
            if (m_MonsterAI.currentStage == EventManager.Stage.Stage1)
            {
                /////// Leave this line blank - UpdateStage1 is an IEnumerator, called by 'InitializeStage1'
            }
            else if (m_MonsterAI.currentStage == EventManager.Stage.Stage2)
            {
                m_MonsterAI.UpdateStage2();
            }
            else if (m_MonsterAI.currentStage == EventManager.Stage.Stage3)
            {
                m_MonsterAI.UpdateStage3();
            }
        }
        public void approach()
        {
            if (m_MonsterAI.firstCollision != AICollisionSide.NONE)
            {
                Vector3 t_rotation = m_MonsterAI.transform.rotation.eulerAngles;
                if (m_MonsterAI.firstCollision == AICollisionSide.RIGHT)
                {
                    t_rotation.y -= Time.deltaTime * 100;
                }
                else
                {
                    t_rotation.y += Time.deltaTime * 100;
                }

                m_MonsterAI.transform.rotation = Quaternion.Euler(t_rotation);
            }
            else
            {
                var lookPos = m_MonsterAI.destinationPosition - m_MonsterAI.transform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                m_MonsterAI.transform.rotation = Quaternion.Slerp(m_MonsterAI.transform.rotation, rotation, Time.deltaTime * 2.2f);
            }

            m_MonsterAI.m_CurrentSpeed = Mathf.Lerp(m_MonsterAI.m_CurrentSpeed, m_MaxApproachSpeed, Time.deltaTime * 0.1f);
            m_MonsterAI.anim.SetFloat("Speed", m_MonsterAI.m_CurrentSpeed);
        }

        public void chase()
        {
            if (m_MonsterAI.firstCollision != AICollisionSide.NONE)
            {
                Vector3 t_rotation = m_MonsterAI.transform.rotation.eulerAngles;
                if (m_MonsterAI.firstCollision == AICollisionSide.RIGHT)
                {
                    t_rotation.y -= Time.deltaTime * 200;
                }
                else
                {
                    t_rotation.y += Time.deltaTime * 200;
                }

                m_MonsterAI.transform.rotation = Quaternion.Euler(t_rotation);
            }
            else
            {
                var lookPos = m_MonsterAI.destinationPosition - m_MonsterAI.transform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                m_MonsterAI.transform.rotation = Quaternion.Slerp(m_MonsterAI.transform.rotation, rotation, Time.deltaTime * 2.5f);
            }
            //Chase
            float distanceToHuman = Mathf.Sqrt(Mathf.Pow(m_MonsterAI.destinationPosition.x - m_MonsterAI.transform.position.x, 2)
                                    + Mathf.Pow(m_MonsterAI.destinationPosition.y - m_MonsterAI.transform.position.y, 2));


            // Game Over
            float distance = (m_MonsterAI.distanceToHuman);
            if (distance < 1.0f)
            {
                SetState(MonsterState.ATTACK);
            }
        }

        bool attack_playerSeesMonster = false;
        float orig_y;
        public void attack()
        {
            m_MonsterAI.player.GetComponentInChildren<Flashlight>().ForceSwitchFlashlight(false);
            m_MonsterAI.transform.position = new Vector3(m_MonsterAI.transform.position.x, orig_y, m_MonsterAI.transform.position.z);
            m_MonsterAI.TeleportVoidInfrontHuman(0.65f);
            var lookPos = m_MonsterAI.destinationPosition - m_MonsterAI.transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            m_MonsterAI.transform.rotation = rotation;
        }

        public void gameover()
        {

        }







    }
}