using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This handles events and stores game objects such as players/objects to be spawned
 */
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //ensures only 1 instance of the client class exist
            Debug.Log("Instance already exist, destroying object");
            Destroy(this);
        }
    }

    //4:40
    public void spawnPlayer(int _id, string _username, Vector2 _position)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, Quaternion.AngleAxis(0, Vector3.forward));
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, Quaternion.AngleAxis(0, Vector3.forward));
        }

        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }
}
