using System;
using Utilities.AttTypeDefine;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

public class EnemyCtr : MonoBehaviour
{
    private AttackType ackTyp;
    private NavMeshAgent _agent;
    private NavMeshObstacle _obstacle;
    private Transform _playerInst;
    private GameObject m_Instance;
    private TankHealth _health;

    public float maxSpeed = 12f;
    [Range(0,1)]
    public float speedRatio = 0.8f;

    public float attackDis;

    #region sys callback

    private void Awake()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
    }

    private void OnEnable()
    {
        _obstacle.enabled = false;
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _health = GetComponent<TankHealth>();
    }

    private void Update()
    {
        if (_health.Dead) return;

        // if (ackTyp == AttackType.eCloseRange)
        // {
        //     MoveTo(_playerInst.position);
        // }
        // else if (ackTyp == AttackType.eLongRange)
        // {
        //     if (Vector3.Distance(_playerInst.position, transform.position) > attackDis)
        //     {
        //         MoveTo(_playerInst.position);
        //     }
        //     else
        //     {
        //         StopMove();
        //         AttackBehaviour();
        //     }
        // }

        switch (ackTyp)
        {
            case AttackType.eCloseRange:
            {
                MoveTo(_playerInst.position);
                break;
            }

            case AttackType.eLongRange:
            {
                if (Vector3.Distance(_playerInst.position, transform.position) > attackDis)
                {
                    _obstacle.enabled = false;
                    // _obstacle.carving = false;
                    this.InvokeNextFrame((() =>
                    {
                        _agent.enabled = true;
                        MoveTo(_playerInst.position);
                    }));
                }
                else
                {
                    StopMove();
                    _agent.enabled = false;
                    this.InvokeNextFrame((() =>
                    {
                        _obstacle.enabled = true;
                        // _obstacle.carving = true;
                    }));
                    
                    AttackBehaviour();
                }

                break;
            }
        }
    }
    
    #endregion

    public void Setup(GameObject obj, AttackType attackType, Color color, Transform player)
    {
        ackTyp = attackType;
        _playerInst = player;
        m_Instance = obj;

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = color;
        }
    }

    private void MoveTo(Vector3 target)
    {
        _agent.SetDestination(target);
        _agent.speed = maxSpeed * speedRatio;
    }
    
    private void AttackBehaviour()
    {
        return;
    }

    private void StopMove()
    {
        _agent.velocity = Vector3.zero;
    }

    public void DisableAI()
    {
        _agent.isStopped = true;
    }

    public void EnableAI()
    {
        _agent.isStopped = false;
    }
}
