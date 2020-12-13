using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatus : MonoBehaviour
{
    private readonly string connectStatusMessage = "  Connection status:  ";

    public Text connectText;
    
    void Update()
    {
        connectText.text = connectStatusMessage + Photon.Pun.PhotonNetwork.NetworkClientState;
    }
}
