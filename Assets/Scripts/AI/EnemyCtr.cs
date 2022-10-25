using System.Collections;
using Utilities.AttTypeDefine;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCtr : MonoBehaviour
{
    private AttackType ackTyp;
    private NavMeshAgent _agent;
    // private NavMeshObstacle _obstacle;
    private Transform _playerInst;
    private GameObject m_Instance;
    private TankHealth _health;

    public float maxSpeed = 12f;
    [Range(0,1)]
    public float speedRatio = 0.8f;
    public float attackDis;
    public float MinAttackDely = 3f;
    public float MaxAttackDely = 5f;

    private Movement _mover;
    private Fighter _fighter;
    private bool _attacking = false;

    #region sys callback

    private void Awake()
    {
        // _obstacle = GetComponent<NavMeshObstacle>();
        _mover = GetComponent<Movement>();
        _fighter = GetComponent<Fighter>();
    }

    private void OnEnable()
    {
        // _obstacle.enabled = false;
        _attacking = false;
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _health = GetComponent<TankHealth>();

        _mover.OnStart(_agent);
        _fighter.OnStart(_playerInst, ackTyp, _mover);
    }

    private void Update()
    {
        if (_health.Dead) return;

        if (Vector3.Distance(_playerInst.position, transform.position) > attackDis)
        {
            StopAllCoroutines();
            _attacking = false;
            MoveTo(_playerInst.position);
        }
        else
        {
            if (_attacking) return;

            _attacking = true;
            StopMove();
            // if (delay < MaxAttackDely)
            // {
            //     delay += Time.deltaTime;
            // }
            // else
            // {
            //     StartAttack();
            //     delay = 0f;
            // }
            StartCoroutine(DelayToAttack());
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
    
    IEnumerator DelayToAttack()
    {
        while (true)
        {
            var num = Random.Range(0, 101);
            if (num < 30)
            {
                var dely = Random.Range(MinAttackDely, MaxAttackDely);
                yield return new WaitForSeconds(dely);
                StartAttack();
            }
            else
                yield return null;
        }
    }

    private void MoveTo(Vector3 target)
    {
        _mover.MoveTo(target, maxSpeed * speedRatio);
    }
    
    private void StartAttack()
    {
        _fighter.StartAttack();
    }

    private void StopMove()
    {
        _mover.Stop();
    }

    public void DisableAI()
    {
        _agent.enabled = false;
    }

    public void EnableAI()
    {
        _agent.enabled = true;
    }
}