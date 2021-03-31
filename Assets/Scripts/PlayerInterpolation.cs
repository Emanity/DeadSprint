using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

/* For smooth movement and to monitor player position
 * 
 */
public class PlayerInterpolation : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 pos = Vector3.zero;
    private Quaternion rot = Quaternion.identity;
    private float lastTime;
    private const byte PLAYER_POSITION_CHANGE_EVENT = 0;
    private float distance;
    private Player sender;

    private void FixedUpdate()
    {
        if (photonView == null || photonView.IsMine) return; //interpolation will only apply to other players

        float rotDiff = Quaternion.Angle(rot, transform.rotation);
        if (rotDiff > 0f) transform.rotation = rot;


        //Debug.Log($"current: {pos}");
        float distance = Vector3.Distance(transform.position, pos);

        if (distance > 10f && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CloseConnection(sender);
        }

        if (distance < 4f)
        {
            transform.position = Vector3.Lerp(transform.position, pos, 0.5f);
            //Debug.Log($"yep: {(Time.time - lastTime) / (1.0f / (1000f / 64))}, lasttime: {lastTime}, {Time.time - lastTime}");
            //transform.position = Vector3.Lerp(transform.position, pos, (Time.time - lastTime) / (1.0f / (1000f / 64)));
        }
        else //sync to real position if previous position is too far from real position
        {
            transform.position = pos;
        }
    }
    public void OnPhotonSerializeView(PhotonStream _stream, PhotonMessageInfo _info)
    {
        if (_stream.IsWriting)
        {
            _stream.SendNext(transform.position);
            _stream.SendNext(transform.rotation);
        }
        else
        {
            pos = (Vector3)_stream.ReceiveNext();
            rot = (Quaternion)_stream.ReceiveNext();
            lastTime = Time.time;
            sender = _info.Sender;
        }
    }
    
}
