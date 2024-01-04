    using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetWorkIngame : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (GameStart.gameMode1 != "Online") { return; }

        //  PhotonNetwork.JoinOrCreateRoom(RoomButton.RoomNameToPass, roomOptions, TypedLobby.Default);
        var roomOptions = new RoomOptions();
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["stickRots"] = new float[] { 0, 0, 0, 0 };
        customProps["goalPlayerID"] = 0;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }

    void Update() 
    {

     }

}