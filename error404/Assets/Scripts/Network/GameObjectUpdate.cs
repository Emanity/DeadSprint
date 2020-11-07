using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class will be used for the interpolation
 * the plan is for the interpolation because there will
 * be a delay which I will need to store the player's position and the time that happened (tick)
 */
public class GameObjectUpdate
{
    public int tick;
    public Vector3 position;
    public float lastTime;

    public GameObjectUpdate(int _tick, Vector3 _position, float _lastTime)
    {
        tick = _tick;
        position = _position;
        lastTime = _lastTime;
    }
}
