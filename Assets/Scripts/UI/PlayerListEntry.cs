using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class PlayerListEntry : MonoBehaviour
{
    [Header("UI References")]
    public Text playerNameText;

    public Image playerColorImage;
    public Button playerReadyButton;

    private int ownerID;
    private bool isPlayerReady;

    #region

    private void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }

    private void Start()
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber != ownerID)
        {
            playerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            Hashtable initialProperty = new Hashtable() {
                // 초기 플레이어 설정
                { TANKSGame.PLAYER_READY, isPlayerReady },
                { TANKSGame.PLAYER_LIVES, TANKSGame.PLAYER_MAX_LIVES }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProperty);
            PhotonNetwork.LocalPlayer.SetScore(0);

            playerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);

                Hashtable property = new Hashtable() {
                    { TANKSGame.PLAYER_READY, isPlayerReady }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(property);

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<LobbyManager>().LocalPlayerPropertiesUpdated();
                }
            });
        }
    }

    public void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }

    #endregion

    public void Initialize(int playerID, string playerName)
    {
        ownerID = playerID;
        playerNameText.text = playerName;
    }

    private void OnPlayerNumberingChanged()
    {
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            if(p.ActorNumber == ownerID)
            {
                playerColorImage.color = TANKSGame.GetColor(p.GetPlayerNumber());
            }
        }
    }

    public void SetPlayerReady(bool playerReady)
    {
        playerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
    }
}
