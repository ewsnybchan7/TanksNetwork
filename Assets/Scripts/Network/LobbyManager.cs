using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login Pannel")]
    public GameObject loginPanel;

    public InputField idInputField;

    [Header("Lobby Panel")]
    public GameObject lobbyPanel;

    public GameObject roomListContent;

    public GameObject roomListEntryPrefab;

    [Header("Create Panel")]
    public GameObject createPanel;

    public InputField roomInputField;
    public InputField playerNumInputField;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header("Room Panel")]
    public GameObject roomPanel;

    public Button startButton;
    public GameObject playerList;
    public GameObject playerListEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;
    
    #region UNITY

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Screen.SetResolution(960, 540, false);
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        playerNumInputField.onValueChanged.AddListener((str) =>
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsDigit(str[i]))
                {
                    str = str.Remove(i, 1);
                    playerNumInputField.text = str;
                }
            }
        });
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        setActivePanel(lobbyPanel.name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        clearRoomListView();
       
        updateCachedRoomList(roomList);
        updateRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();

        clearRoomListView();
    }

    #region FAILED

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        setActivePanel(lobbyPanel.name);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        setActivePanel(lobbyPanel.name);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1, 100);

        RoomOptions options = new RoomOptions { MaxPlayers = 4 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    #endregion

    // room 입장이 됐다면
    // player list를 갱신
    // 플레이어들의 ready 상태를 가지고 와서 엔트리를 생성
    public override void OnJoinedRoom()
    {
        setActivePanel(roomPanel.name);

        if (playerListEntries == null)
            playerListEntries = new Dictionary<int, GameObject>();

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerListEntryPrefab);

            entry.transform.SetParent(playerList.transform);
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            if(p.CustomProperties.TryGetValue(TANKSGame.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        startButton.gameObject.SetActive(checkPlayersReady());

        Hashtable property = new Hashtable
        {
            { TANKSGame.PLAYER_LOADED_LEVEL, false }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }

    public override void OnLeftRoom()
    {
        setActivePanel(lobbyPanel.name);

        foreach(GameObject entry in playerListEntries.Values)
        {
            Destroy(entry);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(playerListEntryPrefab);

        entry.transform.SetParent(playerList.transform);
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber, entry);

        startButton.gameObject.SetActive(checkPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("111111111111");
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        Debug.Log("222222222222");
        playerListEntries.Remove(otherPlayer.ActorNumber);
        Debug.Log("333333333333");

        startButton.gameObject.SetActive(checkPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Hashtable property = PhotonNetwork.CurrentRoom.CustomProperties;
        property["MASTER"] = newMasterClient.NickName;

        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            startButton.gameObject.SetActive(checkPlayersReady());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
            playerListEntries = new Dictionary<int, GameObject>();

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if(changedProps.TryGetValue(TANKSGame.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        startButton.gameObject.SetActive(checkPlayersReady());
    }

    #endregion

    #region UI CALLBACKS

    public void onBackButtonClicked()
    {
        setActivePanel(lobbyPanel.name);
    }

    public void onCreateRoomButtonClicked()
    {
        string roomName = roomInputField.text;
        roomName = (string.IsNullOrEmpty(roomName)) ? "Room " + Random.Range(1, 100) : roomName;

        byte maxPlayers;
        byte.TryParse(playerNumInputField.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 4);

        Hashtable property = new Hashtable();
        property.Add("MASTER", PhotonNetwork.LocalPlayer.NickName);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
        options.CustomRoomProperties = property;
        options.CustomRoomPropertiesForLobby = new string[]
        {
            "MASTER"
        };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void onJoinRandomRoomButtonClicked()
    {
        setActivePanel(joinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void onLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void onLoginButtonClicked()
    {
        string playerName = idInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid");
        }
    }

    public void onJoinRoomButtonClicked()
    {
        RoomListEntry[] roomList = roomListContent.GetComponentsInChildren<RoomListEntry>();
        RoomListEntry selectedEntry = null;

        for(int i = 0; i < roomList.Length; i++)
        {
            if(roomList[i].selected)
            {
                selectedEntry = roomList[i];
                break;
            }
        }

        if(selectedEntry != null)
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(selectedEntry.roomName);
        }
    }

    public void onStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel(1);
    }

    #endregion

    // 패널 활성화 메서드
    public void setActivePanel(string activePanel)
    {
        loginPanel.SetActive(activePanel.Equals(loginPanel.name));
        lobbyPanel.SetActive(activePanel.Equals(lobbyPanel.name));
        createPanel.SetActive(activePanel.Equals(createPanel.name));
        joinRandomRoomPanel.SetActive(activePanel.Equals(joinRandomRoomPanel.name));
        roomPanel.SetActive(activePanel.Equals(roomPanel.name));
    }

    // 룸 리스트를 비우는 메서드
    private void clearRoomListView()
    {
        foreach(GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    // 룸 캐시 메모리를 갱신하는 함수
    private void updateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // 닫히거나 비공개방, 삭제된 방이면 리스트 캐시에서 제거
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // 이미 있는 방이라면 해당 정보로 초기화
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // 새로운 방이라면 추가
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    // Room list 업데아트
    // 네트워크에서 갱신되는 캐시 메모리를 이용하여 엔트리 생성
    private void updateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomListEntryPrefab);
            Hashtable roomProperty = info.CustomProperties;

            entry.transform.SetParent(roomListContent.transform);
            entry.GetComponent<RoomListEntry>().Initialize(info.Name, (string)roomProperty["MASTER"], (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, entry);
        }
    }

    private bool checkPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(TANKSGame.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady) return false;
            }
            else return false;
        }

        return true;
    }

    public void LocalPlayerPropertiesUpdated()
    {
        startButton.gameObject.SetActive(checkPlayersReady());
    }
}
