using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public float m_MaxEnemys = 10f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_ClearText;
    public GameObject m_TankPrefab;
    public static TankManager[] m_Tanks;

    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    //private TankManager[] m_Enemeys;
    private bool RoundClearFlag = false;

    public int kill = 0;


    public SpawnEnemy[] spawnPoints;

    private void Awake()
    {
        spawnPoints = FindObjectsOfType<SpawnEnemy>();
    }

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        //StartCoroutine(GameLoop());

        //foreach(SpawnEnemy sp in spawnPoints)
        //{
        //    sp.isGame = true;
        //    sp.StartCoroutine(sp.spawnEnmey());
        //}
    }

    #region Photon Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

    }

    #endregion

    #region TANKS method


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null && RoundClearFlag == true)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        m_ClearText.SetActive(false);

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_MessageText.text = string.Empty;

        while (!NoneTankLeft() || !NoneEnemyLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        //m_RoundWinner = null;

        //m_RoundWinner = GetRoundWinner();

        //if (m_RoundWinner != null)
        //    m_RoundWinner.m_Wins++;

        //m_GameWinner = GetGameWinner();

        if (GetRoundWinner() != null)
        {
            RoundClearFlag = true;
            m_ClearText.SetActive(true);
        }
        else
        {
            m_MessageText.text = "Clear Failed...";
        }
       
        //string message = EndMessage();
        //m_MessageText.text = message;
        
        yield return m_EndWait;
    }


    private bool NoneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft < 1;
    }

    private bool NoneEnemyLeft()
    {
        return true;

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
    #endregion

    public void increaseKill()
    {
        kill++;
    }
}