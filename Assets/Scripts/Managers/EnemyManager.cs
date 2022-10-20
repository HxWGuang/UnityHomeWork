using System.Collections.Generic;
using Utilities.AttTypeDefine;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public LayerMask m_TankMask;
    public GameObject m_TankEnemyPrefab;
    public EnemySpawnArea[] enemySpawnAreas;
    public EnemyColorMapping[] enemyColorMap;
    public float m_MinRadius = 3f;

    private GameManager gameMgr;
    private Transform palyerInst;
    private List<EnemyCtr> enemyList = new List<EnemyCtr>();

    public void OnStart(GameManager gameMgr, Transform player)
    {
        this.gameMgr = gameMgr;
        palyerInst = player;
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
            enemyList.Add(enemyCtr);
            
            gameMgr.m_CamTargets.Add(tank.transform);
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