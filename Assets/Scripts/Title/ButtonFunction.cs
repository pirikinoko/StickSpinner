using Cysharp.Threading.Tasks;
using Fusion;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonFunction : NetworkBehaviour
{
    [SerializeField]
    private bool haveCooldown = false;

    [SerializeField]
    private bool isLeaderOnly = false;

    public static int[] sensChange = new int[4];

    private Image image;
    private Color imageColor;
    private SelectButton selectButton;
    private GameStartHandler gameStart;
    private GamePlayHandler gamePlayHandler;
    private Button button;
    private SessionManager _sessionManager;
    private int cooldownMills = 500;

    private bool inputButton;

    [SerializeField]
    private KeyCode keyBind;

    private string controllerButton;
    private bool inputCrossXPlus;
    private bool inputCrossXMinus;
    private bool inputCrossYPlus;
    private bool inputCrossYMinus;
    private bool inputLstickXPlus;
    private bool inputLstickXMinus;
    private bool inputLstickYPlus;
    private bool inputLstickYMinus;
    private float lastLstickX;
    private float lastLstickY;

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

    [SerializeField]
    private ControllerButtons selectedButton;

    void Start()
    {
        Initialize();
        _sessionManager = SessionManager.Instance;
    }

    private void Update()
    {
        OnControllerButtonPressed();
        OnlineRoomButtonBehavior();
    }

    void Initialize()
    {
        controllerButton = selectedButton.ToString();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnButtonClick().Forget());
        image = this.gameObject.GetComponent<Image>();
        imageColor = image.color;
        if (SceneManager.GetActiveScene().name == "Title")
        {
            gameStart = GameObject.Find("TitleHandler").GetComponent<GameStartHandler>();
            selectButton = GameObject.Find("TitleHandler").GetComponent<SelectButton>();
        }
        if (SceneManager.GetActiveScene().name == "Stage")
        {
            gamePlayHandler = GameObject.Find("PlaySceneManager").GetComponent<GamePlayHandler>();
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
            if (ControllerInputManager.crossX[0] >= 0.1f) { inputCrossXPlus = true; }
            else if (ControllerInputManager.crossX[0] <= -0.1f) { inputCrossXMinus = true; }
        }

        if (inputCrossYPlus == false && inputCrossYMinus == false)
        {
            if (ControllerInputManager.crossY[0] >= 0.1f) { inputCrossYPlus = true; }
            else if (ControllerInputManager.crossY[0] <= -0.1f) { inputCrossYMinus = true; }
        }


        if (ControllerInputManager.LstickX[0] > 0.5f) { inputLstickXPlus = true; }
        else if (ControllerInputManager.LstickX[0] < -0.5f) { inputLstickXMinus = true; }


        if (ControllerInputManager.LstickY[0] > 0.5f) { inputLstickYPlus = true; }
        else if (ControllerInputManager.LstickY[0] < -0.5f) { inputLstickYMinus = true; }
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
        if (ControllerInputManager.crossX[0] == 0)
        {
            inputCrossXPlus = false;
            inputCrossXMinus = false;
        }
        if (ControllerInputManager.crossY[0] == 0)
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
                inputButton = ControllerInputManager.jump[0];
                break;

            case ControllerButtons.next:
                inputButton = ControllerInputManager.next[0];
                break;

            case ControllerButtons.back:
                inputButton = ControllerInputManager.back[0];
                break;

            case ControllerButtons.plus:
                inputButton = ControllerInputManager.plus[0];
                break;

            case ControllerButtons.minus:
                inputButton = ControllerInputManager.minus[0];
                break;

            case ControllerButtons.start:
                inputButton = ControllerInputManager.start[0];
                break;
        }

        //ボタンをクリックしたことに
        if (inputButton || Input.GetKeyDown(keyBind)) //&& GameStart.buttonPushable)
        {
            GameStartHandler.ButtonPushable = false;
            this.GetComponent<Button>().onClick.Invoke();
        }

        //初期化
        lastLstickX = ControllerInputManager.LstickX[0];
        lastLstickY = ControllerInputManager.LstickY[0];

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
        if (!_sessionManager.NetworkRunner.IsInSession || SceneManager.GetActiveScene().name != "Title")
        {
            return;
        }

        if (this.gameObject.name.Contains("Star") && !this.gameObject.name.Contains("Game"))
        {
            int starID = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));

            Material iconMat = new Material(this.GetComponent<Image>().material);

            Color color = iconMat.color;

            color.a = 0.2f;
            if (starID == _sessionManager.LeaderId)
            {
                color.a = 1;
            }

            iconMat.color = color;

            this.GetComponent<Image>().material = iconMat;
        }
        else if (this.gameObject.name.Contains("Ready"))
        {
            int id = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));

            Material iconMat = new Material(this.GetComponent<Image>().material);
            Color color = iconMat.color;
            color.a = 0.2f;

            if (_sessionManager.IsReadyToSwitchScene[id - 1] == true)
            {
                color.a = 1;
            }

            iconMat.color = color;
            this.GetComponent<Image>().material = iconMat;
        }

        if (this.name.Contains("TeamSelect"))
        {
            button.interactable = _sessionManager.GameMode == "Arcade" && GameStartHandler.PlayerCount > 1;
        }

        if (_sessionManager.PlayingType == "Online" && this.gameObject.activeSelf)
        {
            if (!isLeaderOnly)
            {
                return;
            }
            button.interactable = _sessionManager.NetworkRunner.LocalPlayer.PlayerId == _sessionManager.LeaderId;
        }
    }
    /*↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ボタンにアタッチするスクリプト↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓*/

    public void yesExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void noBack()
    {
        Settings.exitPanelActive = false;
    }

    public void SwitchWayToPlay(string playingType)
    {
        _sessionManager.PlayingType = playingType;
        SoundEffect.soundTrigger[2] = 1;
    }

    public void PhaseSwitch(int difference)
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStartHandler.Phase += difference;
    }


    public void ChangeGameMode(string gameMode)
    {
        //ゲームモード変更の際ステージを１にリセットする
        _sessionManager.GameMode = gameMode;
        _sessionManager.Stage = 1;
    }

    public void LeaveRoom()
    {
        _sessionManager.NetworkRunner.Disconnect(_sessionManager.NetworkRunner.LocalPlayer);
    }


    public void SwitchStage(int difference)
    {
        _sessionManager.Stage += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void ChangePlayerCount(int difference)
    {
        GameStartHandler.PlayerCount += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void ToggleSettingPanel()
    {
        Settings.SettingPanelActive = !Settings.SettingPanelActive;
    }

    public void PauseGame()
    {
        if (gamePlayHandler.IsCountDownEnded && !gamePlayHandler.IsPaused && GameRuleManager.IsGameEnded == false)
        {
            gamePlayHandler.IsPaused = true;
            GamePlayHandler.Playable = false;
            Settings.SettingPanelActive = true;
            Time.timeScale = 0;
        }
        if (_sessionManager.PlayingType == "Online")
        {
            Time.timeScale = 1;
            GamePlayHandler.Playable = true;
        }
    }

    public void UnpauseGame()
    {
        gamePlayHandler.IsPaused = false;
        GamePlayHandler.Playable = true;
        Time.timeScale = 1;
    }

    public void BackToTitle()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            return;
        }

        if (_sessionManager.PlayingType == "Online")
        {
            gamePlayHandler = FindAnyObjectByType<GamePlayHandler>();
        }

        SceneManager.LoadScene("Title");
        SoundEffect.soundTrigger[2] = 1;
    }

    public void SaveSettingData()
    {
        SaveData data = DataManager.Instance.data;
        data.languageNum = Settings.languageNum;
        data.screenModeNum = Settings.screenMode;
        data.BGM = BGM.BGMStage;
        data.SE = SoundEffect.SEStage;
    }

    public void ChangeFlagTime(int difference)
    {
        _sessionManager.TimeLimit += difference;
        SoundEffect.soundTrigger[3] = 1;
    }

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

    public void SetTeam()
    {
        int newTeam = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", "")) - 1;

        _sessionManager.PlayerTeamToSend[_sessionManager.NetworkRunner.LocalPlayer.PlayerId] = newTeam;
        SoundEffect.soundTrigger[3] = 1;
    }

    public void SetLeader()
    {
        if (_sessionManager.NetworkRunner.LocalPlayer.PlayerId != _sessionManager.LeaderId)
        {
            return;
        }
        _sessionManager.LeaderId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", "")) - 1;
    }

    public void ReadyButton()
    {
        int buttonId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", "")) - 1;

        if (_sessionManager.NetworkRunner.LocalPlayer.PlayerId != buttonId) { return; }

        if (_sessionManager.IsReadyToSwitchScene[_sessionManager.NetworkRunner.LocalPlayer.PlayerId] == true)
        {
            SoundEffect.soundTrigger[10] = 1;
        }
        else
        {
            SoundEffect.soundTrigger[9] = 1;
        }

    }

    public void DisconnectRoom()
    {
        if (_sessionManager.NetworkRunner.IsInSession)
        {
            _sessionManager.NetworkRunner.Disconnect(_sessionManager.NetworkRunner.LocalPlayer);
        }
    }
}
