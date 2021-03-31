using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public Text staminaText;
    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("staminaOutput", 0.25f, 0.25f);
    }

    // Update is called once per frame
    void Update()
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

    private void staminaOutput()
    {
        if (playerManager != null && playerManager.getPlayer() != null)
        {
            staminaText.text = ($"Stamina: {playerManager.getPlayer().GetComponent<PlayerController>().getPlayerProperties().getStamina()}");
        }
    }
}
