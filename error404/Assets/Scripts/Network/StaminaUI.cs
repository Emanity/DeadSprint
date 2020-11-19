using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public Text staminaText;
    float stamina;

    void FixedUpdate()
    {
        if (GameManager.players[Client.instance.myId] != null)
        {
            stamina = (int)System.Math.Round(GameManager.players[Client.instance.myId].stamina);
            staminaText.text = "Stamina: " + stamina.ToString();
        }
    }
}
