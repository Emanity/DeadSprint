using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EndOfLevel1Button : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void OnClick()
    {
/*        if (GameManager.Instance.getReadyStatus())
        {
            PhotonNetwork.LoadLevel(2);
        }*/
        PhotonNetwork.LoadLevel(2);
    }
}
