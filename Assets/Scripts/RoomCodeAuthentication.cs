using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCodeAuthentication : MonoBehaviourPun
{ 
    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_RoomCode;
    }


    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_RoomCode;
    }
    private void NetworkingClient_RoomCode(EventData _photonEvent)
    {
        byte b = 1;
        byte eventCode = _photonEvent.Code;
        Debug.Log($"event code: {eventCode}");
        if (eventCode.Equals(b))
        {
            RoomInfo info = (RoomInfo)_photonEvent.CustomData;
            Debug.Log($"on receive: {info.Name}");
        }
    }
}
