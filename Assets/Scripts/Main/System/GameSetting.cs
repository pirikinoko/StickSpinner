using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using ExitGames.Client.Photon;
using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameSetting : NetworkBehaviour
{
    //基本
    [SerializeField]
    Text countDownText;

    [SerializeField]
    Text playTimeTx;

    [SerializeField]
    Text[] nameTagTexts;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject frontCanvas;

    [SerializeField]
    GameObject worldCanvas;

    [SerializeField]
    GameObject quickStartingPanel;

    [SerializeField]
    GameObject pauseButton;

    [SerializeField]
    CameraControl cameraControl;

    [HideInInspector]
    public GameObject[]? players = new GameObject[GameStartHandler.MaxPlayer];

    [HideInInspector]
    public GameObject[] sticks = new GameObject[GameStartHandler.MaxPlayer];

    [HideInInspector]
    public GameObject[] nameTags = new GameObject[GameStartHandler.MaxPlayer];

    [HideInInspector]
    public GameObject[] deadTimer = new GameObject[GameStartHandler.MaxPlayer];

    Vector2[] nameTagPos = new Vector2[GameStartHandler.MaxPlayer];
    Vector2[,] startPos = new Vector2[GameStartHandler.MaxPlayer, GameStartHandler.MaxPlayer];

    string[] startText = { "スタート", "Start" };

    public static bool Playable = false;
    public static bool allJoin = false;
    public static bool setupEnded = false;

    public static bool[] playerLeft = new bool[4];

    public bool isPaused = false;

    //タイム
    float elapsedTime;
    public bool isCountDownEnded;
    public static float playTime;
    public const byte StartTimerEventCode = 1;
    bool coroutineEnded;
    bool gameStartMethodInvoked = false;
    bool startTimerInvoked = false;

    //ステージ切り替え用
    [SerializeField]
    GameObject[] stageObjectSingle;

    [SerializeField]
    GameObject[] stageObjectSingleArcade;

    [SerializeField]
    GameObject[] stageObjectMulti;

    [SerializeField]
    GameObject[] stageObjectMultiArcade;

    [SerializeField]
    GameObject[] keyBoardMouseUI;

    [SerializeField]
    GameObject[] controllerUI;

    [SerializeField]
    GameObject[] battleModeUI;

    //背景色
    [SerializeField]
    Tilemap tilemap;

    public GameObject CountDownGO;

    //UI切り替え用
    int UIMode;
    const int KeyboardMode = 5;
    const int ControllerMode = 6;

    public static Vector2[] respownPos = new Vector2[GameStartHandler.MaxPlayer];
    GameObject[] defaultPosGO = new GameObject[GameStartHandler.MaxPlayer];

    SaveData data;
    private IngameLog ingameLog;


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPCSetStickPos()
    {
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            int playerID = i + 1;
            if (GameObject.Find("Player" + playerID))
            {
                sticks[i] = GameObject.Find("Stick" + playerID.ToString());
                sticks[i].transform.position = GameObject.Find("Player" + playerID.ToString()).transform.position;
            }
        }
    }

    void Awake()
    {
        gameStartMethodInvoked = false;
        allJoin = false;
        setupEnded = false;
        isPaused = false;
        coroutineEnded = false;
        Playable = false;
        startTimerInvoked = false;
        isCountDownEnded = false;
        countDownText.text = null;
        canvas.gameObject.SetActive(true);
        frontCanvas.gameObject.SetActive(true);
        worldCanvas.gameObject.SetActive(true);
        ingameLog = GameObject.Find("Scripts").GetComponent<IngameLog>();

        for (int i = 0; i < 4; i++)
        {
            playerLeft[i] = false;
            nameTags[i] = GameObject.Find("P" + (i + 1).ToString() + "NameTag");
        }

        //ステージを非アクティブ化
        new[] { stageObjectSingle, stageObjectMulti, stageObjectSingleArcade, stageObjectMultiArcade }
                  .SelectMany(array => array)
                  .ToList()
                  .ForEach(obj => obj.gameObject.SetActive(false));

        if (SingletonSettingCanvas.Instance == null)
        {
            Instantiate((GameObject)Resources.Load("SettingCanvas"));
        }

        //ステージのアクティブ管理
        if (GameStartHandler.GameMode1 == "Single")
        {
            quickStartingPanel.SetActive(false);
            playTime = 0;
            elapsedTime = 0;
            for (int i = 0; i < battleModeUI.Length; i++)
            {
                battleModeUI[i].gameObject.SetActive(false);
            }
            //モードごとの初期化
            switch (GameStartHandler.GameMode2)
            {
                case "Nomal":
                    stageObjectSingle[GameStartHandler.Stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    break;
                case "Arcade":
                    stageObjectSingleArcade[GameStartHandler.Stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    playTimeTx.text = GenerateStage.maxHeight + "m";
                    break;
            }
        }
        else
        {
            tilemap.color = new Color32(50, 50, 50, 255);
            switch (GameStartHandler.GameMode2)
            {
                case "Nomal":
                    stageObjectMulti[GameStartHandler.Stage - 1].gameObject.SetActive(true);
                    ingameLog.GenerateIngameBanner("ゲームモード:レース　 ゴールを目指そう");
                    break;

                case "Arcade":
                    stageObjectMultiArcade[GameStartHandler.Stage - 1].gameObject.SetActive(true);
                    playTime = GameStartHandler.FlagTimeLimit;
                    elapsedTime = GameStartHandler.FlagTimeLimit;
                    playTimeTx.text = GameStartHandler.FlagTimeLimit.ToString();
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(true);
                    }
                    ingameLog.GenerateIngameBanner("ゲームモード:フラッグ　旗を占拠しよう");
                    break;

            }
        }
    }
    //すべてのプレイヤーがそろってから行う処理
    private async UniTask AfterAllJoin()
    {
        //プレイヤー生成
        if (GameStartHandler.GameMode1 == "Online")
        {
            if (GameStartHandler.GameMode2 != "Arcade")
            {
                for (int i = 0; i < battleModeUI.Length; i++)
                {
                    battleModeUI[i].gameObject.SetActive(false);
                }
            }
        }
        SetRespownPos();
        InstantiatePlayers();
        await GetPlayers();
        InitializePlayers();
        data = DataManager.Instance.data;
        playTimeTx.color = new Color32(255, 255, 255, 255);
        playTimeTx = GameObject.Find("TimeText").GetComponent<Text>();
        playTimeTx.text = "";

        Playable = false;
    }

    void SetRespownPos()
    {
        //マルチプレイ時のスポーン地点設定
        if (GameStartHandler.GameMode1 != "Single")
        {
            int leftTeamCount = 0, rightTeamCount = 0;
            for (int i = 0; i < GameStartHandler.MaxPlayer; i++)
            {
                //サッカーモード時のスポーン地点設定
                if (GameStartHandler.GameMode2 == "Arcade" && GameStartHandler.Stage == 2)
                {
                    if (GameStartHandler.PlayerTeam[i] == 0)
                    {
                        defaultPosGO[i] = GameObject.Find("LeftDefaultPlayerPos" + (leftTeamCount + 1).ToString());
                        leftTeamCount++;
                    }
                    else if (GameStartHandler.PlayerTeam[i] == 1)
                    {
                        defaultPosGO[i] = GameObject.Find("RightDefaultPlayerPos" + (rightTeamCount + 1).ToString());
                        rightTeamCount++;
                    }
                    else
                    {
                        defaultPosGO[i] = GameObject.Find("RightDefaultPlayerPos1");
                    }
                }
                //サッカーモード以外のスポーン地点設定
                else
                {
                    defaultPosGO[i] = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
                    defaultPosGO[i].GetComponent<SpriteRenderer>().enabled = false;
                }
                respownPos[i] = defaultPosGO[i].gameObject.transform.position;
            }
        }
        //シングルプレイ時のスポーン地点設定
        else
        {
            defaultPosGO[0] = GameObject.Find("DefaultPlayerPos1");
            respownPos[0] = defaultPosGO[0].gameObject.transform.position;
            defaultPosGO[0].GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < GameStartHandler.MaxPlayer; i++)
            {
                GameObject tmpGO = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
                if (tmpGO != null)
                {
                    tmpGO.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }
    void InstantiatePlayers()
    {
        //オンライン　ネットワークオブジェクトとしてプレイヤー生成
        if (GameStartHandler.GameMode1 == "Online")
        {
            var position = respownPos[NetWorkMain.NetWorkId - 1];

            // プレイヤーの生成
            if (Object.HasStateAuthority) // StateAuthorityを持つ側で生成
            {
                // プレイヤーPrefabを事前にNetworkPrefabリストに登録しておく必要があります
                var playerPrefab = (GameObject)Resources.Load("Player" + NetWorkMain.NetWorkId);
                if (playerPrefab != null)
                {
                    Runner.Spawn(playerPrefab, position, Quaternion.identity, Runner.LocalPlayer);
                }
            }

            // スティック位置設定用のRPC呼び出し
            if (Object.HasStateAuthority) // StateAuthorityがある場合にのみ送信
            {
                RPCSetStickPos();
            }

            // サッカーボール生成 (ゲームモードとステージ条件付き)
            if (GameStartHandler.GameMode2 == "Arcade" && GameStartHandler.Stage == 2 && NetWorkMain.NetWorkId == NetWorkMain.leaderId)
            {
                if (Object.HasStateAuthority) // StateAuthorityを持つ場合に生成
                {
                    var ballPrefab = (GameObject)Resources.Load("SoccerBall");
                    if (ballPrefab != null)
                    {
                        var ballObj = Runner.Spawn(ballPrefab, new Vector2(0, -2f), Quaternion.identity, Runner.LocalPlayer);

                        // 親の設定
                        Transform parentTrans = GameObject.Find("Soccer").transform;
                        ballObj.transform.SetParent(parentTrans);
                        ballObj.name = "SoccerBall";
                    }
                }
            }

        }
        //オフライン時のプレイヤー生成処理
        else
        {
            for (int i = 0; i < 4; i++)
            {
                int PlayerId = i + 1;
                players[i] = (GameObject)Resources.Load("Player" + PlayerId);
                Instantiate(players[i], new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
                players[i] = GameObject.Find("Player" + PlayerId + "(Clone)");
                players[i].name = "Player" + PlayerId;
                sticks[i] = GameObject.Find("Stick" + PlayerId.ToString());
            }
            if (GameStartHandler.GameMode1 == "Multi" && GameStartHandler.GameMode2 == "Arcade" && GameStartHandler.Stage == 2)
            {
                Transform parentTrans = GameObject.Find("Soccer").GetComponent<Transform>().transform;
                GameObject ballObj = Instantiate(Resources.Load("SoccerBall") as GameObject, new Vector2(0, -2f), Quaternion.identity);
                ballObj.GetComponent<Transform>().transform.SetParent(parentTrans);
                ballObj.name = "SoccerBall";
            }
        }
    }

    void InitializePlayers()
    {
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            nameTags[i].gameObject.SetActive(true);
            players[i].gameObject.transform.position = respownPos[i];
            sticks[i].gameObject.transform.position = respownPos[i];
        }
        for (int i = GameStartHandler.MaxPlayer - 1; i >= GameStartHandler.PlayerCount; i--)
        {
            nameTags[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < GameStartHandler.MaxPlayer; i++)
        {
            deadTimer[i] = GameObject.Find("P" + (i + 1).ToString() + "CountDown");
            deadTimer[i].SetActive(false);
        }
    }
    private async UniTask GetPlayers()
    {
        if (GameStartHandler.GameMode1 != "Online")
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
                    players[i] = GameObject.Find("Player" + id + "(Clone)");
                    Debug.Log("players[" + i + "]をPlayer" + id + "(Clone)に設定しました。");
                    sticks[i] = GameObject.Find("Stick" + id);
                    Debug.Log("Player" + id + "(Clone)が見つかりました");
                    isPlayerGot = true;
                }
                await UniTask.Delay(100);
            }

            if (players[i].name != "Player" + id)
            {
                players[i].name = "Player" + id;
                Debug.Log("players[" + i + "]の名前をPlayer" + id + "に設定しました。");
            }
        }


    }
    void CheckIsSetUpEnded()
    {
        setupEnded = true;
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            if (players[i] == null)
            {
                setupEnded = false;
            }
        }
    }
    void FixedUpdate()
    {
        if (!allJoin) { return; }
        MoveNameTag();
    }

    private async UniTask Update()
    {
        CheckAllPlayersJoined();
        if (!setupEnded)
        {
            CheckIsSetUpEnded();
        }
        if (GameStartHandler.GameMode1 == "Online" && !allJoin)
        {
            return;
        }
        if (gameStartMethodInvoked == false)
        {
            if (GameStartHandler.GameMode1 == "Online")
            {
                if (!coroutineEnded)
                {
                    return;
                }
            }
            quickStartingPanel.gameObject.SetActive(false);
            gameStartMethodInvoked = true;
            await AfterAllJoin();
        }
        if (cameraControl.isFirstAnimationEnded && !startTimerInvoked)
        {
            if (GameStartHandler.GameMode1 == "Online")
            {
                if (Runner.IsServer)
                {
                    RaiseStartTimerEvent();
                }
                startTimerInvoked = true;
                return;
            }
            startTimerInvoked = true;
            await StartTimer();
        }
        // CheckPlayersLeft();
        SwichUI();
        GameTimeManagement();
        CheckPlayersLeft();
    }

    private void CheckAllPlayersJoined()
    {
        //プレイヤーのシーン遷移確認
        if (GameStartHandler.GameMode1 == "Online")
        {
            if (NetWorkMain.GetCustomProps<bool[]>("isJoined", out var ValueArrayA))
            {
                ValueArrayA[NetWorkMain.NetWorkId - 1] = true;
                NetWorkMain.SetCustomProps<bool[]>("isJoined", ValueArrayA);
            }
        }

        if (GameStartHandler.GameMode1 != "Online")
        {
            allJoin = true;
            return;
        }

        if (!allJoin)
        {
            StartCoroutine(OpenQuickPanel());
            if (NetWorkMain.GetCustomProps<bool[]>("isJoined", out var ValueArrayA))
            {
                for (int i = 0; i < GameStartHandler.PlayerCount; i++)
                {
                    if (!ValueArrayA[i])
                    {
                        Debug.LogWarning("Player " + (i + 1) + " has not joined yet.");
                        allJoin = false;
                        return;
                    }
                }
                allJoin = true;
            }
            else
            {
                Debug.LogError("isJoined プロパティが見つかりません。");
                allJoin = false;
            }
        }
    }
    private async UniTask StartTimer()
    {
        countDownText.text = null;
        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();
            SoundEffect.soundTrigger[3] = 1;
            CountDownGO.gameObject.transform.DOScale(1.2f, 1f);
            await UniTask.Delay(1000);
        }
        countDownText.DOFade(0, 1f);
        countDownText.text = startText[Settings.languageNum];
        SoundEffect.soundTrigger[3] = 1;
        isCountDownEnded = true;
        await UniTask.Delay(1000);
        CountDownGO.gameObject.SetActive(false);
    }

    void GameTimeManagement()
    {
        if (!isCountDownEnded || GameMode.isGameEnded)
        {
            playTimeTx.text = "";
            return;
        }

        if (!isPaused || GameStartHandler.GameMode1 == "Online")
        {
            if (!GameMode.isTimeFinished && !GameMode.isGoaled)
            {
                Playable = true;
            }

            playTime = (playTime > 99) ? Mathf.Floor(elapsedTime) : Mathf.Floor(elapsedTime * 10) / 10;

            //それぞれのモードごとの時間の形式を設定
            if ((GameStartHandler.GameMode1 == "Multi" || GameStartHandler.GameMode1 == "Online") && GameStartHandler.GameMode2 == "Arcade" && playTime > 0)
            {
                elapsedTime -= Time.deltaTime;
                if (playTime < 30)
                {
                    playTimeTx.color = new Color32(255, 180, 0, 255); // 例: 赤色
                }
                playTimeTx.text = playTime.ToString();
            }

            if (GameStartHandler.GameMode1 == "Single" && GameStartHandler.GameMode2 == "Arcade")
            {
                playTimeTx.text = (int)GenerateStage.maxHeight + "m";
            }

            if (GameStartHandler.GameMode2 == "Nomal")
            {
                elapsedTime += Time.deltaTime;
                playTimeTx.text = playTime.ToString();
            }
        }
    }
    public override void OnPlayerLeft(PlayerRef player)
    {
        Debug.Log("Player left: " + player);
        int disconnectedId = player.PlayerId;
        if (players[disconnectedId - 1] == true)
        {
            Debug.Log("Player left: " + player);
            GameStartHandler.PlayerCount--;
            players[disconnectedId - 1].SetActive(false);
            nameTags[disconnectedId - 1].gameObject.SetActive(false);
        }
    }

    private void RaiseStartTimerEvent()
    {
        PhotonNetwork.RaiseEvent(StartTimerEventCode, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == StartTimerEventCode)
        {
            // StartTimerイベントを受信
            StartCoroutine(StartTimerCoroutine());
        }
    }

    private IEnumerator StartTimerCoroutine()
    {
        yield return StartTimer();
    }

    void CheckPlayersLeft()
    {
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            if (playerLeft[i] == true)
            {
                Debug.Log("Player" + i + 1 + " has left");
                GameStartHandler.PlayerCount--;
                players[i].SetActive(false);
                nameTags[i].gameObject.SetActive(false);
                playerLeft[i] = false;
            }
        }
    }

    public void CallDeletePlayerRPC()
    {
        DeleatPlayer(NetWorkMain.NetWorkId);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void DeleatPlayer(int id)
    {
        Debug.Log("DeleatPlayerOccored");
        GameSetting.playerLeft[id - 1] = true;
    }

    void MoveNameTag()
    {

        //ネームタグの位置
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            if (players[i] == null)
            {
                return;
            }
            nameTagPos[i] = players[i].transform.position;
            nameTagPos[i].y += 0.5f;
            nameTags[i].transform.position = nameTagPos[i];
            if (GameStartHandler.GameMode1 == "Online")
            {
                nameTagTexts[i].text = NetWorkMain.playerNames[i];
            }
            else
            {
                int playerId = i + 1;
                string[] playerText = { "プレイヤー", "Player" };
                nameTagTexts[i].text = playerText[Settings.languageNum] + playerId;
            }
        }
    }

    void SwichUI()
    {
        //UI非表示設定時
        if (Settings.guideMode == 1)
        {
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
            for (int i = 0; i < keyBoardMouseUI.Length; i++) { keyBoardMouseUI[i].gameObject.SetActive(false); }
            return;
        }

        //キーボード,マウスのとき
        if (!ControllerInput.usingController)
        {
            for (int i = 0; i < keyBoardMouseUI.Length; i++) { keyBoardMouseUI[i].gameObject.SetActive(true); }
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {
            for (int i = 0; i < keyBoardMouseUI.Length; i++) { keyBoardMouseUI[i].gameObject.SetActive(false); }
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(true); }
        }

        //ポーズボタン表示
        pauseButton.SetActive(!isPaused);
    }



    IEnumerator OpenQuickPanel()
    {
        quickStartingPanel.SetActive(true);
        GameObject[] iconObjects = new GameObject[4];
        Text[] playerNameTexts = new Text[4];
        for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = quickStartingPanel.transform.GetChild(i).gameObject;
            playerNameTexts[i] = iconObjects[i].transform.GetChild(0).gameObject.GetComponent<Text>();
            playerNameTexts[i].text = i < GameStartHandler.PlayerCount ? PhotonNetwork.PlayerList[i].NickName : "";
            if (i >= GameStartHandler.PlayerCount)
            {
                iconObjects[i].gameObject.SetActive(false);
            }
        }
        GameObject imageFrame = quickStartingPanel.transform.GetChild(4).gameObject;
        Image stageImg = imageFrame.transform.GetChild(0).gameObject.GetComponent<Image>();
        stageImg.sprite = Resources.Load<Sprite>("Multi" + GameStartHandler.GameMode2 + GameStartHandler.Stage);
        imageFrame.transform.GetChild(1).gameObject.GetComponent<TextSwicher>().num = GameStartHandler.Stage;
        imageFrame.transform.GetChild(2).gameObject.GetComponent<TextSwicher>().num = GameStartHandler.Stage;
        string[] texts1 = { "プレイヤーを待っています", "Waiting For Players" };
        quickStartingPanel.transform.GetChild(5).gameObject.GetComponent<Text>().text = texts1[Settings.languageNum];
        while (true)
        {
            if (allJoin)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        string[] texts2 = { "ゲームを開始します", "Starting" };
        quickStartingPanel.transform.GetChild(5).gameObject.GetComponent<Text>().text = texts2[Settings.languageNum];
        yield return new WaitForSeconds(1.0f);
        coroutineEnded = true;
    }
}
