using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Photon.Pun;
public class ButtonClick : MonoBehaviourPunCallbacks　//クリック用ボタン
{
    public GameObject pauseButton;
    private GameSetting gameSetting;

    bool inputButton;
    [SerializeField] KeyCode keyBind;
    string controllerButton;
    bool inputCrossXPlus, inputCrossXMinus, inputCrossYPlus, inputCrossYMinus, inputLstickXPlus, inputLstickXMinus, inputLstickYPlus, inputLstickYMinus;
    float lastLstickX, lastLstickY;

    //対応するコントローラーボタンの選択肢
    public enum ControllerButtons
    {
        False,
        LstickXPlus,
        LstickXMinus,
        LStickYPlus,
        LStickYMinus,
        crossXPlus,
        crossXMinus,
        crossYPlus,
        crossYMinus,
        jump,
        next,
        back,
        plus,
        minus,
        start
    }

    // インスペクターから選択したいEnumのフィールド
    [SerializeField]
    private ControllerButtons selectedButton;

    void Start()
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        controllerButton = selectedButton.ToString();
    }

    void Update()
    {
        controllerPushButton();
    }
    void controllerPushButton()
    {
        if (inputCrossXPlus == false && inputCrossXMinus == false)
        {
            if (ControllerInput.crossX[0] >= 0.1f) { inputCrossXPlus = true; }
            else if (ControllerInput.crossX[0] <= -0.1f) { inputCrossXMinus = true; }
        }

        if (inputCrossYPlus == false && inputCrossYMinus == false)
        {
            if (ControllerInput.crossY[0] >= 0.1f) { inputCrossYPlus = true; }
            else if (ControllerInput.crossY[0] <= -0.1f) { inputCrossYMinus = true; }
        }


        if (ControllerInput.LstickX[0] > 0.5f) { inputLstickXPlus = true; }
        else if (ControllerInput.LstickX[0] < -0.5f) { inputLstickXMinus = true; }


        if (ControllerInput.LstickY[0] > 0.5f) { inputLstickYPlus = true; }
        else if (ControllerInput.LstickY[0] < -0.5f) { inputLstickYMinus = true; }
        //入力リセット
        if (lastLstickX > 0.1f || lastLstickY < -0.1f)
        {
            inputLstickXPlus = false;
            inputLstickXMinus = false;
        }
        if (lastLstickY > 0.1f || lastLstickY < -0.1f)
        {
            inputLstickYPlus = false;
            inputLstickYMinus = false;
        }
        if (ControllerInput.crossX[0] == 0)
        {
            inputCrossXPlus = false;
            inputCrossXMinus = false;
        }
        if (ControllerInput.crossY[0] == 0)
        {
            inputCrossYPlus = false;
            inputCrossYMinus = false;
        }




        switch (selectedButton)
        {
            case ControllerButtons.False:
                inputButton = false;
                break;
            case ControllerButtons.LstickXPlus:
                inputButton = inputLstickXPlus;
                break;

            case ControllerButtons.LstickXMinus:
                inputButton = inputLstickXMinus;
                break;

            case ControllerButtons.LStickYPlus:
                inputButton = inputLstickYPlus;
                break;

            case ControllerButtons.LStickYMinus:
                inputButton = inputLstickYMinus;
                break;

            case ControllerButtons.crossXPlus:
                inputButton = inputCrossXPlus;
                break;

            case ControllerButtons.crossXMinus:
                inputButton = inputCrossXMinus;
                break;

            case ControllerButtons.crossYPlus:
                inputButton = inputCrossYPlus;
                break;

            case ControllerButtons.crossYMinus:
                inputButton = inputCrossYMinus;
                break;

            case ControllerButtons.jump:
                inputButton = ControllerInput.jump[0];
                break;

            case ControllerButtons.next:
                inputButton = ControllerInput.next[0];
                break;

            case ControllerButtons.back:
                inputButton = ControllerInput.back[0];
                break;

            case ControllerButtons.plus:
                inputButton = ControllerInput.plus[0];
                break;

            case ControllerButtons.minus:
                inputButton = ControllerInput.minus[0];
                break;

            case ControllerButtons.start:
                inputButton = ControllerInput.start[0];
                break;
        }
        //設定画面オンオフ
        if (Input.GetKeyDown(KeyCode.Escape) || ControllerInput.start[0])
        {
            if (Settings.inSetting && this.name.Contains("Resume"))
            {   
                RestartButton();    
            }
            else if (!Settings.inSetting && this.name.Contains("Pause"))
            {
                SettingPanelTrigger();
                Debug.Log("ポーズします");
            }
            return;
        }
        //ボタンをクリックしたことに
        if (inputButton || Input.GetKeyDown(keyBind))
        {
            if (Settings.inSetting) { return; }
            this.GetComponent<Button>().onClick.Invoke();
        }




        lastLstickX = ControllerInput.LstickX[0];
        lastLstickY = ControllerInput.LstickY[0];

        inputLstickXPlus = false;
        inputLstickXMinus = false;
        inputLstickYPlus = false;
        inputLstickYMinus = false;
        inputCrossXPlus = false;
        inputCrossXMinus = false;
        inputCrossYPlus = false;
        inputCrossYMinus = false;

    }

    public void SettingPanelTrigger()    //設定画面の表示
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

    public void BackToTitle()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 0;
        ButtonInGame.Paused = 0;
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
