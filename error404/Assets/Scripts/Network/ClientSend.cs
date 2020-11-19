using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Like the server, the client will also have a class that's purpose is to
 * handle turning the data to bytes to be able to be sent to the server
 */
public class ClientSend : MonoBehaviour
{
    private static void sendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.sendData(_packet);
    }

    private static void sendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.sendData(_packet);
    }

    public static void welcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            sendTCPData(_packet);
        }
    }

    public static void playerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            sendUDPData(_packet);
        }
    }

    public static void playerShoot(Vector3 _playerForward)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(_playerForward);
            sendTCPData(_packet);
        }
    }
}
