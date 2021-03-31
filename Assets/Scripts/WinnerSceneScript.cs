using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Linq;

public class WinnerSceneScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void NextScene()
    {
        PhotonNetwork.LoadLevel(3);
    }
}
