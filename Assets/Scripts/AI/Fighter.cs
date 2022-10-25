using System;
using UnityEngine;
using Utilities.AttTypeDefine;

public class Fighter : MonoBehaviour
{
    public Rigidbody ShellPre;    
    public Transform FireTransform;
    public AudioSource m_ShootingAudio;  
    public AudioClip m_FireClip; 

    private Transform playerInst;
    private AttackType ackType;
    private Movement mover;

    public float CrashVelocity = 18f;

    private Vector3 gravity;

    private void Awake()
    {
        gravity = Physics.gravity;
    }

    public void OnStart(Transform player, AttackType type, Movement movement)
    {
        playerInst = player;
        ackType = type;
        mover = movement;
    }

    public void StartAttack()
    {
        transform.LookAt(playerInst.transform.position);
        
        switch (ackType)
        {
            case AttackType.eCloseRange:
                CloseRangeAtk();
                break;
            case AttackType.eLongRange:
                LongRangeAtk();
                break;
        }
    }

    private void CloseRangeAtk()
    {
        mover.Crash(CrashVelocity);
    }
    
    private void LongRangeAtk()
    {
        var playerPos = playerInst.position;

        var v = CalculateVelocity(playerPos);

        Fire(v);
    }

    private float CalculateVelocity(Vector3 target)
    {

        var fireDir = FireTransform.forward;
        // Debug.Log($"firePos = {firePos}, fireDir = {fireDir}");

        float[] displacement = CalculateDisplacement(target);

        var hor = displacement[0];
        var ver = displacement[1];
        
        var θ = Vector3.Angle(fireDir, transform.forward) * Mathf.Deg2Rad;
        // Debug.Log($"发射角度 = {Vector3.Angle(fireDir, transform.forward)}");
        // Debug.Log($"炮弹水平位移 = {hor}, 垂直位移 = {ver}");
        
        var g = gravity.magnitude;
        
        var t1 = (2 * hor * Mathf.Cos(θ) * Mathf.Sin(θ))  - (2 * ver * Mathf.Pow(Mathf.Cos(θ), 2));
        if (t1 < 0)
        {
            throw new SystemException("计算结果为负，负数不能进行平方根运算！");
        }

        var v = Mathf.Sqrt((g * Mathf.Pow(hor, 2)) / t1);

        return v;
    }

    /// <summary>
    /// 计算发射点和目标点之间的水平位移和垂直位移
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private float[] CalculateDisplacement(Vector3 targetPos)
    {
        var firePos = FireTransform.position;
        var dir = targetPos - firePos;
        var ver = dir.y;
        var hor = Vector3.ProjectOnPlane(dir, Vector3.up).magnitude;

        // Debug.Log($"targetpos = {targetPos} , firePos = {firePos}");

        return new float[2] {hor, ver};
    }

    // private void Update()
    // {
    //     var firePos = FireTransform.position;
    //     var targetPos = playerInst.position;
    //     Debug.DrawLine(firePos,targetPos, Color.red);
    // }

    private void Fire(float v)
    {

        var shellIns = Instantiate(ShellPre, FireTransform.position, FireTransform.rotation) as Rigidbody;

        shellIns.velocity = v * FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}