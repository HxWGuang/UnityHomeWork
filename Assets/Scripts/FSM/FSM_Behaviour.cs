using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Behaviour : MonoBehaviour
{
    private FSM_System SysInst;
    private EnemyCtr Owner;

    public void OnStart(EnemyCtr ec)
    {
        Owner = ec;
        
    }
}
