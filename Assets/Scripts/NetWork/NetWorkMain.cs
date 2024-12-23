using Cysharp.Threading.Tasks;
using Fusion;
using R3;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NetWorkMain : NetworkBehaviour
{
    public static string[] playerNames = new string[4];

    public static int leaderId = 1;

    public static bool isOnline;

    public static int NetWorkId = 0;

    public GameObject[] playerConfigs;

    public GameObject[] leaderIcons;

    public Text[] playerNameText;

    public Text[] winningsText;

    public Text modeText;

    public Text stageText;

    string[] gameMode = { "Nomal", "Arcade" };

    int setCount = 0, lastPlayerCount;

    private Subject<Unit> _onPlayerCountChange = new Subject<Unit>();

    IngameLog ingameLog;

    private void Start()
    {
        ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();

        setCount = 0;

        PhotonNetwork.ConnectUsingSettings();

        _onPlayerCountChange
            .Subscribe(_ => OnChangePlayerCount());

        if (PhotonNetwork.InRoom)
        {
            if (GetCustomProps<string>("gameMode", out var gameModeCP))
            {
                GameStartHandler.GameMode2 = gameModeCP;
            }
            if (GetCustomProps<int>("stage", out var stageCP))
            {
                GameStartHandler.Stage = stageCP;
            }
            if (GetCustomProps<int[]>("playerTeam", out var playerTeamCP))
            {
                GameStartHandler.PlayerTeam = playerTeamCP;
            }
        }

    }

    void Update()
    {
        modeText.text = GameStartHandler.GameMode2;
        stageText.text = "stage" + GameStartHandler.Stage;

        if (GameStartHandler.GameMode1 == "Online" && Runner.IsInSession)
        {
            GameStartHandler.PlayerCount = Runner.ActivePlayers.Count;
            if (lastPlayerCount != GameStartHandler.PlayerCount)
            {
                _onPlayerCountChange.OnNext(Unit.Default);
            }
            lastPlayerCount = GameStartHandler.PlayerCount;

            isOnline = true;
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                playerConfigs[i].gameObject.SetActive(true);
                playerNameText[i].text = PhotonNetwork.PlayerList[i].NickName;
            }
            if (setCount == 0) //一度のみ実行する項目
            {
                SetPlayerNumber();
                UpdateConfig();

                if (GetCustomProps<int[]>("winnings", out var value))
                {
                    value
                        .Select((winnings, index) => new { winnings, index })
                        .Zip(winningsText, (winningsWithIndex, textField) => new { winningsWithIndex, textField })
                        .ToList()
                        .ForEach(pair => pair.textField.text = $"Win: {pair.winningsWithIndex.winnings}");
                }
                setCount = 1;
            }
        }
    }

    private void OnChangePlayerCount()
    {
        UpdateConfig();
        var playerArray = Runner.ActivePlayers;

        // 自身のIDを再取得                                                                            
        for (int i = 0; i < Mathf.Min(playerArray.Length, 4); i++)
        {
            if (playerArray[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                NetWorkId = i + 1;
            }
        }

        if (leaderId > PhotonNetwork.CurrentRoom.PlayerCount)
        {
            leaderId = 1;
        }

        //leaderId再設定
        if (leaderId == NetWorkId)
        {
            leaderId = NetWorkId;
            SetCustomProps<int>("leaderId", leaderId);
        }
    }

    public override void OnJoinedSession()
    {
        PhotonNetwork.NickName = InputName.TypedTextToString;

        // 自身のIDを再取得
        for (int i = 0; i < Mathf.Min(PhotonNetwork.PlayerList.Length, 4); i++)
        {
            if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                NetWorkId = i + 1;
            }
        }

        var roomOptions = new RoomOptions();

        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        //もともとCustomPropsが設定してある場合はそれらを取得して状態をルームのものと同期する
        if (GetCustomProps<string>("gameMode", out string valueA)) { GameStartHandler.GameMode2 = valueA; }
        if (GetCustomProps<int>("stage", out int valueB)) { GameStartHandler.Stage = valueB; }
        if (GetCustomProps<int[]>("playerTeam", out var valueC))
        {
            for (int i = 0; i < valueC.Length; i++)
            {
                GameStartHandler.PlayerTeam[i] = valueC[i];
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
            customProps["gameMode"] = GameStartHandler.GameMode2;
            customProps["Password"] = "";
            customProps["isJoined"] = new bool[] { false, false, false, false };
            customProps["isReady"] = new bool[] { false, false, false, false };
            customProps["winnings"] = new int[] { 0, 0, 0, 0 };
            customProps["playerTeam"] = new int[] { 0, 1, 2, 3 };
            customProps["userName"] = new string[] { "", "", "", "", };
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
        UpdateConfig();
    }

    // Photonのサーバーから切断された時に呼ばれるコールバック
    public override void OnDisconnected(DisconnectCause cause)
    {
        photonView.RPC("ConfigActive", RpcTarget.All);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void SetPlayerNumber()
    {
        GameStartHandler.PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "名がルームに存在します");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void UpdateConfig()
    {
        for (int i = 3; i >= GameStartHandler.PlayerCount; i--)
        {
            playerConfigs[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            playerConfigs[i].gameObject.SetActive(true);
            playerNameText[i].text = PhotonNetwork.PlayerList[i].NickName;
            playerNames[i] = PhotonNetwork.PlayerList[i].NickName;
        }

        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (GetCustomProps<int>("leaderId", out int value)) { leaderId = value; }
    }

    void SetCustomPropsStage()
    {
        GetCustomProps<int>("stage", out int value);
        GameStartHandler.Stage = value;
    }

    public static void SetCustomProps<type>(string key, type toSet)
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        switch (toSet)
        {
            case int i:
                if (customProps.TryGetValue(key, out _))
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
                break;
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