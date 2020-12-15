using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gameManager;

    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public int m_MaxEnemys;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_ClearText;
    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;

    public int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private bool RoundClearFlag = false;

    public int kill = 0;

    private bool isGameStarting = false;

    public SpawnEnemy[] spawnPoints;
    public List<NetworkPlayer> m_Players;

    private void Awake()
    {
        gameManager = this;
        spawnPoints = FindObjectsOfType<SpawnEnemy>();
    }

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        m_Players = new List<NetworkPlayer>();
    }

    private void Update()
    {
        if(!isGameStarting && m_Players.Count == PhotonNetwork.PlayerList.Length)
        {
            isGameStarting = true;
            StartCoroutine(GameLoop());
        }
    }

    #region Photon Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

    }

    #endregion

    #region TANKS method

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (RoundClearFlag == true)
        {
            SceneManager.LoadScene(m_RoundNumber+1);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        foreach (NetworkPlayer player in m_Players)
        {
            player.photonView.RPC("Reset", RpcTarget.AllBuffered);
        }

        if (!NoneTankLeft())
            DisableTankControl();
        
        m_ClearText.SetActive(false);
        
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        foreach (SpawnEnemy sp in spawnPoints)
        {
            sp.isGame = true;
            sp.StartCoroutine(sp.spawnEnmey());
        }

        m_MessageText.text = string.Empty;

        while ((!NoneTankLeft() ^ !NoneEnemyLeft()))
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        if(!NoneTankLeft())
            DisableTankControl();

        if (NoneEnemyLeft())
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

        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i].gameObject.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft != 0;
    }

    private bool NoneEnemyLeft()
    {
        if (kill == m_MaxEnemys)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EnableTankControl()
    {
        foreach(NetworkPlayer player in m_Players)
        {
            player.photonView.RPC("EnableControl", RpcTarget.AllBuffered);
        }
    }

    private void DisableTankControl()
    {
        foreach(NetworkPlayer player in m_Players)
        {
            player.photonView.RPC("DisableControl", RpcTarget.AllBuffered);        
        }
    }
    #endregion

    public void increaseKill()
    {
        kill++;
    }
}