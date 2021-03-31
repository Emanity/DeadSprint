using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Pause : MonoBehaviourPunCallbacks
{
   public static bool isGamePaused = false;
   
   public GameObject pauseScreen;
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (isGamePaused)
			{
				ResumeGame();
			}
			else
			{
				PauseGame();
			}
		}
    }
	
	public void ResumeGame()
	{
		pauseScreen.SetActive(false);
		isGamePaused = false;
	}
	
	public void PauseGame()
	{
		pauseScreen.SetActive(true);
		isGamePaused = true;
	}
	
	
	public void QuitGame()
	{
		//remove user form the photon network
		StartCoroutine(Disconnect());
		
		
		Debug.Log("Quit");
	}

	IEnumerator Disconnect()
    {
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.IsConnected)
			yield return null;
		SceneManager.LoadScene(0);
	}

}
