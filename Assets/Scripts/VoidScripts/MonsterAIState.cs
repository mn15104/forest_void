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

        public void update_state()
        {
            if (m_MonsterAI.currentState != m_MonsterAI.debugState)
                m_MonsterAI.SetState(m_MonsterAI.debugState);
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
                m_MonsterAI.SetState(MonsterState.FOLLOW);
            }
        }

        public void hidden_walking()
        {
            if ((m_MonsterAI.currentState == MonsterState.HIDDEN_IDLE || m_MonsterAI.currentState == MonsterState.HIDDEN_MOVING)
                    && m_MonsterAI.distanceToHuman < m_MonsterAI.distanceToHuman_FollowTrigger)
            {
                m_MonsterAI.SetState(MonsterState.FOLLOW);
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
                m_MonsterAI.SetState(MonsterState.HIDDEN_IDLE);
                m_MonsterAI.chaseTimer = 20f;
            }

            // Game Over
            float distance = (m_MonsterAI.destinationPosition - m_MonsterAI.transform.position).magnitude;
            if (distance < 1f)
            {
                m_MonsterAI.SetState(MonsterState.GAMEOVER);
            }
        }

        public void GameOver()
        {

        }
    }
}