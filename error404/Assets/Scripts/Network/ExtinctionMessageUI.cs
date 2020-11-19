using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ExtinctionMessageUI : MonoBehaviour
{
    public Text extinctionMessageText;
    private System.Random rng;
    void FixedUpdate()
    {
        if (string.IsNullOrEmpty(GameManager.instance.extinctionEventMessage) == false)
        {
            rng = new System.Random();
            extinctionMessageText.text = GameManager.instance.extinctionEventMessage;
        }
    }
}
