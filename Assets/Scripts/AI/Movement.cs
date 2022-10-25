using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour
{
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;      
    private float m_OriginalPitch;
    private NavMeshAgent _agent;
    private bool isMoving = false;

    private void OnEnable()
    {
        isMoving = false;
    }

    private void FixedUpdate()
    {
        EngineAudio();
    }
    
    public void OnStart(NavMeshAgent agent)
    {
        _agent = agent;

        m_OriginalPitch = m_MovementAudio.pitch;
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

    private void EngineAudio()
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
