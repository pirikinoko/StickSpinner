using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class GameStart : MonoBehaviour
{
    public const int MaxStage  = 4;     // 総ステージ数
    public const int MaxPlayer = 4;     // 総プレイヤー数
    const int KeyboardMode = 5;
    const int ControllerMode = 6;

    public GameObject MainTitle, StartPanel, ChangePlayerNumber, FrontCanvas, Stage1Scores, Stage2Scores, Stage3Scores, Stage4Scores, KeyboardMouseUI1, KeyboardMouseUI2;
    public GameObject[] controllerUI;
    Button StartButton;
    public static bool inDemoPlay = false;

    //以下画像差し替え用
    public SpriteRenderer StageTitle, StageDifficulity, StageDescription;
    public Sprite Stage1Title, Stage1Difficulity, Stage1Description;
    public Sprite Stage2Title, Stage2Difficulity, Stage2Description;
    public Sprite Stage3Title, Stage3Difficulity, Stage3Description;
    public Sprite Stage4Title, Stage4Difficulity, Stage4Description;
    [SerializeField] Image StageImage;
    [SerializeField] Sprite[] stageImages = new Sprite[4];

    //動画
    public VideoPlayer StageVideo;
    public VideoClip Stage1Video, Stage2Video, Stage3Video, Stage4Video;
    public static int phase = 0;
    public static int PlayerNumber{get; set;} = 1;     // 参加プレイヤー数
    public static int Stage = 1;

    void Start()
    {
        inDemoPlay = false;
        Stage = 1;
        PlayerNumber = 1;
        phase = 0;
        FrontCanvas.gameObject.SetActive(false);
        StartPanel.gameObject.SetActive(false);

    }
    void Update()
    {
        SwichUI();
        SwichStageMaterial();
        PhaseControll();
    }

    void SwichStageMaterial() //選択ステージ毎に情報切り替え
    {
        switch (Stage)
        {
            case 1:
                StageTitle.sprite = Stage1Title;
                StageDifficulity.sprite = Stage1Difficulity;
                StageDescription.sprite = Stage1Description;
                DisableScores();
                Stage1Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage1Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;

            case 2:
                StageTitle.sprite = Stage2Title;
                StageDifficulity.sprite = Stage2Difficulity;
                StageDescription.sprite = Stage2Description;
                DisableScores();
                Stage2Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage2Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;

            case 3:
                StageTitle.sprite = Stage3Title;
                StageDifficulity.sprite = Stage3Difficulity;
                StageDescription.sprite = Stage3Description;
                DisableScores();
                Stage3Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage3Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;

            case 4:
                StageTitle.sprite = Stage4Title;
                StageDifficulity.sprite = Stage4Difficulity;
                StageDescription.sprite = Stage4Description;
                DisableScores();
                Stage4Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage4Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;
        }
    }
    void DisableScores()
    {
        Stage1Scores.gameObject.SetActive(false);
        Stage2Scores.gameObject.SetActive(false);
        Stage3Scores.gameObject.SetActive(false);
        Stage4Scores.gameObject.SetActive(false);
    }
    void PhaseControll()　　　//タイトル画面のフェーズごとの処理
    {
        switch (phase)
        {
            case 0:
                DisablePanel();
                MainTitle.gameObject.SetActive(true);
                break;
            case 1:
                DisablePanel();
                FrontCanvas.gameObject.SetActive(true);
                break;
            case 2:
                DisablePanel();
                ChangePlayerNumber.gameObject.SetActive(true);
                break;
            case 3:
                DisablePanel();
                StartPanel.gameObject.SetActive(true);
                inDemoPlay = true;
                break;
            case 4:
                phase = 0;
                SceneManager.LoadScene("Stage" + GameStart.Stage.ToString());
                break;

        }
        //phase 0～4
        phase = System.Math.Min(phase, 4); 
        phase = System.Math.Max(phase, 0);
    }

    void DisablePanel()
    {
        FrontCanvas.gameObject.SetActive(false);
        StartPanel.gameObject.SetActive(false);
        MainTitle.gameObject.SetActive(false);
        ChangePlayerNumber.gameObject.SetActive(false);
        inDemoPlay = false;
    }

    void SwichUI()
    {
        //キーボードマウス用UIとコントローラー用UIの切り替え

        //キーボード,マウスのとき
        if (!(ControllerInput.usingController))
        {
            if (phase == 0)
            {
                KeyboardMouseUI1.gameObject.SetActive(true);
            }
            else if (phase == 1)
            {
                KeyboardMouseUI2.gameObject.SetActive(true);
            }
            for(int i = 0; i < 5; i++) { controllerUI[i].gameObject.SetActive(false); }
            
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {

            for (int i = 0; i < 5; i++) { controllerUI[i].gameObject.SetActive(false); }
            controllerUI[phase].gameObject.SetActive(true);
            controllerUI[4].gameObject.SetActive(true);
            KeyboardMouseUI1.gameObject.SetActive(false);
            KeyboardMouseUI2.gameObject.SetActive(false);
        }
    }

}
