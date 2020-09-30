using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviourPunCallbacks {



    private void LoadLevel() {
        PhotonNetwork.LoadLevel("Level");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    #region Callbacks

    public override void OnLeftRoom() {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other) {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadLevel();
        }
    }


    public override void OnPlayerLeftRoom(Player other) {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadLevel();
        }
    }

    #endregion

}
