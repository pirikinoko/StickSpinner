using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class ButtonInGame : MonoBehaviour
{
    public GameObject PausePanel, pauseButton, TLFrame;
    public static int Paused = 0;

    // プレイヤー
    Controller[] playersController{ get; set;} = new Controller[GameStart.MaxPlayer];

    //animation
    [SerializeField]
    private Animator XButtonAnim1, XButtonAnim2;


    float holdTime = 0;
    float holdGoal = 0.9f;


    void Start()
    {
        // 参加プレイヤー
        for(int i = 0; i < GameStart.PlayerNumber; i++)
        {
            playersController[i] = GameObject.Find("Stick" + (i + 1).ToString()).GetComponent<Controller>();
        }
        Paused = 0;
    }

    void Update()
    {
        MainControll();
    }

    void MainControll()
    {

        // コントローラーのXボタン
        if (ControllerInput.back[0])    // 押された瞬間
        {
            XButtonAnim2.enabled = true;
            XButtonAnim2.SetTrigger("On");
            if (Paused == 1)
            {
                XButtonAnim1.enabled = true;
                XButtonAnim1.SetTrigger("On");
            }
        }
        if (ControllerInput.backHold[0])   // 押し続けたとき
        {

            //ゲームスタートのボタンのみ長押しで動作
            holdTime += Time.deltaTime;
            if (holdTime > holdGoal)
            {
                if (Paused == 1)
                {
                    Paused = 0;
                    SoundEffect.PironTrigger = 1;
                    BackToTitle();
                }
                else if (GameMode.Goaled || GameMode.Finished)
                {
                    SoundEffect.PironTrigger = 1;
                    SetHighScore.ToSetHighscore();
                    BackToTitle();
                }
            }
        }
        if (ControllerInput.backHold[0] == false) // 離されたとき
        {

            XButtonAnim1.SetTrigger("Off");
            XButtonAnim2.SetTrigger("Off");
            XButtonAnim1.enabled = false;
            XButtonAnim2.enabled = false;

            holdTime = 0;
        }

        // 一台目のコントローラーのStartボタン                                                      
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || ControllerInput.start[0])
        {
            if (GameSetting.StartTime < 0 && Paused == 0)
            {
                Paused = 1;
                GameSetting.Playable = false;
                GameSetting.StartTime = 0;
                pauseButton.gameObject.SetActive(false);
                Settings.SettingPanelActive = true;
                Settings.inSetting = true;
            }
            else if (Paused == 1)
            {
                Paused = 0;
                GameSetting.StartTime = -1;
                GameSetting.Playable = true;
                pauseButton.gameObject.SetActive(true);
                Settings.SettingPanelActive = false;
                Settings.inSetting = false;
            }
        }
    }

    public void BackToTitle()　//タイトルに戻る
    {
        SoundEffect.PironTrigger = 1;
        GameStart.phase          = 0;
        GameStart.inDemoPlay     = false;
        GameSetting.Playable     = false;
        GameStart.PlayerNumber   = 1;
        Settings.SettingPanelActive = false;
        SceneManager.LoadScene("Title");
    }
}

