using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

public class ldrBoardScript : MonoBehaviourPun
{
    public bool isFinish;
    public GameObject ldrBoard;
    private List<int> receivedPlayerViewID = new List<int>();
    
    private const byte SYNC_WINNER_EVENT_CODE = 0;
    private const byte SYNC_WINNERS_EVENT_CODE = 2;

    //Note: I moved what was done on the update method to GameManager at function checkIfPlayersFinished()

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

        if (PhotonNetwork.MasterClient.ActorNumber == PhotonNetwork.CurrentRoom.GetPlayer(obj.Sender).ActorNumber)
        {
            if (obj.Code == SYNC_WINNER_EVENT_CODE)
            {
                Debug.Log($"testerino: {PhotonNetwork.MasterClient.ActorNumber == PhotonNetwork.CurrentRoom.GetPlayer(obj.Sender).ActorNumber}");
                object[] data = (object[])obj.CustomData;
                int photonViewID = (int)data[0];
                if (!receivedPlayerViewID.Contains(photonViewID))
                {
                    receivedPlayerViewID.Add(photonViewID);
                    GameObject[] _playerObjects = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < _playerObjects.Length; i++)
                    {
                        PhotonView playerPhotonView = _playerObjects[i].GetComponent<PlayerController>().photonView;
                        if (playerPhotonView.ViewID == photonViewID)
                        {
                            Debug.Log($"player disabled {photonViewID}");
                            isFinish = true;
                            _playerObjects[i].SetActive(false);
                            if (_playerObjects[i].GetComponent<PhotonView>().Owner.IsLocal)
                            {
                                isFinished();
                            }

                            RoomManager.Instance.setWinnerPlayer(playerPhotonView.Owner);

                            if (PhotonNetwork.IsMasterClient) //this is temporary should be done locally on room master
                            {
                                string[] winnerArray = RoomManager.Instance.getWinners();
                                object[] winnersData = new object[winnerArray.Length];
                                winnerArray.CopyTo(winnersData, 0);

                                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                                PhotonNetwork.RaiseEvent(SYNC_WINNERS_EVENT_CODE, winnersData, raiseEventOptions, SendOptions.SendReliable);
                            }
                            break;
                        }
                    }
                }
            }

            if (obj.Code == SYNC_WINNERS_EVENT_CODE) 
            {
                if (PhotonNetwork.IsMasterClient == false) //this is temporary
                {
                    object[] data = (object[])obj.CustomData;
                    string[] winnersArray = new string[data.Length];
                    data.CopyTo(winnersArray, 0);
                    RoomManager.Instance.setWinners(winnersArray);
                    LeaderBoard.Instance.updateBoard();
                }
            }
        }
    }

    public void isFinished()
    {
        if (isFinish)
        {
            ldrBoard.SetActive(true);
            
        }
    }
}
