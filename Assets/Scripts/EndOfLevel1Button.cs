using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EndOfLevel1Button : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void OnClick()
    {
        if(PhotonNetwork.IsMasterClient && (RoomManager.Instance.getWinners().Length == PhotonNetwork.CurrentRoom.PlayerCount))
        {
            PhotonNetwork.LoadLevel(2);
        }
        
    }
}
