using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CI.QuickSave;
using System.Linq;
public class Settings : MonoBehaviour
{
    public GameObject SettingPanel, exitPanel;
    public Text languageText, screenText, guideText;
    public static bool SettingPanelActive = false, inSetting = false;
    bool InputCrossX, InputCrossY;
    int Selected = 0;
    public static int languageNum = 0, guideMode = 0;
    string[] languages = { "JP", "EN" };
    public static int screenModeNum = 0;
    string[] screenMode = { "フルスクリーン", "ウィンドウ", "FullScreen", "Window" };
    string[] guide = { "On", "Off" };
    float[] settingStages = new float[5]; //設定項目の数
    int changeCount = 0, lastScreenNum;
    int max, min = 0;
    public GameObject[] item = new GameObject[3];
    Vector2[] itemPos = new Vector2[10];
    public static float[] rotStage = { 10, 10, 10, 10 }; //感度を保存しておく
    float lastLstickX, lastLstickY;
    Controller controller;
    private Button[] activeButtons;
    //OnExitPanel
    public static bool exitPanelActive = false;
    [SerializeField] Text confirmText, yesText, noText;
    [SerializeField] GameObject yesButton, noButton;
    int buttonSelect = 0;

    void Start()
    {
        Selected = 0;
        changeCount = 0;
        Time.timeScale = 1;
        SettingPanelActive = false;
        inSetting = false;
        exitPanelActive = false;
    }

    void Update()
    {

        SetScreenMode();
        if (SettingPanelActive)
        {
            SettingPanel.gameObject.SetActive(true);
            SettingControl();
            SelectEffect();
            lastLstickX = ControllerInput.LstickX[0];
            lastLstickY = ControllerInput.LstickY[0];
            languageText.text = languages[languageNum];
            screenText.text = screenMode[(languageNum * 2) + screenModeNum];
            guideText.text = guide[guideMode];
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

    }
    void FixedUpdate() 
    {

    }

    void SettingControl()
    {

        for (int i = 0; i < item.Length; i++)
        {
            itemPos[i] = item[i].transform.position;
        }

        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (inSetting)
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
                        Selected = Mathf.Clamp(Selected, min, max);
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
            //上限下限の設定
            rotStage[i] = System.Math.Min(rotStage[i], 20);
            rotStage[i] = System.Math.Max(rotStage[i], 1);
        }

        if(lastScreenNum != screenModeNum)
        {
            changeCount = 0;
        }
        lastScreenNum = screenModeNum;
        
    }

    private void SetScreenMode()
    {
        if(changeCount == 0)
        {
            // ウィンドウモード、フルスクリーンモード、擬似フルスクリーンモードの設定
            if (screenModeNum == 0)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            }
            changeCount = 1;
        }
    
    }

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
        max = stageText.Length　-1;
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
}
