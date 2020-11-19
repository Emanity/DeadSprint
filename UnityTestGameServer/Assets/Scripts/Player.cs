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
    public int health;
    public float stamina;
    public bool playerSpawned = false; //prevent player to be spawned again and to check if player has spawned
    public bool playerNotQualified = false; //prevent players who have lost to be able to spawn in the next round.
    public bool playerQualified = false; //identify to those that are able to spawn next round
    public bool isFrozen = false;

    private bool[] inputs;
    private float yVelocity = 0;
    //this will be a list of spawnPoint later when we have more maps
    private Vector3 spawnPoint = new Vector3(-15f, 0f, 0f);

    private bool isSprinting = false;

    //need to store this locally becaue it will be changed when player is in spectator mode
    private float charControllerRadius = 0.2f;
    private float charControllerHeight = 1.75f;

    public Transform firePoint;


    private void Start()
    {
        //we need the game object to travel on sync with the tickrate
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;

        health = Constants.MAX_PLAYER_HEALTH_STAMINA;
        stamina = Constants.MAX_PLAYER_HEALTH_STAMINA;
    }

    public void initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        inputs = new bool[5]; //probably going to be updated for future input such as Shift to dash
    }

    public void setInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0]) //W key
        {
            _inputDirection.y += 1;
        }
        if (inputs[1]) //S key
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2]) //A key
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3]) //D key
        {
            _inputDirection.x += 1;
        }

        if (playerSpawned)
        {
            move(_inputDirection);
        }
        else
        {
            spectatorMove(_inputDirection);
        }
        regenStamina();
        setCharControllerSpectate();
        checkPlayerFinish();
        checkPlayerFellOffMap();
        checkPlayerHealth();
        //Debug.Log($"Player {id}, xyz position: {transform.position}, player qualified: {playerQualified}");
    }

    public void setMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
        moveSpeed *= Time.fixedDeltaTime;
    }

    public void playerUnqualified()
    {
        playerSpawned = false;
        playerQualified = false;
        playerNotQualified = true;
    }

    public void spectatorMode() 
    {
        ServerSend.playerSpectate(id, playerSpawned);
    }

    //to autoamtically disable/enable collision when player has not spawned
    public void setCharControllerSpectate()
    {
        if (playerSpawned)
        {
            CharacterController playerCharacterController = GetComponent<CharacterController>();
            playerCharacterController.radius = charControllerRadius;
            playerCharacterController.height = charControllerHeight;
        }
        else
        { //this doesnt work properly as I plan to get rid of the collision but somehow it still exist in the game
            CharacterController playerCharacterController = GetComponent<CharacterController>();
            playerCharacterController.radius = 0;
            playerCharacterController.height = 0;
        }
    }

    //for the 2d movement
    private void move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.up * _inputDirection.y; //movement for both x and y

        //for the sprinting mechanic
        if (inputs[4] && (inputs[2] || inputs[3]) && stamina >= 0)
        {
            if (isSprinting == false)
            {
                isSprinting = true;
                setMoveSpeed(Constants.PLAYER_SPRINT_SPEED);
            }
            useStamina();
            _moveDirection *= moveSpeed;
        }
        else if (inputs[4] == false && (inputs[2] || inputs[3]))
        {
            isSprinting = false;
            setMoveSpeed(Constants.PLAYER_MOVESPEED);
            _moveDirection *= moveSpeed;
        }

        //to prevent players from jumping when on air
        if (controller.isGrounded)
        {
            jump();
        }
        yVelocity += gravity;
        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        //Debug.Log($" player {id}: {_moveDirection.x}, {_moveDirection.y}");

        ServerSend.playerPosition(this);
        ServerSend.playerRotation(this);
        ServerSend.playerUpdateStamina(id, stamina);
    }

    public void playerShoot(Vector3 _playerDirection)
    {
        Debug.Log($"player direction: {_playerDirection}");
        if (Physics.Raycast(firePoint.position, _playerDirection, out RaycastHit hit, 25f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Player>().damage(50);
            }
        }
    }

    //later instead of using the player object to spectate we should just have the camera switch to player who is still playing
    private void spectatorMove(Vector2 _inputDirection)
    {
        spectatorMode();

        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.up * _inputDirection.y;
        _moveDirection *= moveSpeed;

        controller.Move(_moveDirection);

        ServerSend.playerPosition(this);
    }

    private void jump()
    {
        yVelocity = 0f;
        if (inputs[0])
        {
            yVelocity += jumpSpeed;
        }
    }

    //damage inflicted on weapons/extinction event
    public void damage(int _damage)
    {
        if (playerSpawned)
        {
            if (health <= 0)
            {
                //if somehow got to this point with less than or is 0 then exit method
                return;
            }
            health -= _damage;
            if (health <= 0)
            {
                health = 0;
            }

            ServerSend.playerUpdateHealth(id, health);
        }
    }

    //used for pickable objects
    public void heal(int _healAmount)
    {
        if (health >= 100) //ensure it won't use any more health if more than or equal to 100 health
        {
            return;
        }
        health += _healAmount; //ensure it will never more than 100 health, otherwise they would avoid the kill extinction event
        if (health > 100)
        {
            health = 100;
        }
    }

    //whenever player uses sprint
    private void useStamina()
    {
        if (stamina <= 0) //ensure it won't use any more stamina if less than or equal to 0 stamina
        {
            return;
        }

        stamina -= 0.75f;
        if (stamina < 0) //ensures that stamina will never go less than 0, otherwise there will be multiple errors
        {
            stamina = 0f;
        }
    }

    //used for pickable objects
    public void recoverStamina(float _staminaAmount)
    {
        if (stamina >= 100) //ensure it won't use any more health if more than or equal to 100 stamina
        {
            return;
        }
        stamina += _staminaAmount; //ensure it will never go more than 100 stamina, otherwise they can stack up stamina
        if (stamina > 100)
        {
            stamina = 100f;
        }
    }

    //later on we will need a cooldown to when stamina can regen again!!!
    private void regenStamina()
    {
        if (isSprinting == false && stamina <= 100)
        {
            stamina += 0.02f;
            if (stamina >= 100) //ensure it doesnt go more than 100
            {
                stamina = 100f;
            }

        }
    }

    private void checkPlayerFinish()
    {
        //later on for future maps we will also have a list of finish x axis position
        if (transform.position.x >= Constants.FINISH_POSITIONS_X && playerSpawned == true)
        {
            playerSpawned = false;
            playerQualified = true;
            playerNotQualified = false;
            GameManager.instance.playerQualified(this);
        }
    }

    //later on we need to apply a delay to respawn
    private void respawn()
    {
        transform.position = spawnPoint;
        health = Constants.MAX_PLAYER_HEALTH_STAMINA;
    }

    private void checkPlayerFellOffMap()
    {
        if (transform.position.y <= -10f)
        {
            respawn();
        }
    }

    private void checkPlayerHealth()
    {
        if (health <= 0)
        {
            respawn();
            health = Constants.MAX_PLAYER_HEALTH_STAMINA;
        }
    }

    //currently does not work
    public void nextRound() //later on we will need to change this to work with the next map
    {
        if (GameManager.instance.winnerExist == false)
        {
            if (playerQualified && playerSpawned == false)
            {
                respawn();
                Debug.Log($"Player {id} nextRound method works, {transform.position}, {playerQualified}");
                setMoveSpeed(0f);
                playerQualified = false;
                GameManager.instance.playerSpawned(this);
            }
        }
    }
}
