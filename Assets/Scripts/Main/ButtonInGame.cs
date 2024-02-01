    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ButtonInGame : MonoBehaviourPunCallbacks
{
    public GameObject PausePanel, pauseButton, TLFrame;
    public static int Paused = 0;
    private GameSetting gameSetting = new GameSetting();

    //animation
    [SerializeField]
    private Animator XButtonAnim1, XButtonAnim2;


    float holdTime = 0;
    float holdGoal = 0.9f;


    void Start()
    {
        Paused = 0;
    }

    void Update()
    {
        MainControll();
    }


    void MainControll()
    {
        // 一台目のコントローラーのStartボタン                                                      
        if ((Input.GetKeyDown(KeyCode.Space) || ControllerInput.start[0] || Input.GetKeyDown(KeyCode.Escape)) && Settings.exitPanelActive == false)
        {
            if (GameSetting.startTime < 0 && Paused == 0 && GameMode.Finished == false && GameMode.Goaled == false)
            {
                Paused = 1;
                GameSetting.Playable = false;
                pauseButton.gameObject.SetActive(false);
                Settings.SettingPanelActive = true;
                Settings.inSetting = true;
                Time.timeScale = 0;
            }
            else if (Paused == 1)
            {
                Paused = 0;
                GameSetting.startTime = -1;
                GameSetting.Playable = true;
                pauseButton.gameObject.SetActive(true);
                Settings.SettingPanelActive = false;
                Settings.inSetting = false;
                Time.timeScale = 1;
            }
            if(GameStart.gameMode1 == "Online") 
            {
                Time.timeScale = 1;
            }
        }
        if(GameMode.Goaled || GameMode.Finished || GameMode.isGameOver) 
        {
            if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return)) 
            {
                BackToTitle();
            }
        }
    }
    [PunRPC]
    void Pause() 
    {
        if (GameSetting.startTime < 0 && Paused == 0 && GameMode.Finished == false && GameMode.Goaled == false)
        {
            Paused = 1;
            GameSetting.Playable = false;
            pauseButton.gameObject.SetActive(false);
            Settings.SettingPanelActive = true;
            Settings.inSetting = true;
            Time.timeScale = 0;
        }
        else if (Paused == 1)
        {
            Paused = 0;
            GameSetting.startTime = -1;
            GameSetting.Playable = true;
            pauseButton.gameObject.SetActive(true);
            Settings.SettingPanelActive = false;
            Settings.inSetting = false;
            Time.timeScale = 1;
        }
    }

    public void BackToTitle()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 0;
        ButtonInGame.Paused = 0;
        GameStart.inDemoPlay = false;
        GameSetting.Playable = false;
        GameStart.PlayerNumber = 1;
        Settings.SettingPanelActive = false;
        SceneManager.LoadScene("Title");
        if (GameStart.gameMode1 == "Online")
        {
            photonView.RPC("DeleatPlayer", RpcTarget.All);
            if (MatchmakingView.gameModeQuick == "Quick")
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
            }
        }
    }
    [PunRPC]
    void DeleatPlayer()
    {
        GameStart.PlayerNumber--;
    }
}

