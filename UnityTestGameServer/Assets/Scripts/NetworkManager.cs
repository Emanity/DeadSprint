using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is what starts the server and where you will configure how many players
 * or server port will run.
 * 
 */
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

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

    private void Start()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        Server.Start(2, 23232);
        /* comment this out otherwise if not, to be able to run the server you must build
        #if UNITY_EDITOR
        Debug.Log("Build the project to Start the server.");
        #else
        Server.Start(15, 23232);
        #endif
        */
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }
}
