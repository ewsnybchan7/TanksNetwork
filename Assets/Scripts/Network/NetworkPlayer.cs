using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    public Color m_PlayerColor;
    public Transform m_SpawnPoint;
    [HideInInspector] public int m_PlayerNumber;
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;

    private TankMovement m_Movement;
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;

    private PhotonView pv;
    private Hashtable playerProperty;


    private void Awake()
    {
        m_Movement = GetComponent<TankMovement>();
        m_Shooting = GetComponent<TankShooting>();
        m_CanvasGameObject = GetComponentInChildren<Canvas>().gameObject;

        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Hashtable playerProperty = photonView.Owner.CustomProperties;

        m_PlayerColor = (Color)playerProperty["Color"];
        m_PlayerNumber = (int)playerProperty["Number"];

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }

        GameManager.gameManager.m_Players.Add(this);

        m_SpawnPoint = FindObjectOfType<NetworkSample>().spawnPositions[m_PlayerNumber];
        m_Instance = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }


    [PunRPC]
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    [PunRPC]
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    [PunRPC]
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
