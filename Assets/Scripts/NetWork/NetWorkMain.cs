using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
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
    public bool isOffline = false;
    int setCount = 0, lastPlayerCount;
    private IngameLog ingameLog = new IngameLog();
    private void Start()
    {
        ingameLog.GenerateIngameLog("leader id ==" + leaderId + "MyId==" + netWorkId);
        setCount = 0;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {

        modeText.text = GameStart.gameMode2;
        stageText.text = "Stage" + GameStart.Stage;
        if (GameStart.gameMode1 == "Online" && PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
           // Debug.Log("NetWorkID == " + netWorkId);
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
                //leaderId�Đݒ�
                if (isLeader)
                {                
                    leaderId = netWorkId;
                    UpdateLeader(leaderId);
                }

                ingameLog.GenerateIngameLog(tmpId + "���� �� �� �� "   + netWorkId);
                // �z��̓��e��\��
                Debug.Log("ActorNumbers: " + string.Join(", ", actorNumbers));
            }
            lastPlayerCount = GameStart.PlayerNumber;

            isOnline = true;
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                playerConfigs[i].gameObject.SetActive(true);
                playerNameText[i].text = PhotonNetwork.PlayerList[i].NickName;
            }
            if (setCount == 0) //��x�̂ݎ��s���鍀��
            {
                Debug.Log("NetWorkID == " + netWorkId);
                photonView.RPC("SetPlayerNumber", RpcTarget.All);
                photonView.RPC("ConfigActive", RpcTarget.All);

             
                if (customProps.ContainsKey("winnings"))
                {
                    int[] winningsLocal = (int[])customProps["winnings"];
                    for (int i = 0; i < GameStart.PlayerNumber && i < winningsLocal.Length; i++)
                    {
                        if (i < winningsText.Length)
                        {
                            winningsText[i].text = "Win:" + winningsLocal[i].ToString();
                        }
                    }
                    Debug.Log("   customProps[winnings] " + winningsLocal[0]);
                    //  PhotonNetwork.JoinOrCreateRoom(RoomButton.RoomNameToPass, roomOptions, TypedLobby.Default);
                }
                setCount = 1;
            }
        }
        else
        {
            isOffline = false;
        }


    }
    public void Connect()
    {
        isOnline = true;

        if (!isOffline)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.OfflineMode = true;
        }
    }

    public void Disconnect()
    {
        isOnline = false;
        Debug.Log("WentOffline");
        if (!isOffline)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            PhotonNetwork.OfflineMode = false;
        }
    }


    public override void OnConnectedToMaster()
    {
        setCount = 0;
        PhotonNetwork.JoinLobby();
        Debug.Log("�}�X�^�[�T�[�o�[�ɐڑ����܂���");
    }


    public override void OnJoinedRoom()
    {

        //var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        //PhotonNetwork.Instantiate("Avatar", position, Quaternion.identity);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }
        Photon.Realtime.Player[] playerArray = PhotonNetwork.PlayerList;
        int[] actorNumbers = new int[4];
        int tmpId = 0;
        for (int i = 0; i < Mathf.Min(playerArray.Length, 4); i++)
        {
            actorNumbers[i] = playerArray[i].ActorNumber;
            if (actorNumbers[i] == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                tmpId = netWorkId;
                netWorkId = i + 1;
            }
        }
        ingameLog.GenerateIngameLog(tmpId + "���� �� �� �� " + netWorkId);
        PhotonNetwork.NickName = InputName.TypedTextToString;
        Debug.Log(PhotonNetwork.NickName + "isConnected");

        var roomOptions = new RoomOptions();

        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("gameMode"))
        {
            GameStart.gameMode2 = customProps["gameMode"].ToString();
            Debug.Log("GameMode��" + customProps["gameMode"].ToString() + "�ɐݒ肵�܂���");
        }
        if (customProps.ContainsKey("winnings"))
        {
            int[] winningsLocal = (int[])customProps["winnings"];
            for (int i = 0; i < GameStart.PlayerNumber && i < winningsLocal.Length; i++)
            {
                if (i < winningsText.Length)
                {
                    winningsText[i].text = "Win:" + winningsLocal[i].ToString();
                }
            }

            int tmpNum = 0;
            if (int.TryParse(customProps["stage"].ToString(), out tmpNum))
            {
                GameStart.Stage = tmpNum;
            }
     

            string[] userNameLocal = (string[])customProps["userName"];
            userNameLocal[netWorkId - 1] = PhotonNetwork.NickName;
            customProps["userName"] = userNameLocal;
        }
        else
        {
            customProps["leaderId"] = 1;
            customProps["stage"] = 1;
            customProps["gameMode"] = "Nomal";
            customProps["Password"] = "";
            customProps["isJoined"] = new bool[] { false, false, false, false };
            customProps["winnings"] = new int[] { 0, 0, 0, 0 };
            customProps["userName"] = new string[] { "", "", "", "", };
        }


        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

    }

    // Photon�̃T�[�o�[����ؒf���ꂽ���ɌĂ΂��R�[���o�b�N
    public override void OnDisconnected(DisconnectCause cause)
    {
        photonView.RPC("ConfigActive", RpcTarget.All);
    }

    [PunRPC]
    private void SetPlayerNumber()
    {
        GameStart.PlayerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "�������[���ɑ��݂��܂�");
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
        if (customProps.ContainsKey("winnings"))
        {
            int[] winningsLocal = (int[])customProps["winnings"];
            for (int i = 0; i < GameStart.PlayerNumber && i < winningsLocal.Length; i++)
            {
                if (i < winningsText.Length)
                {
                    winningsText[i].text = "Win:" + winningsLocal[i].ToString();
                }
            }
        }
        if (customProps.ContainsKey("leaderId"))
        {
            int idTmp = 1;
            if (int.TryParse(customProps["leaderId"].ToString(), out idTmp))
            {
                leaderId = idTmp;
            }
        }
    }

    // ���[���Œl�����L����
    public static void UpdateRoomStats(int updatedStage)
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("stage"))
        {
            int stageTmp;
            if (int.TryParse(customProps["stage"].ToString(), out stageTmp))
            {
                customProps["stage"] = updatedStage;
            }
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }

    // ���[���Œl�����L����
    public static void UpdateGameMode(string updatedMode)
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("gameMode"))
        {
            int[] winningsLocal = (int[])customProps["winnings"];
            customProps["gameMode"] = updatedMode;
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }
    // ���[���Œl�����L����
    public static void UpdateLeader(int updatedId)
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        int idTmp = 1;
        if (int.TryParse(customProps["leaderId"].ToString(), out idTmp))
        {
            customProps["leaderId"] = updatedId;
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }
}