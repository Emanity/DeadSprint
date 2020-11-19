using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/* While there is sending packets, there will also be handling packets that the client will receive
 * this will have functions that are responsible on reverting back the data to its original data type
 * and do what needs to be done with it.
 */
public class ClientHandle : MonoBehaviour
{
    //ClientHadle class will process the packet that has been received to be read and use the data sent from the server.
    public static void welcome(Packet _packet)
    {
        //note: write the code in order the same way from ServerSend class at least for TCP packets

        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from the server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.welcomeReceived();

        Client.instance.udp.connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void spawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();

        GameManager.instance.spawnPlayer(_id, _username, _position);
    }

    public static void playerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        GameManager.players[_id].setPos(_position);
    }

    public static void playerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.players[_id].transform.rotation = _rotation;
    }

    public static void removePlayerObject(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void winner(Packet _packet)
    {
        string _username = _packet.ReadString();

        Debug.Log(_username);
        GameManager.instance.winnerUsername = _username;
        GameManager.instance.loadWinnerScene();
        Client.instance.disconnect();
        //winner announce send everyone back to main menu which should disconnect them from the server
    }

    public static void playerSpectate(Packet _packet)
    {
        int _id = _packet.ReadInt();
        bool _playerSpawned = _packet.ReadBool();
        GameManager.players[_id].spectateMode(_playerSpawned);
    }

    public static void playerProjectile(Packet _packet)
    {
        /* projectile id
         * position of projectile
         */
    }

    public static void playerUpdateHealth(Packet _packet)
    {
        /* player id that got hit
         * health of server player and set to client player health
         */
        int _playerId = _packet.ReadInt();
        int _health = _packet.ReadInt();

        GameManager.players[_playerId].health = _health;
    }

    public static void playerUpdateStamina(Packet _packet)
    {
        /* player id
         * stamina of server player and set to client player stamina
         */
        int _playerId = _packet.ReadInt();
        float _stamina = _packet.ReadFloat();

        GameManager.players[_playerId].stamina = _stamina;
    }

    public static void extinctionEvent(Packet _packet)
    {
        // read message and display message on player's UI
        string _message = _packet.ReadString();
        Debug.Log($"extinction message: {_message}");
        GameManager.instance.extinctionEventMessage = _message;
    }
}
