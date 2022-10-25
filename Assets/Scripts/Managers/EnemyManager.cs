using System;
using System.Collections.Generic;
using Utilities.AttTypeDefine;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public LayerMask m_TankMask;                
    public GameObject m_TankEnemyPrefab;        
    public EnemySpawnArea[] enemySpawnAreas;    
    public EnemyColorMapping[] enemyColorMap;   
    public float m_MinRadius = 3f;              

    private GameManager gameMgr;
    private Transform palyerInst;
    public List<EnemyCtr> enemyList = new List<EnemyCtr>();

    public void OnStart(GameManager gameMgr, Transform player)
    {
        this.gameMgr = gameMgr;
        palyerInst = player;
    }

    public int GetCurEnemyNum()
    {
        var count = 0;
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    public void SpawnEnemy(int total)
    {
        Collider[] colliders;
        Vector3 spawnPoint;

        for (int i = 0; i < total; i++)
        {
            var areaIdx = Random.Range(0, enemySpawnAreas.Length);
            
            do
            {
                spawnPoint = enemySpawnAreas[areaIdx].pos.position + 0.5f * new Vector3(
                Random.Range(-enemySpawnAreas[areaIdx].size.x, enemySpawnAreas[areaIdx].size.x),
                0,
                Random.Range(-enemySpawnAreas[areaIdx].size.z, enemySpawnAreas[areaIdx].size.z));

                colliders = Physics.OverlapSphere(spawnPoint, m_MinRadius, m_TankMask);
            } while (colliders.Length > 0);

            var type = (AttackType) Random.Range((int) AttackType.eCloseRange, (int) AttackType.eLongRange + 1);

            GameObject tank = Instantiate(m_TankEnemyPrefab, spawnPoint, Quaternion.identity);

            var enemyCtr = tank.GetComponent<EnemyCtr>();
            enemyCtr.Setup(tank, type, GetColor(type), palyerInst);
            
            // tank.GetComponent<TankHealth>().onEnemyDeath.AddListener(OnEnemyDead);
            
            enemyList.Add(enemyCtr);
            // gameMgr.m_CamTargets.Add(tank.transform);
        }
    }

    private Color GetColor(AttackType type)
    {
        for (int k = 0; k < enemyColorMap.Length; k++)
        {
            if (enemyColorMap[k].type == type)
            {
                return enemyColorMap[k].color;
            }
        }

        return Color.white;
    }

    // public void OnEnemyDead()
    // {
    // for (int i = 0; i < enemyList.Count; i++)
    // {
    //     if (!enemyList[i].gameObject.activeSelf)
    //     {
    //         enemyList.Remove(enemyList[i]);
    //     }
    // }

        // 耦合太高了，这里要改gameMgr.m_CamTargets这个列表，但是CameraControl里也需要用到这个列表的信息
        // var targetList = gameMgr.m_CamTargets;
        // var remIdx = 0;
        // for (int i = 0; i < targetList.Count; i++)
        // {
        //     if (targetList[i].CompareTag("Enemy") && !targetList[i].gameObject.activeSelf)
        //     {
        //         targetList.Remove(targetList[i]);
        //         
        //         var health = targetList[i].GetComponent<TankHealth>();
        //         health.DestroyTank();
        //         return;
        //     }
        // }

        // DestroyTank(remIdx);
    // }

    private void DestroyDeadTank()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (!enemyList[i].gameObject.activeSelf)
            {
                enemyList.Remove(enemyList[i]);
            }
        }
    }

    public void EnableEnemy()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].EnableAI();
        }
    }
    
    public void DisableEnemy()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].DisableAI();
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < enemySpawnAreas.Length; i++)
        {
            Gizmos.DrawWireCube(enemySpawnAreas[i].pos.position, enemySpawnAreas[i].size);
        }
    }
#endif
}