using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Settings : MonoBehaviour
{
    public GameObject SettingPanel, TLFrame, exitPanel;
    public Text[] targetText;
    public Text languageText;
    public GameObject[] targetObject;
    public static bool SettingPanelActive = false, inSetting = false;
    bool InputCrossX, InputCrossY;
    int Selected = 0;
    public static int languageNum = 0;
    string[] languages = { "JP", "EN" };
    float[] settingStages = new float[3];
    int max, min = 0;
    public GameObject[] item = new GameObject[3];
    Vector2[] itemPos = new Vector2[10];
    public static float[] rotStage = { 10, 10, 10, 10 }; //感度を保存しておく
    float lastLstickX, lastLstickY;
    Controller controller;
    //OnExitPanel
    public static bool exitPanelActive = false;
    [SerializeField] Text confirmText, yesText, noText;
    [SerializeField] GameObject yesButton, noButton;
    int buttonSelect = 0;
    void Start()
    {
        Selected = 0;
        SettingPanelActive = false;
        inSetting = false;
        exitPanelActive = false;
    }
    void Update()
    {
        Debug.Log(languageNum);
        if (SettingPanelActive)
        {
            SettingPanel.gameObject.SetActive(true);
        }
        else
        {
            SettingPanel.gameObject.SetActive(false);
        }
        SettingControl();
        SelectEffect();
        lastLstickX = ControllerInput.LstickX[0];
        lastLstickY = ControllerInput.LstickY[0];
        languageText.text = languages[languageNum];

    }


    //@@一旦保留
    void SettingControl()
    {

        //設定項目の割り当て
        settingStages[0] = BGM.BGMStage;
        settingStages[1] = SoundEffect.SEStage;
        settingStages[2] = languageNum;
        max = item.Length - 1;
        for (int i = 0; i < item.Length; i++)
        {
            itemPos[i] = item[i].transform.position;
        }

        Transform TLFrameTransform = TLFrame.transform;
        Vector2 TLFramePos = TLFrameTransform.position;
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

                        //上限下限の設定
                        Selected = Mathf.Clamp(Selected, min, max);
                    }
                    /*設定項目の選択*/

                    /*数値変更*/
                    if (InputCrossX == false)
                    {
                        /*十字ボタン横*/
                        if (ControllerInput.crossX[0] >= 0.1f) { settingStages[Selected]++; InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.crossX[0] <= -0.1f) { settingStages[Selected]--; InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
                        /*十字ボタン横*/


                        /*Lスティック横*/
                        if (ControllerInput.LstickX[0] > 0.5f) { settingStages[Selected]++; SoundEffect.soundTrigger[3] = 1; }
                        else if (ControllerInput.LstickX[0] < -0.5f) { settingStages[Selected]--; SoundEffect.soundTrigger[3] = 1; }
                    }
                    /*数値変更*/
                }


                /*ゲーム終了*/
                if (ControllerInput.back[0] || Input.GetKeyDown(KeyCode.Return))
                {
                    exitPanelActive = true;
                }
                if (exitPanelActive)
                {
                    exitPanel.gameObject.SetActive(true);

                    Button yes = yesButton.gameObject.GetComponent<Button>();
                    Button no =  noButton.gameObject.GetComponent<Button>();
                    RectTransform yesButtonRect = yes.GetComponent<RectTransform>();
                    RectTransform noButtonRect = no.GetComponent<RectTransform>();
                    if(buttonSelect == 0)
                    {
                        yesButtonRect.sizeDelta = new Vector2(350, 175);
                        noButtonRect.sizeDelta = new Vector2(300, 150);
                        if (ControllerInput.jump[0] || ControllerInput.next[0] || Input.GetKeyDown(KeyCode.Return))
                        {
                            UnityEditor.EditorApplication.isPlaying = false;
                            Application.Quit();
                        }
                    }
                    else
                    {
                        noButtonRect.sizeDelta = new Vector2(350, 175);
                        yesButtonRect.sizeDelta = new Vector2(300, 150);
                        if (ControllerInput.jump[0] || ControllerInput.next[0] || Input.GetKeyDown(KeyCode.Return))
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

        //変更した数値を各スクリプトに反映
        BGM.BGMStage = settingStages[0];
        SoundEffect.SEStage = settingStages[1];
        languageNum = (int)settingStages[2];
        languageNum = Mathf.Clamp(languageNum, 0, 1);
        TLFramePos.y = itemPos[Selected].y;
        TLFrameTransform.position = TLFramePos;

    }

    void SelectEffect()
    {
        for (int i = 0; i < targetText.Length; i++)
        {
            targetText[i].color = Color.white;
        }
        switch (Selected)
        {
            case 0:
                targetText[0].color = Color.yellow;
                targetText[1].color = Color.yellow;
                break;
            case 1:
                targetText[2].color = Color.yellow;
                targetText[3].color = Color.yellow;
                break;
            case 2:
                targetText[4].color = Color.yellow;
                targetText[5].color = Color.yellow;
                break;

            case 3:
                targetText[6].color = Color.yellow;
                if (ControllerInput.jump[0])
                {
                    SoundEffect.soundTrigger[2] = 1;
                    GameStart.phase = 0;
                    GameStart.inDemoPlay = false;
                    GameSetting.Playable = false;
                    GameStart.PlayerNumber = 1;
                    SettingPanelActive = false;
                    SceneManager.LoadScene("Title");
                }
                break;
        }
    }
}
