using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Photon.Pun;
public class TitleButtonClick : MonoBehaviourPunCallbacks　//クリック用ボタン
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
    //コントローラー対応 
    bool inputButton;
    [SerializeField] KeyCode keyBind;
    string controllerButton;
    bool inputCrossXPlus, inputCrossXMinus, inputCrossYPlus, inputCrossYMinus, inputLstickXPlus,  inputLstickXMinus, inputLstickYPlus, inputLstickYMinus;
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
        //どのボタンを押されたときに処理を行うかを決定（Falseが選ばれた場合はコントローラーボタンに対応しない）
        controllerButton = selectedButton.ToString();
        if (SceneManager.GetActiveScene().name == "Title")
        {
            gameStart = GameObject.Find("Systems").GetComponent<GameStart>();
            titleButton = GameObject.Find("Systems").GetComponent<TitleButton>();
        }

    }

    void Update()
    {
        controllerPushButton();
    }
    void controllerPushButton()
    {
        //スティックや十字ボタンを数値ではなく1回入力されたという形で受け付ける処理
        if (inputCrossXPlus == false && inputCrossXMinus == false)
        {
            if (ControllerInput.crossX[0] >= 0.1f) {  inputCrossXPlus = true;  }
            else if (ControllerInput.crossX[0] <= -0.1f) { inputCrossXMinus = true;  }
        }

        if (inputCrossYPlus == false && inputCrossYMinus == false)
        {
            if (ControllerInput.crossY[0] >= 0.1f) { inputCrossYPlus = true; }
            else if (ControllerInput.crossY[0] <= -0.1f) { inputCrossYMinus = true;  }
        }


        if (ControllerInput.LstickX[0] > 0.5f) { inputLstickXPlus = true; }
        else if (ControllerInput.LstickX[0] < -0.5f) { inputLstickXMinus = true; }


        if (ControllerInput.LstickY[0] > 0.5f) { inputLstickYPlus = true; }
        else if (ControllerInput.LstickY[0] < -0.5f) { inputLstickYMinus = true; }
        //入力リセット
        if (lastLstickX> 0.1f || lastLstickY < -0.1f ) 
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



        //boolプロパティ「inputButton」を設定されたボタンに対応いさせる
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
                SettingPanelTrigger();
            }
            else if (!Settings.inSetting && this.name.Contains("Pause"))
            {
                SettingPanelTrigger();
            }
            return;
        }
        //ボタンをクリックしたことに
        if ((inputButton || Input.GetKeyDown(keyBind)) && GameStart.buttonPushable)  
        {
            if (Settings.inSetting) { return; }
            GameStart.buttonPushable = false;
            this.GetComponent<Button>().onClick.Invoke();
        }


      
        //初期化
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

    //対応するボタンを表示
    void ShowTriggerButton(GameObject targetButtom, string buttonName, string direction, float space)
    {
        Transform rootTransform = transform.root;
        Vector2 generatePos = targetButtom.transform.position;
        if(direction == "UP")
        {
            generatePos.y += space;
        }
        else if(direction == "DOWN")
        {
            generatePos.y -= space;
        }
        else if (direction == "LEFT")
        {
            generatePos.x -= space;
        }
        else if (direction == "RIGHT")
        {
            generatePos.x += space;
        }
        GameObject imageObj = (GameObject)Resources.Load(buttonName);
        Instantiate(imageObj, generatePos, Quaternion.identity, rootTransform);
    }
    /*↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ボタンにアタッチするスクリプト↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓*/
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
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }

    public void SetModeMulti()
    {
        GameStart.gameMode1 = "Multi";
    }
    public void GoOnlineMode()
    {
        GameStart.gameMode1 = "Online";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    public void GoStageSelect()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 6;
    }
    public void GoTeamSelect()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 8;
    }
    public void NextPhaseOnline()
    {
        GameStart.gameMode1 = "Online";
        if (NetWorkMain.netWorkId != NetWorkMain.leaderId)
        {
            return;
        }
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    //モード変更
    public void setNomalMode()
    {
        GameStart.gameMode2 = "Nomal";
        NetWorkMain.UpdateRoomStats(1);
        photonView.RPC(nameof(SetCustomPropsStage), RpcTarget.All);
        NetWorkMain.UpdateGameMode("Nomal");
        photonView.RPC(nameof(SetCustomPropsGameMode), RpcTarget.All);
    }
    public void setArcadeMode()
    {
        GameStart.gameMode2 = "Arcade";
        NetWorkMain.UpdateRoomStats(1);
        photonView.RPC(nameof(SetCustomPropsStage), RpcTarget.All);
        NetWorkMain.UpdateGameMode("Arcade");
        photonView.RPC(nameof(SetCustomPropsGameMode), RpcTarget.All);
    }


    [PunRPC]
    void SetCustomPropsStage()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("stage"))
        {
            int stageTmp;
            if (int.TryParse(customProps["stage"].ToString(), out stageTmp))
            {
                GameStart.Stage = stageTmp;
                Debug.Log("GameStart.Stageを" + stageTmp + "に設定しました");
            }
        }
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
        GameStart.phase--;
        StopAllCoroutines();
        SoundEffect.soundTrigger[2] = 1;
    }

    //部屋を抜ける
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    //ステージ変更
    public void NextStage()
    {
        GameStart.Stage++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void PrevStage()
    {
        GameStart.Stage--;
        SoundEffect.soundTrigger[3] = 1;
    }

    //プレイヤー数増減
    public void PlusButton()
    {
        if (GameStart.PlayerNumber < GameStart.maxPlayer)
        {
            GameStart.PlayerNumber++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void MinusButton()
    {
        if (GameStart.PlayerNumber > GameStart.minPlayer)
        {
            GameStart.PlayerNumber--;
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


    [PunRPC]
    void SetCustomPropsGameMode()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("gameMode"))
        {
            GameStart.gameMode2 = customProps["gameMode"].ToString();
            Debug.Log("GameModeを" + customProps["gameMode"].ToString() + "に設定しました");
        }
    }

    
    public void SettingPanelTrigger()    //設定画面の表示
    {
        Settings.SettingPanelActive = !(Settings.SettingPanelActive);
        Settings.inSetting = !(Settings.inSetting);
    }

    public void PlusFlagTime()
    {
        SoundEffect.soundTrigger[3] = 1;
        if (PhotonNetwork.InRoom && NetWorkMain.leaderId == NetWorkMain.netWorkId)
        {
            photonView.RPC(nameof(RPCPlusFlagTime), RpcTarget.All);
            return;
        }

        GameStart.flagTimeLimit += 10;
        GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 150);
    }
    public void MinusFlagTime()
    {
        SoundEffect.soundTrigger[3] = 1;
        if (PhotonNetwork.InRoom && NetWorkMain.leaderId == NetWorkMain.netWorkId)
        {
            photonView.RPC(nameof(RPCMinusFlagTime), RpcTarget.All);
            return;
        }

        GameStart.flagTimeLimit -= 10;
        GameStart.flagTimeLimit = System.Math.Max(40, GameStart.flagTimeLimit);

    }
    [PunRPC]
    void RPCPlusFlagTime()
    {
        GameStart.flagTimeLimit += 10;
        GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 150);
    }
    [PunRPC]
    void RPCMinusFlagTime()
    {
        GameStart.flagTimeLimit -= 10;
        GameStart.flagTimeLimit = System.Math.Max(40, GameStart.flagTimeLimit);
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
        if (Settings.screenMode < 1)
        {
            Settings.screenMode++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void PrevScreenMode()
    {
        if (Settings.screenMode > 0)
        {
            Settings.screenMode--;
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


    [PunRPC]
    public void RPCsyncLeader()
    {
        photonView.RPC(nameof(syncLeader), RpcTarget.All);
    }
    [PunRPC]
    public void RPCSetTeam()
    {
        SoundEffect.soundTrigger[3] = 1;
        photonView.RPC(nameof(SetTeam), RpcTarget.All, NetWorkMain.netWorkId);
    }
    [PunRPC]
    public void RPCSetLeader()
    {
        if (NetWorkMain.netWorkId != NetWorkMain.leaderId)
        {
            return;
        }
        photonView.RPC(nameof(SetLeader), RpcTarget.All);
    }

    [PunRPC]
    void syncLeader()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        int idTmp = 1;
        if (customProps.ContainsKey("leaderID"))
        {
            if (int.TryParse(customProps["leaderId"].ToString(), out idTmp))
            {
                NetWorkMain.leaderId = idTmp;
            }
        }
    }
    [PunRPC]
    void SetLeader()
    {
        int newLeaderId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));
        NetWorkMain.leaderId = newLeaderId;

        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        int idTmp = 0;
        if (customProps.ContainsKey("leaderId"))
        {
            if (int.TryParse(customProps["leaderId"].ToString(), out idTmp))
            {
                customProps["leaderId"] = newLeaderId;
            }
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }
    public void ReadyButton()
    {
        int targetId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));
        if(NetWorkMain.netWorkId != targetId) { return; }
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("isReady"))
        {
            bool[] isReadyLocal = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["isReady"];
            isReadyLocal[targetId - 1] = !isReadyLocal[targetId - 1];
            customProps["isReady"] = isReadyLocal;
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
            if(isReadyLocal[targetId - 1] == true) 
            {
                SoundEffect.soundTrigger[10] = 1;
            }
            else 
            {
                SoundEffect.soundTrigger[9] = 1;
            }
        }
    }
    [PunRPC]
    void SetTeam(int netWorkID)
    {
        int newTeamNum = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", "")) - 1;
        GameStart.playerTeam[netWorkID - 1] = newTeamNum;
    }
    //再接続
    public void ReconnectToLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public void DisconnectRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }   
    }
    public void DisconnectLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

}
