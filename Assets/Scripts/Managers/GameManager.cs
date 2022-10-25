using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public TextMeshProUGUI m_MessageText;
    public TextMeshProUGUI m_EnemyNumText;
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;

    public EnemyManager EnemyMgr;       

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private int enemyNumRange = 3;
    private int enemyNum;

    public float roundTimer = 30f;
    public int initialEnemyCount = 3;
    public int eachRoundTime = 30;

    [HideInInspector]
    public List<Transform> m_CamTargets;    

    private Transform player;

    const float k_MaxDepenetrationVelocity = float.PositiveInfinity;

    #region sys callback
    private void Start()
    {
        // This line fixes a change to the physics engine.
        Physics.defaultMaxDepenetrationVelocity = k_MaxDepenetrationVelocity;
        
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnPlayerTanks();

        #region 敌人初始化

        EnemyMgr.OnStart(this, player);
        EnemyMgr.SpawnEnemy(initialEnemyCount);
        enemyNum = initialEnemyCount;

        #endregion

        StartCoroutine(GameLoop());
    }
    #endregion

    #region 玩家初始化
    private void SpawnPlayerTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            var m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            var m_PlayerNumber = i + 1;
            m_Tanks[i].Setup(m_Instance, m_PlayerNumber);
            player = m_Instance.transform;
        }
    }
    #endregion

    #region 相机初始化

    private void SetCameraTargets()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_CamTargets.Add(m_Tanks[i].m_Instance.transform);
        }

        // for (int i = 0; i < EnemyMgr.enemyList.Count; i++)
        // {
        //     m_CamTargets.Add(EnemyMgr.enemyList[i].transform);
        // }
        
        m_CameraControl.m_Targets = m_CamTargets.ToArray();
    }

    #endregion

    #region 游戏状态
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (PlayerWinTheGame() || !PlayerWinTheRound())
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        SetCameraTargets();
        
        ResetAllTanks();
        DisableTankControl();
        DisableEnemy();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "第 " + m_RoundNumber + " 波敌人来袭！";

        m_EnemyNumText.text = enemyNum.ToString();

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        EnableEnemy();

        m_MessageText.text = string.Empty;

        while (HaveTankLeft() && HaveEnemyLeft())
        {
            m_EnemyNumText.text = EnemyMgr.GetCurEnemyNum().ToString();
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        DisableEnemy();

        if (PlayerWinTheRound())
        {
            m_EnemyNumText.text = "0";
            RefreshEnemy();
        }

        m_MessageText.text = EndMessage();

        yield return m_EndWait;
    }

    private void RefreshEnemy()
    {
        var num = Random.Range(enemyNum, enemyNum + enemyNumRange + 1);
        EnemyMgr.SpawnEnemy(num);
        enemyNum = num;
    }

    private bool HaveTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft >= 1;
    }

    private bool HaveEnemyLeft()
    {
        return EnemyMgr.GetCurEnemyNum() >= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }

    private bool PlayerWinTheRound()
    {
        if (!HaveTankLeft())
        {
            return false;
        }

        return true;
    }

    private bool PlayerWinTheGame()
    {
        if (m_RoundNumber >= m_NumRoundsToWin)
        {
            if (HaveTankLeft())
            {
                return true;
            }
        }

        return false;
    }

    private string EndMessage()
    {
        string message = "结束！";

        // if (m_RoundWinner != null)
        //     message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
        
        message += "\n\n\n\n";
        //
        // for (int i = 0; i < m_Tanks.Length; i++)
        // {
        //     message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        // }
        //
        // if (m_GameWinner != null)
        //     message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        if (PlayerWinTheRound())
        {
            message += "成功抵挡住了进攻！";
        }
        else
        {
            message += "挑战失败！";
        }
        
        if (PlayerWinTheGame())
        {
            message = "游戏结束，挑战成功！";
        }

        return message;
    }

    private void ShowMessage(string msg)
    {
        m_MessageText.text = msg;
    }
    
    #endregion

    #region 坦克状态控制
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    private void EnableEnemy()
    {
        EnemyMgr.EnableEnemy();
    }
    
    private void DisableEnemy()
    {
        EnemyMgr.DisableEnemy();
    }
    #endregion
}