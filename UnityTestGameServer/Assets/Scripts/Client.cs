using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Net;
using System;

/* Stores data for the game but mainly to store network information and connection (such as tcp/udp)
 * which also handles certain network events such as connect/disconnect from the server, sending
 * the player to into the game
 */
public class Client
{
    public static int dataBufferSize = 4096;
    public int id;
    public Player player;
    public TCP tcp;
    public UDP udp;

    public Client(int _id)
    {
        id = _id;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        public void connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            receivedData = new Packet();

            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, receiveCallback, null);

            ServerSend.welcome(id, "welcome to el server");
        }

        public void sendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); //note: begin write(converts data to packet to be sent), beginread(revert data to original to be read)
                }
            }
            catch (Exception _e)
            {
                Debug.Log($"Error sending TCP data to player {id} via TCP: {_e}");
            }
        }

        private void receiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    //disconnect
                    Server.clients[id].disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                //handle data
                receivedData.Reset(handleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, receiveCallback, null);
            }
            catch (Exception _e)
            {
                Debug.Log($"Error receiving TCP data: {_e}");
                Server.clients[id].disconnect();
            }
        }

        private bool handleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                        /*note: packetHandlers is the dictionary within the Server class and [_packetId] is the index given to the dictionary which (id, _packet)
                         * is what is used to create the ServerHandle, which within the ServerHandle it will call the method that matches the data within _packet
                         */
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }
            return false;
        }

        public void disconnect()
        {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        public void connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }

        public void sendData(Packet _packet)
        {
            Server.sendUDPData(endPoint, _packet);
        }

        public void handleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet);
                }
            });
        }

        public void disconnect()
        {
            endPoint = null;
        }
    }

    public void sendIntoGame(string _playerName)
    {
        /* implement a wait system for game to start with minimum players
         * if (Server.clients.Count >= Constants.MIN_PLAYERS)
         * {
         *      start countdown from 15 seconds
         *      once countdown finished start game
         *      just paste the code below here
         * }
         */

        //later we need to handle this to players who are trying to join we can notify them that game has started and reject their connection
        if (GameManager.instance.rejectMorePlayers == false) //prevent more players to join, and prevent those that lost to rejoin to be able to play on the next round
        {
            player = GameManager.instance.instantiatePlayer();
            Debug.Log(player.transform.rotation);
            player.initialize(id, _playerName);

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.id != id)
                    {
                        ServerSend.spawnPlayer(id, _client.player);
                        GameManager.instance.playerSpawned(_client.player);
                    }
                }
            }

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerSend.spawnPlayer(_client.id, player);
                    GameManager.instance.playerSpawned(_client.player);
                }
            }
        }
    }

    public void disconnect() //this will also be used to disconnect players from server when game ended
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            try
            {
                UnityEngine.Object.Destroy(player.gameObject);
            }
            catch (Exception _e) //just in case player disconnects while unqualified/qualified spectating
            {
                Debug.Log($"No player game object to be destroyed: {_e}");
            }
            player = null;
        });

        tcp.disconnect();
        udp.disconnect();

        ServerSend.removePlayerObject(id);
    }

    public void banIP()
    {
        /* when malicious activity or cheating has been detected this function will be used to reject
         * the client to connect with the server
         * 
         * we will be storing these banned IP addresses locally
         */
    }
}
