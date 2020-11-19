using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is what the server will send
 * the methods here will get who to send it to and what to send
 * which will turn what is needed to send to the client into Byte
 */
public class ServerSend
{
    //in this class is what packets the server will send to the client
    private static void sendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.sendData(_packet);

    }

    private static void sendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.sendData(_packet);
        }
    }

    private static void sendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.sendData(_packet);
            }
        }
    }

    private static void sendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.sendData(_packet);
    }

    private static void sendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.sendData(_packet);
        }
    }

    private static void sendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.sendData(_packet);
            }
        }
    }

    public static void welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            sendTCPData(_toClient, _packet);
        }
    }

    public static void spawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            sendTCPData(_toClient, _packet);
        }
    }

    public static void playerPosition(Player _player)
    {
        //using a using block will automatically dispose because the packet class inherets from IDisposable and needs to be disposed once done with it
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            sendUDPDataToAll(_packet);
        }
    }

    public static void playerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            sendUDPDataToAll(_player.id, _packet);
        }
    }

    //to remove the player's object when they disconnected from the server
    public static void removePlayerObject(int _playerId)
    {
        using(Packet _packet = new Packet((int)ServerPackets.removePlayerObject))
        {
            _packet.Write(_playerId);

            sendTCPDataToAll(_packet); //tcp because we want to guarantee that the packet will be sent to all
        }
    }

    public static void winner(string _playerUsername)
    {
        using (Packet _packet = new Packet((int)ServerPackets.winner))
        {
            _packet.Write(_playerUsername);

            sendTCPDataToAll(_packet);
        }

        for (int i = 1; i < Server.clients.Count; i++)
        {
            Server.clients[i].disconnect();
        }
    }

    public static void playerSpectate(int _playerID, bool _playerSpawned)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerSpectate))
        {
            _packet.Write(_playerID);
            _packet.Write(_playerSpawned);
            sendTCPDataToAll(_packet);
        }
    }

    public static void playerUpdateHealth(int _playerID, int _health)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerUpdateHealth))
        {
            _packet.Write(_playerID);
            _packet.Write(_health);
            sendTCPDataToAll(_packet);
        }
    }

    public static void playerUpdateStamina(int _playerID, float _stamina)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerUpdateStamina))
        {
            _packet.Write(_playerID);
            _packet.Write(_stamina);
            sendUDPDataToAll(_packet);
        }
    }
    public static void extinctionEvent(string _extinctionEventMessage)
    {
        using (Packet _packet = new Packet((int)ServerPackets.extinctionEvent))
        {
            _packet.Write(_extinctionEventMessage);
            sendTCPDataToAll(_packet);
        }
    }

    /*
    public static void playerProjectile(int _playerID, int _projectileId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerProjectile))
        {
            _packet.Write(_playerID);
            sendUDPDataToAll(_packet);
        }
    }

    */
    //packet to send to get latency between the client, this is for lag compensation
}
