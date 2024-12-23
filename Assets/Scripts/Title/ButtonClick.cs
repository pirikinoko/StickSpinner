using Cysharp.Threading.Tasks;
using Fusion;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonClick : NetworkBehaviour
{
    [SerializeField]
    bool haveCooldown = false;
    [SerializeField]
    bool isLeaderOnly = false;
    public static int[] sensChange = new int[4];
    Image image;
    Color imageColor;
    SelectButton selectButton;
    GameStartHandler gameStart;
    GameSetting gameSetting;
    Button button;
    int cooldownMills = 500;

    //コントローラー対応 
    bool inputButton;
    bool onCooldown;
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
    }

    async UniTask Update()
    {
        OnControllerButtonPressed();
        OnlineRoomButtonBehavior();
    }

    void Initialize()
    {
        //どのボタンを押されたときに処理を行うかを決定（Falseが選ばれた場合はコントローラーボタンに対応しない）
        controllerButton = selectedButton.ToString();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnButtonClick().Forget());
        image = this.gameObject.GetComponent<Image>();
        imageColor = image.color;
        if (SceneManager.GetActiveScene().name == "Title")
        {
            gameStart = GameObject.Find("Systems").GetComponent<GameStartHandler>();
            selectButton = GameObject.Find("Systems").GetComponent<SelectButton>();
        }
        if (SceneManager.GetActiveScene().name == "Stage")
        {
            gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        }
    }
    async UniTask OnButtonClick()
    {
        if (!haveCooldown)
        {
            return;
        }
        button.interactable = false;
        await UniTask.Delay(cooldownMills);
        button.interactable = true;
    }
    // このスクリプトが有効になったときにイベントリスナーを登録
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // このスクリプトが無効になったときにイベントリスナーを解除
    void OnDisable()
    {
        onCooldown = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialize();
    }
    void OnControllerButtonPressed()
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
        if (inputButton || Input.GetKeyDown(keyBind)) //&& GameStart.buttonPushable)
        {
            GameStartHandler.ButtonPushable = false;
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

    void OnlineRoomButtonBehavior()
    {
        if (!PhotonNetwork.InRoom || SceneManager.GetActiveScene().name != "Title")
        {
            return;
        }
        if (this.gameObject.name.Contains("Star") && !this.gameObject.name.Contains("Game"))
        {
            int starID = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));

            // オリジナルのマテリアルをコピー
            Material iconMat = new Material(this.GetComponent<Image>().material);

            Color color = iconMat.color;

            color.a = 0.2f;
            if (starID == NetWorkMain.leaderId)
            {
                color.a = 1;
            }

            iconMat.color = color;

            // 新しいマテリアルをImageコンポーネントに設定
            this.GetComponent<Image>().material = iconMat;
        }
        else if (this.gameObject.name.Contains("Ready"))
        {
            if (NetWorkMain.GetCustomProps<bool[]>("isReady", out var valueArrayA))
            {
                int myId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));

                Material iconMat = new Material(this.GetComponent<Image>().material);
                Color color = iconMat.color;
                color.a = 0.2f;
                if (valueArrayA[myId - 1] == true)
                {
                    color.a = 1;
                }
                iconMat.color = color;
                this.GetComponent<Image>().material = iconMat;
            }
        }

        //チームセレクトボタンのアクティブ
        if (this.name.Contains("TeamSelect"))
        {
            button.interactable = GameStartHandler.GameMode2 == "Arcade" && GameStartHandler.PlayerCount > 1;
        }

        if (GameStartHandler.GameMode1 == "Online" && this.gameObject.activeSelf)
        {
            if (!isLeaderOnly)
            {
                return;
            }
            button.interactable = NetWorkMain.NetWorkId == NetWorkMain.leaderId;
        }
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
        GameStartHandler.GameMode1 = gameMode;
        SoundEffect.soundTrigger[2] = 1;
    }
    //Phase移動
    public void PhaseSwitch(int difference)
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStartHandler.Phase += difference;
    }
    //ルームのプレイヤーたちのステージをリーダーの値と同期
    public void SyncStage()
    {
        if (!NetWorkMain.isOnline) { return; }
        NetWorkMain.SetCustomProps<int>("stage", GameStartHandler.Stage);
        RPC_GetCPStage();
    }

    public void SetCustomPropsGameMode()
    {
        if (!NetWorkMain.isOnline) { return; }
        NetWorkMain.SetCustomProps<string>("gamemode", GameStartHandler.GameMode2);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_GetCPStage()
    {
        if (NetWorkMain.GetCustomProps<int>("stage", out var stageValue))
        {
            GameStartHandler.Stage = stageValue;
        }
    }

    //モード変更
    public void SetAndSyncGameMode(string gameMode)
    {
        //ゲームモード変更の際ステージを１にリセットする
        GameStartHandler.GameMode2 = gameMode;
        NetWorkMain.SetCustomProps<int>("stage", 1);
        GameStartHandler.Stage = RPC_GetCustomPropsStage();
        NetWorkMain.SetCustomProps<string>("gameMode", gameMode);
        GameStartHandler.GameMode2 = RPC_GetCustomPropsGameMode();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private int RPC_GetCustomPropsStage()
    {
        int stage = 0;
        if (NetWorkMain.GetCustomProps<int>("stage", out int valueA))
        {
            stage = valueA;
        }
        return stage;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private string RPC_GetCustomPropsGameMode()
    {
        string gamemode = "";
        if (NetWorkMain.GetCustomProps<string>("gameMode", out string valueB))
        {
            gamemode = valueB;
        }
        return gamemode;
    }

    //ゲームモード変更
    public void SwitchGameMode(string gameMode)
    {
        GameStartHandler.GameMode2 = gameMode;
    }


    //部屋を抜ける
    public void LeaveRoom()
    {
        Runner.LeaveRoom();
    }


    //ステージ変更
    public void StageSwitch(int difference)
    {
        GameStartHandler.Stage += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    //プレイヤー数増減
    public void ChangePlayerNum(int difference)
    {
        GameStartHandler.PlayerCount += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void SettingPanelTrigger()    //設定画面の表示
    {
        Settings.SettingPanelActive = !Settings.SettingPanelActive;
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
        if (GameStartHandler.GameMode1 == "Online")
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
        if (SceneManager.GetActiveScene().name == "Title")
        {
            return;
        }

        SoundEffect.soundTrigger[2] = 1;
        if (GameStartHandler.GameMode1 == "Online")
        {
            gameSetting = FindAnyObjectByType<GameSetting>();

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
        if (PhotonNetwork.InRoom && NetWorkMain.leaderId == NetWorkMain.NetWorkId)
        {
            RPC_ChangeFlagTime(difference);
            return;
        }
        GameStartHandler.FlagTimeLimit += difference;
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_ChangeFlagTime(int difference)
    {
        GameStartHandler.FlagTimeLimit += difference;
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
        Settings.ClampFields();
        SoundEffect.soundTrigger[3] = 1;
    }

    public void ChangeScreenMode(int difference)
    {
        Settings.screenMode += difference;
        Settings.ClampFields();
        SoundEffect.soundTrigger[3] = 1;
    }
    public void ChangeGuideMode(int difference)
    {
        Settings.guideMode += difference;
        Settings.ClampFields();
        SoundEffect.soundTrigger[3] = 1;
    }


    public void SyncLeader()
    {
        RPC_SyncLeader();
    }

    public void RPCSetTeam()
    {
        SoundEffect.soundTrigger[3] = 1;
        RPC_SetTeam(NetWorkMain.NetWorkId);
    }

    public void RPCSetLeader()
    {
        if (NetWorkMain.NetWorkId != NetWorkMain.leaderId)
        {
            return;
        }
        RPC_SetLeader();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SyncLeader()
    {
        var customProps = Runner.SessionInfo.Properties;
        int idTmp = 1;
        if (customProps.TryGetValue("leaderID", out var leaderId))
        {
            if (int.TryParse(leaderId.ToString(), out idTmp))
            {
                NetWorkMain.leaderId = idTmp;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetLeader()
    {
        int newLeaderId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));
        NetWorkMain.leaderId = newLeaderId;
        NetWorkMain.SetCustomProps("leaderId", newLeaderId);
    }

    public void ReadyButton()
    {
        int targetId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));
        if (NetWorkMain.NetWorkId != targetId) { return; }
        if (NetWorkMain.GetCustomProps<bool[]>("isReady", out var arrayValue1))
        {
            arrayValue1[targetId - 1] = !arrayValue1[targetId - 1];
            NetWorkMain.SetCustomProps("isReady", arrayValue1);
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetTeam(int netWorkID)
    {
        int newTeamNum = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", "")) - 1;
        GameStartHandler.PlayerTeam[netWorkID - 1] = newTeamNum;
    }

    // 再接続
    public void ReconnectToLobby()
    {
        if (Runner.IsInSession)
        {
            Runner.LeaveSession();
        }
        else
        {
            Runner.JoinSession();
        }
    }

    public void DisconnectRoom()
    {
        if (Runner.IsInSession)
        {
            Runner.LeaveSession();
        }
    }

    public void DisconnectLobby()
    {
        Runner.LeaveSession();
    }


}
