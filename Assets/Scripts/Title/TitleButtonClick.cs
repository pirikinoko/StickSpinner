using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonClick : MonoBehaviour　//クリック用ボタン
{
    const int Title = 0;
    const int SelectStage = 1;
    const int SelectPNumber = 2;
    const int Stage1 = 1;
    const int Stage2 = 2;
    const int Stage3 = 3;
    const int Stage4 = 4;   
    public GameObject StartPanel, FrontCanvas;
    public static int[] sensChange = new int[4];

    //次の画面
    public void NextPhase()
    {
        SoundEffect.PironTrigger = 1;
        GameStart.phase++;
    }
    public void PrevPhase()
    {
        SoundEffect.PironTrigger = 1;
        GameStart.phase--;
    }

    //ステージ変更
    public void NextStage()
    {
        if (GameStart.Stage < Stage4)
        {
            GameStart.Stage++;
        }
        SoundEffect.BunTrigger = 1;
    }
    public void PrevStage()　
    {
        if (GameStart.Stage > Stage1)
        {
            GameStart.Stage--;
        }
        SoundEffect.BunTrigger = 1;
    }

    //プレイヤー数増減
    public void PlusButton()
    {
        if (GameStart.PlayerNumber < 4)
        {
            GameStart.PlayerNumber++;
            SoundEffect.BunTrigger = 1;
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
            GameStart.PlayerNumber--;
            SoundEffect.BunTrigger = 1;
        }
    }
    public void StartGame()
    {
        // Stage1 ～ 3 は Int型なので、↓でいいと思う
        GameStart.inDemoPlay = false;
        SoundEffect.PironTrigger = 1;
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
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP2()
    {
        Settings.rotStage[1] += 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP3()
    {
        Settings.rotStage[2] += 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP4()
    {
        Settings.rotStage[3] += 1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP1()
    {
        Settings.rotStage[0] -= 1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP2()
    {
        Settings.rotStage[1] -= 1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP3()
    {
        Settings.rotStage[2] -= 1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP4()
    {
        Settings.rotStage[3] -= 1;
        SoundEffect.BunTrigger = 1;
    }


    //設定画面のボタン
    public void GainBGMVol()
    {
        BGM.BGMStage++;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseBGMVol()
    {
        BGM.BGMStage--;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSEVol()
    {
        SoundEffect.SEStage++;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSEVol()
    {
        SoundEffect.SEStage--;
        SoundEffect.BunTrigger = 1;
    }

}
