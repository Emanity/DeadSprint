using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this listens for any input from the computer peripherals of the client
 */
public class PlayerController : MonoBehaviour
{
    private void FixedUpdate()
    {
        sendInputToServer();
    }

    private void sendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D)
        };

        ClientSend.playerMovement(_inputs);
    }
}
