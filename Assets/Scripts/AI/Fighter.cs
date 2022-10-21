using UnityEngine;
using Utilities.AttTypeDefine;

public class Fighter : MonoBehaviour
{
    public GameObject ShellPre;    
    public Transform FireTransform;

    private Transform playerInst;
    private AttackType ackType;
    private Movement mover;

    public float CrashVelocity = 18f;
    
    #region sys callback

    public void OnStart(Transform player, AttackType type, Movement movement)
    {
        playerInst = player;
        ackType = type;
        mover = movement;
    }

    public void StartAttack()
    {
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
        transform.LookAt(playerInst.transform.position);
        mover.Crash(CrashVelocity);
    }

    //todo
    private void LongRangeAtk()
    {
        
    }

    #endregion
}