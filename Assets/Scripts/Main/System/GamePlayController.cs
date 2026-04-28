using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using Fusion;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GamePlayHandler : NetworkBehaviour, IPlayerLeft
{
    [SerializeField]
    private GameObject[] _canvasies;
    [SerializeField]
    private Text _countDownText;

    [SerializeField]
    private Text _topText;

    [SerializeField]
    private GameObject _quickStartingPanel;

    [SerializeField]
    private GameObject _pauseButton;

    [HideInInspector]
    public GameObject[] Players = new GameObject[GameStartHandler.MaxPlayerCount];

    [HideInInspector]
    public GameObject[] Sticks = new GameObject[GameStartHandler.MaxPlayerCount];

    public static bool Playable = false;
    public static bool AllJoin = false;
    public static bool IsSetupEnded = false;

    public bool IsPaused { get; set; } = false;

    //タイム
    public bool IsCountDownEnded { get; set; } = false;
    public bool IsCameraAnimationEnded { get; set; } = false;
    public static float ElapsedTime;
    public static float RemainingTime;
    public static int Height;

    //ステージ切り替え用
    [SerializeField]
    private GameObject[] _stageObjectSingle;

    [SerializeField]
    private GameObject[] _stageObjectSingleArcade;

    [SerializeField]
    private GameObject[] _stageObjectMulti;

    [SerializeField]
    private GameObject[] _stageObjectMultiArcade;

    //　タグで管理するUI
    private GameObject[] _battleModeUI;

    private GameObject[] _keyBoardMouseUI;

    private GameObject[] _controllerUI;

    [SerializeField]
    private Tilemap _tilemap;

    public static Vector2[] RespownPos = new Vector2[GameStartHandler.MaxPlayerCount];
    private GameObject[] _defaultPosGO = new GameObject[GameStartHandler.MaxPlayerCount];

    private SessionManager _sessionManager;


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPCSetStickPos()
    {
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            int playerID = i + 1;
            if (GameObject.Find("Player" + playerID))
            {
                Sticks[i] = GameObject.Find("Stick" + playerID.ToString());
                Sticks[i].transform.position = GameObject.Find("Player" + playerID.ToString()).transform.position;
            }
        }
    }

    void Awake()
    {
        AllJoin = false;
        IsSetupEnded = false;
        Playable = false;

        _countDownText.text = "";

        new[] { _stageObjectSingle, _stageObjectMulti, _stageObjectSingleArcade, _stageObjectMultiArcade }
                  .SelectMany(array => array)
                  .ToList()
                  .ForEach(obj => obj.gameObject.SetActive(false));

        if (SingletonSettingCanvas.Instance == null)
        {
            Instantiate((GameObject)Resources.Load("SettingCanvas"));
        }

        foreach (var canvas in _canvasies)
        {
            canvas.SetActive(true);
        }
    }

    private async UniTask Start()
    {
        Debug.Log("Start: Initializing SessionManager");
        _sessionManager = SessionManager.Instance;

        Debug.Log("Start: Finding UI elements");
        _controllerUI = GameObjectExtentions.FindGameObjectsWithTagIncludingInactive("ControllerUI").ToArray();
        _keyBoardMouseUI = GameObjectExtentions.FindGameObjectsWithTagIncludingInactive("KeyBoardMouseUI").ToArray();
        _battleModeUI = GameObjectExtentions.FindGameObjectsWithTagIncludingInactive("BattleModeUI").ToArray();
        Debug.Log($"_battleModeUI.Count:{_battleModeUI.Length}");

        Debug.Log("Start: Activating stage");
        ActivateStage();

        Debug.Log("Start: Waiting until all players join");
        await WaitUntilAllPlayersJoin();
        Debug.Log("Start: All players joined");

        _quickStartingPanel.gameObject.SetActive(false);

        Debug.Log("Start: Instantiating objects");
        InstantiateObjects();

        Debug.Log("Start: Getting players");
        await GetPlayers();
        Debug.Log("Start: Players obtained");

        Debug.Log("Start: Setting respawn positions");
        SetRespownPos();

        Debug.Log("Start: Waiting until setup ends");
        await WaitUntilSetupEnds();
        Debug.Log("Start: Setup ended");


        if (_sessionManager.PlayingType == "Online")
        {
            if (Runner.IsServer)
            {
                Debug.Log("Start: Starting timer via RPC");
                // サーバー側でタイマー開始のRPCを全クライアントに送信
                RPC_StartTimer();
            }
        }
        else
        {
            Debug.Log("Start: Starting timer locally");
            // ローカルマルチプレイ時は直接タイマーを開始
            await StartTimer();
        }

        Debug.Log("Start: Initialization complete");
    }


    private void Update()
    {
        ToggleGuideUiActive();
        GameTimeManagement();
    }

    private void ActivateStage()
    {
        //ステージのアクティブ管理
        if (_sessionManager.PlayingType == "Single")
        {
            _quickStartingPanel.SetActive(false);
            ElapsedTime = 0;
            for (int i = 0; i < _battleModeUI.Length; i++)
            {
                _battleModeUI[i].gameObject.SetActive(false);
            }
            //モードごとの初期化
            switch (_sessionManager.GameMode)
            {
                case "Nomal":
                    _stageObjectSingle[_sessionManager.Stage - 1].gameObject.SetActive(true);
                    _tilemap.color = new Color32(50, 50, 50, 255);
                    break;
                case "Arcade":
                    _stageObjectSingleArcade[_sessionManager.Stage - 1].gameObject.SetActive(true);
                    _tilemap.color = new Color32(50, 50, 50, 255);
                    _topText.text = GenerateStage.maxHeight + "m";
                    break;
            }
        }
        else
        {
            _tilemap.color = new Color32(50, 50, 50, 255);
            switch (_sessionManager.GameMode)
            {
                case "Nomal":
                    _stageObjectMulti[_sessionManager.Stage - 1].gameObject.SetActive(true);
                    for (int i = 0; i < _battleModeUI.Length; i++)
                    {
                        _battleModeUI[i].gameObject.SetActive(false);
                    }
                    IngameLog.GenerateIngameBanner("ゲームモード:レース　 ゴールを目指そう");
                    break;

                case "Arcade":
                    _stageObjectMultiArcade[_sessionManager.Stage - 1].gameObject.SetActive(true);
                    RemainingTime = _sessionManager.TimeLimit;
                    _topText.text = _sessionManager.TimeLimit.ToString();

                    for (int i = 0; i < _battleModeUI.Length; i++)
                    {
                        _battleModeUI[i].gameObject.SetActive(true);
                    }
                    IngameLog.GenerateIngameBanner("ゲームモード:フラッグ　旗を占拠しよう");
                    break;
            }
        }
    }
    void SetRespownPos()
    {
        //マルチプレイ時のスポーン地点設定
        if (_sessionManager.PlayingType != "Single")
        {
            int leftTeamCount = 0, rightTeamCount = 0;
            for (int i = 0; i < GameStartHandler.MaxPlayerCount; i++)
            {
                //サッカーモード時のスポーン地点設定
                if (_sessionManager.GameMode == "Arcade" && _sessionManager.Stage == 2)
                {
                    if (_sessionManager.PlayerTeam[i] == 0)
                    {
                        _defaultPosGO[i] = GameObject.Find("LeftDefaultPlayerPos" + (leftTeamCount + 1).ToString());
                        leftTeamCount++;
                    }
                    else if (_sessionManager.PlayerTeam[i] == 1)
                    {
                        _defaultPosGO[i] = GameObject.Find("RightDefaultPlayerPos" + (rightTeamCount + 1).ToString());
                        rightTeamCount++;
                    }
                    else
                    {
                        _defaultPosGO[i] = GameObject.Find("RightDefaultPlayerPos1");
                    }
                }
                //サッカーモード以外のスポーン地点設定
                else
                {
                    _defaultPosGO[i] = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
                    _defaultPosGO[i].GetComponent<SpriteRenderer>().enabled = false;
                }
                RespownPos[i] = _defaultPosGO[i].gameObject.transform.position;
            }
        }
        //シングルプレイ時のスポーン地点設定
        else
        {
            _defaultPosGO[0] = GameObject.Find("DefaultPlayerPos1");
            RespownPos[0] = _defaultPosGO[0].gameObject.transform.position;
            _defaultPosGO[0].GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < GameStartHandler.MaxPlayerCount; i++)
            {
                GameObject tmpGO = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
                if (tmpGO != null)
                {
                    tmpGO.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }

        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            Players[i].gameObject.transform.position = RespownPos[i];
            Sticks[i].gameObject.transform.position = RespownPos[i];
        }
    }
    void InstantiateObjects()
    {
        if (_sessionManager.PlayingType == "Online")
        {
            var position = RespownPos[Runner.LocalPlayer.PlayerId];

            if (Object.HasStateAuthority)
            {
                var playerPrefab = (GameObject)Resources.Load("Player" + (Runner.LocalPlayer.PlayerId + 1));
                if (playerPrefab != null)
                {
                    Runner.Spawn(playerPrefab, position, Quaternion.identity, Runner.LocalPlayer);
                }
            }

            if (Object.HasStateAuthority)
            {
                RPCSetStickPos();
            }

            if (_sessionManager.GameMode == "Arcade" && _sessionManager.Stage == 2 && Runner.LocalPlayer.PlayerId == _sessionManager.LeaderId)
            {
                if (Object.HasStateAuthority)
                {
                    var ballPrefab = (GameObject)Resources.Load("SoccerBall");
                    if (ballPrefab != null)
                    {
                        var ballObj = Runner.Spawn(ballPrefab, new Vector2(0, -2f), Quaternion.identity, Runner.LocalPlayer);

                        Transform parentTrans = GameObject.Find("Soccer").transform;
                        ballObj.transform.SetParent(parentTrans);
                        ballObj.name = "SoccerBall";
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                int PlayerId = i + 1;
                Players[i] = (GameObject)Resources.Load("Player" + PlayerId);
                Instantiate(Players[i], new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
                Players[i] = GameObject.Find("Player" + PlayerId + "(Clone)");
                Players[i].name = "Player" + PlayerId;
                Sticks[i] = Players[i].transform.Find("Stick").gameObject;
            }
            if (_sessionManager.PlayingType == "Multi" && _sessionManager.GameMode == "Arcade" && _sessionManager.Stage == 2)
            {
                Transform parentTrans = GameObject.Find("Soccer").GetComponent<Transform>().transform;
                GameObject ballObj = Instantiate(Resources.Load("SoccerBall") as GameObject, new Vector2(0, -2f), Quaternion.identity);
                ballObj.GetComponent<Transform>().transform.SetParent(parentTrans);
                ballObj.name = "SoccerBall";
            }
        }
    }

    private async UniTask GetPlayers()
    {
        if (_sessionManager.PlayingType != "Online")
        {
            return;
        }
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            int id = i + 1;
            bool isPlayerGot = false;
            while (!isPlayerGot)
            {
                if (GameObject.Find("Player" + id + "(Clone)") == true)
                {
                    Players[i] = GameObject.Find("Player" + id + "(Clone)");
                    Debug.Log("players[" + i + "]をPlayer" + id + "(Clone)に設定しました。");
                    Sticks[i] = Players[i].transform.Find("Stick").gameObject;
                    Debug.Log("Player" + id + "(Clone)が見つかりました");
                    isPlayerGot = true;
                }
                await UniTask.Delay(100);
            }

            if (Players[i].name != "Player" + id)
            {
                Players[i].name = "Player" + id;
                Debug.Log("players[" + i + "]の名前をPlayer" + id + "に設定しました。");
            }
        }
    }

    private async UniTask WaitUntilSetupEnds()
    {
        while (!IsSetupEnded)
        {
            IsSetupEnded = true;
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                if (Players[i] == null)
                {
                    IsSetupEnded = false;
                    await UniTask.Delay(1000);
                }
            }
        }
    }

    private async UniTask WaitUntilAllPlayersJoin()
    {
        if (_sessionManager.PlayingType != "Online")
        {
            AllJoin = true;
            return;
        }

        _sessionManager.IsReadyToStartTheGameToSend[Runner.LocalPlayer.PlayerId] = true;

        while (!AllJoin)
        {
            OpenInformationPanel().Forget();

            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                if (!_sessionManager.IsReadyToStartTheGame[i])
                {
                    Debug.Log("Player " + (i + 1) + " has not joined yet.");
                    AllJoin = false;

                    await UniTask.Delay(1000);
                }
            }

            AllJoin = true;
            break;
        }
    }

    private async UniTask StartTimer()
    {
        _countDownText.gameObject.SetActive(true);

        while (!IsCameraAnimationEnded)
        {
            await UniTask.Delay(500);
        }

        for (int i = 3; i > 0; i--)
        {
            _countDownText.text = i.ToString();
            SoundEffect.soundTrigger[3] = 1;
            await _countDownText.gameObject.transform.DOScale(1.2f, 1f);
            _countDownText.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        _countDownText.text = Constants.TextOnStart[Settings.languageNum];
        IsCountDownEnded = true;
        SoundEffect.soundTrigger[3] = 1;
        await _countDownText.DOFade(0, 1f);
        _countDownText.gameObject.SetActive(false);

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartTimer()
    {
        StartTimer().Forget();
    }

    void GameTimeManagement()
    {
        if (!IsCountDownEnded || GameRuleManager.IsGameEnded)
        {
            _topText.text = "";
            return;
        }

        if (!IsPaused || _sessionManager.PlayingType == "Online")
        {
            if (!GameRuleManager.IsTimeExpired && !GameRuleManager.isGoaled)
            {
                Playable = true;
            }

            ElapsedTime += Time.deltaTime;
            RemainingTime -= Time.deltaTime;
            Height = (int)GenerateStage.maxHeight;

            if ((_sessionManager.PlayingType == "Multi" || _sessionManager.PlayingType == "Online") && _sessionManager.PlayingType == "Arcade" && ElapsedTime > 0)
            {
                if (RemainingTime < 30)
                {
                    _topText.color = new Color32(255, 180, 0, 255); // 例: 赤色
                }
                _topText.text = (ElapsedTime > 99) ? Mathf.Floor(RemainingTime).ToString() : (Mathf.Floor(RemainingTime * 10) / 10).ToString();
            }
            else if (_sessionManager.PlayingType == "Single" && _sessionManager.GameMode == "Arcade")
            {
                _topText.text = Height.ToString() + "m";
            }
            if (_sessionManager.GameMode == "Nomal")
            {
                _topText.text = (ElapsedTime > 99) ? Mathf.Floor(ElapsedTime).ToString() : (Mathf.Floor(ElapsedTime * 10) / 10).ToString();
            }
        }
    }

    void IPlayerLeft.PlayerLeft(PlayerRef player)
    {
        if (Players[player.PlayerId])
        {
            Debug.Log("Player left: " + player);
            GameStartHandler.PlayerCount--;
            Players[player.PlayerId].SetActive(false);
        }
    }

    /// <summary>
    /// 操作ガイドの表示のコントロール
    /// </summary>
    void ToggleGuideUiActive()
    {
        if (Settings.guideMode == 1)
        {
            for (int i = 0; i < _controllerUI.Length; i++) { _controllerUI[i].gameObject.SetActive(false); }
            for (int i = 0; i < _keyBoardMouseUI.Length; i++) { _keyBoardMouseUI[i].gameObject.SetActive(false); }
            return;
        }

        if (!ControllerInputManager.usingController)
        {
            for (int i = 0; i < _keyBoardMouseUI.Length; i++) { _keyBoardMouseUI[i].gameObject.SetActive(true); }
            for (int i = 0; i < _controllerUI.Length; i++) { _controllerUI[i].gameObject.SetActive(false); }
        }
        else if (ControllerInputManager.usingController)
        {
            for (int i = 0; i < _keyBoardMouseUI.Length; i++) { _keyBoardMouseUI[i].gameObject.SetActive(false); }
            for (int i = 0; i < _controllerUI.Length; i++) { _controllerUI[i].gameObject.SetActive(true); }
        }

        _pauseButton.SetActive(!IsPaused);
    }

    private async UniTask OpenInformationPanel()
    {
        _quickStartingPanel.SetActive(true);

        GameObject[] iconObjects = new GameObject[4];
        Text[] playerNameTexts = new Text[4];

        for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = _quickStartingPanel.transform.GetChild(i).gameObject;
            playerNameTexts[i] = iconObjects[i].transform.GetChild(0).gameObject.GetComponent<Text>();
            playerNameTexts[i].text = i < GameStartHandler.PlayerCount ? _sessionManager.PlayerNames[i] : "";
            if (i >= GameStartHandler.PlayerCount)
            {
                iconObjects[i].gameObject.SetActive(false);
            }
        }

        GameObject imageFrame = _quickStartingPanel.transform.GetChild(4).gameObject;
        Image stageImg = imageFrame.transform.GetChild(0).gameObject.GetComponent<Image>();

        stageImg.sprite = Resources.Load<Sprite>("Multi" + _sessionManager.GameMode + _sessionManager.Stage);
        imageFrame.transform.GetChild(1).gameObject.GetComponent<TextSwicher>().num = _sessionManager.Stage;
        imageFrame.transform.GetChild(2).gameObject.GetComponent<TextSwicher>().num = _sessionManager.Stage;

        _quickStartingPanel.transform.GetChild(5).gameObject.GetComponent<Text>().text = Constants.TextOnWaiting[Settings.languageNum];

        while (true)
        {
            if (AllJoin)
            {
                break;
            }
            await UniTask.Delay(1000);
        }

        _quickStartingPanel.transform.GetChild(5).gameObject.GetComponent<Text>().text = Constants.TextOnStart[Settings.languageNum];

        await UniTask.Delay(1000);
    }
}
