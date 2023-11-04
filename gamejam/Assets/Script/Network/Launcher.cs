using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    public GameObject controlPanel;
    public GameObject progressLabel;

    #region Private Serializable Fields
    
    #endregion

    #region Private Fields

    string gameVersion = "1";
    bool isConnecting;
    #endregion

    #region Monobehaviour Callbacks
    void Awake()
    {

    }

    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion

    #region Public Methods
    public void Connect()
    {
        
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        if(isConnecting )
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
        
        
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one. \n Calling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom});
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        if(PhotonNetwork.CurrentRoom.PlayerCount <= 3)
        {
            Debug.Log("We load the Room for wait");
            PhotonNetwork.LoadLevel("Room for wait");
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount==4){
            Debug.Log("Game Start");
            PhotonNetwork.LoadLevel("BattleField");
        }
    }
    #endregion
}
