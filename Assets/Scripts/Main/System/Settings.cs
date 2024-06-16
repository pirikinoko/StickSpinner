using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
public class Settings : MonoBehaviour
{
    [SerializeField] Text confirmText, yesText, noText, BGMText, SEText;
    [SerializeField] GameObject yesButton, noButton, resumeBtnTitle, resumeBtnGame;
    [SerializeField] GameObject[] controllerUI;
    Controller controller;
    public GameObject SettingPanel, exitPanel;
    public Text languageText, screenText, guideText;
    public static bool SettingPanelActive = false;
    bool InputCrossX, InputCrossY;
    int Selected = 0, buttonSelect = 0, itemLength  , lastScreenNum;
    public static int languageNum = 0, guideMode = 0, screenMode = 0;
    string[] languages = { "JP", "EN" };
    string[] screenModeValues = { "ウィンドウ", "フルスクリーン", "Window", "FullScreen" };
    string[] guide = { "On", "Off" };
    float[] settingStages = new float[5]; //設定項目の数
    public GameObject[] item = new GameObject[3];
    Vector2[] itemPos = new Vector2[10];
    float lastLstickX, lastLstickY;
    string sceneName;
    private Button[] activeButtons;
    //OnExitPanel
    public static bool exitPanelActive = false;
    public static bool isDataLoaded = false;                
    void Start()
    {
        
        Selected = 0;
        Time.timeScale = 1;
        lastScreenNum = screenMode;
        SetScreenMode();
        Debug.Log(languageNum);
        SettingPanelActive = false;
        exitPanelActive = false;
        GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //シーンによっての設定画面の機能の切り替え
        sceneName = SceneManager.GetActiveScene().name;
        SwitchButtonFunction(); 
        LoadSettingData();  
    }
    void FixedUpdate()
    {
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            //初期化
            Selected = 0;
            Time.timeScale = 1;
            lastScreenNum = screenMode;
            SetScreenMode();
            Debug.Log(languageNum);
            SettingPanelActive = false;
            exitPanelActive = false;
            GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            //シーンによってのBGM切り替え
            sceneName = SceneManager.GetActiveScene().name;
            SwitchButtonFunction();
        }
        //データ保存
        SaveData data = new SaveData();
        data.languageNum = languageNum;
        data.screenModeNum = screenMode;
        data.BGM = BGM.BGMStage;
        data.SE = SoundEffect.SEStage;
    }

    void Update()
    {

        if (SettingPanelActive)
        {
            SettingPanel.gameObject.SetActive(true);
            SettingControl();
            SelectEffect();
            SwitchUIMode();
            lastLstickX = ControllerInput.LstickX[0];
            lastLstickY = ControllerInput.LstickY[0];
            languageText.text = languages[languageNum];
            screenText.text = screenModeValues[(languageNum * 2) + screenMode];
            guideText.text = guide[guideMode];
            BGMText.text = BGM.BGMStage.ToString(); 
            SEText.text = SoundEffect.SEStage.ToString();
            if (languageNum == 0)
            {
                screenText.fontSize = 160;
            }
            else
            {
                screenText.fontSize = 200;
            }
        }


        else
        {
            SettingPanel.gameObject.SetActive(false);
        }
        //上限下限の制限
        languageNum = Mathf.Clamp(languageNum, 0, languages.Length - 1);
        screenMode = Mathf.Clamp(screenMode, 0, screenModeValues.Length / languages.Length);
        guideMode = Mathf.Clamp(guideMode, 0, 1);
    }

    void SettingControl()
    {

        for (int i = 0; i < item.Length; i++)
        {
            itemPos[i] = item[i].transform.position;
        }

        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (SettingPanelActive)
            {
                if (!exitPanelActive)
                {
                    exitPanel.gameObject.SetActive(false);
                    if (ControllerInput.crossX[0] == 0) { InputCrossX = false; }
                    if (ControllerInput.crossY[0] == 0) { InputCrossY = false; }
                    /*ボタン選択（縦）*/
                    if (lastLstickX > 0.1f || lastLstickX < -0.1f || lastLstickY > 0.1f || lastLstickY < -0.1f) { return; }


                    /*設定項目の選択*/
                    if (InputCrossY == false)
                    {
                        /*十字ボタン縦*/
                        if (ControllerInput.crossY[0] >= 0.1f) { Selected++; InputCrossY = true; SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.crossY[0] <= -0.1f) { Selected--; InputCrossY = true; SoundEffect.soundTrigger[3] = 1; }
                        /*十字ボタン縦*/

                        /*Lスティック縦*/
                        if (ControllerInput.LstickY[0] > 0.5f) { Selected--; SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.LstickY[0] < -0.5f) { Selected++; SoundEffect.soundTrigger[3] = 1; }
                        /*Lスティック縦*/

                        /*矢印キー縦*/
                        if (Input.GetKeyDown(KeyCode.DownArrow)) { Selected++; SoundEffect.soundTrigger[3] = 1; }
                        else if (Input.GetKeyDown(KeyCode.UpArrow)) { Selected--; SoundEffect.soundTrigger[3] = 1; }
                        /*矢印キー縦*/

                        //上限下限の設定
                        Selected = Mathf.Clamp(Selected, 0, itemLength);
                    }
                    /*設定項目の選択*/
                    activeButtons = FindObjectsOfType<Button>()
                            .Where(button => button.gameObject.activeSelf &&
                             !button.gameObject.name.Contains("Pause") &&
                             !button.gameObject.name.Contains("Resume"))
                            .ToArray();

                    if (InputCrossX == false)
                    {
                        //十字ボタン横
                        if (ControllerInput.crossX[0] >= 0.1f) { ClickSelectedButton("Right"); InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.crossX[0] <= -0.1f) { ClickSelectedButton("Left"); InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
                        //十字ボタン横


                        //Lスティック横
                        if (ControllerInput.LstickX[0] > 0.5f) { ClickSelectedButton("Right"); SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.LstickX[0] < -0.5f) { ClickSelectedButton("Left"); SoundEffect.soundTrigger[3] = 1; }


                        //矢印キー横
                        if (Input.GetKeyDown(KeyCode.RightArrow)) { ClickSelectedButton("Right"); SoundEffect.soundTrigger[3] = 1; }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow)) { ClickSelectedButton("Left"); SoundEffect.soundTrigger[3] = 1; }
                        //矢印キー横

                    }
                }

                /*ゲーム終了*/
                if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                {
                    if (GameSetting.Playable == false && !(exitPanelActive))
                    {
                        ClickSelectedButton("Item");
                        return;
                    }
                }
               
                //ゲーム終了画面表示時
                if (exitPanelActive)
                {

                    exitPanel.gameObject.SetActive(true);
                    if (ControllerInput.crossX[0] == 0) { InputCrossX = false; }
                    if (ControllerInput.crossY[0] == 0) { InputCrossY = false; }
                    Button yes = yesButton.gameObject.GetComponent<Button>();
                    Button no = noButton.gameObject.GetComponent<Button>();
                    RectTransform yesButtonRect = yes.GetComponent<RectTransform>();
                    RectTransform noButtonRect = no.GetComponent<RectTransform>();
                    if (buttonSelect == 0)
                    {
                        yesButtonRect.sizeDelta = new Vector2(350, 175);
                        noButtonRect.sizeDelta = new Vector2(300, 150);
                        if (ControllerInput.jump[0] || ControllerInput.next[0] || Input.GetKeyDown(KeyCode.Return))
                        {
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
                            Application.Quit();//ゲームプレイ終了
#endif
                        }
                        else if (ControllerInput.back[0] )
                        {
                            exitPanelActive = false;
                        }
                    }
                    else
                    {
                        noButtonRect.sizeDelta = new Vector2(350, 175);
                        yesButtonRect.sizeDelta = new Vector2(300, 150);
                        if (ControllerInput.jump[0] || ControllerInput.next[0] || Input.GetKeyDown(KeyCode.Return) || ControllerInput.back[0])
                        {
                            exitPanelActive = false;
                        }
                    }
                    /*数値変更*/
                    if (InputCrossX == false)
                    {
                        /*十字ボタン横*/
                        if (ControllerInput.crossX[0] >= 0.1f) { buttonSelect++; InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.crossX[0] <= -0.1f) { buttonSelect--; InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
                        /*十字ボタン横*/


                        /*Lスティック横*/
                        if (ControllerInput.LstickX[0] > 0.5f) { buttonSelect++; SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.LstickX[0] < -0.5f) { buttonSelect--; SoundEffect.soundTrigger[3] = 1; }

                        /*矢印キー横*/
                        if (Input.GetKeyDown(KeyCode.RightArrow)) { buttonSelect++; SoundEffect.soundTrigger[3] = 1; }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow)) { buttonSelect--; SoundEffect.soundTrigger[3] = 1; }
                        /*矢印キー横*/
                    }
                    /*数値変更*/
                    buttonSelect = Mathf.Clamp(buttonSelect, 0, 1);

                }
                /*ゲーム終了*/
            }
        }
        //ウィンドウモード切替
        if(lastScreenNum != screenMode)
        {
            SetScreenMode();
        }
        lastScreenNum = screenMode;
        
    }

    private void SetScreenMode()
    {
        // 0のときはフルスクリーン
        if (screenMode == 0)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        // 1のときはウィンドウモード
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
    }

    //選択されている項目を強調する
    void SelectEffect()
    {
        Text[] stageText = FindObjectsOfType<Text>()
            .Where(text => text.gameObject.activeSelf &&
                             text.gameObject.name.Contains("Stage"))
            .ToArray();
        Text[] itemText = FindObjectsOfType<Text>()
           .Where(item => item.gameObject.activeSelf &&
                            item.gameObject.name.Contains("Item"))
           .ToArray();
        itemLength = stageText.Length　-1;
        for (int i = 0; i < itemText.Length; i++)
        {
            itemText[i].color = Color.white;

            if (itemText[i].name.Contains((Selected + 1).ToString()))
            {
                itemText[i].color = Color.yellow;
            }
        }
        for (int i = 0; i < stageText.Length; i++)
        {
            stageText[i].color = Color.white;
            if (stageText[i].name.Contains((Selected + 1).ToString()))
            {
                stageText[i].color = Color.yellow;
            }
        }
    }

    void ClickSelectedButton(string buttonName) 
    {
        for (int i = 0; i < activeButtons.Length; i++)
        {
            if (activeButtons[i].name.Contains(buttonName) && activeButtons[i].name.Contains((Selected + 1).ToString())) 
            {
                activeButtons[i].onClick.Invoke();
            }    
        }
    }

    void SwitchUIMode() 
    {
        //キーボード,マウスのとき
        if (!(ControllerInput.usingController))
        {
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {

            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(true); }
        }
    }

    void SwitchButtonFunction() 
    {
        Button exitButton = item[5].GetComponent<Button>();
        exitButton.onClick.RemoveAllListeners();
        SwitchLanguage switchLanguage = item[5].transform.GetChild(0).gameObject.GetComponent<SwitchLanguage>();
        //ゲーム内とタイトルで挙動が違うボタンの管理
        if (SceneManager.GetActiveScene().name == "Title")
        {
            resumeBtnTitle.SetActive(true);
            resumeBtnGame.SetActive(false);
            //タイトルではゲーム終了ボタン
            switchLanguage.texts[0] = "ゲーム終了";
            switchLanguage.texts[1] = "QuitGame";
            exitButton.onClick.AddListener(() => exitPanelActive = true);
            Debug.Log("タイトルボタン");
        }
        else
        {
            resumeBtnGame.SetActive(true);
            resumeBtnTitle.SetActive(false);
            //ゲーム中ではタイトルに戻るボタン
            switchLanguage.texts[0] = "タイトルに戻る";
            switchLanguage.texts[1] = "BackToTitle";
            exitButton.onClick.AddListener(() => item[5].GetComponent<ButtonClick>().BackToTitle());
            Debug.Log("タイトルボタン");
        }
    }

    void LoadSettingData() 
    {
        if (!isDataLoaded)
        {
            var data = FindObjectOfType<DataManager>().data;
            languageNum = data.languageNum;
            screenMode = data.screenModeNum;
            BGM.BGMStage = data.BGM;
            SoundEffect.SEStage = data.SE;
        }
    }
}
