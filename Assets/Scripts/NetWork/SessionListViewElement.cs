using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListViewElement : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameLabel = default;
    [SerializeField]
    private TextMeshProUGUI playerCounter = default;

    SessionList roomList = new SessionList();
    private MatchmakingView matchmakingView;
    private Button button;
    bool isOpen;
    int maxPlayers = 4;

    void Start()
    {
        button = this.GetComponent<Button>();
    }

    void Update()
    {
        int playerCount = 1;
        if (roomList.TryGetSessionInfo(nameLabel.text, out SessionInfo session))
        {
            playerCount = session.PlayerCount;

            isOpen = true;
            if (session.Properties.TryGetValue("Password", out object passwordObj) && passwordObj is string correctPassword)
            {
                // パスワードが設定されている場合はisOpen = false
                if (correctPassword != "")
                {
                    isOpen = false;
                }
            }
        }

        button.interactable = playerCount < maxPlayers;
    }

    public void Init(MatchmakingView parentView)
    {
        matchmakingView = parentView;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (InputName.TypedTextToString == null)
        {
            IngameLog.GenerateIngameLog("Please type player name");
            return;
        }
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        runner.JoinSessionLobby(nameLabel.text);
    }

    public void Show(SessionInfo sessionInfo)
    {
        nameLabel.text = sessionInfo.Name;
        playerCounter.SetText("{0} / {1}", sessionInfo.PlayerCount, sessionInfo.MaxPlayers);

        // ルームが満員でない時のみ、参加ボタンを押せるようにする
        button.interactable = sessionInfo.PlayerCount < sessionInfo.MaxPlayers;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
