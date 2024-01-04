using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
public class RoomButton : MonoBehaviourPunCallbacks
{
    private const int MaxPlayers = 4;

    [SerializeField]
    private TextMeshProUGUI label = default;

    private MatchmakingView matchmakingView;
    private Button button;

    public string RoomName { get; private set; }

    public static string RoomNameToPass;
    public void Init(MatchmakingView parentView, int roomId)
    {
        matchmakingView = parentView;
        RoomName = $"Room{roomId}";

        button = GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // ���[���Q���������́A�S�Ă̎Q���{�^���������Ȃ��悤�ɂ���
        matchmakingView.OnJoiningRoom();

        // �{�^���ɑΉ��������[�����̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���Ă���Q������j
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = MaxPlayers;

        // �J�X�^���v���p�e�B���܂�Hashtable���쐬
        Hashtable customRoomProperties = new Hashtable();
        customRoomProperties.Add("stage", 0);
        customRoomProperties.Add("leaderId", 1);
        customRoomProperties.Add("gameMode", "Nomal");
        customRoomProperties.Add("Winnings", "Nomal");
   
        int[] winnings = new int[] { 0, 0, 0, 0};
        bool[] isJoined = new bool[] { false, false, false, false };
        customRoomProperties.Add("isJoined", isJoined);
        customRoomProperties.Add("winnings", winnings);

        roomOptions.CustomRoomProperties = customRoomProperties;
        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
        RoomNameToPass = RoomName;
        GameStart.phase++;
    }

/*    public void SetPlayerCount(int playerCount)
    {
        label.text = $"{RoomName}\n{playerCount} / {MaxPlayers}";
        // ���[���������łȂ����̂݁A���[���Q���{�^����������悤�ɂ���
        button.interactable = (playerCount < MaxPlayers);
    }*/


}