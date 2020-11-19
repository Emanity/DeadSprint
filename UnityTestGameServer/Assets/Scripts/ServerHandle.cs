using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is where the packets received from the client will
 * be turned back to its original data type, then do what needs to be
 * done with that data such as a packet containing the client input
 * which will then be reverted back to its original data type and be processed
 * to get the next position of the player
 */
public class ServerHandle
{
    //ServerHandle class will process the packet that has been received to be read and use the data sent from the client.
    public static void welcomeReceived(int _fromClient, Packet _packet)
    {
        //note: write code by packet order from ClientSend class (for this it is welcomeReceived method in the client)
        int _clientId = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientId)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientId})!");
        }
        Server.clients[_fromClient].sendIntoGame(_username);
    }

    public static void playerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.setInput(_inputs, _rotation);
    }

    public static void playerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _playerDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.playerShoot(_playerDirection);
    }

    //packet to handle a get latency between the client, this is for lag compensation
}
