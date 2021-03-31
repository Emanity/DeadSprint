using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenuButtonScripts : MonoBehaviourPunCallbacks
{
    public int scene;



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
