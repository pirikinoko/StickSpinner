using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
public class TitleButtonClick : MonoBehaviour　//クリック用ボタン
{
    const int Title = 0;
    const int SelectStage = 1;
    const int SelectPNumber = 2;
    const int Stage1 = 1;
    const int Stage2 = 2;
    const int Stage3 = 3;
    const int Stage4 = 4;
    public static int[] sensChange = new int[4];
    TitleButton titleButton;
    GameStart gameStart;
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Title")
        {
            gameStart = GameObject.Find("Systems").GetComponent<GameStart>();
            titleButton = GameObject.Find("Systems").GetComponent<TitleButton>();
        }
   
    }

    //ゲーム終了ボタン
    public void ExitGame()
    {
        Settings.exitPanelActive = true;
    }
    public void yesExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
        #else
        Application.Quit();//ゲームプレイ終了
        #endif
    }
    public void noBack()
    {
        Settings.exitPanelActive = false;
    }
    //次の画面(シングルプレイ)
    public void NextPhaseSingle()
    {
        GameStart.gameMode1 = "Single";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    //次の画面(マルチプレイ)
    public void NextPhase()
    {
        GameStart.gameMode1 = "Multi";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
        titleButton.targetNum = 0;
    }
    //次の画面(通常モード)
    public void NextPhaseNomal()
    {
        GameStart.gameMode2 = "Nomal";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    //次の画面(ミニゲーム)
    public void NextPhaseArcade()
    {
        GameStart.gameMode2 = "Arcade";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    public void PrevPhase()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase--;
    }



    //ステージ変更
    public void NextStage()
    {
        titleButton.targetNum++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void PrevStage()
    {
        titleButton.targetNum--;
        SoundEffect.soundTrigger[3] = 1;
    }

    //プレイヤー数増減
    public void PlusButton()
    {
        if (GameStart.PlayerNumber < 4)
        {
            titleButton.targetNum++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void MinusButton()
    {
        bool bProcessed = false;

        //ステージ4
        if (GameStart.Stage == Stage4 && GameStart.PlayerNumber > 2)
        {
            bProcessed = true;
        }
        //ステージ4以外
        else if (GameStart.Stage != Stage4 && GameStart.PlayerNumber > 1)
        {
            bProcessed = true;
        }
        if (bProcessed)
        {
            titleButton.targetNum--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void OpenInfo()
    {
        gameStart.stageInfoActive = true;
    }

    public void CloseInfo()
    {
        gameStart.stageInfoActive = false;
    }
    public void StartGame()
    {
        if (GameStart.phase == 3 && GameStart.gameMode2 == "Arcade" && GameStart.Stage < 3)
        {
            GameStart.phase++;
            return;
        }
        SceneManager.LoadScene("Stage");
    }

    public void OpenSetting()    //設定画面の表示
    {
        Settings.SettingPanelActive = !(Settings.SettingPanelActive);
        Settings.inSetting = !(Settings.inSetting);
    }

    public void PlusFlagTime()
    {
        GameStart.flagTimeLimit += 10;
        GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 150);
    }
    public void MinusFlagTime()
    {
        GameStart.flagTimeLimit -= 10;
        GameStart.flagTimeLimit = System.Math.Max(40, GameStart.flagTimeLimit);

    }


    //感度変更ボタン
    public void GainSensP1()
    {
        Settings.rotStage[0] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSensP2()
    {
        Settings.rotStage[1] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSensP3()
    {
        Settings.rotStage[2] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSensP4()
    {
        Settings.rotStage[3] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP1()
    {
        Settings.rotStage[0] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP2()
    {
        Settings.rotStage[1] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP3()
    {
        Settings.rotStage[2] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP4()
    {
        Settings.rotStage[3] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }


    //設定画面のボタン
    public void GainBGMVol()
    {
        BGM.BGMStage++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseBGMVol()
    {
        BGM.BGMStage--;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSEVol()
    {
        SoundEffect.SEStage++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSEVol()
    {
        SoundEffect.SEStage--;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void NextLanguage()
    {
        Settings.languageNum++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void PrevLanguage()
    {
        Settings.languageNum--;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void NextScreenMode()
    {
        Settings.screenModeNum++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void PrevScreenMode()
    {
        Settings.screenModeNum--;
        SoundEffect.soundTrigger[3] = 1;
    }
}
