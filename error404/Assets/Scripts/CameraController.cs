using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player; 

    // Update is called once per frame
    void Update()
    {
        try
        {
            //finds the localPlayer
            if (player == null)
            {
                player = GameObject.Find("player1(Clone)");
            }
            //need to set the z position for the camera properly
            Vector3 setZValue = new Vector3(player.transform.position.x, player.transform.position.y, -10);
            transform.position = setZValue;

        }
        catch (Exception _e)
        {
            print(_e);
        }
    }
}
