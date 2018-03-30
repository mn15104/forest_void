using UnityEngine;
using UnityEditor;

public partial class MonsterAI
{
    public class MonsterAIState : ScriptableObject
    {
        MonsterAI m_MonsterAI;
        public MonsterAIState(MonsterAI monsterAI)
        {
            m_MonsterAI = monsterAI;
        }

        public void SetState(MonsterState state)
        {
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
                        m_MonsterAI.InitialiseCurrentAppearBehaviour(m_MonsterAI.currentAppear);           // CALL APPEAR BEHAVIOUR TYPE
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
                        m_MonsterAI.NextAppearStage();
                        break;
                    case MonsterState.GAMEOVER:
                        m_MonsterAI.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                        m_MonsterAI.anim.SetBool("Idle", true);
                        m_MonsterAI.anim.SetBool("Walk", false);
                        m_MonsterAI.anim.SetBool("Run", false);
                        break;
                    default:
                        m_MonsterAI.m_MonsterStateMachine.hidden_idle();
                        break;
                }
                m_MonsterAI.stage1_playerLookingAtMonster = false;
                m_MonsterAI.stage2_coroutineCalled = false;
                m_MonsterAI.stage3_invokeCalled = false;
                m_MonsterAI.debugState = state;
                m_MonsterAI.currentState = state;
                m_MonsterAI.OnMonsterStateChange(state);
            }
        }

        
        public void update_state()
        {
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
                case MonsterState.GAMEOVER:
                    hidden_idle();
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

            if (!m_MonsterAI.follow_finished && (m_MonsterAI.currentAppear == MonsterAppear.STAGE1 || m_MonsterAI.currentAppear == MonsterAppear.NONE))
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
                    m_MonsterAI.StartCoroutine(m_MonsterAI.DelayStateChange(MonsterState.APPEAR, 3f));
                    m_MonsterAI.follow_finished = true;
                }
            }
            if (!m_MonsterAI.follow_finished && (m_MonsterAI.currentAppear == MonsterAppear.STAGE2))
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
            if (m_MonsterAI.currentAppear == MonsterAppear.STAGE1)
            {
                m_MonsterAI.UpdateStage1();
            }
            else if (m_MonsterAI.currentAppear == MonsterAppear.STAGE2)
            {

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
                m_MonsterAI.transform.rotation = Quaternion.Slerp(m_MonsterAI.transform.rotation, rotation, Time.deltaTime * 1.2f);
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
                m_MonsterAI.transform.rotation = Quaternion.Slerp(m_MonsterAI.transform.rotation, rotation, Time.deltaTime * 1.5f);
            }
            //Chase
            float distanceToHuman = Mathf.Sqrt(Mathf.Pow(m_MonsterAI.destinationPosition.x - m_MonsterAI.transform.position.x, 2)
                                    + Mathf.Pow(m_MonsterAI.destinationPosition.y - m_MonsterAI.transform.position.y, 2));

            m_MonsterAI.chaseTimer -= Time.deltaTime;
            if (m_MonsterAI.chaseTimer < 0)
            {
                SetState(MonsterState.HIDDEN_IDLE);
                m_MonsterAI.chaseTimer = 20f;
            }

            // Game Over
            float distance = (m_MonsterAI.destinationPosition - m_MonsterAI.transform.position).magnitude;
            if (distance < 1f)
            {
                SetState(MonsterState.GAMEOVER);
            }
        }

        public void GameOver()
        {

        }
    }
}