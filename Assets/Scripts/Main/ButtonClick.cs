using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour　//クリック用ボタン
{
    public GameObject pauseButton;

    public void BackToTitle()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 0;
        GameStart.inDemoPlay = false;
        GameSetting.Playable = false;
        GameStart.PlayerNumber = 1;
        Settings.SettingPanelActive = false;
        SceneManager.LoadScene("Title");
    }
    public void PauseButton()
    {
        if (GameSetting.startTime < 0)
        {
            ButtonInGame.Paused = 1;
            GameSetting.Playable = false;
            GameSetting.startTime = 0;
            pauseButton.gameObject.SetActive(false);
            Settings.SettingPanelActive = true;
            Settings.inSetting = true;
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
    public void RestartButton()
    {
        ButtonInGame.Paused = 0;
        GameSetting.startTime = -1;
        GameSetting.Playable = true;
        pauseButton.gameObject.SetActive(true);
        Settings.SettingPanelActive = false;
        Settings.inSetting = false;
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
}
