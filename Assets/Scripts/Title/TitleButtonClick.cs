using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonClick : MonoBehaviour
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
    //設定画面を開く

    public void OpenSetting()
    {
        //設定画面の表示
        Settings.SettingPanelActive = !(Settings.SettingPanelActive);
        Settings.inSetting = !(Settings.inSetting);
    }
    //感度ステージ変更ボタン

    public void GainSensP1()
    {
        sensChange[0] = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP2()
    {
        sensChange[1] = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP3()
    {
        sensChange[2] = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP4()
    {
        sensChange[3] = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP1()
    {
        sensChange[0] = -1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP2()
    {
        sensChange[1] = -1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP3()
    {
        sensChange[2] = -1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP4()
    {
        sensChange[3] = -1;
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
