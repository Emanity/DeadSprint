using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    private GameObject playerManagerObject;
    private Player winnerPlayer;
    private List<int> disqualifiedPlayersActNumber;
    private List<string> authenticatedUserIDList = new List<string>();

    public string roomCode; //note: rpc is already encrypted, should i encrypt locally? if so what key to use

    private int count;
    private Scene currentScene;

    private void Awake()
    {
        //checks if another RoomManager exits and if there is it will destroy it
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        count = 0;
        disqualifiedPlayersActNumber = new List<int>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    //i will be using the RoomManager to print the player who won because GameManager will be destroyed when switching to the winner scene
    public Player getWinnerPlayer()
    {
        return winnerPlayer;
    }

    public int getCount()
    {
        return count;
    }

    public void playerDisqualified(Player _player)
    {
        disqualifiedPlayersActNumber.Add(_player.ActorNumber);
    }

    public List<int> getDisqualifiedPlayers()
    {
        return disqualifiedPlayersActNumber;
    }

    public PlayerManager getPlayerManager()
    {
        return playerManagerObject.GetComponent<PlayerManager>();
    }

    public void setWinnerPlayer(Player _winnerPlayer)
    {
        if (currentScene.buildIndex == 1)
        {
            winnerPlayer = _winnerPlayer;

            LeaderBoard.Instance.addToBoard(_winnerPlayer);
            count++;
            Debug.Log("winner added to list" + " and the current count is: " + count);
        }
        if (currentScene.buildIndex == 4)
        {
            winnerPlayer = _winnerPlayer;
            count++;
        }
    }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GameManager"), Vector3.zero, Quaternion.identity);

            currentScene = scene;
            instantiatePlayerManager();
        }
        if (scene.buildIndex == 4)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GameManager"), Vector3.zero, Quaternion.identity);
            currentScene = scene;
            count = 0;
            instantiatePlayerManager();
        }
    }

    private void instantiatePlayerManager()
    {
        playerManagerObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }

    public void sendRoomCode(Player _player, string _roomcode)
    {
        photonView.RpcSecure("authenticateRoomCode", RpcTarget.MasterClient, true, _player, _roomcode);
    }

    //Added additional authentication another reason for it is so I can add a delay to prevent players to enter by bruteforcing random room name
    public void authenticate(string _roomCode, Player _player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(delay()); //Add a delay
            if (String.Equals(_roomCode, roomCode))
            {
                photonView.RpcSecure("authenticateSuccess", RpcTarget.All, true, _player);
                authenticatedUserIDList.Add(_player.UserId);
            }
            else
            {
                PhotonNetwork.CloseConnection(_player);
            }
        }
    }

    public void startGame()
    {
        foreach (Player _player in PhotonNetwork.PlayerListOthers)
        {
            bool isAuth = false;
            string _playerID = _player.UserId;
            foreach (string _userID in authenticatedUserIDList)
            {
                if (_playerID.Equals(_userID))
                {
                    isAuth = true;
                    break;
                }
            }
            if (isAuth == false)
            {
                PhotonNetwork.CloseConnection(_player);
            }
        }
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(5.0f);
    }

    [PunRPC]
    private void authenticateRoomCode(Player _player, string _roomCode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"receivedCode: {_roomCode}");
            authenticate(_roomCode, _player);
        }
    }

    [PunRPC]
    private void authenticateSuccess(Player _player)
    {
        if (String.Equals(PhotonNetwork.LocalPlayer.UserId, _player.UserId) && !PhotonNetwork.IsMasterClient)
        {
            MainMenuScript.Instance.authenticationSuccess();
        }
    }
}
