using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartHandler : NetworkBehaviour
{
    [SerializeField]
    private int _minFlagTime;

    [SerializeField]
    private int _maxFlagTime;

    [SerializeField]
    private Image _fadePanel;

    public int MaxStageNomal;  // 総ステージ数

    private float _difficulty;

    private float _timeFromLastAction;

    private float _cycle = 0.3f;

    public GameObject MainTitle;

    public GameObject StartPanel;

    public GameObject ChangePlayerNumber;

    public GameObject StageSelect;

    public GameObject SelectGameMode;

    public GameObject SetArcadeGame;

    public GameObject KeyBoardMouseUI;

    public GameObject SelectOnlineLobby;

    public GameObject OnlineLobby;

    public GameObject LoadScreen;

    public GameObject Cursor;

    public GameObject[] ControllerUI;

    public GameObject[] PlayerIcon;

    public GameObject[] PlayerSlot;

    private IngameLog _ingameLog;

    // チーム選択
    public Vector3[] PlayerIconPos { get; set; } = new Vector3[4];

    public Vector3[] Slot1Pos = new Vector3[4];

    private int _lastPlayerNum;

    private int _lastPhase;

    public Text _playerNumberText;

    public Text _stageNumberText;

    public Text _flagTimeLimitTx;

    // 画像
    public Image StageImage;

    private Sprite _imageSprite;

    // テキスト
    private string[] _singleArcadeText = { "無限の塔", "InfinityTower", };

    private string[] _multiArcadeText = { "旗取りバトル", "FlagBattle", "サッカー", "FootBall", };

    // static変数
    public static string GameMode1 = "Single";

    public static string GameMode2 = "Nomal";

    public static string TeamMode = "FreeForAll";

    public static int Phase = 0;

    public static int PlayerCount { get; set; } = 1;

    public static int Stage = 1;

    public static int FlagTimeLimit = 90;

    public static int[] PlayerTeam { get; set; } = { 0, 1, 2, 3 };

    public static int[] TeamSize = new int[4];

    public static int TeamCount = 0;

    public static int MaxPlayer = 4;

    public static int MinPlayer;

    public static bool ButtonPushable = true;

    private bool _isPhaseProcessing;

    // ロード画面
    private bool _reconnectable;

    private bool _joinedLobby = false;

    private PlayerInput _playerInput;

    private void Awake()
    {
        if (SingletonSettingCanvas.Instance == null)
        {
            Instantiate(Resources.Load("SettingCanvas"));
        }
    }

    void Start()
    {
        _ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();
        Time.timeScale = 1;
        GameSetting.Playable = false;
        _reconnectable = false;
        _isPhaseProcessing = false;
        Stage = 1;
        PlayerCount = 1;
        TeamMode = "FFA";
        Phase = 0;
        _lastPhase = -1;
        FlagTimeLimit = 90;
        for (int i = 0; i < 4; i++)
        {
            PlayerIconPos[i] = Slot1Pos[i];
            PlayerTeam[i] = i;
            TeamSize[i] = 0;
        }

        NetworkRunner networkRunner = GameObject.Find("Systems").GetComponent<NetworkRunner>();

        //オンラインでルームにいる場合はルーム画面に行く
        if (GameMode1 == "Online" && networkRunner.IsInSession)
        {
            Phase = 3;
        }
    }
    private async UniTask Update()
    {
        SwichUI();
        SwichStageMaterial();
        _playerNumberText.text = PlayerCount.ToString();

        if (SetArcadeGame.gameObject.activeSelf)
        {
            TeamSelect();
        }

        if (_timeFromLastAction > _cycle)
        {
            if (_lastPhase != Phase)
            {
                await PhaseControll();
            }
            ButtonPushable = true;
            _timeFromLastAction = 0;
        }

        Phase = Mathf.Clamp(Phase, 0, 9);
        if (GameMode1 == "Single" && GameMode2 == "Nomal")
        {
            Stage = Mathf.Clamp(Stage, 1, 7);
        }
        else if (GameMode1 == "Single" && GameMode2 == "Arcade")
        {
            Stage = Mathf.Clamp(Stage, 1, 1);
        }
        else if ((GameMode1 == "Multi" || GameMode1 == "Online") && GameMode2 == "Nomal")
        {
            Stage = Mathf.Clamp(Stage, 1, 4);
        }
        else if ((GameMode1 == "Multi" || GameMode1 == "Online") && GameMode2 == "Arcade")
        {
            Stage = Mathf.Clamp(Stage, 1, 2);
        }

        _timeFromLastAction += Time.deltaTime;

        PlayerCount = Mathf.Clamp(PlayerCount, MinPlayer, MaxPlayer);
        FlagTimeLimit = Mathf.Clamp(FlagTimeLimit, _minFlagTime, _maxFlagTime);
    }



    void SwichStageMaterial() //選択ステージ毎に情報切り替え
    {
        switch (GameMode1)
        {
            case "Single":
                _stageNumberText.text = (GameMode2 == "Arcade") ? _singleArcadeText[Settings.languageNum] : "Stage" + Stage.ToString();
                _imageSprite = Resources.Load<Sprite>(GameMode1 + GameMode2 + Stage);
                break;
            case "Multi":
                _stageNumberText.text = (GameMode2 == "Arcade") ? _multiArcadeText[Stage + (2 * Settings.languageNum)] : "Stage" + Stage.ToString();
                _imageSprite = Resources.Load<Sprite>(GameMode1 + GameMode2 + Stage);
                break;
            case "Online":
                _stageNumberText.text = (GameMode2 == "Arcade") ? _multiArcadeText[Stage + (2 * Settings.languageNum)] : "Stage" + Stage.ToString();
                _imageSprite = Resources.Load<Sprite>("Multi" + GameMode2 + Stage);
                break;
        }

        StageImage.sprite = _imageSprite;
    }

    private async UniTask PhaseControll()
    {
        StopCoroutine(Reconnect());
        DisablePanel();
        switch (GameMode1)
        {
            case "Single":
                MinPlayer = 1;
                switch (Phase)
                {
                    case 0:
                        MainTitle.gameObject.SetActive(true);
                        break;
                    case 1:
                        MinPlayer = 1;
                        GameStartHandler.PlayerCount = 1;
                        SelectGameMode.gameObject.SetActive(true);
                        break;
                    case 2:
                        StageSelect.gameObject.SetActive(true);
                        break;
                    case 3:
                        FadeAndSwitchScene();
                        break;
                }

                break;

            case "Multi":
                MinPlayer = 2;
                switch (Phase)
                {
                    case 0:
                        MainTitle.gameObject.SetActive(true);
                        break;
                    case 1:
                        GameStartHandler.PlayerCount = 2;
                        MinPlayer = 2;
                        ChangePlayerNumber.gameObject.SetActive(true);
                        break;
                    case 2:
                        SelectGameMode.gameObject.SetActive(true);
                        _lastPlayerNum = PlayerCount;
                        break;
                    case 3:
                        StageSelect.gameObject.SetActive(true);
                        break;
                    case 4:
                        //ノーマルモードならチーム選択画面はスキップ
                        if (GameMode2 == "Nomal") { Phase++; return; }
                        SetArcadeGame.gameObject.SetActive(true);
                        SetArcade();
                        break;
                    case 5:
                        FadeAndSwitchScene();
                        break;
                }

                break;

            case "Online":
                Debug.Log("Phase == " + Phase);
                switch (Phase)
                {
                    case 0:
                        MainTitle.gameObject.SetActive(true);
                        NetWorkMain.isOnline = false;
                        break;
                    case 1:
                        MinPlayer = 1;
                        SelectOnlineLobby.gameObject.SetActive(true);
                        _joinedLobby = false;
                        break;
                    case 2:
                        if (InputName.TypedTextToString == null)
                        {
                            IngameLog.GenerateIngameLog("Please type player name");
                            Phase--;
                            return;
                        }
                        OnlineLobby.gameObject.SetActive(true);
                        if (NetWorkMain.NetWorkId == NetWorkMain.leaderId)
                        {
                            RPC_SyncPhase(Phase);
                        }
                        break;
                    case 3:
                        if (MatchmakingView.mode == "Quick" && NetworkRunner.Instance.LocalPlayer.IsLeader)
                        {
                            NetworkRunner.Instance.SetCustomProperties(new Dictionary<string, object> { { "isReady", new bool[4] { true, true, true, true } } });
                            RPC_SetDefaultArcade(MatchmakingView.stageQuick);
                            RPC_CallStartIfReady();
                            RPC_SyncArcadeTime(FlagTimeLimit);
                            return;
                        }
                        RPC_SyncArcadeTime(FlagTimeLimit);
                        RPC_CallStartIfReady();
                        Phase = 3;
                        OnlineLobby.gameObject.SetActive(true);
                        break;
                    case 4:
                        Phase = 3;
                        RPC_SetDefaultArcade(Stage);
                        return;
                    case 5:
                        StageSelect.gameObject.SetActive(true);
                        break;
                    case 6:
                        Phase = 3;
                        RPC_SetDefaultArcade(Stage);
                        return;
                    case 7:
                        SetArcadeGame.gameObject.SetActive(true);
                        RPC_SyncPhase(Phase);
                        SetArcade();
                        break;
                    case 8:
                        Phase = 3;
                        return;
                }
                break;
        }
        _lastPhase = Phase;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SyncArcadeTime(int timeLimit)
    {
        FlagTimeLimit = timeLimit;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SyncPhase(int phaseLocal)
    {
        Phase = phaseLocal;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetDefaultArcade(int updatedStage)
    {
        Stage = updatedStage;
        SetArcade();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_CallStartIfReady()
    {
        if (GameStartHandler.PlayerCount > 1)
        {
            //全プレイヤーが準備完了か確認する
            bool allReady = true;
            if (NetWorkMain.GetCustomProps<bool[]>("isReady", out bool[] isReadyValueArray))
            {
                for (int i = 0; i < PlayerCount; i++)
                {
                    if (isReadyValueArray[i] == false)
                    {
                        allReady = false;
                    }
                }
                if (allReady)
                {
                    //次のために全プレイヤーのreadyを解除しておく
                    for (int i = 0; i < MaxPlayer; i++)
                    {
                        isReadyValueArray[i] = false;
                    }
                    NetWorkMain.SetCustomProps<bool[]>("isReady", isReadyValueArray);
                    RPC_SwitchScene();
                }
                else
                {
                    IngameLog.GenerateIngameLog("全てのプレイヤーの準備が完了していません");
                }
            }
        }
        else
        {
            IngameLog.GenerateIngameLog("プレイヤー数が足りていません");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SwitchScene()
    {
        FadeAndSwitchScene();
    }

    void SetArcade()
    {
        //旗鳥モードのデフォルト設定
        if (Stage == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerIcon[i].gameObject.SetActive(false);
                PlayerSlot[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < PlayerCount; i++)
            {
                PlayerIcon[i].gameObject.SetActive(true);
                PlayerSlot[i].gameObject.SetActive(true);
                PlayerTeam[i] = i;
            }
            if (GameMode1 == "Online")
            {
                NetWorkMain.SetCustomProps<int[]>("playerTeam", new int[] { 0, 1, 2, 3 });
            }
        }
        //サッカーモードのデフォルト設定
        if (Stage == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerIcon[i].gameObject.SetActive(false);
                PlayerSlot[i].gameObject.SetActive(false);
                PlayerTeam[i] = (i + 1) % 2 == 0 ? 1 : 0;
            }
            for (int i = 0; i < PlayerCount; i++)
            {
                PlayerIcon[i].gameObject.SetActive(true);
            }
            PlayerSlot[0].gameObject.SetActive(true);
            PlayerSlot[1].gameObject.SetActive(true);
            if (GameMode1 == "Online")
            {
                NetWorkMain.SetCustomProps<int[]>("playerTeam", new int[] { 0, 1, 0, 1 });
            }

        }
        TeamSelect();
    }
    void TeamSelect()
    {
        _flagTimeLimitTx.text = FlagTimeLimit.ToString();
        for (int i = 0; i < PlayerCount; i++)
        {
            RectTransform rectTransform = PlayerIcon[i].GetComponent<RectTransform>();
            float spacing = rectTransform.sizeDelta.x + 10f;
            rectTransform.localPosition = PlayerIconPos[i];
            Debug.Log(PlayerIcon[i].transform.position.z);

            for (int j = 0; j < PlayerCount; j++)
            {
                if (PlayerTeam[i] == j)
                {
                    PlayerIconPos[i] = Slot1Pos[j];
                    int iconsInSlot = 0;
                    for (int k = 0; k < PlayerCount; k++)
                    {
                        if (k != i)
                        {
                            if (PlayerTeam[k] == PlayerTeam[i] && k < i)
                            {
                                iconsInSlot++;
                                PlayerIconPos[i].x = Slot1Pos[PlayerTeam[i]].x + (spacing * iconsInSlot);
                            }
                        }
                    }
                }
            }
            TeamSize[i] = 0;
        }

        for (int i = 0; i < PlayerCount; i++)
        {
            PlayerTeam[i] = Mathf.Clamp(PlayerTeam[i], 0, PlayerCount - 1);
        }
        bool allOtherTeam = PlayerTeam.Take(PlayerCount).Distinct().Count() == PlayerCount;

        if (allOtherTeam)
        {
            TeamMode = "FFA";
        }
        else
        {
            TeamMode = "Team";
            TeamCount = 0;
            for (int i = 0; i < PlayerCount; i++)
            {
                TeamSize[PlayerTeam[i]]++;
            }
            for (int j = 0; j < PlayerCount; j++)
            {
                if (TeamSize[j] != 0)
                {
                    TeamCount++;
                }
            }
        }
    }


    void DisablePanel()
    {
        StageSelect.gameObject.SetActive(false);
        SelectGameMode.gameObject.SetActive(false);
        MainTitle.gameObject.SetActive(false);
        ChangePlayerNumber.gameObject.SetActive(false);
        SetArcadeGame.gameObject.SetActive(false);
        SelectOnlineLobby.gameObject.SetActive(false);
        OnlineLobby.gameObject.SetActive(false);
        LoadScreen.gameObject.SetActive(false);
    }

    private void FadeAndSwitchScene()
    {
        _fadePanel.DOFade(1f, 1f)
            .OnComplete(() => SceneManager.LoadScene("Stage"));
    }

    void SwichUI()
    {
        //キーボード,マウスのとき
        if (!ControllerInput.usingController)
        {
            //keyBoardMouseUI.gameObject.SetActive(true);
            for (int i = 0; i < ControllerUI.Length; i++) { ControllerUI[i].gameObject.SetActive(false); }
            Cursor.gameObject.SetActive(false);
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {

            for (int i = 0; i < ControllerUI.Length; i++) { ControllerUI[i].gameObject.SetActive(true); }
            Cursor.gameObject.SetActive(true);
            //keyBoardMouseUI.gameObject.SetActive(false);
        }

        //UI非表示設定時
        if (Settings.guideMode == 1)
        {
            for (int i = 0; i < ControllerUI.Length; i++) { ControllerUI[i].gameObject.SetActive(false); }
        }
    }

}
