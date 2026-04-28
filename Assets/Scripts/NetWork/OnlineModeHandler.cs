using Fusion;
using R3;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OnlineModeHandler : NetworkBehaviour
{
    private SessionManager _sessionManager;

    public GameObject[] PlayerConfigs;

    public GameObject[] LeaderIcons;

    public Text[] PlayerNameText;

    public Text[] WinningsText;

    public Text ModeText;

    public Text StageText;

    private IngameLog _ingameLog;

    private void Start()
    {
        _sessionManager = SessionManager.Instance;
        _ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();

        _sessionManager.OnPlayerCountChange
            .Subscribe(_ => HandlePlayerCountChange());

        if (_sessionManager.NetworkRunner.IsInSession)
        {
            RPC_SetPlayerNumber();
            RPC_UpdateUi();
        }
    }

    private void Update()
    {
        ModeText.text = _sessionManager.GameMode;
        StageText.text = "stage" + _sessionManager.Stage;

        if (_sessionManager.PlayingType == "Online" && _sessionManager.NetworkRunner.IsInSession)
        {
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                PlayerConfigs[i].gameObject.SetActive(true);
                PlayerNameText[i].text = _sessionManager.PlayerNames[i];
            }
        }
    }

    /// <summary>
    /// セッション参加時に呼び出される初期化処理
    /// </summary>
    public void Initialize()
    {
        RPC_UpdateUi();
        RPC_SetPlayerNumber();
    }

    private void HandlePlayerCountChange()
    {
        RPC_UpdateUi();
        RPC_SetPlayerNumber();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetPlayerNumber()
    {
        GameStartHandler.PlayerCount = _sessionManager.NetworkRunner.ActivePlayers.Count();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateUi()
    {
        for (int i = 3; i >= GameStartHandler.PlayerCount; i--)
        {
            PlayerConfigs[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            PlayerConfigs[i].gameObject.SetActive(true);
            PlayerNameText[i].text = _sessionManager.PlayerNames[i];
        }
    }
}
