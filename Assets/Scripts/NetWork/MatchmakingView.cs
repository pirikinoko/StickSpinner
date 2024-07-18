using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MatchmakingView : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomListView roomListView = default;
    [SerializeField]
    private InputField roomNameInputField = default;
    [SerializeField]
    private Button createRoomButton = default, createRoomLockedButton = default,joinRoomButton = default, quickMatchButton = default;
    [SerializeField]
    Text playerCountQuick, playButtonText;
    [SerializeField]
    GameObject quickStartingPanel;
    private CanvasGroup canvasGroup;
    IngameLog ingameLog;
    public static string mode;
    public static int stageQuick = 0;
    public static string gameModeQuick = "Nomal";
    public int ccuLimit = 80;
    GameStart gameStart;
    private void Start()
    {
        quickStartingPanel.SetActive(false);
        gameStart = GameObject.Find("Systems").GetComponent<GameStart>();
        ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();
        canvasGroup = GetComponent<CanvasGroup>();

        // ルームリスト表示を初期化する
        roomListView.Init(this);

        roomNameInputField.onValueChanged.AddListener(OnRoomNameInputFieldValueChanged);
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
        createRoomLockedButton.onClick.AddListener(OnCreateRoomLockedButtonClick);
        joinRoomButton.onClick.AddListener(OnJoinRoomButtonClick);
        quickMatchButton.onClick.AddListener(OnQuickMatchButtonClick);

        playerCountQuick.text = "";

        if(roomNameInputField.text.Length > 0) 
        {
            createRoomButton.interactable = true;
            createRoomLockedButton.interactable = true;
            joinRoomButton.interactable = true;
        }
        else 
        {
            createRoomButton.interactable = false;
            createRoomLockedButton.interactable = false;
            joinRoomButton.interactable = false;
        }
    }
    [PunRPC]
    void StartQuickGame(int stage, string gameMode)
    {
        GameStart.gameMode2 = gameMode;
        GameStart.stage = stage;
        GameStart.phase = 4;
        playerCountQuick.text = "";
    }

    IEnumerator OpenQuickPanel() 
    {
        quickStartingPanel.SetActive(true);
        GameObject[] iconObjects = new GameObject[4];
        Text[] playerNameTexts = new Text[4];
        for (int i = 0; i < iconObjects.Length; i++) 
        {
            iconObjects[i] = quickStartingPanel.transform.GetChild(i).gameObject;
            playerNameTexts[i] = iconObjects[i].transform.GetChild(0).gameObject.GetComponent<Text>();
            playerNameTexts[i].text = PhotonNetwork.PlayerList[i].NickName;
        }
        GameObject imageFrame = quickStartingPanel.transform.GetChild(4).gameObject;
        Image stageImg = imageFrame.transform.GetChild(0).gameObject.GetComponent<Image>();
        stageImg.sprite = Resources.Load<Sprite>("Multi" + GameStart.gameMode2 + GameStart.stage);
        imageFrame.transform.GetChild(1).gameObject.GetComponent<TextSwicher>().num =  GameStart.stage;
        imageFrame.transform.GetChild(2).gameObject.GetComponent<TextSwicher>().num = GameStart.stage;
        yield return new WaitForSeconds(3.0f);
        if (NetWorkMain.netWorkId == NetWorkMain.leaderId)
        {
            photonView.RPC(nameof(StartQuickGame), RpcTarget.All, stageQuick, gameModeQuick);
        }
    }
    void Update()
    {
        if (PhotonNetwork.InRoom && mode == "Quick")
        {
            int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
            playerCountQuick.text = "Waiting " + PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + maxPlayers;
            playButtonText.text = "Cancel";
            if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
            {
                StartCoroutine(OpenQuickPanel());
                quickMatchButton.interactable = false;
            }
        }
        else
        {
            playButtonText.text = "Play";
            playerCountQuick.text = "";
        }
        if (PhotonNetwork.InRoom) 
        {
            createRoomButton.interactable = false;
            createRoomLockedButton.interactable = false;
            joinRoomButton.interactable = false;
        }

        // ロビーに参加するまでは、入力できないようにする
        if (!PhotonNetwork.InLobby)
        {
            canvasGroup.interactable = false;
        }
        else 
        {
            canvasGroup.interactable = true;
        }


    }
    public override void OnJoinedLobby()
    {
        // ロビーに参加したら、入力できるようにする
        canvasGroup.interactable = true;
        this.gameObject.SetActive(true);
    }

    private void OnRoomNameInputFieldValueChanged(string value)
    {
        // ルーム名が1文字以上入力されている時のみ、ルーム作成ボタンを押せるようにする
        createRoomButton.interactable = (value.Length > 0);
        createRoomLockedButton.interactable = (value.Length > 0);
        joinRoomButton.interactable = (value.Length > 0);
    }
    private void OnJoinRoomButtonClick()
    {
        if (isCCUNotOver() == false)
        {
            IngameLog.GenerateIngameLog("The server is full right now!");
            return;
        }
        if (InputName.TypedTextToString == null) 
        {
            IngameLog.GenerateIngameLog("Please type player name");
            return;
        }
        PhotonNetwork.JoinRoom(roomNameInputField.text);
        mode = "Nomal";
    }
    private void OnCreateRoomLockedButtonClick()
    {
        if (isCCUNotOver() == false)
        {
            IngameLog.GenerateIngameLog("The server is full right now!");
            return;
        }
        if (InputName.TypedTextToString == null)
        {
            IngameLog.GenerateIngameLog("Please type player name");
            return;
        }
        string roomName = roomNameInputField.text + "!Locked!";
        // ルーム作成処理中は、入力できないようにする
        canvasGroup.interactable = false;

        // 入力フィールドに入力したルーム名のルームを作成する
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomName, roomOptions);

    }
    private void OnCreateRoomButtonClick()
    {
        if(isCCUNotOver() == false) 
        {
            IngameLog.GenerateIngameLog("The server is full right now!");
            return;
        }
        if (InputName.TypedTextToString.Length == 0) 
        {
            IngameLog.GenerateIngameLog("Please type player name");
            return;
        }
        mode = "Nomal";
        // ルーム作成処理中は、入力できないようにする
        canvasGroup.interactable = false;

        // 入力フィールドに入力したルーム名のルームを作成する
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // ルームの作成が失敗したら、再び入力できるようにする
        roomNameInputField.text = string.Empty;
        canvasGroup.interactable = true;
    }
    private void OnQuickMatchButtonClick()
    {
        if (PhotonNetwork.InRoom) 
        {
            PhotonNetwork.LeaveRoom();
            mode = "Nomal";
            if (roomNameInputField.text.Length > 0) 
            {
                createRoomButton.interactable = true;
                createRoomLockedButton.interactable = true;
                joinRoomButton.interactable = true;
            }
     
            return;
        }
        if (isCCUNotOver() == false)
        {
            IngameLog.GenerateIngameLog("The server is full right now!");
            return;
        }
        if (InputName.TypedTextToString == null) 
        {
            IngameLog.GenerateIngameLog("Please type player name");
            return;
        }
        string roomName = "!Quick!" + roomListView.quickRoomCount;
        PhotonNetwork.JoinRoom(roomName);
        mode = "Quick";
    }
    public void OnJoiningRoom()
    {
        // ルーム参加処理中は、入力できないようにする
        canvasGroup.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        if(mode != "Quick")
        {
            GameStart.phase++;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(mode == "Nomal") { PhotonNetwork.JoinRoom(roomNameInputField.text + "!Locked!"); }
        else
        {
            int roomNum = roomListView.quickRoomCount + 1;
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            string roomName = "!Quick!" + roomNum;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
            int randomMode = Random.Range(0, 2);
            randomMode = 1;
            // 0または1のいずれかに対応する処理を行う
            if (randomMode == 0)
            {
                gameModeQuick = "Nomal";
                stageQuick = Random.Range(1, 4);
            }
            else
            {
                gameModeQuick = "Arcade";
                stageQuick = Random.Range(1, 3);
            }
            gameModeQuick = "Arcade";
        }
        // ルームへの参加が失敗したら、再び入力できるようにする
        canvasGroup.interactable = true;
    }
    private bool isCCUNotOver()
    {
        // 現在の同時接続数を取得
        int currentCCU = PhotonNetwork.CountOfPlayers;

        Debug.Log($"CCU Limit: {ccuLimit}, Current CCU: {currentCCU}");

        // 接続制限に達している場合は接続を試みない
        if (currentCCU >= ccuLimit)
        {         
            return false;   
        }
        else { return true; }
    }
}