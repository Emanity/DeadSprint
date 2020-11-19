using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to store all constant variable
public class Constants
{
    public const int TICKS_PER_SEC = 30;
    public const float MS_PER_TICK = 1000f / TICKS_PER_SEC;
    public const int MAX_PLAYER_HEALTH_STAMINA = 100;
    public const int MIN_PLAYERS = 2;
    //change this later as a data structure when more levels have been created
    public const float FINISH_POSITIONS_X = 1f;
    public static readonly float[] SPAWN_POINTS = {-15f}; //won't be used yet as we don't have more maps
    public const float PLAYER_MOVESPEED = 5f;
    public const float PLAYER_SPRINT_SPEED = 7.5f;

    /*example of how we will setup object positions etc.
    public static readonly Dictionary<int, Vector3> PICKABLE_OBJECT_LEVEL_ONE = new Dictionary<int, Vector3>()
    {
        {1, new Vector3(-10f, -1.7f, -1f)},
        {1, new Vector3(-5f, -1.7f, -1f)}
    };
    */
}
