using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Photon.Pun;
using System.IO;
using Cysharp.Threading.Tasks;
public class ButtonClick : MonoBehaviourPunCallbacks　//クリック用ボタン
{
    public static int[] sensChange = new int[4];
    SelectButton selectButton;
    GameStart gameStart;
    GameSetting gameSetting;
    //コントローラー対応 
    bool inputButton;
    float oneSecCount;
    bool inCoolDown;
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
        Initialize();
        oneSecCount = 1;
    }

    void Update()
    {
        controllerPushButton();
        if (inCoolDown) 
        {
            oneSecCount -= Time.deltaTime;
            if(oneSecCount < 0) 
            {
                inCoolDown = false;
                oneSecCount = 1;
            }
        }
    }

    void Initialize() 
    {
        //どのボタンを押されたときに処理を行うかを決定（Falseが選ばれた場合はコントローラーボタンに対応しない）
        controllerButton = selectedButton.ToString();
        if (SceneManager.GetActiveScene().name == "Title")
        {
            gameStart = GameObject.Find("Systems").GetComponent<GameStart>();
            selectButton = GameObject.Find("Systems").GetComponent<SelectButton>();
        }
        if (SceneManager.GetActiveScene().name == "Stage")
        {
            gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        }
    }
    // このスクリプトが有効になったときにイベントリスナーを登録
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // このスクリプトが無効になったときにイベントリスナーを解除
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialize();
    }
    void controllerPushButton()
    {
        //スティックや十字ボタンを数値ではなく1回入力されたという形で受け付ける処理
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



        //bool「inputButton」を設定されたボタンに対応させる
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

        //ボタンをクリックしたことに
        if ((inputButton || Input.GetKeyDown(keyBind))) //&& GameStart.buttonPushable)
        {
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
        if (direction == "UP")
        {
            generatePos.y += space;
        }
        else if (direction == "DOWN")
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
    public void yesExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
        Application.Quit();
#endif
    }
    public void noBack()
    {
        Settings.exitPanelActive = false;
    }
    //ゲームモード変更
    public void SwitchWayToPlay(string gameMode)
    {
        GameStart.gameMode1 = gameMode;
        SoundEffect.soundTrigger[2] = 1;
    }
    //Phase移動
    public void PhaseSwitch(int difference)
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase += difference;
    }
    //ルームのプレイヤーたちのステージをリーダーの値と同期
    public void SyncStage() 
    {
        if (!NetWorkMain.isOnline) { return; }
        NetWorkMain.SetCustomProps<int>("stage", GameStart.stage);
        photonView.RPC(nameof(GetCPStage), RpcTarget.All);
    }
    public void SetCustomPropsGameMode()
    {
        if (!NetWorkMain.isOnline) { return; }
        NetWorkMain.SetCustomProps<string>("gamemode", GameStart.gameMode2);
    }
    [PunRPC]
    void GetCPStage()
    {
        if (NetWorkMain.GetCustomProps<int>("stage", out var stageValue))
        {
            GameStart.stage = stageValue;
        }
    }
    //モード変更
    public void SetAndSyncGameMode(string gameMode)
    {
        //ゲームモード変更の際ステージを１にリセットする
        GameStart.gameMode2 = gameMode;
        NetWorkMain.SetCustomProps<int>("stage", 1);
        photonView.RPC(nameof(GetCustomPropsStage), RpcTarget.All);
        NetWorkMain.SetCustomProps<string>("gameMode", gameMode);
        photonView.RPC(nameof(GetCustomPropsGameMode), RpcTarget.All);
    }

    [PunRPC]
    void GetCustomPropsStage()
    {
        if (NetWorkMain.GetCustomProps<int>("stage", out int valueA))
        {
            GameStart.stage = valueA;
        }
    }
    [PunRPC]
    void GetCustomPropsGameMode()
    {
        if (NetWorkMain.GetCustomProps<string>("gameMode", out string valueB))
        {
            GameStart.gameMode2 = valueB;
        }
    }

    //ゲームモード変更
    public void SwitchGameMode(string gameMode)
    {
        GameStart.gameMode2 = gameMode;
    }


    //部屋を抜ける
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    //ステージ変更
    public void StageSwitch(int difference)
    {
        GameStart.stage += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    //プレイヤー数増減
    public void ChangePlayerNum(int difference)
    {
        GameStart.PlayerNumber += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void SettingPanelTrigger()    //設定画面の表示
    {
        Settings.SettingPanelActive = !(Settings.SettingPanelActive);
    }

    //ポーズ処理
    public void PauseGame()
    {
        if (gameSetting.isCountDownEnded && !gameSetting.isPaused && GameMode.isGameEnded == false)
        {
            gameSetting.isPaused = true;
            GameSetting.Playable = false;
            Settings.SettingPanelActive = true;
            Time.timeScale = 0;
        }
        if (GameStart.gameMode1 == "Online")
        {
            Time.timeScale = 1;
            GameSetting.Playable = true;
        }
    }

    public void RestartGame()
    {
        gameSetting.isPaused = false;
        GameSetting.Playable = true;
        Time.timeScale = 1;
    }


    public void BackToTitle()
    {
        SoundEffect.soundTrigger[2] = 1;
        if (GameStart.gameMode1 == "Online")
        {
            gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
            gameSetting.CallDeletePlayerRPC();
            if (MatchmakingView.gameModeQuick == "Quick")
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
            }
        }
        SceneManager.LoadScene("Title");
    }

    public void SaveSettingData()
    {
        //データ保存
        SaveData data = DataManager.Instance.data;
        data.languageNum = Settings.languageNum;
        data.screenModeNum = Settings.screenMode;
        data.BGM = BGM.BGMStage;
        data.SE = SoundEffect.SEStage;
    }

    public void ChangeFlagTime(int difference)
    {
        SoundEffect.soundTrigger[3] = 1;
        //オンラインならRPCで全体に適用する
        if (PhotonNetwork.InRoom && NetWorkMain.leaderId == NetWorkMain.netWorkId)
        {
            photonView.RPC(nameof(RPCChangeFlagTime), RpcTarget.All, difference);
            return;
        }
        GameStart.flagTimeLimit += difference;
    }


    [PunRPC]
    void RPCChangeFlagTime(int difference)
    {
        GameStart.flagTimeLimit += difference;
    }


    //設定画面のボタン
    public void ChangeBGMVol(int difference)
    {
        BGM.BGMStage += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void ChangeSEVol(int difference)
    {
        SoundEffect.SEStage += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void ChangeLanguage(int difference)
    {
        Settings.languageNum += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void ChangeScreenMode(int difference)
    {
        Settings.screenMode += difference;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void ChangeGuideMode(int difference)
    {
        Settings.guideMode += difference;
        SoundEffect.soundTrigger[3] = 1;
    }


    public void SyncLeader()
    {
        photonView.RPC(nameof(syncLeader), RpcTarget.All);
    }

    public void RPCSetTeam()
    {
        SoundEffect.soundTrigger[3] = 1;
        photonView.RPC(nameof(SetTeam), RpcTarget.All, NetWorkMain.netWorkId);
    }

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
        if (inCoolDown) 
        {
            return;
        }
        inCoolDown = true;  
        int targetId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));
        if (NetWorkMain.netWorkId != targetId) { return; }
        if (NetWorkMain.GetCustomProps<bool[]>("isReady", out var arrayValue1))
        {
            arrayValue1[targetId - 1] = !arrayValue1[targetId - 1];
            NetWorkMain.SetCustomProps<bool[]>("isReady", arrayValue1);
            if (arrayValue1[targetId - 1] == true)
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
