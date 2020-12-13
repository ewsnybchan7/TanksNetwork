using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkSample : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPositions;

    private void Awake()
    {
        //Screen.SetResolution(960, 540, false);
        //PhotonNetwork.ConnectUsingSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        Hashtable playerProperty = PhotonNetwork.LocalPlayer.CustomProperties;

        int i = (int)playerProperty["Number"];
        PhotonNetwork.Instantiate("NetworkTank", spawnPositions[i].position, spawnPositions[i].rotation)
            .GetComponent<NetworkPlayer>().m_PlayerColor = (Color)playerProperty["Color"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {

    }
    #endregion
}
