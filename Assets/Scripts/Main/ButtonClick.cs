using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Photon.Pun;
public class ButtonClick : MonoBehaviourPunCallbacks　//クリック用ボタン
{
    public GameObject pauseButton;
    private GameSetting gameSetting = new GameSetting(); 
    public void BackToTitle()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 0;
        ButtonInGame.Paused = 0;
        GameStart.inDemoPlay = false;
        GameSetting.Playable = false;
        GameStart.PlayerNumber = 1;
        Settings.SettingPanelActive = false;
        
        if (GameStart.gameMode1 == "Online")
        {
            photonView.RPC("DeleatPlayer", RpcTarget.All, NetWorkMain.netWorkId);
            if (MatchmakingView.gameModeQuick == "Quick") 
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
            }
        }
        SceneManager.LoadScene("Title");
    }
    [PunRPC]
    void DeleatPlayer(int id)
    {
        GameSetting.playerLeft[id - 1] = true;
    }
    public void PauseButton()
    {
        if (GameSetting.startTime < 0 && GameMode.Finished == false && GameMode.Goaled == false)
        {
            ButtonInGame.Paused = 1;
            GameSetting.Playable = false;
            pauseButton.gameObject.SetActive(false);
            Settings.SettingPanelActive = true;
            Settings.inSetting = true;
            Time.timeScale = 0;
        }
        if (GameStart.gameMode1 == "Online")
        {
            Time.timeScale = 1;
            GameSetting.Playable = true;
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
        Time.timeScale = 1;
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
        if (Settings.languageNum < 1)
        {
            Settings.languageNum++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void PrevLanguage()
    {
        if (Settings.languageNum > 0)
        {
            Settings.languageNum--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void NextScreenMode()
    {
        if (Settings.screenModeNum < 1)
        {
            Settings.screenModeNum++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void PrevScreenMode()
    {
        if (Settings.screenModeNum > 0)
        {
            Settings.screenModeNum--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void NextGuideMode()
    {
        if (Settings.guideMode < 1)
        {
            Settings.guideMode++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void PrevGuideMode()
    {
        if (Settings.guideMode > 0)
        {
            Settings.guideMode--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
}
