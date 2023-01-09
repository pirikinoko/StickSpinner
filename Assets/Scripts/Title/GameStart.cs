using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameStart : MonoBehaviour
{
    const int Title = 0;
    const int SelectStage = 1;
    const int SelectPNumber = 2;
    const int Stage1 = 1;
    const int Stage2 = 2;
    const int Stage3 = 3;
    const int Stage4 = 4;
    public const int MaxStage = 4;     // 総ステージ数
    public const int MaxPlayer = 4;    // 総プレイヤー数

    int UIMode;
    const int KeyboardMode = 5;
    const int ControllerMode = 6;
    public GameObject StartPanel, FrontCanvas, Stage1Scores, Stage2Scores, Stage3Scores, Stage4Scores, KeyboardMouseUI1, KeyboardMouseUI2, ControllerUI1, ControllerUI2, ControllerUI3, ControllerUI4;
    Button StartButton;
    public static bool InSelectPN = false;
    //以下画像差し替え用
    public SpriteRenderer StageTitle, StageDifficulity, StageDescription;
    public Sprite Stage1Title, Stage1Difficulity, Stage1Description;
    public Sprite Stage2Title, Stage2Difficulity, Stage2Description;
    public Sprite Stage3Title, Stage3Difficulity, Stage3Description;
    public Sprite Stage4Title, Stage4Difficulity, Stage4Description;
    //動画
    public VideoPlayer StageVideo;
    public VideoClip Stage1Video, Stage2Video, Stage3Video, Stage4Video;
    public static int phase = 0;
    public static int PlayerNumber = 1;
    public static int Stage = 1;
    void Start()
    {
        InSelectPN = false;
        Stage = Stage1;
        PlayerNumber = 1;
        phase = Title;
        FrontCanvas.gameObject.SetActive(false);
        StartPanel.gameObject.SetActive(false);
        //コントローラーの名前を取得
        var controllerNames = Input.GetJoystickNames();
        //コントローラーが接続されている場合はコントローラー用のUIを表示
        if (controllerNames[0] == "")
        {
            UIMode = KeyboardMode;
        }
        else
        {
            UIMode = ControllerMode;
        }

    }
    void Update()
    {
        //キーボードマウス用UIとコントローラー用UIの切り替え
        SwichUI();
        SwichStageMaterial();
        PhaseControll();

    }

    void SwichStageMaterial()
    {
        switch (Stage)
        {
            case 1:
                StageTitle.sprite = Stage1Title;
                StageDifficulity.sprite = Stage1Difficulity;
                StageDescription.sprite = Stage1Description;
                Stage1Scores.gameObject.SetActive(true);
                Stage2Scores.gameObject.SetActive(false);
                Stage3Scores.gameObject.SetActive(false);
                Stage4Scores.gameObject.SetActive(false);
                StageVideo.clip = Stage1Video;
                break;

            case 2:
                StageTitle.sprite = Stage2Title;
                StageDifficulity.sprite = Stage2Difficulity;
                StageDescription.sprite = Stage2Description;
                Stage2Scores.gameObject.SetActive(true);
                Stage1Scores.gameObject.SetActive(false);
                Stage3Scores.gameObject.SetActive(false);
                Stage4Scores.gameObject.SetActive(false);
                StageVideo.clip = Stage2Video;
                break;

            case 3:
                StageTitle.sprite = Stage3Title;
                StageDifficulity.sprite = Stage3Difficulity;
                StageDescription.sprite = Stage3Description;
                Stage3Scores.gameObject.SetActive(true);
                Stage1Scores.gameObject.SetActive(false);
                Stage2Scores.gameObject.SetActive(false);
                Stage4Scores.gameObject.SetActive(false);
                StageVideo.clip = Stage3Video;
                break;

            case 4:
                StageTitle.sprite = Stage4Title;
                StageDifficulity.sprite = Stage4Difficulity;
                StageDescription.sprite = Stage4Description;
                Stage4Scores.gameObject.SetActive(true);
                Stage1Scores.gameObject.SetActive(false);
                Stage2Scores.gameObject.SetActive(false);
                Stage3Scores.gameObject.SetActive(false);
                StageVideo.clip = Stage4Video;
                break;
        }
    }
    void PhaseControll()
    {
        switch (phase)
        {
            case 0:
                FrontCanvas.gameObject.SetActive(false);
                StartPanel.gameObject.SetActive(false);
                InSelectPN = false;
                break;
            case 1:
                FrontCanvas.gameObject.SetActive(true);
                StartPanel.gameObject.SetActive(false);
                InSelectPN = false;
                break;
            case 2:
                StartPanel.gameObject.SetActive(true);
                FrontCanvas.gameObject.SetActive(false);
                InSelectPN = true;
                break;
            case 3:
                phase = 0;
                SceneManager.LoadScene("Stage" + GameStart.Stage.ToString());
                InSelectPN = false;
                break;
        }
    }


    void SwichUI()
    {
        //キーボードマウス用UIとコントローラー用UIの切り替え
        if (Controller.usingController)
        {
            UIMode = ControllerMode;
        }
        else
        {
            UIMode = KeyboardMode;
        }

        //キーボード,マウスのとき
        if (UIMode == KeyboardMode)
        {
            if (phase == SelectStage)
            {
                KeyboardMouseUI1.gameObject.SetActive(true);
            }
            else if (phase == SelectPNumber)
            {
                KeyboardMouseUI2.gameObject.SetActive(true);
            }

            ControllerUI1.gameObject.SetActive(false);
            ControllerUI2.gameObject.SetActive(false);
            ControllerUI3.gameObject.SetActive(false);
            ControllerUI4.gameObject.SetActive(false);
        }
        //コントローラーのとき
        else if (UIMode == ControllerMode)
        {

            if (phase == Title)
            {
                ControllerUI3.gameObject.SetActive(true);
                ControllerUI4.gameObject.SetActive(true);
            }
            if (phase == SelectStage)
            {
                ControllerUI1.gameObject.SetActive(true);
                ControllerUI3.gameObject.SetActive(false);
                ControllerUI4.gameObject.SetActive(true);
            }
            if (phase == SelectPNumber)
            {
                ControllerUI2.gameObject.SetActive(true);
                ControllerUI3.gameObject.SetActive(false);
                ControllerUI4.gameObject.SetActive(true);
            }

            KeyboardMouseUI1.gameObject.SetActive(false);
            KeyboardMouseUI2.gameObject.SetActive(false);
        }
    }
}
