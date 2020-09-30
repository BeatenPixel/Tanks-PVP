using Morpeh;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback {

    public int networkViewID;
    private PlayerProvider playerProvider;

    public void Initialize() {
        ref TankComponent tankComponent = ref playerProvider.Entity.GetComponent<TankComponent>();
        tankComponent.receivedPosX = new float[2];
        tankComponent.receivedPosY = new float[2];
        networkViewID = photonView.ViewID;
    }

    private void Update() {
        if (!photonView.IsMine) {
            return;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        ref PlayerComponent playerComponent = ref playerProvider.Entity.GetComponent<PlayerComponent>();
        ref TankComponent tankComponent = ref playerProvider.Entity.GetComponent<TankComponent>();

        if (stream.IsWriting) {
            stream.SendNext(tankComponent.x);
            stream.SendNext(tankComponent.y);
            stream.SendNext(tankComponent.faceDirection);
        } else {
            float targetX = (float)stream.ReceiveNext();
            float targetY = (float)stream.ReceiveNext();
            byte faceDirection = (byte)stream.ReceiveNext();

            tankComponent.receivedPosX[1] = tankComponent.receivedPosX[0];
            tankComponent.receivedPosX[0] = targetX;

            tankComponent.receivedPosY[1] = tankComponent.receivedPosY[0];
            tankComponent.receivedPosY[0] = targetY;

            tankComponent.faceDirection = faceDirection;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        Debug.Log("OnPhotonInstantiate");

        if (!photonView.IsMine) {
            Debug.Log("IsNotMine");
        } else {
            Debug.Log("IsMine");
        }

        playerProvider = GetComponent<PlayerProvider>();
        Initialize();
    }

    public PlayerProvider GetPlayerProvider() {
        return playerProvider;
    }
}
