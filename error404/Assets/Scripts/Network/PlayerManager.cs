using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This will handle events of the player and store information that relates to the client's player object
 * example: playersHit() which will reduce the health of a player
 * Note: I do plan on seperating the netcode(interpolation, input prediction &  to its own class but for now it is placed here
 */
public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public bool[] inputs;

    private Vector3 to = Vector3.zero;
    private Vector3 from = Vector3.zero;
    private float lastTime;
    private bool interp = true;

    /*
    private List<GameObjectUpdate> objectUpdates = new List<GameObjectUpdate>();
    private GameObjectUpdate interpolatePlayerTo;
    private GameObjectUpdate interpolatePlayerFrom;
    private GameObjectUpdate interpolatePlayerPrevious;
    */
    private float lerpTime;

    private void Update()
    {
        interpolate();
    }
    private void predictMove(Vector2 _inputDirection)
    {
        //use last input key from PlayerController and just += to the axis
    }

    private void inputComparePrediction(int _tick, Vector3 _serverPosition)
    {
        //compare the predicted position of player
        //if not equal to server position then set it to
        //note: we would need to send the tick of each input because to be able to compare we need the time simulation of the game
    }

    private void syncToActualPosition(Vector3 _position)
    {
        //if prediction was wrong then correct the position
    }

    /*
    #region player interpolation
    public void interpolate()
    {
        try
        {
            //entity interpolation
            for (int i = 0; i < objectUpdates.Count; i++)
            {
                if (GameManager.instance.getTick() >= objectUpdates[i].tick)
                {
                    //note: implement previous interpolation object to use it to prevent extrapolating
                    interpolatePlayerFrom = interpolatePlayerTo;
                    interpolatePlayerTo = objectUpdates[i];
                    objectUpdates.RemoveAt(i);
                    lerpTime = (interpolatePlayerFrom.lastTime - interpolatePlayerTo.lastTime) / (1.0f / Constants.TICKS_PER_SEC);
                }
            }
            interpolatePosition(lerpTime);
        }
        catch (Exception _e)
        {
            Debug.Log($"Error no GameObjectUpdate is null: {_e}");
        }
    }

    public void interpolatePosition(float _lerp)
    {
        //Debug.Log($"from: {interpolatePlayerFrom.tick} to: {interpolatePlayerTo.tick}, lerp: {_lerp}");
        try
        {
            this.transform.position = Vector3.Lerp(interpolatePlayerFrom.position, interpolatePlayerTo.position, _lerp);
        }
        catch (Exception _e)
        {
            Debug.Log($"Error nothing to interpolate: {_e}");
        }
    }

    public void newObjectUpdate(int _tick, Vector3 _position)
    {
        if (_tick <= GameManager.instance.getTickDelay())
        {
            return;
        }
        //Debug.Log($"position: {_position}  tick: {_tick}  currentTick: {GameManager.instance.tick}  tickDelay: {GameManager.instance.tickDelay}");

        if (objectUpdates.Count == 0)
        {
            objectUpdates.Add(new GameObjectUpdate(_tick, _position, Time.time));
            return;
        }

        for (int i = 0; i < objectUpdates.Count; i++)
        {
            //Debug.Log($"position: {_position}  tick: {_tick}");
            if (_tick < objectUpdates[i].tick)
            {
                objectUpdates.Insert(i, new GameObjectUpdate(_tick, _position, Time.time));
                break;
            }
        }
    }
    #endregion
    */

    private void interpolate()
    {
        if (interp)
        {
            this.transform.position = Vector3.Lerp(from, to, (Time.time - lastTime) / (1.0f / Constants.TICKS_PER_SEC));
        }

    }

    public void setPos(Vector3 _pos)
    {
        if (interp)
        {
            from = to;
            to = _pos;
            lastTime = Time.time;
            return;
        }
        this.transform.position = _pos;
    }
}