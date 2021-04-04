using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraTrackLevel2 : MonoBehaviourPunCallbacks
{
    private GameObject player;

    void Update()
    {
        if (player == null)
        {
            GameObject[] _playerManagerObjects = GameObject.FindGameObjectsWithTag("PlayerManager");
            if (_playerManagerObjects.Length == 0)
            {
                return;
            }
            for (int i = 0; i < _playerManagerObjects.Length; i++)
            {
                PlayerManager _playerManager = _playerManagerObjects[i].GetComponent<PlayerManager>();
                //Debug.Log($"is player manager object null: {_playerManager == null}, is player object null: {_playerManagerObjects[i] == null}");
                if (_playerManager.isLocalPlayer())
                {
                    player = _playerManager.getPlayer();
                    return;
                }
            }
        }


        if (player.transform.position.y >= 5.1 || player.transform.position.y <= -4.9)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, -10f);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        }

    }
}
