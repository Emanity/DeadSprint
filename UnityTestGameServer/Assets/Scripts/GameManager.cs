using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

/* This is where we could put all the events that occur in the game
 * Scoreboard, Objects picked up, tick, player respawn etc
 * reason to this is because the multiplayer game is server side and we want
 * everything to be simulated in the server to prevent cheating
 */
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public bool rejectMorePlayers = false;
    public bool hasGameStarted = false;
    public GameObject startBlock;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public bool winnerExist = false; //prevent game to continue when game is over


    private int currentTick;
    private Dictionary<int, Player> qualifiedPlayers = new Dictionary<int, Player>();
    private Dictionary<int, Player> unqualifiedPlayers = new Dictionary<int, Player>();
    private int tickStart;
    private bool hasAllPlayerSpawned = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //ensures only 1 instance of this class exist
            Debug.Log("Instance already exist, destroying object");
            Destroy(this);
        }
    }

    public void FixedUpdate()
    {
        if (winnerExist == false)
        {
            updatePlayerList();
            tick();
            checkIfAllPlayerSpawned();
            startGame();
            checkIfLastPlayerLeft();
            if (hasGameStarted) //ensure the extinction events only happens after game started
            {
                ExtinctionEvents.Kill.executeKill();
                //ExtinctionEvents.Freeze.executeFreeze();
            }
            //Debug.Log($"time elapsed: {currentTick / Constants.TICKS_PER_SEC}, time start: {tickStart / Constants.TICKS_PER_SEC} , {currentTick >= tickStart}");


        }
    }

    public Player instantiatePlayer()
    {
        //later on when we have more maps we will give it a Vector3 object for the spawnpoint on the other maps
        return Instantiate(playerPrefab, new Vector3(-15f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public void playerQualified(Player _player)
    {
        qualifiedPlayers.Add(_player.id, _player);
        Debug.Log($"number of qualified players: {qualifiedPlayers.Count}");
    }

    public void playerUnqualified(Player _player)
    {
        _player.playerUnqualified();
        unqualifiedPlayers.Add(_player.id, _player);
        nextRound();
    }

    public void playerSpawned(Player _player)
    {
        if (_player.playerSpawned == false)
        {
            _player.playerSpawned = true;
        }
    }

    private void checkIfLastPlayerLeft() //getting the winner works but trying to loop the game does not
    {
        try
        {
            if (hasAllPlayerSpawned && hasGameStarted)
            {
                Dictionary<int, Player> playersAlive = new Dictionary<int, Player>();
                foreach  (Player _player in players.Values)
                {
                    if (_player.playerSpawned == true)
                    {
                        playersAlive.Add(_player.id, _player);
                    }
                }

                Debug.Log($"number of players alive: {playersAlive.Count}");
                if (playersAlive.Count == 1)
                {
                    foreach (Player _player in players.Values)
                    {
                        if(qualifiedPlayers.Count == 1) //if last one alive and there are no qualified players then this is the winner
                        {
                            Debug.Log($"winner: {_player.username}");
                            if (_player.playerQualified)
                            {
                                Debug.Log($"player {_player.id} won");
                                winnerExist = true;
                                ServerSend.winner(_player.username);
                                //player wins
                                //end game (for now we should just return them to menu)
                                break;
                            }
                        }

                    }

                    if (winnerExist == false) //prevents from game to continue when a winner has been found
                    {
                        foreach (Player _player in playersAlive.Values)
                        {
                            //Debug.Log($"Player {_player.id} is not qualified for the next round");
                            playerUnqualified(_player);
                            //next round
                        }
                    }
                }
            }
        }
        catch (Exception _e)
        {
            Debug.Log($"Player Object is null: {_e}");
        }
    }


    private void tick()
    {
        //Debug.Log($"current tick: {currentTick}");
        currentTick += 1;
    }

    private void checkIfAllPlayerSpawned()
    {
        /* get all clients from Server class
         * loop to check all clients's playerobject that they have spawned 
         * (and maybe also consider if notQualified is false) once all players 
         * have spawned then proceed to start the game
         * 
         * Note: it must be player objects because the client classes have already been instantiated
         * why? because I use the number of players to ensure that the game does not start with no players
         * 
         * later on we should probably make a ready button for the players to be able to start the game
         */
        if (hasAllPlayerSpawned == false && currentTick >= Constants.TICKS_PER_SEC * 10) //only run this when not all players have spawned and 10 seconds to give players time to join (only at first round)
        {
            foreach (Player _player in players.Values)
            {
                if (_player != null)
                {
                    if (_player.playerSpawned == false)
                    {
                        //Debug.Log($"playerId: {_client.player.id}, playerSpawned: {_client.player.playerSpawned}");
                        break; //stops loop when one of players have not spawned
                    }
                }
            }

            if (players.Count >= Constants.MIN_PLAYERS) //minimum of 2 players atleast to be able to start
            {
                hasAllPlayerSpawned = true;
                tickStart = tickEventOver(5);
            }
        }
    }

    private void updatePlayerList()
    {
        Dictionary<int, Player> tempPlayers = new Dictionary<int, Player>(); //temp (temporary)

        foreach (Client _client in Server.clients.Values)
        {
            if (_client != null)
            {
                if (_client.player != null)
                {
                    tempPlayers.Add(_client.id, _client.player);
                }
            }
        }
        players = tempPlayers;
    }

    //what tick the event (effects in the game or when the game starts etc) finishes
    private int tickEventOver(int _durationInSeconds)
    {
        int tempTick = _durationInSeconds * Constants.TICKS_PER_SEC;
        return currentTick + tempTick;
    }

    private void startGame()
    {
        if (hasAllPlayerSpawned == true)
        {
            //Debug.Log($"time elapsed: {currentTick / Constants.TICKS_PER_SEC}, time start: {tickStart / Constants.TICKS_PER_SEC} , {currentTick >= tickStart}");
            if (currentTick >= tickStart)
            {
                if (hasGameStarted == false) //to ensure this only happens once every round
                {
                    Destroy(startBlock);
                    hasGameStarted = true;
                    rejectMorePlayers = true;
                }
            }
        }
    }

    private void nextRound()
    {
        if(winnerExist == false)
        {
            foreach (Player _player in players.Values)
            {
                if (_player.playerQualified == true)
                {
                    _player.nextRound();
                }
            }
            hasGameStarted = false;
            hasAllPlayerSpawned = false;
            qualifiedPlayers = new Dictionary<int, Player>();
        }
    }

    //function to reset variables for next game when unqualified player has been chosen
}