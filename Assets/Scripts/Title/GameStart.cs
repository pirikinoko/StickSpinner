using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Linq;


public class GameStart : MonoBehaviour
{
    public const int MaxStage = 4;     // 総ステージ数
    public const int MaxPlayer = 4;     // 総プレイヤー数
    const int KeyboardMode = 5;
    const int ControllerMode = 6;

    public GameObject mainTitle, startPanel, changePlayerNumber, stageSelect, selectGameMode, setArcadeGame, keyBoardMouseUI;
    public GameObject[] controllerUI, playerIcon, playerSlot;
    //チーム選択
    public Vector2[] playerIconPos { get; set; } = new Vector2[4];
    public Vector2[] slot1Pos = new Vector2[4];
    public static int[] playerTeam { get; set; } = { 0, 1, 2, 3 }; // {p1, p2, p3, p4}が TeamA, TeamB, TeamC, TeamDにいることを示す。ex..a = 1, c =3
    public static int[] teamSize { get; set; } = { 0, 0, 0, 0 }; // チーム　A, B, C, Dにいるプレイヤーの人数
    public static int teamCount = 0; //チームの数
    public static string teamMode = "FreeForAll"; //対戦チーム分け 
    public bool stageInfoActive { get; set; } = false;
    int lastPlayerNum;
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
    public static string gameMode1 = "Single";
    public static string gameMode2 = "Nomal";
    public static int phase = 0;
    public static int PlayerNumber { get; set; } = 1;     // 参加プレイヤー数
    public static int Stage = 1;

    void Start()
    {
        inDemoPlay = false;
        Stage = 1;
        PlayerNumber = 1;
        teamMode = "FreeForAll";
        phase = 0;
        videoMaterial.gameObject.SetActive(false);
        blinkTextGO.gameObject.SetActive(false);
        titleVideo.Stop();
        idleTimer = 0;
        SetTextAlpha(1.0f);
        //titleVideo.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            playerIconPos[i] = slot1Pos[i];
            playerTeam[i] = i;

        }
    }
    void Update()
    {
        SwichUI();
        SwichStageMaterial();
        playerNumberText.text = PlayerNumber.ToString();
        if (Settings.SettingPanelActive)
        {
            DisablePanel();
            return;
        }
        PhaseControll();
        //phase 0～3
        phase = System.Math.Min(phase, 4);
        phase = System.Math.Max(phase, 0);
        //タイトル動画再生
        if (Input.anyKeyDown)
        {
            HideVideo();
        }

        // 一定時間放置したら動画再生
        if (!isVideoPlaying)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= 15f)
            {
                PlayVideo();
            }
        }
        //テキスト点滅
        // テキストの透明度を変化させる
        float alpha = blinkText.color.a;

        if (isFadingOut)
        {
            alpha -= blinkSpeed * Time.deltaTime;
            if (alpha <= 0.0f)
            {
                alpha = 0.0f;
                isFadingOut = false;
            }
        }
        else
        {
            alpha += blinkSpeed * Time.deltaTime;
            if (alpha >= 1.0f)
            {
                alpha = 1.0f;
                isFadingOut = true;
            }
        }

        SetTextAlpha(alpha);
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
                    imageSprite = Resources.Load<Sprite>("SingleArcade" + Stage + "Img");
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

        }
        stageImage.sprite = imageSprite;
    }

    void PhaseControll()　　　//タイトル画面のフェーズごとの処理
    {
        switch (gameMode1)
        {
            case "Single":
                switch (phase)
                {
                    case 0:
                        DisablePanel();
                        GameStart.PlayerNumber = 1;
                        mainTitle.gameObject.SetActive(true);
                        break;
                    case 1:
                        phase++;
                        gameMode2 = "Nomal";
                        return;
                        DisablePanel();
                        selectGameMode.gameObject.SetActive(true);
                        break;
                    case 2:
                        DisablePanel();
                        stageSelect.gameObject.SetActive(true);
                        break;

                }
                break;
            case "Multi":
                switch (phase)
                {
                    case 0:
                        DisablePanel();
                        GameStart.PlayerNumber = 1;
                        mainTitle.gameObject.SetActive(true);
                        break;
                    case 1:
                        DisablePanel();
                        changePlayerNumber.gameObject.SetActive(true);
                        break;
                    case 2:
                        DisablePanel();
                        selectGameMode.gameObject.SetActive(true);
                        lastPlayerNum = PlayerNumber;
                        break;
                    case 3:
                        DisablePanel();
                        stageSelect.gameObject.SetActive(true);
                        break;
                        if (!(gameMode2 == "Arcade") || !(Stage < 3))
                        {
                            return;
                        }
                    case 4:
                        DisablePanel();
                        setArcadeGame.gameObject.SetActive(true);
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
                        break;
                }
                break;
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
        inDemoPlay = false;
    }

    void SwichUI()
    {
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
