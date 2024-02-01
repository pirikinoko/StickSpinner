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
        // ���r�[�ɎQ������܂ł́A���͂ł��Ȃ��悤�ɂ���
        if (!PhotonNetwork.InLobby) 
        {
            canvasGroup.interactable = false;
        }

        // ���[�����X�g�\��������������
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
        // ���r�[�ɎQ��������A���͂ł���悤�ɂ���
        canvasGroup.interactable = true;
        this.gameObject.SetActive(true);
    }

    private void OnRoomNameInputFieldValueChanged(string value)
    {
        // ���[������1�����ȏ���͂���Ă��鎞�̂݁A���[���쐬�{�^����������悤�ɂ���
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
        // ���[���쐬�������́A���͂ł��Ȃ��悤�ɂ���
        canvasGroup.interactable = false;

        // ���̓t�B�[���h�ɓ��͂������[�����̃��[�����쐬����
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomName, roomOptions);

    }
    private void OnCreateRoomButtonClick()
    {
        if (InputName.TypedTextToString.Length == 0) 
        {
            ingameLog.GenerateIngameLog("���O����͂��Ă�������");
            return;
        }
        mode = "Nomal";
        // ���[���쐬�������́A���͂ł��Ȃ��悤�ɂ���
        canvasGroup.interactable = false;

        // ���̓t�B�[���h�ɓ��͂������[�����̃��[�����쐬����
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // ���[���̍쐬�����s������A�Ăѓ��͂ł���悤�ɂ���
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
        // ���[���Q���������́A���͂ł��Ȃ��悤�ɂ���
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

            // 0�܂���1�̂����ꂩ�ɑΉ����鏈�����s��
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
        // ���[���ւ̎Q�������s������A�Ăѓ��͂ł���悤�ɂ���
        canvasGroup.interactable = true;
    }
}