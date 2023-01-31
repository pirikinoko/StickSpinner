using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    public GameObject pauseButton;

    public void BackToTitle()
    {
        SoundEffect.PironTrigger = 1;
        GameStart.phase = 0;
        GameStart.inDemoPlay = false;
        GameSetting.Playable = false;
        GameStart.PlayerNumber = 1;
        Settings.SettingPanelActive = false;
        SceneManager.LoadScene("Title");
    }
    public void PauseButton()
    {
        if (GameSetting.StartTime < 0)
        {
            ButtonInGame.Paused = 1;
            GameSetting.Playable = false;
            GameSetting.StartTime = 0;
            pauseButton.gameObject.SetActive(false);
            Settings.SettingPanelActive = true;
            Settings.inSetting = true;
        }
    }


    public void RestartButton()
    {
        ButtonInGame.Paused = 0;
        GameSetting.StartTime = -1;
        GameSetting.Playable = true;
        pauseButton.gameObject.SetActive(true);
        Settings.SettingPanelActive = false;
        Settings.inSetting = false;
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
