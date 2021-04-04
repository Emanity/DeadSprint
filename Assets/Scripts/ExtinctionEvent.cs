using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExtinctionEvent : MonoBehaviour
{
    //for game test purposes we will have it occur every tick instead randomly or by probability
    private static System.Random rng;
    public static bool isExecuting = false;
    private bool isCounting = false;

    private void Update()
    {
        if (isExecuting)
        {
            StartCoroutine(countDown());
        }
    }

    IEnumerator countDown()
    {
        yield return new WaitForSeconds(5f);
        isExecuting = false;
    }

    public static int rollEvent()
    {
        if (rollDice(2) <= 1 && isExecuting == false)
        {
            Debug.Log($"success roll");
            return rollDice(4); //reason it is 4 is because return value of random.Next() returns min but not max
        }
        return 0;
    }

    private static int rollDice(int _max)
    {
        rng = new System.Random();
        int rngNum = rng.Next(1, _max);
        return rngNum;
    }

    public class Kill
    {
        public static Kill killInstance;
        private static int killDamage = 100;
        private static string killMessage = "DEATH TO ALL";

        public static int getDamage()
        {
            return killDamage;
        }

        public static string getMessage()
        {
            return killMessage;
        }

        public static void execute()
        {
            if (isExecuting)
            {
                GameObject[] _playersObject = GameObject.FindGameObjectsWithTag("Player");
                if (_playersObject == null)
                {
                    return;
                }

                for (int i = 0; i < _playersObject.Length; i++)
                {
                    PlayerProperties _playerProperties = _playersObject[i].GetComponent<PlayerController>().getPlayerProperties();
                    _playerProperties.damage(getDamage());
                    Debug.Log("extinction event");
                }
            }
        }
    }

    public class Laser
    {
        public static Laser laserInstance;
        private static int killDamage = 15;
        private static string killMessage = "What is that above?";
        private static int numLasers = 0;

        public static int getDamage()
        {
            return killDamage;
        }

        public static string getMessage()
        {
            return killMessage;
        }

        public static void execute()
        {
            while (isExecuting)
            {
                //either enable or instantiate for 10 seconds
                GameObject[] _playerObjects = GameObject.FindGameObjectsWithTag("Player");
                float greatestPlayerPos = 0f;
                for (int i = 0; i < _playerObjects.Length; i++)
                {
                    float _playerPosX = _playerObjects[i].transform.position.x;
                    if (_playerPosX > greatestPlayerPos)
                    {
                        greatestPlayerPos = _playerPosX;
                    }
                }
                //GameManager.Instance.laserObject.transform.position = new Vector3(greatestPlayerPos, 0, 0);
                //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Laser"), new Vector3(0, 0), Quaternion.identity);
                //ExtinctionEvent.Laser.laserInstance.StartCoroutine(delay(3));
            }
        }

        private static IEnumerator instantiate()
        {
            yield return new WaitForSeconds(5f);
            //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Laser"), new Vector3(0, 0), Quaternion.identity);
        }

        private static IEnumerator destroy()
        {
            yield return new WaitForSeconds(5f);
            GameObject[] gumObjects = GameObject.FindGameObjectsWithTag("Laser");
            if (gumObjects.Length > 0)
            {
                for (int i = 0; i < gumObjects.Length; i++)
                {
                    if (gumObjects[i].transform.position.y <= -14)
                    {
                        PhotonNetwork.Destroy(gumObjects[i]);
                    }
                }
            }
        }
    }

    public class Amnesia : ExtinctionEvent
    {
        public static Amnesia AmnesiaInstance;
        private static string AmnesiaMessage = "Event Amnesia initiated, players movement control will be randomised";

        public static string getMessage()
        {
            return AmnesiaMessage;
        }

        public static void execute(float minX, float maxX, int numOfLasers)
        {
            //implement bool variable to use to check whether event is happening and randomise player controls (a,w,d) for 25 seconds
            // randomise every 5 seconds

        }

    }

    public class GumAttack : ExtinctionEvent
    {
        public static GumAttack GumAttackInstance;
        private static string GumAttackMessage = "INCOMING STORM!";
        private static float minX = -10;

        public static string getMessage()
        {
            return GumAttackMessage;
        }

        public static void execute()
        {
            //instantiate gum from the sky to drop
            //just use tag gum to do the same effect (reduce current stamina and set player to freeze/stuck
            if (isExecuting)
            {
                GameManager.Instance.StartCoroutine(instantiate());
            }
        }

        private static IEnumerator instantiate()
        {
            yield return new WaitForSeconds(7f);
            float previousPos = minX;
            for (int i = 0; i < 11; i++)
            {
                float gumballXPos = UnityEngine.Random.Range(previousPos, previousPos + 10f);
                previousPos += 10f;
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GumExtinction"), new Vector3(gumballXPos, 10f), Quaternion.identity);
            }
            GameManager.Instance.StartCoroutine(instantiate());
        }
    }
}
