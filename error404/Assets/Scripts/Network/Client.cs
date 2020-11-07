using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

/* Like from the server's client class this stores network information which handles
 * network events such as the connect/disconnect, initialize and store packets to be handled
 * 
 */
public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 23232;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //ensures only 1 instance of the client class exist
            Debug.Log("Instance already exist, destroying object");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        disconnect();
    }

    public void connectToServer()
    {
        initializeClientData();
        isConnected = true;
        tcp.connect();
        Debug.Log($"Client connecting to {ip}");
    }

    public class TCP
    {
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet receivedData;

        public void connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, connectCallback, socket);
        }

        private void connectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, receiveCallback, null);
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
                Debug.Log($"Error sending data to server via TCP: {_e}");
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
                    instance.disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                //handle data 6:38 part 2
                receivedData.Reset(handleData(_data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, receiveCallback, null);
            }
            catch
            {
                //disconnect
                disconnect();
            }
        }

        //NEEDS COMMENT 7:30 Part 2
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
                        packetHandlers[_packetId](_packet);
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
            instance.disconnect();
            stream = null;
            receiveBuffer = null;
            receivedData = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(receiveCallback, null);

            using (Packet _packet = new Packet())
            {
                sendData(_packet);
            }
        }

        //3:28 part 3
        public void sendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null); //note ToArray() is the packet's bytes
                }
            }
            catch (Exception _e)
            {
                Debug.Log($"Error sending data to server via UDP: {_e}");
            }
        }

        //1:20 part 3 
        private void receiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(receiveCallback, null);

                if (_data.Length < 4) //check if there is a packet
                {
                    //disconnect
                    instance.disconnect();
                    return;
                }

                handleData(_data);
            }
            catch
            {
                //disconnect
                disconnect();
            }
        }

        //2:56 part 3
        private void handleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void disconnect()
        {
            instance.disconnect();
            endPoint = null;
            socket = null;
        }
    }
    private void initializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.welcome },
            {(int)ServerPackets.spawnPlayer, ClientHandle.spawnPlayer },
            {(int)ServerPackets.playerPosition, ClientHandle.playerPosition },
            {(int)ServerPackets.playerRotation, ClientHandle.playerRotation },
            {(int)ServerPackets.removePlayerObject, ClientHandle.removePlayerObject },
        };
        Debug.Log("Initialized packets.");
    }

    private void disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("disconnected from the server.");
        }
    }
}
