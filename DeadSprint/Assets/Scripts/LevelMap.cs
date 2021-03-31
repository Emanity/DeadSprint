using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Linq;

public class LevelMap : MonoBehaviourPunCallbacks
{

    public GameObject startGameButton;

    public GameObject startGameCanvas;


    public void OnClick()
    {
        startGameCanvas.SetActive(true);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(4);
    }
}
