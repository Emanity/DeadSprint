using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraTrackPlayer : MonoBehaviourPunCallbacks
{
    private GameObject player;
    public GameObject ldrBoard;
    
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


        if (ldrBoard.activeSelf && (transform.position.y <= 8.5 || transform.position.y >= -10.3))
        {
            

            float movex = Input.GetAxisRaw("Horizontal"); //x = key A (-1) , key D (1)

            float x = transform.position.x + (movex / 20);

            float movey = Input.GetAxisRaw("Vertical");

            float y = transform.position.y + (movey / 20);

            transform.position = new Vector3(x, y, -10f);
        }
        else if (player.transform.position.y >= 8.5 || player.transform.position.y <= -10.3)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, -10f);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        }

    }
}
