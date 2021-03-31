using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private GameObject player;
    private Dictionary<Player, int> playerChosenPrefabIndex;
    private int latency; //Note: make a client manager class to store, set and change client (network) data

    private void Awake()
    {
        playerChosenPrefabIndex = new Dictionary<Player, int>();
        Player[] _playerList = PhotonNetwork.PlayerList;
        for (int i = 0; i < _playerList.Length; i++)
        {
            if (playerChosenPrefabIndex.ContainsKey(_playerList[i]) == false)
            {
                List<int> disqualifiedPlayers = RoomManager.Instance.getDisqualifiedPlayers();
                Debug.Log($"disqualified test {_playerList[i].ActorNumber}: {disqualifiedPlayers.Contains(_playerList[i].ActorNumber) == false}");
                if (disqualifiedPlayers.Contains(_playerList[i].ActorNumber))
                {
                    return;
                }
                playerChosenPrefabIndex.Add(_playerList[i], i % 4);
            }
        }

        if (photonView.IsMine) //check if the photon view is owned by the local player
        {
            CreateController();
            GameManager.Instance.resetVariables();
        }
    }

    public bool isLocalPlayer()
    {
        return photonView.IsMine;
    }

    public int getPlayerLatency()
    {
        return PhotonNetwork.GetPing();
    }

    public GameObject getPlayer()
    {
        return player;
    }

    private void CreateController()
    {
        if (playerChosenPrefabIndex.ContainsKey(photonView.Owner))
        {
            string _s = "Player" + (playerChosenPrefabIndex[photonView.Owner] + 1);
            Debug.Log("Instantiated Player Controller");
            player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", _s), new Vector3(-8f, -2.5f), Quaternion.identity);
        }
    }
}