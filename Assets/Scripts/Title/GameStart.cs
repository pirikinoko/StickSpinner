using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Linq;
using Photon.Pun;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class GameStart : MonoBehaviourPunCallbacks
{
    [SerializeField]
    int minFlagTime;

    [SerializeField]
    int maxFlagTime;

    [SerializeField]
    Image fadePanel;

    public int maxStageNomal;  // 総ステージ数

    float difficultyl;

    float timeFromLastAction;

    float cycle = 0.3f;

    public GameObject mainTitle;

    public GameObject startPanel;

    public GameObject changePlayerNumber;

    public GameObject stageSelect;

    public GameObject selectGameMode;

    public GameObject setArcadeGame;

    public GameObject keyBoardMouseUI;

    public GameObject selectOnlineLobby;

    public GameObject onlineLobby;

    public GameObject loadScreen;

    public GameObject cursor;

    public GameObject[] controllerUI;

    public GameObject[] playerIcon;

    public GameObject[] playerSlot;

    IngameLog ingameLog;

    // チーム選択
    public Vector3[] playerIconPos { get; set; } = new Vector3[4];

    public Vector3[] slot1Pos = new Vector3[4];

    int lastPlayerNum;

    int lastPhase;

    public Text playerNumberText;

    public Text stageNumberText;

    public Text flagTimeLimitTx;

    // 画像
    public Image stageImage;

    private Sprite imageSprite;

    // テキスト
    string[] singleArcadeText = { "無限の塔", "InfinityTower", };

    string[] MultiArcadeText = { "旗取りバトル", "FlagBattle", "サッカー", "FootBall", };

    // static変数
    public static string gameMode1 = "Single";

    public static string gameMode2 = "Nomal";

    public static string teamMode = "FreeForAll";

    public static int phase = 0;

    public static int PlayerCount { get; set; } = 1;

    public static int stage = 1;

    public static int flagTimeLimit = 90;

    public static int[] playerTeam { get; set; } = { 0, 1, 2, 3 }; // {p1, p2, p3, p4}が TeamA, TeamB, TeamC, TeamDにいることを示す。ex..a = 1, c =3

    public static int[] teamSize = new int[4];

    public static int teamCount = 0;

    public static int maxPlayer = 4;

    public static int minPlayer;

    public static bool buttonPushable = true;

    private bool isPhaseProcessing;

    // ロード画面
    bool reconnectable;

    bool joinedLobby = false;

    private PlayerInput playerInput;

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
        isPhaseProcessing = false;
        stage = 1;
        PlayerCount = 1;
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
    private async UniTask Update()
    { 
        SwichUI();
        SwichStageMaterial();
        playerNumberText.text = PlayerCount.ToString();
        if (setArcadeGame.gameObject.activeSelf)
        {
            TeamSelect();
        }
        if (timeFromLastAction > cycle) 
        {
            if (lastPhase != phase)
            {
                await PhaseControll(); 
            }
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
        PlayerCount = Mathf.Clamp(PlayerCount, minPlayer, maxPlayer);
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

    private async UniTask PhaseControll()　　　//タイトル画面のフェーズごとの処理
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
                        GameStart.PlayerCount = 1;
                        selectGameMode.gameObject.SetActive(true);
                        break;
                    case 2:
                        stageSelect.gameObject.SetActive(true);
                        break;
                    case 3:
                        FadeAndSwitchScene();
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
                        GameStart.PlayerCount = 2;
                        minPlayer = 2;
                        changePlayerNumber.gameObject.SetActive(true);
                        break;
                    case 2:
                        selectGameMode.gameObject.SetActive(true);
                        lastPlayerNum = PlayerCount;
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
                        FadeAndSwitchScene();
                        break;
                }
                break;
            case "Online":
                Debug.Log("Phase == " + phase);
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
                        if (InputName.TypedTextToString == null)
                        {
                            IngameLog.GenerateIngameLog("Please type player name");
                            phase--;
                            return;
                        }
                        onlineLobby.gameObject.SetActive(true);
                        if (NetWorkMain.NetWorkId == NetWorkMain.leaderId)
                        {
                            photonView.RPC(nameof(SyncPhase), RpcTarget.All, phase);
                        }
                        break;
                    case 4:
                        if (MatchmakingView.mode == "Quick" && NetWorkMain.NetWorkId == NetWorkMain.leaderId)
                        {
                            NetWorkMain.SetCustomProps<bool[]>("isReady", new bool[4] { true, true, true, true });
                            photonView.RPC(nameof(SetDefaultArcade), RpcTarget.All, MatchmakingView.stageQuick);
                            CallStartIfReady();
                            photonView.RPC(nameof(SyncArcadeTime), RpcTarget.All, flagTimeLimit);
                            return;
                        }
                        photonView.RPC(nameof(SyncArcadeTime), RpcTarget.All, flagTimeLimit);
                        CallStartIfReady();
                        phase = 3;
                        onlineLobby.gameObject.SetActive(true);
                        break;
                    case 5:
                        phase = 3;
                        photonView.RPC(nameof(SetDefaultArcade), RpcTarget.All, stage);
                        return;
                    case 6:
                        stageSelect.gameObject.SetActive(true);
                        break;
                    case 7:
                        phase = 3;
                        photonView.RPC(nameof(SetDefaultArcade), RpcTarget.All, stage);
                        return;
                    case 8:
                        setArcadeGame.gameObject.SetActive(true);
                        photonView.RPC(nameof(SyncPhase), RpcTarget.All, phase);
                        SetArcade();
                        break;
                    case 9:
                        phase = 3;
                        return;
                }
                break;
            }

        lastPhase = phase;
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
    void CallStartIfReady()
    {
        if (GameStart.PlayerCount > 1)
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
                    for (int i = 0; i < maxPlayer; i++)
                    {
                        isReadyValueArray[i] = false;
                    }
                    NetWorkMain.SetCustomProps<bool[]>("isReady", isReadyValueArray);
                    photonView.RPC(nameof(SwitchScene), RpcTarget.All);
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

    [PunRPC]
    void SwitchScene()
    {
        FadeAndSwitchScene();
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
            for (int i = 0; i < PlayerCount; i++)
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
            for (int i = 0; i < PlayerCount; i++)
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
        for (int i = 0; i < PlayerCount; i++)
        {
            RectTransform rectTransform = playerIcon[i].GetComponent<RectTransform>();
            float spacing = rectTransform.sizeDelta.x + 10f;
            rectTransform.localPosition = playerIconPos[i];
            Debug.Log(playerIcon[i].transform.position.z);

            for (int j = 0; j < PlayerCount; j++)
            {
                if (playerTeam[i] == j)
                {
                    playerIconPos[i] = slot1Pos[j];
                    int iconsInSlot = 0;
                    for (int k = 0; k < PlayerCount; k++)
                    {
                        if (k != i)
                        {
                            if (playerTeam[k] == playerTeam[i] && k < i)
                            {
                                iconsInSlot++;
                                playerIconPos[i].x = slot1Pos[playerTeam[i]].x + (spacing * iconsInSlot);               
                            }
                        }
                    }
                }
            }
            teamSize[i] = 0;
        }

        for (int i = 0; i < PlayerCount; i++)
        {
            playerTeam[i] = Mathf.Clamp(playerTeam[i], 0, PlayerCount - 1);
        }
        bool allOtherTeam = playerTeam.Take(PlayerCount).Distinct().Count() == PlayerCount;

        if (allOtherTeam)
        {
            teamMode = "FFA";
        }
        else
        {
            teamMode = "Team";
            teamCount = 0;
            for (int i = 0; i < PlayerCount; i++)
            {
                teamSize[playerTeam[i]]++;
            }
            for (int j = 0; j < PlayerCount; j++)
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

    private void FadeAndSwitchScene() 
    {
        fadePanel.DOFade(1f, 1f)
            .OnComplete(() => SceneManager.LoadScene("Stage"));
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
