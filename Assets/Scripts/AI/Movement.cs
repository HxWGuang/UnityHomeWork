using UnityEngine;
using UnityEngine.AI;

public class Movement : TankMovement
{
    private NavMeshAgent _agent;
    private bool isMoving = false;

    private void OnEnable()
    {
        isMoving = false;
    }

    public void OnStart(NavMeshAgent agent)
    {
        _agent = agent;
    }

    public void MoveTo(Vector3 des, float speed)
    {
        _agent.speed = speed;
        _agent.SetDestination(des);

        _agent.isStopped = false;
        isMoving = true;
    }

    public void Crash(float v)
    {
        _agent.velocity = transform.forward * v;
        // _agent.isStopped = true;
    }

    public void Stop()
    {
        _agent.isStopped = true;

        isMoving = false;
    }

    protected override void EngineAudio()
    {
        if (isMoving)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.volume = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.volume = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }
}
