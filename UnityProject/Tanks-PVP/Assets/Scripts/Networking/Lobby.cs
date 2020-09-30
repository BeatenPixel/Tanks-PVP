using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lobby : MonoBehaviourPunCallbacks {

    public TMP_InputField nicknameInputField;

    private void Awake() {
        Initialize();        
    }

    private void Initialize() {
        NetworkGameManager.inst.Initialize();

        nicknameInputField.text = "Soldier" + Random.Range(0, 100);
        NetworkGameManager.inst.SetNickname(nicknameInputField.text);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void FindGame() {
        NetworkGameManager.inst.ConnectAndJoinRoom();
    }    

    #region UI

    public void OnNicknameEditEnd(string nickname) {
        NetworkGameManager.inst.SetNickname(nickname);
    }

    #endregion

}
