using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WinnerScene : MonoBehaviourPunCallbacks
{

    public Text winnerText;
   
    // Start is called before the first frame update
    void Start()
    {

        winnerText.text = RoomManager.Instance.getWinners()[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
