using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviourPunCallbacks {

    private static NetworkGameManager _inst;
    public static NetworkGameManager inst {
        get {
            if(_inst == null) {
                _inst = new GameObject("NetworkGameManager").AddComponent<NetworkGameManager>();
                DontDestroyOnLoad(_inst.gameObject);
            }

            return _inst;
        }
    }

    private string gameVersion = "1";
    private byte maxPlayersPerRoom = 2;

    private bool isConnecting;

    public void Initialize() {
        
    }

    private void LoadGameLevel() {
        PhotonNetwork.LoadLevel("Level");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public void ConnectAndJoinRoom() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void SetNickname(string n) {
        PhotonNetwork.NickName = n;
    }

    #region RoomCallbacks

    public override void OnLeftRoom() {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other) {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadGameLevel();
        }
    }


    public override void OnPlayerLeftRoom(Player other) {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadGameLevel();
        }
    }

    #endregion

    #region ConnectionCallbacks

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Master");

        if (isConnecting) {
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

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.LoadLevel("Level");
        }
    }

    #endregion

}
