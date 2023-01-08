
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    const int title = 0;
    const int selectStage = 1;
    const int selectPlayerNumber = 2;
    const int stage1 = 1;
    const int stage2 = 2;
    const int stage3 = 3;
    const int stage4 = 4;
    const float holdGoal = 0.85f;
    float holdTime = 0;
    // ボタンの長押し時間                           
    [SerializeField]
    private Animator YButtonAnim;
    Controller controller;
    void Update()
    {
        TitleControll();  
    }
    void TitleControll()
    {
        /*一台目のコントローラーのYボタン*/
        if (controller.playerKey[0].next) //押されたとき
        {
            switch (GameStart.phase)
            {
                case title:
                    GameStart.phase++;
                    if (GameStart.Stage == stage4)
                    {
                        GameStart.PlayerNumber = 2;
                    }
                    break;
                case selectStage:
                    GameStart.phase++;
                    if (GameStart.Stage == stage4)
                    {
                        GameStart.PlayerNumber = 2;
                    }
                    break;
                case selectPlayerNumber:
                    YButtonAnim.enabled = true;
                    YButtonAnim.SetTrigger("On");
                    break;
            }
            controller.playerKey[0].next = false;
        }
        if (controller.playerKey[0].nextHold) //押し続けたとき
        {
            if (GameStart.phase == selectPlayerNumber)
            {
                //ゲームスタートのボタンのみ長押しで動作
                holdTime += Time.deltaTime;
                if (holdTime > holdGoal)
                {
                    // Stage1 ～ 3 は Int型なので、↓でいいと思う
                    GameStart.InSelectPN = false;
                    SoundEffect.PironTrigger = 1;
                    SceneManager.LoadScene("Stage" + GameStart.Stage.ToString());
                }
            }
        }
        if (controller.playerKey[0].nextHold == false) //離されたとき
        {
            YButtonAnim.SetTrigger("Off");
            YButtonAnim.enabled = false;
            holdTime = 0;
        }
        /*/一台目のコントローラーのYボタン*/

        /*一台目のコントローラーのXボタン*/
        if (controller.playerKey[0].back) //押されたとき
        {
            GameStart.phase--;
            controller.playerKey[0].back = false;
        }
        /*/一台目のコントローラーのXボタン*/

        /*一台目のコントローラーのRボタン*/
        if (controller.playerKey[0].plus)
        {
            switch (GameStart.phase)
            {
                case selectStage:
                    GameStart.Stage++;
                    break;
                case selectPlayerNumber:
                    GameStart.PlayerNumber++;
                    break;
            }
            controller.playerKey[0].plus = false;
        }

        /*/一台目のコントローラーのRボタン*/

        /*一台目のコントローラーのLボタン*/
        if (controller.playerKey[0].minus)
        {
            if (GameStart.Stage != stage4)
            {
                GameStart.PlayerNumber = 1;
            }
            switch (GameStart.phase)
            {
                case selectStage:
                    GameStart.Stage--;
                    break;
                case selectPlayerNumber:
                    bool bProcessed = false;
                    if (GameStart.Stage == stage4 && GameStart.PlayerNumber > 2)
                    {
                        bProcessed = true;
                    }
                    //ステージ4以外
                    else if (GameStart.Stage != stage4 && GameStart.PlayerNumber > 1)
                    {
                        bProcessed = true;
                    }
                    if (bProcessed)
                    {
                        GameStart.PlayerNumber--;
                    }
                    break;
            }
            controller.playerKey[0].minus = false;
        }
        /*/一台目のコントローラーのLボタン*/

        /*一台目のコントローラーのStartボタン*/
        if (controller.playerKey[0].start)
        {
            //設定画面の表示
            Settings.SettingPanelActive = !(Settings.SettingPanelActive);
            Settings.inSetting = !(Settings.inSetting);
            controller.playerKey[0].start = false;
        }
        /*/一台目のコントローラーのStartボタン*/

    }
}

