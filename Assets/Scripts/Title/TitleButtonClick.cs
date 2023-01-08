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
    public static int P1SensChange, P2SensChange, P3SensChange, P4SensChange;

    //ステージ選択画面に移行
    public void GoStageSelect()
    {
        SoundEffect.PironTrigger = 1;
        GameStart.phase = SelectStage;
        if (GameStart.Stage != Stage4)
        {
            GameStart.PlayerNumber = 1;
        }
    }
    //タイトル画面に戻る
    public void BackToTitle()
    {
        SoundEffect.PironTrigger = 1;
        GameStart.phase = Title;
    }
    //スタートパネルを開く
    public void OpenStartPanel()
    {
        if (GameStart.Stage == Stage4)
        {
            GameStart.PlayerNumber = 2;
        }
        SoundEffect.PironTrigger = 1;
        GameStart.phase = SelectPNumber;
        StartPanel.gameObject.SetActive(true);
        FrontCanvas.gameObject.SetActive(false);
        GameStart.InSelectPN = true;
    }
    public void BackToStageSelect()
    {
        GameStart.InSelectPN = false;
        GameStart.phase = SelectStage;
    }
    public void NextStage()
    {
        if (GameStart.Stage < Stage4)
        {
            GameStart.Stage++;
        }
    }
    public void PrevStage()
    {
        if (GameStart.Stage > Stage1)
        {
            GameStart.Stage--;
        }
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
        GameStart.InSelectPN = false;
        SoundEffect.PironTrigger = 1;
        SceneManager.LoadScene("Stage" + GameStart.Stage.ToString());
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
        P1SensChange = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP2()
    {
        P2SensChange = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP3()
    {
        P3SensChange = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void GainSensP4()
    {
        P4SensChange = 1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP1()
    {
        P1SensChange = -1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP2()
    {
        P2SensChange = -1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP3()
    {
        P3SensChange = -1;
        SoundEffect.BunTrigger = 1;
    }
    public void LoseSensP4()
    {
        P4SensChange = -1;
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
