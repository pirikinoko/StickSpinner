using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Photon.Pun;
public class GameSetting : MonoBehaviourPunCallbacks
{
    //基本
    [SerializeField] Text countDown, playTimeTx;
    [SerializeField] Text[] nameTagTexts;
    [SerializeField] GameObject canvas, frontCanvas, quickStartingPanel, pauseButton;
    [HideInInspector] public GameObject[] players = new GameObject[GameStart.maxPlayer];
    [HideInInspector] public GameObject[] sticks = new GameObject[GameStart.maxPlayer];
    [HideInInspector] public GameObject[] nameTags = new GameObject[GameStart.maxPlayer];
    [HideInInspector] public GameObject[] deadTimer = new GameObject[GameStart.maxPlayer];
    Vector2[] nameTagPos = new Vector2[GameStart.maxPlayer];
    Vector2[,] startPos = new Vector2[GameStart.maxPlayer, GameStart.maxPlayer];
    string[] startText = { "スタート", "Start" };
    public static bool Playable = false, allJoin = false, setupEnded = false;
    public static bool[] playerLeft = new bool[4];
    public bool isPaused = false;
    //タイム
    float elapsedTime;
    public static float playTime;
    public static float startTime = 6;
    float SoundTime = 1f;
    bool StartFlag, coroutineEnded;
    int startTrigger = 0;
    //ステージ切り替え用
    [SerializeField] GameObject[] stageObjectSingle, stageObjectSingleArcade, stageObjectMulti, stageObjectMultiArcade;
    [SerializeField] GameObject[] keyBoardMouseUI, controllerUI, battleModeUI;
    //背景色
    [SerializeField] Tilemap tilemap;
    public GameObject CountDownGO;
    //UI切り替え用
    int UIMode;
    const int KeyboardMode = 5;
    const int ControllerMode = 6;

    public static Vector2[] respownPos = new Vector2[GameStart.maxPlayer];
    GameObject[] defaultPosGO = new GameObject[GameStart.maxPlayer];

    SaveData data;
    private IngameLog ingameLog;

    [PunRPC]
    private void RPCSetStickPos()
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            int playerID = i + 1;
            if (GameObject.Find("Player" + playerID))
            {
                sticks[i] = GameObject.Find("Stick" + playerID.ToString());
                sticks[i].transform.position = GameObject.Find("Player" + playerID.ToString()).transform.position;
            }
        }

    }




    void Start()
    {
        if (GameStart.gameMode1 == "Online")
        {
            PhotonNetwork.IsMessageQueueRunning = true;
        }
        Debug.Log("Pnum == " + GameStart.PlayerNumber);
        startTrigger = 0;
        allJoin = false;
        setupEnded = false;
        isPaused = false;
        coroutineEnded = false;
        Playable = false;
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

        if (GameStart.gameMode1 == "Single")
        {
            quickStartingPanel.SetActive(false);
            playTime = 0;
            elapsedTime = 0;
            for (int i = 0; i < battleModeUI.Length; i++)
            {
                battleModeUI[i].gameObject.SetActive(false);
            }
            //モードごとの初期化
            switch (GameStart.gameMode2)
            {
                case "Nomal":
                    stageObjectSingle[GameStart.stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    break;
                case "Arcade":
                    stageObjectSingleArcade[GameStart.stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    playTimeTx.text = GenerateStage.maxHeight + "m";
                    break;
            }
        }
        else
        {
            tilemap.color = new Color32(50, 50, 50, 255);
            switch (GameStart.gameMode2)
            {
                case "Nomal":
                    stageObjectMulti[GameStart.stage - 1].gameObject.SetActive(true);
                    ingameLog.GenerateIngameBanner("ゲームモード:レース　 ゴールを目指そう");
                    break;

                case "Arcade":
                    stageObjectMultiArcade[GameStart.stage - 1].gameObject.SetActive(true);
                    playTime = GameStart.flagTimeLimit;
                    elapsedTime = GameStart.flagTimeLimit;
                    playTimeTx.text = GameStart.flagTimeLimit.ToString();
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
    void AfterAllJoin()
    {
        //プレイヤー生成
        if (GameStart.gameMode1 == "Online")
        {
            if (GameStart.gameMode2 != "Arcade")
            {
                for (int i = 0; i < battleModeUI.Length; i++)
                {
                    battleModeUI[i].gameObject.SetActive(false);
                }
            }
        }
        //マルチプレイ時のスポーン地点設定
        if (GameStart.gameMode1 != "Single")
        {
            int leftTeamCount = 0, rightTeamCount = 0;
            for (int i = 0; i < GameStart.maxPlayer; i++)
            {
                //サッカーモード時のスポーン地点設定
                if (GameStart.gameMode2 == "Arcade" && GameStart.stage == 2)
                {
                    if (GameStart.playerTeam[i] == 0)
                    {
                        defaultPosGO[i] = GameObject.Find("LeftDefaultPlayerPos" + (leftTeamCount + 1).ToString());
                        leftTeamCount++;
                    }
                    else if (GameStart.playerTeam[i] == 1)
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
        }
        //オンライン　ネットワークオブジェクトとしてプレイヤー生成
        if (GameStart.gameMode1 == "Online")
        {
            var position = respownPos[NetWorkMain.netWorkId - 1];
            PhotonNetwork.Instantiate("Player" + NetWorkMain.netWorkId, position, Quaternion.identity);
            photonView.RPC("RPCSetStickPos", RpcTarget.All);
            if(GameStart.gameMode2 == "Arcade" && GameStart.stage == 2 && NetWorkMain.netWorkId == NetWorkMain.leaderId)
            {
                Transform parentTrans = GameObject.Find("Soccer").GetComponent<Transform>().transform;
                GameObject ballObj = PhotonNetwork.Instantiate("SoccerBall", new Vector2(0, -2f), Quaternion.identity);
                ballObj.GetComponent<Transform>().transform.SetParent(parentTrans);
                ballObj.name = "SoccerBall";
            }
        }
        else //オフライン時のプレイヤー生成処理
        {
            for (int i = 0; i < 4; i++)
            {
                int PlayerId = i + 1;
                players[i] = (GameObject)Resources.Load("Player" + PlayerId);
                Instantiate(players[i], new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
                players[i] = GameObject.Find("Player" + PlayerId + "(Clone)");
                players[i].name = "Player" + PlayerId;
            }
            if (GameStart.gameMode1 == "Multi" && GameStart.gameMode2 == "Arcade" && GameStart.stage == 2) 
            {
                Transform parentTrans = GameObject.Find("Soccer").GetComponent<Transform>().transform;
                GameObject ballObj = Instantiate(Resources.Load("SoccerBall") as GameObject, new Vector2(0, -2f), Quaternion.identity);
                ballObj.GetComponent<Transform>().transform.SetParent(parentTrans);
                ballObj.name = "SoccerBall";
            }    
        }


        data = GetComponent<DataManager>().data;
        canvas.gameObject.SetActive(true);
        frontCanvas.gameObject.SetActive(true);
        playTimeTx.color = new Color32(255, 255, 255, 255); // 例: 赤色
        Debug.Log("PlayerNumber: " + GameStart.PlayerNumber + " stage: " + GameStart.stage);
        countDown = GameObject.Find("CountDown").GetComponent<Text>();
        playTimeTx = GameObject.Find("TimeText").GetComponent<Text>();
        playTimeTx.text = "";
        for (int i = 0; i < GameStart.maxPlayer; i++) 
        {
            nameTags[i].gameObject.SetActive(false);
            sticks[i] = GameObject.Find("Stick" + (i + 1).ToString());
            deadTimer[i] = GameObject.Find("P" + (i + 1).ToString() + "CountDown");
            deadTimer[i].SetActive(false);
        }

        SoundTime = 1f;
        startTime = 6;
        Playable = false;
        StartFlag = true;
        countDown.text = ("3");

        //プレイヤー人数の反映
        if (GameStart.gameMode1 != "Online")
        {

            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                nameTags[i].gameObject.SetActive(true);
                players[i].gameObject.SetActive(true);
                sticks[i].gameObject.SetActive(true);
                //初期位置
                players[i].gameObject.transform.position = respownPos[i];
                sticks[i].gameObject.transform.position = respownPos[i];
            }
            for (int i = 3; i >= GameStart.PlayerNumber; i--)
            {
                players[i].gameObject.SetActive(false);
                nameTags[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                nameTags[i].gameObject.SetActive(true);
            }
            photonView.RPC(nameof(GetPlayers), RpcTarget.All, NetWorkMain.netWorkId);
        }
        setupEnded = true;
    }
    [PunRPC]
    void GetPlayers(int id)
    {
        int i = id - 1;
        if (GameObject.Find("Player" + id + "(Clone)") == true)
        {
            players[i] = GameObject.Find("Player" + id + "(Clone)");
            Debug.Log("players[" + i + "]をPlayer" + id + "(Clone)に設定しました。");
            sticks[i] = GameObject.Find("Stick" + id);
        }
        if (players[i].name != "Player" + id)
        {
            players[i].name = "Player" + id;
            Debug.Log("players[" + i + "]の名前をPlayer" + id + "に設定しました。");
        }
    }
    void FixedUpdate()
    {
        if (!allJoin) { return; }
        NameTag();
    }

    void Update()
    {
        //プレイヤーのシーン遷移確認
        if (GameStart.gameMode1 == "Online") 
        {
            if (NetWorkMain.GetCustomProps<bool[]>("isJoined", out var ValueArrayA))
            {
                ValueArrayA[NetWorkMain.netWorkId - 1] = true;
                NetWorkMain.SetCustomProps<bool[]>("isJoined", ValueArrayA);
            }
        }

        if (!allJoin)
        {
            //全員ジョインするまでゲーム説明パネルを表示しておく
            StartCoroutine(OpenQuickPanel());
            if (GameStart.gameMode1 == "Online")
            {
                allJoin = true;
                if (NetWorkMain.GetCustomProps<bool[]>("isJoined", out var ValueArrayA)) 
                {
                    for (int i = 0; i < GameStart.PlayerNumber; i++)
                    {
                        //Debug.Log(isJoinedLocal[0] + "  " + isJoinedLocal[1]);
                        if (ValueArrayA[i] == false)
                        {
                            allJoin = false;
                        }
                    }           
                }
                else
                {
                    allJoin = false;
                    Debug.Log("isJoinedNotFound");
                }
            }
            else
            {
                allJoin = true;
                Debug.Log("NotOnline");
            }

        }

        if (GameStart.gameMode1 == "Online" && !allJoin)
        {
            return;
        }
        if (startTrigger == 0)
        {
            if (GameStart.gameMode1 == "Online")
            {
                if (!coroutineEnded) 
                {
                    return;
                }
            }
            quickStartingPanel.gameObject.SetActive(false);
            Debug.Log("StartInUpdateTriggered");
            AfterAllJoin();
            startTrigger = 1;
        }
        CheckPlayersLeft();
        SwichUI();
        StartTimer();

    }


    void StartTimer()
    {
        //3・2・1カウントダウン　→　スタート
        if (StartFlag)
        {
            startTime -= Time.deltaTime;
            SoundTime -= Time.deltaTime;
            if (startTime > 3)
            {
                countDown.text = null;
                return;
            }
            else if (startTime > 2)
            {
                CountDownGO.gameObject.SetActive(true);
                countDown.text = ("3");
            }
            else if (startTime > 1)
            {
                countDown.text = ("2");
            }
            else if (startTime > 0)
            {
                countDown.text = ("1");
            }
            else if (startTime < 0 && startTime > -0.5f)
            {
                countDown.text = startText[Settings.languageNum];
            }
            else if (startTime < 0.9f)
            {
                countDown.text = ("");
                CountDownGO.gameObject.SetActive(false);
                StartFlag = false;
            }
            if (SoundTime < 0)
            {
                SoundEffect.soundTrigger[3] = 1;
                SoundTime = 1;
            }
        }
        if (startTime < 0 && (!isPaused || GameStart.gameMode1 == "Online")) //ゲーム開始
        {

            if(!GameMode.Finished && !GameMode.Goaled) 
            {
                Playable = true;
            }

            if (playTime > 99)
            {
                playTime = elapsedTime;
                playTime = Mathf.Floor(playTime);
            }
            else
            {
                playTime = elapsedTime * 10;
                playTime = Mathf.Floor(playTime) / 10;
            }

            if ((GameStart.gameMode1 == "Multi" || GameStart.gameMode1 == "Online") && GameStart.gameMode2 == "Arcade" && playTime > 0)
            {
                elapsedTime -= Time.deltaTime;
                if (playTime < 30)
                {
                    playTimeTx.color = new Color32(255, 180, 0, 255); // 例: 赤色
                }
                playTimeTx.text = (playTime.ToString());
            }
            else if (GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Arcade") 
            {
                playTimeTx.text = (int)GenerateStage.maxHeight + "m";
            }
            else if (GameStart.gameMode2 != "Arcade")
            {
                elapsedTime += Time.deltaTime;
                playTimeTx.text = (playTime.ToString());
            }
            else { playTimeTx.text = "--"; }
        }
    }
    //他のプレイヤー切断時
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int disconnectedId = otherPlayer.ActorNumber;
        if (players[disconnectedId - 1] == true)
        {
            Debug.Log("Player left: " + otherPlayer.NickName);
            GameStart.PlayerNumber--;
            players[disconnectedId - 1].SetActive(false);
            nameTags[disconnectedId - 1].gameObject.SetActive(false);
        }
    }
    void NameTag()
    {

        //ネームタグの位置
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (players[i] == null)
            {
                return;
            }
            nameTagPos[i] = players[i].transform.position;
            nameTagPos[i].y += 0.5f;
            nameTags[i].transform.position = nameTagPos[i];
            if (GameStart.gameMode1 == "Online")
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
        if (!(ControllerInput.usingController))
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

    void CheckPlayersLeft()
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (playerLeft[i] == true)
            {
                Debug.Log("Player" + i + 1 + " has left");
                GameStart.PlayerNumber--;
                players[i].SetActive(false);
                nameTags[i].gameObject.SetActive(false);
                playerLeft[i] = false;
            }
        }
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
            if(i < GameStart.PlayerNumber) 
            {
                playerNameTexts[i].text = PhotonNetwork.PlayerList[i].NickName;
            }
            else { playerNameTexts[i].text = ""; }
            if (i >= GameStart.PlayerNumber)
            {
                iconObjects[i].gameObject.SetActive(false);
            }
        }
        GameObject imageFrame = quickStartingPanel.transform.GetChild(4).gameObject;
        Image stageImg = imageFrame.transform.GetChild(0).gameObject.GetComponent<Image>();
        stageImg.sprite = Resources.Load<Sprite>("Multi" + GameStart.gameMode2 + GameStart.stage);
        imageFrame.transform.GetChild(1).gameObject.GetComponent<TextSwicher>().num = GameStart.stage;
        imageFrame.transform.GetChild(2).gameObject.GetComponent<TextSwicher>().num = GameStart.stage;
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
