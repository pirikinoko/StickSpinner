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
    private TMP_InputField roomNameInputField = default;
    [SerializeField]
    private Button createRoomButton = default, createRoomLockedButton = default,joinRoomButton = default, quickMatchButton = default;
    [SerializeField]
    Text playerCountQuick, playButtonText;
    private CanvasGroup canvasGroup;
    private IngameLog ingameLog = new IngameLog();
    string mode;
    int stageQuick = 0;
    public static string gameModeQuick = "Nomal";
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // ロビーに参加するまでは、入力できないようにする
        if (!PhotonNetwork.InLobby) 
        {
            canvasGroup.interactable = false;
        }

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
        GameStart.Stage = stage;
        SceneManager.LoadScene("Stage");
        playerCountQuick.text = "";
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
                if (NetWorkMain.netWorkId == NetWorkMain.leaderId)
                {
                    photonView.RPC(nameof(StartQuickGame), RpcTarget.All, stageQuick, gameModeQuick);
                }

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
        PhotonNetwork.JoinRoom(roomNameInputField.text);
        mode = "Nomal";
    }
    private void OnCreateRoomLockedButtonClick()
    {
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
        if (InputName.TypedTextToString.Length == 0) 
        {
            ingameLog.GenerateIngameLog("名前を入力してください");
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
            if(roomNameInputField.text.Length > 0) 
            {
                createRoomButton.interactable = true;
                createRoomLockedButton.interactable = true;
                joinRoomButton.interactable = true;
            }
     
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

            // 0または1のいずれかに対応する処理を行う
            if (randomMode == 0)
            {
                gameModeQuick = "Nomal";
                stageQuick = Random.Range(1, 4);
            }
            else
            {
                gameModeQuick = "Arcade";
                stageQuick = Random.Range(1, 2);
            }
            stageQuick = 1;
            gameModeQuick = "Arcade";
        }
        // ルームへの参加が失敗したら、再び入力できるようにする
        canvasGroup.interactable = true;
    }
}