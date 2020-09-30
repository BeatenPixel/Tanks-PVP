using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lobby : MonoBehaviourPunCallbacks {

    public TMP_InputField nicknameInputField;

    private string gameVersion = "1";
    private byte maxPlayersPerRoom = 2;

    private bool isConnecting;

    private void Awake() {
        Initialize();        
    }

    private void Initialize() {
        nicknameInputField.text = "Soldier" + Random.Range(0, 100);
        SetNickname(nicknameInputField.text);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void FindGame() {
        Connect();
    }

    private void Connect() {
        if(PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    private void SetNickname(string n) {
        PhotonNetwork.NickName = n;
        Debug.Log("new nickname = " + n);
    }

    #region Callbacks

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Master");

        if(isConnecting) {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("Disconnected: " + cause);

        isConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("No rooms available. Creating one");
        
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room!");

        if(PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.LoadLevel("Level");
        }
    }

    #endregion

    #region UI

    public void OnNicknameEditEnd(string nickname) {
        SetNickname(nickname);
    }

    #endregion

}
