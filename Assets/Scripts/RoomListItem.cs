using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public Text text;

    public RoomInfo info;


    public void SetUp(RoomInfo _info) 
    {
        info = _info;
        text.text = _info.Name;
    }

    public void OnClick()
    {
        MainMenuScript.Instance.JoinRoom(info.Name);
    }
}
