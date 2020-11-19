using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;

/* This sets up the server.
 * the Server will be waiting for any udp/tcp wanted to connect to the chosen port
 * for the clients to be able to connect with the server.
 */
public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>(); //to be able to track client's on server class
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers; //dictionary for packet handlers like the client

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    //Start method will assign and call all the methods needed to Start the server 
    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Debug.Log("Server is Starting...");
        initializeServerData(); //method to store the players and what packets needs to be handled, sent by the client

        //server will be listening for tcp packets on all network interface in the given port
        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(tcpConnectCallback, null);

        //server will be listening for udp packets on all network interface in the given port
        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(udpreceiveCallback, null);

        Debug.Log($"Server Started on port: {Port}");
        Debug.Log($"ticks per second: {Constants.TICKS_PER_SEC}");
        Debug.Log($"ms per tick: {Constants.MS_PER_TICK}");
    }

    //this method is what it will do once there is a tcp connection 
    private static void tcpConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(tcpConnectCallback, null); //this continues listening for more clients
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}");

        //later implement to prevent more players to join when game has started
        //assign new clients to an id
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.connect(_client);
                return; //ensure client takes only 1 player slot
            }
        }

        //once the for loop above is finished, it will mean the server is full 
        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect, server is full");
    }

    //this method is what it will do once there is a udp connection
    private static void udpreceiveCallback(IAsyncResult _result)
    {
        try //this is a try method because it is not guaranteed that a udp packet will be received
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(udpreceiveCallback, null);

            if (_data.Length < 4) //check if there is data to handle
            {
                return;
            }
            

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0) //check for the client id to prevent errors as I expect the client id to Start at 1
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null) //checks if the client is connected
                {
                    clients[_clientId].udp.connect(_clientEndPoint);
                    return;
                }

                //check if endpoint stored for the client matches the endpoint where the packet came from to prevent client impersonation by sending in a different client ID
                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString()) //ToString is used because without it, comparing the endpoint will return false even if IP & port matches
                {
                    clients[_clientId].udp.handleData(_packet);
                }
            }
        }
        catch (Exception _e)
        {
            Debug.Log($"Error receiving UDP data: {_e}");
        }
    }
    
    public static void sendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _e)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_e}");
        }
    }

    private static void initializeServerData()
    {
        //store clients in a data structure which i represents as the id
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }
        
        //store ServerHandle in the dictionary as those stored will handle the packets sent by the client, the storing will be done within the client class
        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.welcomeReceived },
                {(int)ClientPackets.playerMovement, ServerHandle.playerMovement },
                {(int)ClientPackets.playerShoot, ServerHandle.playerShoot },
            };
        Debug.Log("initialized packets.");
    }

    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }
}
