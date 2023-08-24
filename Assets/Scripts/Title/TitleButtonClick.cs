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
        gameStart = GameObject.Find("Systems").GetComponent<GameStart>();
        titleButton = GameObject.Find("Systems").GetComponent<TitleButton>();
        gameStart.clicks = 0;
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
    }
    public void PrevPhase()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase--;
        GameObject.Find("Systems").GetComponent<GameStart>().trigger = 1;
        gameStart.clicks = 0;
    }
    //
    public void RollTrigger()
    {
        if (gameStart.inProgress) 
        {
            return;
        }

        if (this.name == "RightButtonNomal")
        {
            if (gameStart.clicks < gameStart.normalButtons.Length - 1)
            {
                gameStart.rollDirection = "RightNomal";
                gameStart.clicks++;
            }
        }
        else if (this.name == "LeftButtonNomal")
        {
            if (0 <= gameStart.clicks)
            {
                gameStart.rollDirection = "LeftNomal";
                gameStart.clicks--; 
            }
        }

        else if (this.name == "RightButtonArcade")
        {
            if (gameStart.clicks < gameStart.arcadeButtons.Length - 1) 
            {
                gameStart.rollDirection = "RightArcade";
                gameStart.clicks++;
            }        
        }
        else if (this.name == "LeftButtonArcade") 
        { 
            if(0 <= gameStart.clicks)
            {
                gameStart.rollDirection = "LeftArcade";
                gameStart.clicks--; 
            } 
        }

        else if (this.name == "RightButtonSingle") 
        { 
            if (gameStart.clicks < gameStart.singleButtons.Length - 1) 
            {
                gameStart.rollDirection = "RightSingle";
                gameStart.clicks++;
            }    
        }
        else if (this.name == "LeftButtonSingle") 
        {
            if(0 <= gameStart.clicks)
            {
                gameStart.rollDirection = "LeftSingle";
                gameStart.clicks--;
            }
        }

        Debug.Log("Clicks:" + gameStart.clicks);

    }
    //ステージ変更
    public void NextStage()
    {
        if (GameStart.Stage < Stage4)
        {
            GameStart.Stage++;
        }
        SoundEffect.soundTrigger[3] = 1;
    }
    public void PrevStage()　
    {
        if (GameStart.Stage > Stage1)
        {
            GameStart.Stage--;
        }
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
    public void StartGame()
    {
        string str = Regex.Replace(this.gameObject.name, @"[^0-9]", "");
        GameStart.Stage = int.Parse(str);
        if (this.gameObject.name.Contains("Arcade")) 
        {
            GameStart.gameMode2 = "Arcade";
        }
        else { GameStart.gameMode2 = "Nomal"; }
        SceneManager.LoadScene("Stage");
    }


    public void OpenSetting()    //設定画面の表示
    {
        Settings.SettingPanelActive = !(Settings.SettingPanelActive);
        Settings.inSetting = !(Settings.inSetting);
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

}
