using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LatencyUI : MonoBehaviour
{
    public Text latencyText;
    private PlayerManager playerManager;

    private void Start()
    {
        InvokeRepeating("latencyOutput", 1f, 1f );
    }
    
    private void Update()
    {
        if (playerManager == null)
        {
            GameObject[] _playerManagerObjects = GameObject.FindGameObjectsWithTag("PlayerManager");
            if (_playerManagerObjects.Length == 0)
            {
                return;
            }
            for (int i = 0; i < _playerManagerObjects.Length; i++)
            {
                PlayerManager _playerManager = _playerManagerObjects[i].GetComponent<PlayerManager>();
                Debug.Log($"is player manager object null: {_playerManager == null}, is player object null: {_playerManagerObjects[i] == null}");
                if (_playerManager.isLocalPlayer())
                {
                    playerManager = _playerManager;
                    return;
                }
            }
        }
        
    }

    private void latencyOutput()
    {
        if (playerManager != null)
        {
            latencyText.text = ($"Latency: {playerManager.getPlayerLatency()}");
        }
    }
}
