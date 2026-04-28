using DG.Tweening;
using Fusion;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartHandler : NetworkBehaviour
{
    private SessionManager _sessionManager;

    public static int MaxPlayerCount = 4;

    public static int MinPlayerCount = 1;

    [SerializeField]
    private Image _fadePanel;

    private float _timeFromLastAction;

    public GameObject MainTitle;

    public GameObject ChangePlayerNumber;

    public GameObject StageSelect;

    public GameObject SelectGameMode;

    public GameObject SetArcadeGame;

    public GameObject SelectOnlineLobby;

    public GameObject OnlineLobby;

    public GameObject Cursor;

    public GameObject[] PlayerIcon;

    public GameObject[] PlayerSlot;

    private GameObject[] _controllerUI;

    // チーム選択
    public Vector3[] PlayerIconPos { get; set; } = new Vector3[4];

    private int _lastPlayerNum;

    private int _lastPhase = -1;

    public Text PlayerCountText;

    public Text StageNameText;

    public Text TimeLimitText;

    // 画像
    public Image StageImage;

    private Sprite _imageSprite;

    // static変数
    public static int Phase = 0;

    public static int PlayerCount { get; set; } = 1;

    public static int[] TeamSize = new int[4];

    public static int TeamCount = 0;

    public static bool ButtonPushable = true;

    private void Awake()
    {
        if (SingletonSettingCanvas.Instance == null)
        {
            Instantiate(Resources.Load("SettingCanvas"));
        }

        _controllerUI = GameObjectExtentions.FindGameObjectsWithTagIncludingInactive("ControllerUI").ToArray();
    }

    void Start()
    {
        _sessionManager = SessionManager.Instance;

        Time.timeScale = 1;
        GamePlayHandler.Playable = false;
        PlayerCount = 1;
        Phase = 0;

        for (int i = 0; i < 4; i++)
        {
            PlayerIconPos[i] = Constants.FirstSlotPosition[i];
            TeamSize[i] = 0;
        }

        //オンラインでルームにいる場合はルーム画面に行く
        if (_sessionManager.PlayingType == "Online" && _sessionManager.NetworkRunner.IsInSession)
        {
            Phase = 3;
        }
    }

    private void Update()
    {
        SwichUI();
        SwichStageMaterial();
        PlayerCountText.text = PlayerCount.ToString();

        if (SetArcadeGame.gameObject.activeSelf)
        {
            TeamSelect();
        }

        if (_timeFromLastAction > Constants.ReInputCooldown)
        {
            if (_lastPhase != Phase)
            {
                PhaseControll();
            }
            ButtonPushable = true;
            _timeFromLastAction = 0;
        }

        Phase = Mathf.Clamp(Phase, 0, 9);
        if (_sessionManager.PlayingType == "Single" && _sessionManager.GameMode == "Nomal")
        {
            _sessionManager.Stage = Mathf.Clamp(_sessionManager.Stage, 1, 7);
        }
        else if (_sessionManager.PlayingType == "Single" && _sessionManager.PlayingType == "Arcade")
        {
            _sessionManager.Stage = Mathf.Clamp(_sessionManager.Stage, 1, 1);
        }
        else if ((_sessionManager.PlayingType == "Multi" || _sessionManager.PlayingType == "Online") && _sessionManager.PlayingType == "Nomal")
        {
            _sessionManager.Stage = Mathf.Clamp(_sessionManager.Stage, 1, 4);
        }
        else if ((_sessionManager.PlayingType == "Multi" || _sessionManager.PlayingType == "Online") && _sessionManager.PlayingType == "Arcade")
        {
            _sessionManager.Stage = Mathf.Clamp(_sessionManager.Stage, 1, 2);
        }

        _timeFromLastAction += Time.deltaTime;

        PlayerCount = Mathf.Clamp(PlayerCount, MinPlayerCount, MaxPlayerCount);
        _sessionManager.TimeLimit = Mathf.Clamp(_sessionManager.TimeLimit, Constants.MinFlagTime, Constants.MaxFlagTime);
    }



    void SwichStageMaterial() //選択ステージ毎に情報切り替え
    {
        switch (_sessionManager.PlayingType)
        {
            case "Single":
                StageNameText.text = (_sessionManager.GameMode == "Arcade") ? Constants.SingleArcadeText[Settings.languageNum] : "Stage" + _sessionManager.Stage.ToString();
                _imageSprite = Resources.Load<Sprite>(_sessionManager.PlayingType + _sessionManager.GameMode + _sessionManager.Stage);
                break;
            case "Multi":
                StageNameText.text = (_sessionManager.GameMode == "Arcade") ? Constants.MultiArcadeText[_sessionManager.Stage + (2 * Settings.languageNum)] : "Stage" + _sessionManager.Stage.ToString();
                _imageSprite = Resources.Load<Sprite>(_sessionManager.PlayingType + _sessionManager.GameMode + _sessionManager.Stage);
                break;
            case "Online":
                StageNameText.text = (_sessionManager.GameMode == "Arcade") ? Constants.MultiArcadeText[_sessionManager.Stage + (2 * Settings.languageNum)] : "Stage" + _sessionManager.Stage.ToString();
                _imageSprite = Resources.Load<Sprite>("Multi" + _sessionManager.GameMode + _sessionManager.Stage);
                break;
        }

        StageImage.sprite = _imageSprite;
    }

    private void PhaseControll()
    {
        DisablePanel();
        switch (_sessionManager.PlayingType)
        {
            case "Single":
                MinPlayerCount = 1;
                switch (Phase)
                {
                    case 0:
                        MainTitle.gameObject.SetActive(true);
                        break;
                    case 1:
                        MinPlayerCount = 1;
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
                MinPlayerCount = 2;
                switch (Phase)
                {
                    case 0:
                        MainTitle.gameObject.SetActive(true);
                        break;
                    case 1:
                        GameStartHandler.PlayerCount = 2;
                        MinPlayerCount = 2;
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
                        if (_sessionManager.GameMode == "Nomal") { Phase++; return; }
                        SetArcadeGame.gameObject.SetActive(true);
                        RPC_SetArcade();
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
                        break;
                    case 1:
                        MinPlayerCount = 1;
                        SelectOnlineLobby.gameObject.SetActive(true);
                        break;
                    case 2:
                        if (InputName.TypedTextToString == null)
                        {
                            IngameLog.GenerateIngameLog("Please type player name");
                            Phase--;
                            return;
                        }
                        OnlineLobby.gameObject.SetActive(true);
                        if (_sessionManager.NetworkRunner.LocalPlayer.PlayerId == _sessionManager.LeaderId)
                        {
                            RPC_SyncPhase(Phase);
                        }
                        break;
                    case 3:
                        StartGame();
                        Phase = 3;
                        OnlineLobby.gameObject.SetActive(true);
                        break;
                    case 4:
                        Phase = 3;
                        RPC_SetArcade();
                        return;
                    case 5:
                        StageSelect.gameObject.SetActive(true);
                        break;
                    case 6:
                        Phase = 3;
                        RPC_SetArcade();
                        return;
                    case 7:
                        SetArcadeGame.gameObject.SetActive(true);
                        RPC_SyncPhase(Phase);
                        RPC_SetArcade();
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
    public void RPC_SyncPhase(int phaseLocal)
    {
        Phase = phaseLocal;
    }

    void StartGame()
    {
        if (GameStartHandler.PlayerCount < 2)
        {
            IngameLog.GenerateIngameLog("プレイヤー数が足りていません");
            return;
        }

        bool allReady = true;

        for (int i = 0; i < PlayerCount; i++)
        {
            if (_sessionManager.IsReadyToSwitchScene[i] == false)
            {
                allReady = false;
            }
        }
        if (allReady)
        {
            _sessionManager.IsReadyToSwitchSceneToSend = new bool[4] { false, false, false, false };
            RPC_SwitchScene();
        }
        else
        {
            IngameLog.GenerateIngameLog("全てのプレイヤーの準備が完了していません");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SwitchScene()
    {
        FadeAndSwitchScene();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SetArcade()
    {
        if (_sessionManager.Stage == 1)
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
            }
            _sessionManager.PlayerTeamToSend = new int[] { 0, 1, 2, 3 };
        }
        if (_sessionManager.Stage == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerIcon[i].gameObject.SetActive(false);
                PlayerSlot[i].gameObject.SetActive(false);
                _sessionManager.PlayerTeamToSend[i] = (i + 1) % 2 == 0 ? 1 : 0;
            }
            for (int i = 0; i < PlayerCount; i++)
            {
                PlayerIcon[i].gameObject.SetActive(true);
            }
            PlayerSlot[0].gameObject.SetActive(true);
            PlayerSlot[1].gameObject.SetActive(true);
        }
        TeamSelect();
    }

    void TeamSelect()
    {
        TimeLimitText.text = _sessionManager.TimeLimit.ToString();

        for (int i = 0; i < PlayerCount; i++)
        {
            RectTransform rectTransform = PlayerIcon[i].GetComponent<RectTransform>();
            float spacing = rectTransform.sizeDelta.x + 10f;
            rectTransform.localPosition = PlayerIconPos[i];
            Debug.Log(PlayerIcon[i].transform.position.z);

            for (int j = 0; j < PlayerCount; j++)
            {
                if (_sessionManager.PlayerTeam[i] == j)
                {
                    PlayerIconPos[i] = Constants.FirstSlotPosition[j];
                    int iconsInSlot = 0;
                    for (int k = 0; k < PlayerCount; k++)
                    {
                        if (k != i)
                        {
                            if (_sessionManager.PlayerTeam[k] == _sessionManager.PlayerTeam[i] && k < i)
                            {
                                iconsInSlot++;
                                PlayerIconPos[i].x = Constants.FirstSlotPosition[_sessionManager.PlayerTeam[i]].x + (spacing * iconsInSlot);
                            }
                        }
                    }
                }
            }
            TeamSize[i] = 0;
        }

        for (int i = 0; i < PlayerCount; i++)
        {
            _sessionManager.PlayerTeamToSend[i] = Mathf.Clamp(_sessionManager.PlayerTeam[i], 0, PlayerCount - 1);
        }

        bool allOtherTeam = _sessionManager.PlayerTeam.Take(PlayerCount).Distinct().Count() == PlayerCount;

        if (allOtherTeam)
        {
            _sessionManager.TeamMode = "FFA";
        }
        else
        {
            _sessionManager.TeamMode = "Team";
            TeamCount = 0;
            for (int i = 0; i < PlayerCount; i++)
            {
                TeamSize[_sessionManager.PlayerTeam[i]]++;
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
    }

    private void FadeAndSwitchScene()
    {
        _fadePanel.DOFade(1f, 1f)
            .OnComplete(() => SceneManager.LoadScene("Stage"));
    }

    void SwichUI()
    {
        if (!ControllerInputManager.usingController)
        {
            for (int i = 0; i < _controllerUI.Length; i++) { _controllerUI[i].gameObject.SetActive(false); }
            Cursor.gameObject.SetActive(false);
        }
        else if (ControllerInputManager.usingController)
        {

            for (int i = 0; i < _controllerUI.Length; i++) { _controllerUI[i].gameObject.SetActive(true); }
            Cursor.gameObject.SetActive(true);
        }

        if (Settings.guideMode == 1)
        {
            for (int i = 0; i < _controllerUI.Length; i++) { _controllerUI[i].gameObject.SetActive(false); }
        }
    }
}
