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
        // ルーム参加処理中は、全ての参加ボタンを押せないようにする
        matchmakingView.OnJoiningRoom();

        // ボタンに対応したルーム名のルームに参加する（ルームが存在しなければ作成してから参加する）
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = MaxPlayers;

        // カスタムプロパティを含むHashtableを作成
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
        // ルームが満員でない時のみ、ルーム参加ボタンを押せるようにする
        button.interactable = (playerCount < MaxPlayers);
    }*/


}