using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame : MonoBehaviourPunCallbacks {

    public TestPlayer localTestPlayer;
    public TestPlayer playerPrefab;

    private void Start() {
        if(localTestPlayer == null) {
            localTestPlayer = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, new Vector3(8, 3, 0), Quaternion.identity).GetComponent<TestPlayer>();

        }
    }

}
