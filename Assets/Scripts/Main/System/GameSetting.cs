using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Text[] nameTagTexts ;
    [SerializeField] float timeLimit;
    [SerializeField] GameObject canvas, frontCanvas;
    [HideInInspector] public GameObject[] players = new GameObject[GameStart.maxPlayer];
    [HideInInspector] public GameObject[] sticks = new GameObject[GameStart.maxPlayer];
    [HideInInspector] public GameObject[] nameTags = new GameObject[GameStart.maxPlayer];
    [HideInInspector] public GameObject[] deadTimer = new GameObject[GameStart.maxPlayer];
    Vector2[] nameTagPos = new Vector2[GameStart.maxPlayer];
    Vector2[,] startPos = new Vector2[GameStart.maxPlayer, GameStart.maxPlayer];
    string[] startText = { "スタート", "Start" };
    public static bool Playable = false, allJoin = false;
    public static bool[] playerLeft = new bool[4];
    //タイム
    float elapsedTime;
    public static float playTime;
    public static float startTime = 6;
    float SoundTime = 1f;
    bool StartFlag;
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
    GameObject[] defaultPlayerPos = new GameObject[GameStart.maxPlayer];

    SaveData data;
    private IngameLog ingameLog = new IngameLog();

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

    [PunRPC]
    private void AllJoin()
    {
        allJoin = true;
    }



    void Start()
    {
        Debug.Log("Pnum == " + GameStart.PlayerNumber);
       startTrigger = 0;
        allJoin = false;
        for (int i = 0; i < 4; i++)
        {
            playerLeft[i] = false;
            nameTags[i] = GameObject.Find("P" + (i + 1).ToString() + "NameTag");
            if(nameTags[i] != null)
            {
                Debug.Log("nameTag " + i + "= true");
            }
        }
        //ステージ切り替え
        for (int i = 0; i < stageObjectSingle.Length; i++)
        {
            stageObjectSingle[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < stageObjectMulti.Length; i++)
        {
            stageObjectMulti[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < stageObjectSingleArcade.Length; i++)
        {
            stageObjectSingleArcade[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < stageObjectMultiArcade.Length; i++)
        {
            stageObjectMultiArcade[i].gameObject.SetActive(false);
        }
        if (GameStart.gameMode1 == "Single")
        {
            switch (GameStart.gameMode2)
            {
                case "Nomal":
                    stageObjectSingle[GameStart.Stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    playTime = 0;
                    elapsedTime = 0;
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(false);
                    }
                    break;
                case "Arcade":
                    stageObjectSingleArcade[GameStart.Stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    playTime = 0;
                    elapsedTime = 0;
                    playTimeTx.text = GenerateStage.maxHeight + "m";
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(false);
                    }
                    break;
            }
        }
        else
        {
            tilemap.color = new Color32(50, 50, 50, 255);
            switch (GameStart.gameMode2)
            {
                case "Nomal":
                    stageObjectMulti[GameStart.Stage - 1].gameObject.SetActive(true);
                    playTime = 0;
                    elapsedTime = 0;
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(false);
                    }
                    ingameLog.GenerateIngameBanner("ゲームモード:レース　 ゴールを目指そう");
                    break;

                case "Arcade":
                    stageObjectMultiArcade[GameStart.Stage - 1].gameObject.SetActive(true);
                    playTimeTx.text = timeLimit.ToString();
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
     void AfterAllJoin()
    {
        //プレイヤー生成
        if (GameStart.gameMode1 == "Online")
            {
            if(GameStart.gameMode2 != "Arcade")
            {
                for (int i = 0; i < battleModeUI.Length; i++)
                {
                    battleModeUI[i].gameObject.SetActive(false);
                }
            }
                
                for (int i = 0; i < GameStart.maxPlayer; i++)
                {
                    defaultPlayerPos[i] = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
                    respownPos[i] = defaultPlayerPos[i].gameObject.transform.position;
                    defaultPlayerPos[i].gameObject.SetActive(false);
                }
            }
            //オンライン　ネットワークオブジェクトとしてプレイヤー生成
            if (GameStart.gameMode1 == "Online")
            {
                var position = respownPos[NetWorkMain.netWorkId - 1];
                PhotonNetwork.Instantiate("Player" + NetWorkMain.netWorkId, position, Quaternion.identity);
                photonView.RPC("RPCSetStickPos", RpcTarget.All);
            }
            else //オフライン
            {
                for (int i = 0; i < 4; i++)
                {
                    int PlayerId = i + 1;
                    players[i] = (GameObject)Resources.Load("Player" + PlayerId);
                    Instantiate(players[i], new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
                    players[i] = GameObject.Find("Player" + PlayerId + "(Clone)");
                    players[i].name = "Player" + PlayerId;
                }
            }


            data = GetComponent<DataManager>().data;
            canvas.gameObject.SetActive(true);
            frontCanvas.gameObject.SetActive(true);
            playTimeTx.color = new Color32(255, 255, 255, 255); // 例: 赤色
            Debug.Log("PlayerNumber: " + GameStart.PlayerNumber + " Stage: " + GameStart.Stage);
            countDown = GameObject.Find("CountDown").GetComponent<Text>();
            playTimeTx = GameObject.Find("TimeText").GetComponent<Text>();
            playTimeTx.text = "";
        for (int i = 0; i < GameStart.maxPlayer; i++) //初期化処理
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

        // リスポーン位置
        if (GameStart.gameMode1 == "Multi")
        {
            for (int i = 0; i < GameStart.maxPlayer; i++)
            {
                defaultPlayerPos[i] = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
                respownPos[i] = defaultPlayerPos[i].gameObject.transform.position;
                defaultPlayerPos[i].gameObject.SetActive(false);
            }
        }
        else if (GameStart.gameMode1 == "Single")
        {
            defaultPlayerPos[0] = GameObject.Find("DefaultPlayerPos1");
            respownPos[0] = defaultPlayerPos[0].gameObject.transform.position;
            defaultPlayerPos[0].gameObject.SetActive(false);
        }


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
            for (int i = 0; i  < GameStart.PlayerNumber; i++)
            {
                nameTags[i].gameObject.SetActive(true);
            }
            photonView.RPC(nameof(GetPlayers), RpcTarget.All, NetWorkMain.netWorkId);
        }
       
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
        if (!allJoin) 
        {
            if (GameStart.gameMode1 == "Online")
            {
                ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
                if (customProps.ContainsKey("isJoined"))
                {
                    bool[] isJoinedLocal = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["isJoined"];
                    if (isJoinedLocal[NetWorkMain.netWorkId - 1] == false) 
                    {
                        isJoinedLocal[NetWorkMain.netWorkId - 1] = true;
                        customProps["isJoined"] = isJoinedLocal;
                        Debug.Log("Player" + NetWorkMain.netWorkId + "が接続しました");
                    }    
                }
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);


                allJoin = true;

                if (customProps.ContainsKey("isJoined"))
                {
                    for (int i = 0; i < GameStart.PlayerNumber; i++)
                    {
                        // カスタムプロパティを取得

                        bool[] isJoinedLocal = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["isJoined"];
                        //Debug.Log(isJoinedLocal[0] + "  " + isJoinedLocal[1]);
                        if (isJoinedLocal[i] == false)
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
        if(startTrigger == 0) 
        {
            if(GameStart.gameMode1 == "Online") { photonView.RPC(nameof(AllJoin), RpcTarget.All); }
            Debug.Log("StartInUpdateTriggered");

            AfterAllJoin();
            startTrigger = 1;
        }
        CheckPlayersLeft();
        SwichUI();
        StartTimer();
        // data.languageNum = Settings.languageNum;
        // data.BGM = BGM.BGMStage;
        // data.SE = SoundEffect.SEStage;

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
        if (startTime < 0 && (ButtonInGame.Paused != 1 || GameStart.gameMode1 == "Online")) //ゲーム開始
        {

            Playable = true;
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
            else if (GameStart.gameMode2 != "Arcade")
            {
                elapsedTime += Time.deltaTime;
                playTimeTx.text = (playTime.ToString());
            }
            else
            {
                playTimeTx.text = (int)GenerateStage.maxHeight + "m";
            }
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
            if(GameStart.gameMode1 == "Online") 
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

        //キーボードマウス用UIとコントローラー用UIの切り替え

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
    }

    void CheckPlayersLeft() 
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if(playerLeft[i] == true) 
            {
                Debug.Log("Player" + i + 1 + " has left");
                GameStart.PlayerNumber--;
                players[i].SetActive(false);
                nameTags[i].gameObject.SetActive(false);
                playerLeft[i] = false;
            }
        }
    }
}
