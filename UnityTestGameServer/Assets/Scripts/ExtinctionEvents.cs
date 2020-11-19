using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinctionEvents
{
    //for game test purposes we will have it occur every tick instead randomly or by probability
    private static System.Random rng;

    protected static bool execute(int _probability)
    {
        return (rollDice() <= _probability);
    }

    protected static int rollDice()
    {
        rng = new System.Random();
        int rngNum = rng.Next(0, 100);
        return rngNum;
    }

    //seperate these into its own file after
    public class Kill : ExtinctionEvents
    {
        public static Kill instance;
        private static int killDamage = 100;
        private static string killMessage = "Death to all";
        private static int killProbability = 1; //chance of this event to happen

        public static void executeKill()
        {
            if (execute(killProbability))
            {
                Dictionary<int, Player> players =  GameManager.instance.players;
                foreach (Player _player in players.Values)
                {
                    _player.damage(killDamage);
                }
                ServerSend.extinctionEvent(killMessage);
            }
        }
    }

    /*
    public class Freeze : ExtinctionEvents
    {
        public static Freeze instance;
        private static string freezeMessage = "Freeze to all";
        private static int freezeProbability = 5;

        public static void executeFreeze()
        {
            if (execute())
            {
                Dictionary<int, Player> players = GameManager.instance.players;
                foreach (Player _player in players.Values)
                {
                    _player.isFrozen = true;
                }
                ServerSend.extinctionEvent(freezeMessage);
            }
        }
    }
    */
}
