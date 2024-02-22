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
    public  int maxStageNomal;     // 総ステージ数
    public static int maxPlayer = 4, minPlayer;     // 総プレイヤー数
    const int KeyboardMode = 5;
    const int ControllerMode = 6;

    public GameObject mainTitle, startPanel, changePlayerNumber, stageSelect, selectGameMode, setArcadeGame, keyBoardMouseUI, selectOnlineLobby, onlineLobby, loadScreen;
    public GameObject[] controllerUI, playerIcon, playerSlot;
    //チーム選択
    public Vector2[] playerIconPos { get; set; } = new Vector2[4];
    public Vector2[] slot1Pos = new Vector2[4];
    public static int[] playerTeam { get; set; } = { 0, 1, 2, 3 }; // {p1, p2, p3, p4}が TeamA, TeamB, TeamC, TeamDにいることを示す。ex..a = 1, c =3
    public static int[] teamSize { get; set; } = { 0, 0, 0, 0 }; // チーム　A, B, C, Dにいるプレイヤーの人数
    public static int teamCount = 0; //チームの数
    public static string teamMode = "FreeForAll"; //対戦チーム分け 
    public bool stageInfoActive { get; set; } = false;
    int lastPlayerNum, lastPhase;
    Button StartButton;
    public static bool inDemoPlay = false;
    public Text playerNumberText, stageNumberText, flagTimeLimitTx;
    public static int flagTimeLimit = 90;
    //動画
    public VideoPlayer stageVideo, titleVideo;
    public VideoClip[] singleStageVideo, singleArcadeVideo, multiStageVideo, multiArcadeVideo;
    [SerializeField] GameObject videoMaterial,blinkTextGO;
    float idleTimer = 0f, blinkSpeed = 1.5f;
    bool isVideoPlaying = false;
    bool isFadingOut = true;
    //画像
    public Image stageImage;
    private Sprite imageSprite;
    //テキスト
    public Text difficultyText, blinkText;
    string[] difficultyStage = { "簡単", "普通", "難しい", "Easy", "Nomal", "Hard" };
    string[] singleArcadeText = { "無限の塔", "InfinityTower",};
    public static string gameMode1 = "Single";
    public static string gameMode2 = "Nomal";
    public static int phase = 0;
    public static int PlayerNumber { get; set; } = 1;     // 参加プレイヤー数
    public static int Stage = 1;
    public static int loadData = 0;
    //ロード画面
    bool reconnectable, joinedLobby = false;
    void Start()
    {
        Time.timeScale = 1;
        inDemoPlay = false;
        GameSetting.Playable = false;
        reconnectable = false;
        Stage = 1;
        PlayerNumber = 1;
        teamMode = "FreeForAll";
        phase = 0;
        lastPhase = -1;
        videoMaterial.gameObject.SetActive(false);
        blinkTextGO.gameObject.SetActive(false);
        titleVideo.Stop();
        idleTimer = 0;
        flagTimeLimit = 90;
        SetTextAlpha(1.0f);
        for (int i = 0; i < 4; i++)
        {
            playerIconPos[i] = slot1Pos[i];
            playerTeam[i] = i;
        }
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
        PhaseControll();
        //phase 0～3
        phase = System.Math.Min(phase,8 );
        phase = System.Math.Max(phase, 0);
    }



    void SwichStageMaterial() //選択ステージ毎に情報切り替え
    {
        switch (gameMode1)
        {
            case "Single":
                if (gameMode2 == "Nomal")
                {
                    stageNumberText.text = "Stage" + Stage.ToString();
                    imageSprite = Resources.Load<Sprite>("SingleNomal" + Stage + "Img");
                    switch (Stage)
                    {
                        case 1:
                            difficultyText.text = difficultyStage[0 + (Settings.languageNum * 3)];
                            break;
                        case 2:
                            difficultyText.text = difficultyStage[0 + (Settings.languageNum * 3)];
                            break;
                        case 3:
                            difficultyText.text = difficultyStage[1 + (Settings.languageNum * 3)];
                            break;
                        case 4:
                            difficultyText.text = difficultyStage[1 + (Settings.languageNum * 3)];
                            break;
                        case 5:
                            difficultyText.text = difficultyStage[2 + (Settings.languageNum * 3)];
                            break;

                    }


                    //stageVideo.clip = singleStageVideo[Stage - 1];

                }
                else
                {
                    stageNumberText.text = singleArcadeText[Settings.languageNum    ];
                    imageSprite = Resources.Load<Sprite>("SingleArcade" + Stage + "Img");
                    difficultyText.text = "Hard";
                    //stageVideo.clip = singleArcadeVideo[Stage - 1];
                }
                break;
            case "Multi":
                if (gameMode2 == "Nomal")
                {
                    stageNumberText.text = "Stage" + Stage.ToString();
                    imageSprite = Resources.Load<Sprite>("MultiNomal" + Stage + "Img");
                    switch (Stage)
                    {
                        case 1:
                            difficultyText.text = difficultyStage[0 + (Settings.languageNum * 3)];
                            break;
                        case 2:
                            difficultyText.text = difficultyStage[0 + (Settings.languageNum * 3)];
                            break;
                        case 3:
                            difficultyText.text = difficultyStage[1 + (Settings.languageNum * 3)];
                            break;
                        case 4:
                            difficultyText.text = difficultyStage[1 + (Settings.languageNum * 3)];
                            break;

                    }
                }
                else
                {
                    if (Stage < 3) { stageNumberText.text = "FlagMode" + Stage.ToString(); }
                    imageSprite = Resources.Load<Sprite>("MultiArcade" + Stage + "Img");
                    //stageVideo.clip = multiArcadeVideo[Stage - 1];
                    switch (Stage)
                    {
                        case 1:
                            difficultyText.text = "Easy";
                            break;

                        case 2:
                            difficultyText.text = "Nomal";
                            break;
                    }


                }
                break;
            case "Online":
                if (gameMode2 == "Nomal")
                {
                    stageNumberText.text = "Stage" + Stage.ToString();
                    imageSprite = Resources.Load<Sprite>("MultiNomal" + Stage + "Img");
                    switch (Stage)
                    {
                        case 1:
                            difficultyText.text = difficultyStage[0 + (Settings.languageNum * 3)];
                            break;
                        case 2:
                            difficultyText.text = difficultyStage[0 + (Settings.languageNum * 3)];
                            break;
                        case 3:
                            difficultyText.text = difficultyStage[1 + (Settings.languageNum * 3)];
                            break;
                        case 4:
                            difficultyText.text = difficultyStage[1 + (Settings.languageNum * 3)];
                            break;
                    }
                }
                else
                {
                    if (Stage < 3) { stageNumberText.text = "FlagMode" + Stage.ToString(); }
                    imageSprite = Resources.Load<Sprite>("MultiArcade" + Stage + "Img");
                    //stageVideo.clip = multiArcadeVideo[Stage - 1];
                    switch (Stage)
                    {
                        case 1:
                            difficultyText.text = "Easy";
                            break;

                        case 2:
                            difficultyText.text = "Nomal";
                            break;
                    }
                }
                break;

        }
        stageImage.sprite = imageSprite;
    }

    void PhaseControll()　　　//タイトル画面のフェーズごとの処理
    {
        if(lastPhase != phase) 
        {
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
                            GameStart.PlayerNumber = 1;
                            selectGameMode.gameObject.SetActive(true);
                            break;
                        case 2:
                            stageSelect.gameObject.SetActive(true);
                            break;
                        case 3:
                            SceneManager.LoadScene("Stage");
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
                            changePlayerNumber.gameObject.SetActive(true);
                            break;
                        case 2:
                            selectGameMode.gameObject.SetActive(true);
                            lastPlayerNum = PlayerNumber;
                            break;
                        case 3:
                            stageSelect.gameObject.SetActive(true);
                            break;
                            if (!(gameMode2 == "Arcade") || !(Stage < 3))
                            {
                                return;
                            }
                        case 4:
                            if(gameMode2 == "Nomal") { phase++; return; }
                            setArcadeGame.gameObject.SetActive(true);
                            SetArcade();
                            break;
                        case 5:
                            SceneManager.LoadScene("Stage");
                            break;
                    }
                    break;
                case "Online":
                    switch (phase)
                    {
                        case 0:
                            mainTitle.gameObject.SetActive(true);
                            reconnectable = true;
                            break;
                        case 1:
                            if(lastPhase > phase) { phase = 0; return; }
                            loadScreen.gameObject.SetActive(true);
                            StartCoroutine(Reconnect());
                            break;
                        case 2:
                            selectOnlineLobby.gameObject.SetActive(true);
                            joinedLobby = false;
                            break;
                        case 3:
                            onlineLobby.gameObject.SetActive(true);
                            break;
                        case 4:
                            phase = 3;
                            return;
                            break;
                        case 5:
                            stageSelect.gameObject.SetActive(true);
                            break;
                        case 6:
                            phase = 3;
                            return;
                            break;
                        case 7:
                            setArcadeGame.gameObject.SetActive(true);
                            break;
                        case 8:
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
            SetArcade();
        }
    }
    IEnumerator Reconnect()
    {
        while (true)
        {
            if (reconnectable)
            {
                PhotonNetwork.JoinLobby();
                reconnectable = false;
                StartCoroutine(Loading());
            }

            yield return null; // 1フレーム待つ

            if (PhotonNetwork.InLobby && !joinedLobby)
            {
                joinedLobby = true; // 重複して呼ばれないようにフラグを立てる
                yield return new WaitForSeconds(2.0f); // 1フレーム待つ
                PhotonNetwork.JoinLobby();
                phase++;
                break;
            }

        }
    }

    IEnumerator Loading() 
    {
        yield return new WaitForSeconds(2.0f);
        reconnectable = true;
    }

    [PunRPC]
    public void RPCSyncPhase(int phaseLocal)
    {
        phase = phaseLocal;
    }


    void SetArcade() 
    {
        flagTimeLimitTx.text = flagTimeLimit.ToString();
        for (int i = 0; i < 4; i++)
        {
            playerIcon[i].gameObject.SetActive(false);
            playerSlot[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < PlayerNumber; i++)
        {
            playerIcon[i].gameObject.SetActive(true);
            playerSlot[i].gameObject.SetActive(true);
        }
        TeamSelect();

        for (int i = 0; i < PlayerNumber; i++)
        {
            playerIcon[i].transform.position = playerIconPos[i];
        }
    }
    void TeamSelect()
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
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
            teamMode = "FreeForAll";
        }
        else
        {
            bool oneVSThree = false;
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
                if (teamSize[j] == 3)
                {
                    oneVSThree = true;
                }
            }

            if (teamCount == 2 && PlayerNumber == 3)
            {
                teamMode = "1vs2";
            }
            else if (teamCount == 2 && PlayerNumber == 4 && oneVSThree)
            {
                teamMode = "1vs3";
            }
            else if (teamCount == 2 && PlayerNumber == 4)
            {
                teamMode = "2vs2";
            }
            else { teamMode = "1vs1vs2"; }
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
        inDemoPlay = false;
    }

    void SwichUI()
    {
        //UI非表示設定時
        if (Settings.guideMode == 1) 
        {
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
            return;
        }

        //キーボードマウス用UIとコントローラー用UIの切り替え
        
        //キーボード,マウスのとき
        if (!(ControllerInput.usingController))
        {
            //keyBoardMouseUI.gameObject.SetActive(true);
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {

            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(true); }
            //keyBoardMouseUI.gameObject.SetActive(false);
        }
    }
    void PlayVideo()
    {
        isVideoPlaying = true;
        // 動画プレイヤーをアクティブにして再生
        videoMaterial.gameObject.SetActive(true);
        titleVideo.Play();
        blinkTextGO.gameObject.SetActive(true);
    }

    void HideVideo()
    {
        isVideoPlaying = false;
        idleTimer = 0;
        // 動画プレイヤーを非アクティブにして非表示
        videoMaterial.gameObject.SetActive(false);
        titleVideo.Stop();
        blinkTextGO.gameObject.SetActive(false);
    }
    // テキストの透明度を設定するヘルパー関数
    private void SetTextAlpha(float alpha)
    {
        Color textColor = blinkText.color;
        textColor.a = alpha;
        blinkText.color = textColor;
    }
}
