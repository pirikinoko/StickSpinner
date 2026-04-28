using Cysharp.Threading.Tasks;
using Fusion;
using R3;
using UnityEngine;
using UnityEngine.UI;
public class EnterSessionPresenter : NetworkBehaviour
{
    [SerializeField]
    private OnlineModeHandler _onlineModeHandler;

    private SessionManager _sessionManager;

    public Observable<string> OnJoinedSession => _onJoinedSession;

    private Subject<string> _onJoinedSession = new();

    [SerializeField]
    private InputField _roomNameInputField = default;

    [SerializeField]
    private Button _createOrJoinRoomButton = default;

    [SerializeField]
    private Button _createRoomLockedButton = default;

    [SerializeField]
    private Text _playButtonText;

    private CanvasGroup _canvasGroup;

    private IngameLog _ingameLog;

    public int CcuLimit = 80;

    private GameStartHandler _gameStart;

    private void Start()
    {
        _sessionManager = SessionManager.Instance;

        _gameStart = GameObject.Find("Systems").GetComponent<GameStartHandler>();
        _ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _roomNameInputField.onValueChanged.AddListener(OnRoomNameInputFieldValueChanged);
        _createOrJoinRoomButton.onClick.AddListener(() => OnJoinOrCreateRoomButtonClicked().Forget());
        _createRoomLockedButton.onClick.AddListener(() => OnCreateRoomLockedButtonClick().Forget());

        if (_roomNameInputField.text.Length > 0)
        {
            _createOrJoinRoomButton.interactable = true;
            _createRoomLockedButton.interactable = true;
        }
        else
        {
            _createOrJoinRoomButton.interactable = false;
            _createRoomLockedButton.interactable = false;
        }
    }

    private void Update()
    {
        _canvasGroup.interactable = _sessionManager.NetworkRunner.IsConnectedToServer;
    }

    private void OnRoomNameInputFieldValueChanged(string value)
    {
        _createOrJoinRoomButton.interactable = value.Length > 0;
        _createRoomLockedButton.interactable = value.Length > 0;
    }

    private async UniTask OnJoinOrCreateRoomButtonClicked()
    {
        if (await TryCreateOrJoinRoom(_roomNameInputField.text))
        {
            _onlineModeHandler.Initialize();
        }
        else
        {
            IngameLog.GenerateIngameLog("セッションに参加できませんでした");
            await OnJoinSessionFailed();
        }
    }

    private async UniTask OnCreateRoomLockedButtonClick()
    {
        string roomName = _roomNameInputField.text + "!Locked!";
        if (await TryCreateOrJoinRoom(roomName))
        {
            _onlineModeHandler.Initialize();
        }
        else
        {
            IngameLog.GenerateIngameLog("ルームの作成に失敗しました");
            await OnJoinSessionFailed();
        }
    }


    private async UniTask<bool> TryCreateOrJoinRoom(string roomName)
    {
        if (IsCcuOver() == false)
        {
            IngameLog.GenerateIngameLog("The server is full right now!");
            return false;
        }

        if (InputName.TypedTextToString == "" || InputName.TypedTextToString.Contains(" "))
        {
            IngameLog.GenerateIngameLog("Please type player name");
            return false;
        }

        _canvasGroup.interactable = false;

        var result = await _sessionManager.NetworkRunner.StartGame(new StartGameArgs
        {
            SessionName = roomName,
        });

        return result.Ok;
    }

    public void OnJoiningRoom()
    {
        _canvasGroup.interactable = false;
    }


    private async UniTask OnJoinSessionFailed()
    {
        await TryCreateOrJoinRoom(_roomNameInputField.text + "!Locked!");

        _canvasGroup.interactable = true;
    }

    private bool IsCcuOver()
    {
        // 同時接続数の取得方法が不明なため、放置
        return true;
        //int currentCCU = _sessionManager.NetworkRunner..Sum(session => session.PlayerCount);

        //Debug.Log($"CCU Limit: {CcuLimit}, Current CCU: {currentCCU}");

        //// 接続制限に達している場合は接続を試みない
        //return currentCCU > CcuLimit;
    }
}
