using ExitGames.Client.Photon;
using Morpeh.Globals;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Forsis/NetworkEventsManager")]
public class NetworkEventsManager : ScriptableObject, IOnEventCallback {

    public static NetworkEventsManager inst;

    public List<NetworkEventString> events;

    public void Initialize() {
        inst = this;

        PhotonNetwork.AddCallbackTarget(this);
    }

    public void Deinitialize() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void PublishEvent(string eventName, string data, ReceiverGroup receiveGroup) {
        NetworkEventString e = events.First((x) => x.eventName == eventName);
        PublishEvent(e, data, receiveGroup);
    }

    public void PublishEvent(NetworkEventString networkEvent, string data, ReceiverGroup receiveGroup) {
        object[] eventData = new object[1] { data };

        PhotonNetwork.RaiseEvent(networkEvent.eventCode, eventData,
            new RaiseEventOptions() { Receivers = receiveGroup },
            (networkEvent.sendOptions == EventSendOptions.SendReliable)?SendOptions.SendReliable:SendOptions.SendUnreliable);
    }

    public void OnEvent(EventData photonEvent) {
        byte eventCode = photonEvent.Code;

        if (eventCode == 199) {
            NetworkEventString e = events.First((x) => x.eventCode == eventCode);

            if (e != null) {
                object[] data = (object[])photonEvent.CustomData;

                e.globalEvent.Publish((string)data[0]);
            }
        }
    }

}

[System.Serializable]
public class NetworkEventString {
    public string eventName;
    public byte eventCode;
    public GlobalEventString globalEvent;
    public EventSendOptions sendOptions;
}

public enum EventSendOptions {
    SendReliable,
    SendUnreliable
}