using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class LdrListItem : MonoBehaviourPunCallbacks
{
    public Text text;

    public void SetUp(String name)
    {
       
        text.text = name;
    }
}
