using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Linq;
using Photon.Pun;

public class GameStart : MonoBehaviourPunCallbacks
{
    [SerializeField] int minFlagTime, maxFlagTime;
    public int maxStageNomal;     // 総ステージ数
    float difficultyl;
    float timeFromLastAction;
    float cycle = 0.3f;
    public GameObject mainTitle, startPanel, changePlayerNumber, stageSelect, selectGameMode, setArcadeGame, keyBoardMouseUI, selectOnlineLobby, onlineLobby, loadScreen, cursor;
    public GameObject[] controllerUI, playerIcon, playerSlot;
    IngameLog ingameLog;
    //チーム選択
    public Vector2[] playerIconPos { get; set; } = new Vector2[4];
    public Vector2[] slot1Pos = new Vector2[4];

    int lastPlayerNum;
    int lastPhase;
    public Text playerNumberText, stageNumberText, flagTimeLimitTx;
    //画像
    public Image stageImage;
    private Sprite imageSprite;
    //テキスト
    string[] singleArcadeText = { "無限の塔", "InfinityTower", };
    string[] MultiArcadeText = { "旗取りバトル", "FlagBattle", "サッカー", "FootBall", };
    //static変数
    public static string gameMode1 = "Single";
    public static string gameMode2 = "Nomal";
    public static string teamMode = "FreeForAll"; 
    public static int phase = 0;
    public static int PlayerNumber { get; set; } = 1;   
    public static int stage = 1;
    public static int flagTimeLimit = 90;
    public static int[] playerTeam { get; set; } = { 0, 1, 2, 3 }; // {p1, p2, p3, p4}が TeamA, TeamB, TeamC, TeamDにいることを示す。ex..a = 1, c =3
    public static int[] teamSize = new int[4];
    public static int teamCount = 0;
    public static int maxPlayer = 4;
    public static int minPlayer;
    public static bool buttonPushable = true;

    //ロード画面
    bool reconnectable, joinedLobby = false;

    private void Awake()
    {
        if (SingletonSettingCanvas.Instance == null)
        {
            Instantiate(Resources.Load("SettingCanvas"));
        }
    }
    void Start()
    {

        ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();
        Time.timeScale = 1;
        GameSetting.Playable = false;
        reconnectable = false;
        stage = 1;
        PlayerNumber = 1;
        teamMode = "FFA";
        phase = 0;
        lastPhase = -1;
        flagTimeLimit = 90;
        for (int i = 0; i < 4; i++)
        {
            playerIconPos[i] = slot1Pos[i];
            playerTeam[i] = i;
            teamSize[i] = 0;
        }
        //オンラインでルームにいる場合はルーム画面に行く
        if (gameMode1 == "Online" && PhotonNetwork.InRoom)
        {
            phase = 3;
        }
    }
    void Update()
    {
        SwichUI();
        SwichStageMaterial();
        playerNumberText.text = PlayerNumber.ToString();

        if(timeFromLastAction > cycle) 
        {
            PhaseControll();
            buttonPushable = true;
            timeFromLastAction = 0;
        }

        //上限下限の設定
        phase = Mathf.Clamp(phase, 0, 9);
        if (gameMode1 == "Single" && gameMode2 == "Nomal")
        {
            stage = Mathf.Clamp(stage, 1, 7);
        }
        else if (gameMode1 == "Single" && gameMode2 == "Arcade")
        {
            stage = Mathf.Clamp(stage, 1, 1);
        }
        else if ((gameMode1 == "Multi" || gameMode1 == "Online") && gameMode2 == "Nomal")
        {
            stage = Mathf.Clamp(stage, 1, 4);
        }
        else if ((gameMode1 == "Multi" || gameMode1 == "Online") && gameMode2 == "Arcade")
        {
            stage = Mathf.Clamp(stage, 1, 2);
        }

        timeFromLastAction += Time.deltaTime;
        //プレイヤー数制限
        PlayerNumber = Mathf.Clamp(PlayerNumber, minPlayer, maxPlayer);
        //フラッグモード時間範囲
        flagTimeLimit = Mathf.Clamp(flagTimeLimit, minFlagTime, maxFlagTime);
    }



    void SwichStageMaterial() //選択ステージ毎に情報切り替え
    {
        switch (gameMode1)
        {
            case "Single":
                stageNumberText.text = (gameMode2 == "Arcade") ? singleArcadeText[Settings.languageNum] : "Stage" + stage.ToString();
                imageSprite = Resources.Load<Sprite>(gameMode1 + gameMode2 + stage);
                break;
            case "Multi":
                stageNumberText.text = (gameMode2 == "Arcade") ? MultiArcadeText[stage + (2 * Settings.languageNum)] : "Stage" + stage.ToString();
                imageSprite = Resources.Load<Sprite>(gameMode1 + gameMode2 + stage);
                break;
            case "Online":
                stageNumberText.text = (gameMode2 == "Arcade") ? MultiArcadeText[stage + (2 * Settings.languageNum)] : "Stage" + stage.ToString();
                imageSprite = Resources.Load<Sprite>("Multi" + gameMode2 + stage);
                break;
        }
     
        stageImage.sprite = imageSprite;
    }

    void PhaseControll()　　　//タイトル画面のフェーズごとの処理
    {
        if (lastPhase != phase)
        {
            StopCoroutine(Reconnect());
            DisablePanel();
            switch (gameMode1)
            {
                case "Single":
                    minPlayer = 1;
                    switch (phase)
                    {
                        case 0:
                            mainTitle.gameObject.SetActive(true);
                            break;
                        case 1:
                            minPlayer = 1;
                            GameStart.PlayerNumber = 1;
                            selectGameMode.gameObject.SetActive(true);
                            break;
                        case 2:
                            stageSelect.gameObject.SetActive(true);
                            break;
                        case 3:
                            SceneManager.LoadScene("Stage");
                            loadScreen.gameObject.SetActive(true);
                            break;

                    }
                    break;
                case "Multi":
                    minPlayer = 2;
                    switch (phase)
                    {
                        case 0:
                            mainTitle.gameObject.SetActive(true);
                            break;
                        case 1:
                            GameStart.PlayerNumber = 2;
                            minPlayer = 2;
                            changePlayerNumber.gameObject.SetActive(true);
                            break;
                        case 2:
                            selectGameMode.gameObject.SetActive(true);
                            lastPlayerNum = PlayerNumber;
                            break;
                        case 3:
                            stageSelect.gameObject.SetActive(true);
                            break;
                        case 4:
                            //ノーマルモードならチーム選択画面はスキップ
                            if (gameMode2 == "Nomal") { phase++; return; }
                            setArcadeGame.gameObject.SetActive(true);
                            SetArcade();
                            break;
                        case 5:
                            SceneManager.LoadScene("Stage");
                            loadScreen.gameObject.SetActive(true);
                            break;
                    }
                    break;
                case "Online":
                    switch (phase)
                    {
                        case 0:
                            mainTitle.gameObject.SetActive(true);
                            NetWorkMain.isOnline = false;
                            StopCoroutine(Reconnect());
                            reconnectable = true;
                            break;
                        case 1:
                            if (lastPhase > phase) { phase = 0; return; }
                            loadScreen.gameObject.SetActive(true);
                            StartCoroutine(Reconnect());
                            break;
                        case 2:
                            minPlayer = 1;
                            selectOnlineLobby.gameObject.SetActive(true);
                            joinedLobby = false;
                            break;
                        case 3:
                            onlineLobby.gameObject.SetActive(true);
                            if (NetWorkMain.netWorkId == NetWorkMain.leaderId)
                            {
                                photonView.RPC(nameof(SyncPhase), RpcTarget.All, phase);
                            }
                            break;
                        case 4:
                            if (MatchmakingView.mode == "Quick" && NetWorkMain.netWorkId == NetWorkMain.leaderId)
                            {
                                NetWorkMain.SetCustomProps<bool[]>("isReady", new bool[4] { true, true, true, true });
                                photonView.RPC(nameof(SetDefaultArcade), RpcTarget.All, MatchmakingView.stageQuick);
                                photonView.RPC(nameof(RPCStartGame), RpcTarget.All);
                                photonView.RPC(nameof(SyncArcadeTime), RpcTarget.All, flagTimeLimit);
                                return;
                            }
                            photonView.RPC(nameof(SyncArcadeTime), RpcTarget.All, flagTimeLimit);
                            photonView.RPC(nameof(RPCStartGame), RpcTarget.All);
                            phase = 3;
                            onlineLobby.gameObject.SetActive(true);
                            break;
                        case 5:
                            phase = 3;
                            photonView.RPC(nameof(SetDefaultArcade), RpcTarget.All, stage);
                            return;
                            break;
                        case 6:
                            stageSelect.gameObject.SetActive(true);  
                            break;
                        case 7:
                            phase = 3;
                            photonView.RPC(nameof(SetDefaultArcade), RpcTarget.All, stage);
                            return;
                            break;
                        case 8:
                            setArcadeGame.gameObject.SetActive(true);
                            photonView.RPC(nameof(SyncPhase), RpcTarget.All, phase);
                            SetArcade();
                            break;
                        case 9:
                            phase = 3;
                            return;
                            break;
                    }
                    break;
            }

            lastPhase = phase;
        }
        if (setArcadeGame.gameObject.activeSelf)
        {
            TeamSelect();
        }
    }
    IEnumerator Reconnect()
    {
        while (true)
        {
            if (reconnectable && phase  == 1)
            {
                if (PhotonNetwork.IsConnected) { PhotonNetwork.JoinLobby(); }
                reconnectable = false;
                StartCoroutine(Loading());
            }

            yield return null; // 1フレーム待つ

            if (PhotonNetwork.InLobby && !joinedLobby && phase == 1 && gameMode1 == "Online")
            {
                joinedLobby = true; // 重複して呼ばれないようにフラグを立てる
                yield return new WaitForSeconds(2.0f); // 1フレーム待つ
                phase++;
            }
        }
    }

    IEnumerator Loading()
    {
        yield return new WaitForSeconds(2.0f);
        reconnectable = true;
    }

    [PunRPC]
    void SyncArcadeTime(int timeLimit)
    {
        flagTimeLimit = timeLimit;
    }
    [PunRPC]
    public void SyncPhase(int phaseLocal)
    {
        phase = phaseLocal;
    }
    [PunRPC]
    public void SetDefaultArcade(int updatedStage)
    {
        stage = updatedStage;    
        SetArcade();
    }
    [PunRPC]
    void RPCStartGame()
    {
        if (GameStart.PlayerNumber > 1)
        {
            //全プレイヤーが準備完了か確認する
            bool allReady = true;
            ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (customProps.ContainsKey("isReady"))
            {
                bool[] isReadyLocal = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["isReady"];
                for (int i = 0; i < PlayerNumber; i++)
                {
                    if (isReadyLocal[i] == false)
                    {
                        allReady = false;
                    }
                }
                if (allReady)
                {
                    //次のために全プレイヤーのreadyを解除しておく
                    for (int i = 0; i < maxPlayer; i++)
                    {
                        isReadyLocal[i] = false;
                    }
                    PhotonNetwork.IsMessageQueueRunning = false;
                    SceneManager.LoadScene("Stage");
                    customProps["isReady"] = isReadyLocal;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
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

    void SetArcade()
    {
        //旗鳥モードのデフォルト設定
        if (stage == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                playerIcon[i].gameObject.SetActive(false);
                playerSlot[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < PlayerNumber; i++)
            {
                playerIcon[i].gameObject.SetActive(true);
                playerSlot[i].gameObject.SetActive(true);
                playerTeam[i] = i;
            }
            if (gameMode1 == "Online")
            {
                NetWorkMain.SetCustomProps<int[]>("playerTeam", new int[] { 0, 1, 2, 3 });
            }
        }
        //サッカーモードのデフォルト設定
        if (stage == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                playerIcon[i].gameObject.SetActive(false);
                playerSlot[i].gameObject.SetActive(false);
                if ((i + 1) % 2 == 0)
                {
                    playerTeam[i] = 1;
                }
                else
                {
                    playerTeam[i] = 0;
                }
            }
            for (int i = 0; i < PlayerNumber; i++)
            {
                playerIcon[i].gameObject.SetActive(true);
            }
            playerSlot[0].gameObject.SetActive(true);
            playerSlot[1].gameObject.SetActive(true);
            if (gameMode1 == "Online")
            {
                NetWorkMain.SetCustomProps<int[]>("playerTeam", new int[] { 0, 1, 0, 1 });
            }

        }
        TeamSelect();
    }
    void TeamSelect()
    {
        flagTimeLimitTx.text = flagTimeLimit.ToString();
        for (int i = 0; i < PlayerNumber; i++)
        {
            playerIcon[i].transform.position = playerIconPos[i];

            for (int j = 0; j < PlayerNumber; j++)
            {
                if (playerTeam[i] == j)
                {
                    playerIconPos[i] = slot1Pos[j];
                    int count = 0;
                    for (int k = 0; k < PlayerNumber; k++)
                    {
                        if (k != i)
                        {
                            if (playerTeam[k] == playerTeam[i] && k < i)
                            {
                                count++;
                                playerIconPos[i].x = slot1Pos[playerTeam[i]].x + (1.2f * count);
                            }
                        }
                    }
                }
            }
            teamSize[i] = 0;
        }

        for (int i = 0; i < PlayerNumber; i++)
        {
            playerTeam[i] = Mathf.Clamp(playerTeam[i], 0, PlayerNumber - 1);
        }
        bool allOtherTeam = playerTeam.Take(PlayerNumber).Distinct().Count() == PlayerNumber;

        if (allOtherTeam)
        {
            teamMode = "FFA";
        }
        else
        {
            teamMode = "Team";
            teamCount = 0;
            for (int i = 0; i < PlayerNumber; i++)
            {
                teamSize[playerTeam[i]]++;
            }
            for (int j = 0; j < PlayerNumber; j++)
            {
                if (teamSize[j] != 0)
                {
                    teamCount++;
                }
            }
        }
    }


    void DisablePanel()
    {
        stageSelect.gameObject.SetActive(false);
        selectGameMode.gameObject.SetActive(false);
        mainTitle.gameObject.SetActive(false);
        changePlayerNumber.gameObject.SetActive(false);
        setArcadeGame.gameObject.SetActive(false);
        selectOnlineLobby.gameObject.SetActive(false);
        onlineLobby.gameObject.SetActive(false);
        loadScreen.gameObject.SetActive(false);
    }

    void SwichUI()
    {
        //キーボード,マウスのとき
        if (!(ControllerInput.usingController))
        {
            //keyBoardMouseUI.gameObject.SetActive(true);
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
            cursor.gameObject.SetActive(false);
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {

            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(true); }
            cursor.gameObject.SetActive(true);
            //keyBoardMouseUI.gameObject.SetActive(false);
        }

        //UI非表示設定時
        if (Settings.guideMode == 1)
        {
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
        }
    }

}
