using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Text healthText;
    int health;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.players[Client.instance.myId] != null)
        {
            health = GameManager.players[Client.instance.myId].health;
            healthText.text = "Health: " + health.ToString();
        }
    }
}
