using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

/*
 * Everything should be updated from the RoomManager
 * This will contain methods to store and manage the players properties
 */
public class GameManager : MonoBehaviourPun
{
    private bool isWinner;
    private bool gameFinished;
    public static GameManager Instance;

    private bool isReady;

    private int total;

    private const byte SYNC_WINNER_EVENT_CODE = 0;
    private const byte SYNC_EXTINCTION_EVENT_CODE = 1;

    private void Awake()
    {
        
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        resetVariables();
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient) //GameManager will only be performed on master client except for receiving RaiseEvents
        {
            if (gameFinished)
            {
                return;
            }
            checkIfPlayersFinished();
            //rollExtinctionEvent();
            //demoRollExtinctionEvent();
        }
    }

    public void resetVariables()
    {
        total = PhotonNetwork.CurrentRoom.PlayerCount;
        isWinner = false;
        isReady = false;
        gameFinished = false;
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }


    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (PhotonNetwork.MasterClient.ActorNumber == PhotonNetwork.CurrentRoom.GetPlayer(obj.Sender).ActorNumber) //check if sender is master client
        {
            Debug.Log($"event code: {obj.Code}");
            if (obj.Code == SYNC_EXTINCTION_EVENT_CODE)
            {
                /*
                object[] data = (object[])obj.CustomData;
                string extinctionEvent = (string)data[0];
                if (extinctionEvent.Equals("Kill"))
                {
                    ExtinctionEvent.Kill.execute();
                }
                if (extinctionEvent.Equals("Laser"))
                {
                    ExtinctionEvent.Laser.execute();
                }
                if (extinctionEvent.Equals("Blizzard"))
                {
                    ExtinctionEvent.Blizzard.execute();
                }
                */
            }
        }
    }

    public bool getReadyStatus()
    {
        return isReady;
    }

    private void playerFinished(Player _player)
    {
        Debug.Log($"{_player} finished");
        if (!isWinner || !photonView.IsMine)
        {
            //RoomManager.Instance.setWinnerPlayer(_player);
        }
        isWinner = true;
        if (photonView.IsMine && RoomManager.Instance.getCount() == total) //example of why checks are important remove this and have 2 players, it should give a warning which does not sync scene on master client
        {
            // PhotonNetwork.LoadLevel(2); //make this load the level once the last player has finished
            isReady = true;
            Debug.Log("ready to move to the next level");

        }
    }

    private void checkIfPlayersFinished()
    {
        /*if (isWinner)
        {
           // Debug.Log("Player finished");
            return;
        }
        else
        {*/
        GameObject[] _playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < _playerObjects.Length; i++)
        {
            if (_playerObjects[i].transform.position.x >= 80f && _playerObjects[i].transform.position.y >= 0f && _playerObjects[i].transform.position.y <= 7f) //4f represents the finish point
            {
                Debug.Log($"check player finished method is fucking successful, player length: {_playerObjects.Length}");

                int playerViewID = _playerObjects[i].GetComponent<PlayerController>().photonView.ViewID;
                object[] dataWinner = new object[] { playerViewID };
                sendRaiseEvent(SYNC_WINNER_EVENT_CODE, dataWinner);

                Player player = _playerObjects[i].GetComponent<PhotonView>().Owner;

                //reset game level once last player has finished
                if (_playerObjects.Length == 1)
                {
                    gameFinished = true;
                    RoomManager.Instance.playerDisqualified(player);
                }
                playerFinished(player);
            }
        }
        //}

    }

    /* this won't work on your current version
    private void demoRollExtinctionEvent() //only for demo purposes
    {
        if (Input.GetKey(KeyCode.Q))
        {
            //photonView.RPC("ReceiveExtinctionEvent", RpcTarget.All, true);
            string extinctionEvent = ExtinctionEvent.rollEvent();
            if (extinctionEvent.Length != 0)
            {
                object[] data = new object[] { extinctionEvent };
                sendRaiseEvent(SYNC_EXTINCTION_EVENT_CODE, data);
            }
            //extinctionEvent(true); commented to test whether photon RaiseEvent/RPC works (because it takes some time to build & run)
            
        }
    }

    private void rollExtinctionEvent()
    {
        string extinctionEvent = ExtinctionEvent.rollEvent();
        if (extinctionEvent.Length != 0)
        {
            object[] data = new object[] { extinctionEvent };
            sendRaiseEvent(SYNC_EXTINCTION_EVENT_CODE, data);
        }
    }
    */

    private void sendRaiseEvent(byte _eventCode, object[] _data)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(_eventCode, _data, raiseEventOptions, SendOptions.SendReliable);
    }
    private void extinctionEvent(bool _extinctionEventBool)
    {
        if (_extinctionEventBool)
        {
            GameObject[] _playersObject = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < _playersObject.Length; i++)
            {
                PlayerProperties _playerProperties = _playersObject[i].GetComponent<PlayerController>().getPlayerProperties();
                _playerProperties.damage(ExtinctionEvent.Kill.getDamage());
                Debug.Log("extinction event");
            }
        }
    }
}
