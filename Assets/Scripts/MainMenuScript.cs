using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Linq;

public class MainMenuScript : MonoBehaviourPunCallbacks
{

    public static MainMenuScript Instance;

    public InputField roomNameInputField;
    public Text errorText;
    public Text nameOfRoom;
    public Text roomCodeText;
    public Transform roomListContent;
    public Transform playerListContent;
    public GameObject roomListItemPrefab;
    public GameObject playerListItemPrefab;
    public InputField setNameInputField;
    public InputField setNameCRIF;
    public InputField roomCodeInputField;
    public Toggle privateRoomToggle;
    public InputField privateRoomInputField;


    public GameObject startGameButton;

    public GameObject loadingMenu;
    public GameObject mainMenu;
    public GameObject roomMenu;
    public GameObject errorMenu;
    public GameObject findRoomMenu;
    public GameObject roomCodeMenu;

    //public string playerName;
    public int maxPlayer = 4;
    public int scene;
    private string roomCode;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Conecting to master...");

        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        loadingMenu.SetActive(false);
        mainMenu.SetActive(true);
        Debug.Log("Joined Lobby");
        Debug.Log($"{PhotonNetwork.IsMasterClient}");
    }
    
    public void QuitGame() {
        Application.Quit();
    }
    
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text)) {
            roomNameInputField.text = "Room" + Random.Range(0, 10000).ToString("00000");
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayer;
        Debug.Log(roomNameInputField.text);
        
        //setup room code
        //roomCode = string.IsNullOrEmpty(roomCodeInputField.text) ? Random.Range(0, 100000).ToString("000000") : roomCodeInputField.text;
        if (privateRoomToggle.isOn)
        {
            roomCode = Random.Range(0, 100000).ToString("000000");
            roomOptions.IsVisible = false;
            roomOptions.PublishUserId = true;
        }

        RoomManager.Instance.roomCode = roomCode;
        Debug.Log($"roomCode = {RoomManager.Instance.roomCode}");

        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        Debug.Log("creating room");
        if (string.IsNullOrEmpty(setNameCRIF.text))
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 10000).ToString("00000");

        }
        else
        {
            PhotonNetwork.NickName = setNameCRIF.text;
        }

    }

    public override void OnJoinedRoom()
    {
        foreach (Player item in PhotonNetwork.PlayerList)
        {
            Debug.Log($"player list lenght: {PhotonNetwork.PlayerList.Length}");
            Debug.Log($"Player from list: {item.UserId}");
            Debug.Log($"Is my userID?: {item.UserId == PhotonNetwork.LocalPlayer.UserId}");

        }

        if (PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.IsVisible)
        {
            authenticationSuccess();
            return;
        }
        loadingMenu.SetActive(false);
        roomCodeMenu.SetActive(true);
    }

    public void authenticateRoomCode()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            roomCode = roomCodeInputField.text;
            RoomManager.Instance.sendRoomCode(PhotonNetwork.LocalPlayer, roomCode);
        }
    }

    public void authenticationSuccess()
    {
        loadingMenu.SetActive(false);
        roomCodeMenu.SetActive(false);

        Debug.Log("room joined");
        nameOfRoom.text = PhotonNetwork.CurrentRoom.Name;
        roomCodeText.text = roomCode;
        roomMenu.SetActive(true);

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        Debug.Log($" {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Could not join Room: " + returnCode + message;
        errorMenu.SetActive(true);
        loadingMenu.SetActive(false);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Could Not Create Room: " + message;
        errorMenu.SetActive(true);
        roomMenu.SetActive(false);
        loadingMenu.SetActive(false);
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        if (PhotonNetwork.CurrentRoom.IsVisible == false)
        {
            RoomManager.Instance.startGame();
            roomCodeMenu.SetActive(false);
        }
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        Debug.Log("leaving room...");
        roomCode = null;
        roomNameInputField.text = "";
        PhotonNetwork.LeaveRoom();
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(privateRoomInputField.text))
        {
            JoinRoom(privateRoomInputField.text);
        }
    }

    public void JoinRoom(string _roomName)
    {
        PhotonNetwork.JoinRoom(_roomName);

        if (!PhotonNetwork.IsConnected) return;

        if (string.IsNullOrEmpty(setNameInputField.text))
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");

        } else
        {
            PhotonNetwork.NickName = setNameInputField.text;
        }

        findRoomMenu.SetActive(false);
        loadingMenu.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Room left");
        loadingMenu.SetActive(false);
        roomCodeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
