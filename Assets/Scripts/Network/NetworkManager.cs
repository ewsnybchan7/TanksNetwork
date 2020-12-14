using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;

    public SpawnEnemy[] spawnEnemies;

    private bool isSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        spawnEnemies = FindObjectsOfType<SpawnEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.PlayerList.Length == 2 && !isSpawning)
        {
            foreach(SpawnEnemy sp in spawnEnemies)
            {
                sp.isGame = true;
                sp.StartCoroutine(sp.spawnEnmey());
            }

            isSpawning = true;
        }
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        int i = PhotonNetwork.CountOfPlayersInRooms;
        PhotonNetwork.Instantiate("NetworkTank", spawnPoints[i].position, spawnPoints[i].rotation);
    }
    #endregion

}
