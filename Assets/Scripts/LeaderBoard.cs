﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class LeaderBoard : MonoBehaviourPunCallbacks
{
    public static LeaderBoard Instance;


    public Transform ldrListContent;
    public GameObject ldrListItemPrefab;

    private int winners;
    private void Awake()
    {
        //checks if another RoomManager exits and if there is it will destroy it
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        //DontDestroyOnLoad(gameObject);
        Instance = this;

        winners = RoomManager.Instance.getCount();
        
        Debug.Log("leaderboard is awake - current count on leaderboard is " + winners);
    }
    private void Start()
    {
        Instantiate(ldrListItemPrefab, ldrListContent).GetComponent<LdrListItem>().SetUp(RoomManager.Instance.getWinnerPlayer().NickName);

    }
    private void FixedUpdate()
    {

    }
        
    public void addToBoard(Player _player)
    {
        Debug.Log("addtoboard method");

        Instantiate(ldrListItemPrefab, ldrListContent).GetComponent<LdrListItem>().SetUp(_player.NickName);
        winners = RoomManager.Instance.getCount();
        Debug.Log("player added to board"+ "count:" + RoomManager.Instance.getCount() );
        
    }

}