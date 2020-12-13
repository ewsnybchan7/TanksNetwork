using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class RoomListEntry : MonoBehaviour, ISelectHandler
{
    public Text roomNameText;
    public Text roomPlayersText;
    public Text roomMasterText;

    public bool selected = false;

    public string roomName;
    
    public void Initialize(string name ,string master , byte currentPlayers, byte maxPlayers)
    {
        roomName = name;

        roomNameText.text = " " + name;
        roomPlayersText.text = " " + currentPlayers + " / " + maxPlayers + " ";
        roomMasterText.text = " " + master;
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameObject content = this.gameObject.transform.parent.gameObject;
        RoomListEntry[] roomList = content.GetComponentsInChildren<RoomListEntry>();
        foreach(RoomListEntry room in roomList)
        {
            room.selected = false;
        }

        selected = true;
    }
}
