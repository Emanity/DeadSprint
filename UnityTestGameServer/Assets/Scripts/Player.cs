using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The player object that stores information that will be used for the game's simulation
 * In here it will process input from the client which will be used for only events that relate
 * to the player and store data that is crucial for the game such as health or points
 */
public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public float gravity = -10f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;

    private bool[] inputs;
    private float yVelocity = 0;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        inputs = new bool[4]; //probably going to be updated for future input such as Shift to dash
    }

    public void setInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        move(_inputDirection);
    }

    private void move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.up * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[0])
            {
                yVelocity += jumpSpeed;
            }
        }
        yVelocity += gravity;
        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        Debug.Log($" player {id}: {_moveDirection.x}, {_moveDirection.y}");
        //transform.Translate(_inputDirection.x * moveSpeed, _inputDirection.y * moveSpeed, 0);

        ServerSend.playerPosition(this);
        ServerSend.playerRotation(this);
    }
}
