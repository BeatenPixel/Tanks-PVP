using Morpeh;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public class TestPlayer : MonoBehaviour, IPunObservable {

    public Vector3 pos;
    public PhotonView photonView;

    private void Update() {
        transform.position = pos;

        if (!photonView.IsMine) {
            return;
        }

        if(Input.GetMouseButtonDown(0)) {
            pos += Vector3.up * 0.5f;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            stream.SendNext(pos);
        } else {
            pos = (Vector3)stream.ReceiveNext();
        } 
    }

}
