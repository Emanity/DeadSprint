using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/* This is where we could put all the events that occur in the game
 * Scoreboard, Objects picked up, tick, player respawn etc
 * reason to this is because the multiplayer game is server side and we want
 * everything to be simulated in the server to prevent cheating
 */
public class GameLogic : MonoBehaviour
{
    public static int currentTick;

    public void Update()
    {
        tick();
    }

    private void tick()
    {
        //Debug.Log($"current tick: {currentTick}");
        currentTick += 1;
    }
}
