using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this listens for any input from the computer peripherals of the client
 */
public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;

    private bool isFacingRight = true;
    private bool[] inputs;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientSend.playerShoot(playerTransform.transform.forward);
        }
    }
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
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.LeftShift),
        };

        inputs = _inputs;
        //flipPlayerFacing(); //ensure player is facing at the right direction according to its input

        ClientSend.playerMovement(_inputs);
    }

    private void flipPlayerFacing()
    {
        if (inputs[2] && isFacingRight) //if direction going to left and facing right
        {
            flip();
        }
        if (inputs[3] && isFacingRight == false) //if direction going to right and facing left
        {
            flip();
        }

        Debug.Log($"current rotation: {transform.rotation}, facing right: {isFacingRight}");
    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
