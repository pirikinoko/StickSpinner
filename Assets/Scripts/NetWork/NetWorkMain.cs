using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

public class NetWorkMain : MonoBehaviourPunCallbacks
{
    public static string[] playerNames = new string[4];
    public static int leaderId = 1;
    public static bool isOnline;
    public static int netWorkId = 0;
    public GameObject[] playerConfigs, leaderIcons;
    public Text[] playerNameText, winningsText;
    public Text modeText, stageText;
    string[] gameMode = { "Nomal", "Arcade" };
    int setCount = 0, lastPlayerCount;
    IngameLog ingameLog;
    private void Start()
    {
        ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();
        setCount = 0;
        PhotonNetwork.ConnectUsingSettings();
        if (PhotonNetwork.InRoom) 
        {
            if(GetCustomProps<string>("gameMode", out var gameModeCP))
            {
                GameStart.gameMode2 = gameModeCP;
            } 
            if(GetCustomProps<int>("stage", out var stageCP))
            {
                GameStart.stage = stageCP;
            }
            if (GetCustomProps<int[]>("playerTeam", out var playerTeamCP))
            {
                GameStart.playerTeam = playerTeamCP;
            }
        }
     
    }

    void Update()
    {
        modeText.text = GameStart.gameMode2;
        stageText.text = "stage" + GameStart.stage;
        if (GameStart.gameMode1 == "Online" && PhotonNetwork.InRoom)
        {
            GameStart.PlayerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            if (lastPlayerCount != GameStart.PlayerNumber)
            {
                ConfigActive();
                Photon.Realtime.Player[] playerArray = PhotonNetwork.PlayerList;
                int[] actorNumbers = new int[4];
                int tmpId = 0;
                bool isLeader = false;
                if(leaderId == netWorkId) { isLeader = true; }
                for (int i = 0; i < Mathf.Min(playerArray.Length, 4); i++)
                {
                    actorNumbers[i] = playerArray[i].ActorNumber;
                    if (actorNumbers[i] == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        tmpId = netWorkId;
                        netWorkId = i + 1;
                    }
                }
                //leaderId再設定
                if (isLeader)
                {                
                    leaderId = netWorkId;
                    SetCustomProps<int>("leaderId", leaderId);
                }

                // 配列の内容を表示
                Debug.Log("ActorNumbers: " + string.Join(", ", actorNumbers));
            }
            lastPlayerCount = GameStart.PlayerNumber;

            isOnline = true;
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                playerConfigs[i].gameObject.SetActive(true);
                playerNameText[i].text = PhotonNetwork.PlayerList[i].NickName;
            }
            if (setCount == 0) //一度のみ実行する項目
            {
                Debug.Log("NetWorkID == " + netWorkId);
                photonView.RPC("SetPlayerNumber", RpcTarget.All);
                photonView.RPC("ConfigActive", RpcTarget.All);

                if (GetCustomProps<int[]>("winnings", out var value))
                {
                    value
                        .Select((winnings, index) => new { winnings, index }) // 各要素とそのインデックスをペアにする
                        .Zip(winningsText, (winningsWithIndex, textField) => new { winningsWithIndex, textField }) //{Selectで作られた匿名オブジェクトのコレクションとwinningsText.textをペアにしてさらに匿名オブジェクトを作る}
                        .ToList() //クエリの結果をすぐに評価し、リストとして取得します。
                        .ForEach(pair => pair.textField.text = $"Win: {pair.winningsWithIndex.winnings}");
                }
                setCount = 1;
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        setCount = 0;
        PhotonNetwork.JoinLobby();
        Debug.Log("マスターサーバーに接続しました");
    }


    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = InputName.TypedTextToString;
        Debug.Log(PhotonNetwork.NickName + "isConnected");

        var roomOptions = new RoomOptions();


        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        //もともとCustomPropsが設定してある場合はそれらを取得して状態をルームのものと同期する
        if (GetCustomProps<string>("gameMode", out string valueA)) { GameStart.gameMode2 = valueA; }
        if (GetCustomProps<int>("stage", out int valueB)) { GameStart.stage = valueB; }
        if (GetCustomProps<int[]>("playerTeam", out var valueC))
        {
            for (int i = 0; i < valueC.Length; i++)
            {
                GameStart.playerTeam[i] = valueC[i];
            }
        }
        if (GetCustomProps<string[]>("userName", out var valueD))
        {
            for (int i = 0; i < valueD.Length; i++)
            {
                playerNames[i] = valueD[i];
            }
        }
        else
        {
            customProps["leaderId"] = 1;
            customProps["stage"] = 1;
            customProps["gameMode"] = GameStart.gameMode2;
            customProps["Password"] = "";
            customProps["isJoined"] = new bool[] { false, false, false, false };
            customProps["isReady"] = new bool[] { false, false, false, false };
            customProps["winnings"] = new int[] { 0, 0, 0, 0 };
            customProps["playerTeam"] = new int[] { 0, 1, 2, 3 };
            customProps["userName"] = new string[] { "", "", "", "", };
        }

        
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

    }

    // Photonのサーバーから切断された時に呼ばれるコールバック
    public override void OnDisconnected(DisconnectCause cause)
    {
        photonView.RPC("ConfigActive", RpcTarget.All);
    }

    [PunRPC]
    private void SetPlayerNumber()
    {
        GameStart.PlayerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "名がルームに存在します");
    }
    [PunRPC]
    private void ConfigActive()
    {
        for (int i = 3; i >= GameStart.PlayerNumber; i--)
        {
            playerConfigs[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            playerConfigs[i].gameObject.SetActive(true);
            playerNameText[i].text = PhotonNetwork.PlayerList[i].NickName;
            playerNames[i] = PhotonNetwork.PlayerList[i].NickName;
        }

        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if(GetCustomProps<int>("leaderId", out int value)) {leaderId = value;}
    }

    [PunRPC]
    void SetCustomPropsStage()
    {
        GetCustomProps<int>("stage", out int value);
        GameStart.stage = value;
    }

    public static void SetCustomProps<type>(string key, type toSet)
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        switch (toSet)
        {
            case int i:
                if (customProps.TryGetValue(key,  out _))
                {
                    customProps[key] = toSet;
                }
                break;
            case string s:
                customProps[key] = toSet;
                break;
            case float f:
                if (customProps.TryGetValue(key, out _))
                {
                    customProps[key] = toSet;
                }
                break;
            case bool b:
                customProps[key] = toSet;
                break;
            case int[] intArray:
                if (customProps.TryGetValue(key, out _))
                {
                    customProps[key] = toSet;
                }
                break;
            case string[] stringArray:
                customProps[key] = toSet;
                break;
            case float[] floatArray:
                if (customProps.TryGetValue(key, out _))
                {
                    customProps[key] = toSet;
                }
                break;
            case bool[] boolArray:
                customProps[key] = toSet;
                break ;
            default:
                throw new Exception($"Unsupported type: {typeof(type)}");
                break;
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

    }
    public static bool GetCustomProps<type>(string key, out type value)
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;

        if (customProps.TryGetValue(key, out var rawValue))
        {
            switch (rawValue)
            {
                case int i when typeof(type) == typeof(int):
                    value = (type)(object)i;
                    return true;
                case string s when typeof(type) == typeof(string):
                    value = (type)(object)s;
                    return true;
                case float f when typeof(type) == typeof(float):
                    value = (type)(object)f;
                    return true;
                case bool b when typeof(type) == typeof(bool):
                    value = (type)(object)b;
                    return true;
                case int[] intArray when typeof(type) == typeof(int[]):
                    value = (type)(object)intArray;
                    return true;
                case string[] stringArray when typeof(type) == typeof(string[]):
                    value = (type)(object)stringArray;
                    return true;
                case float[] floatArray when typeof(type) == typeof(float[]):
                    value = (type)(object)floatArray;
                    return true;
                case bool[] boolArray when typeof(type) == typeof(bool[]):
                    value = (type)(object)boolArray;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        value = default;
        return false;
    }

}