    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Linq;
public class TitleButton : MonoBehaviourPunCallbacks
{
    // ボタンの長押し時間
    [SerializeField]
    private Animator YButtonAnim;
    public GameObject YButtonGO;
    //タイトル画面操作
    [SerializeField] GameObject cursor;
    bool inputCrossXPlus, inputCrossXMinus, inputCrossYPlus, inputCrossYMinus, inputLstickXPlus, inputLstickXMinus, inputLstickYPlus, inputLstickYMinus;
    float[] lastLstickX = new float[4], lastLstickY = new float[4];
    float lastCrossX, lastCrossY; 
    private Button[] activeButtons;
    const int maxButtonCount = 20;
    GameObject[] buttonsInTheScene = new GameObject[maxButtonCount];
    Vector2[] buttonPositions = new Vector2[maxButtonCount];
    int targetButton = 0, lastPhase;

    void Start()
    {


    }
    void Update()
    {
        FindAllButtons();

        //選択中のボタンをクリック
        if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
        {
            if (Settings.inSetting) { return; }
            if (activeButtons.Length != 0)
            {
                activeButtons[targetButton].onClick.Invoke();
            }
        }

        for (int i = 0; i < 4; i++)
        {
            lastLstickX[i] = ControllerInput.LstickX[i];
            lastLstickY[i] = ControllerInput.LstickY[i];
        }

    }



    void FindAllButtons()
    {
        cursor.gameObject.SetActive(true);
        int pauseButtonNum = 0;
        //特定のボタンを除外してシーン上のアクティブなボタンを取得
          activeButtons = FindObjectsOfType<Button>()
            .Where(button => button.gameObject.activeSelf &&
                             !button.gameObject.name.Contains("Pause") &&
                             !button.gameObject.name.Contains("Resume") &&
                             !button.gameObject.name.Contains("Next") &&
                             !button.gameObject.name.Contains("Back") &&
                             !button.gameObject.name.Contains("Prev")&&
                             !button.gameObject.name.Contains("Plus") &&
                              !button.gameObject.name.Contains("Increase") &&
                             !button.gameObject.name.Contains("Decrease") &&
                                !button.gameObject.name.Contains("Team") &&
                             !button.gameObject.name.Contains("Minus") )
            .ToArray();
        if (activeButtons.Length == 0)
        {
            Debug.Log("ボタンが見つかりませんでした。");
            cursor.gameObject.SetActive(false); 
            return;
        }
        for (int i = 0; i < activeButtons.Length; i++)
        {
            buttonsInTheScene[i] = activeButtons[i].gameObject;
            buttonPositions[i] = buttonsInTheScene[i].transform.position;
            if (buttonsInTheScene[i].name.Contains("Pause") || buttonsInTheScene[i].name.Contains("Resume"))
            {
                pauseButtonNum = i;
            }
            Debug.Log("ButtonName"+ i + activeButtons[i].name);
        }

        if (Settings.SettingPanelActive)
        {
            targetButton = pauseButtonNum;
            return;
        }
        
        //カーソル位置
        RectTransform titleFrameRectTransform = cursor.GetComponent<RectTransform>();
        Vector2 cursorPos = buttonPositions[targetButton];
        cursorPos.x += titleFrameRectTransform.rect.width / 25;
        cursorPos.y -= titleFrameRectTransform.rect.height / 25;
        cursor.transform.position = cursorPos;


        Vector2 basePos = buttonPositions[targetButton];
        float buttonDistance = 9999;

        int newTarget = 0;

        //コントローラー入力設定
        if (ControllerInput.crossX[0] >= 0.1f) { inputCrossXPlus = true; }
        else if (ControllerInput.crossX[0] <= -0.1f) { inputCrossXMinus = true; }
        else if (ControllerInput.crossY[0] <= -0.1f) { inputCrossYPlus = true; }
        else if (ControllerInput.crossY[0] >= 0.1f) { inputCrossYMinus = true; }

        if (ControllerInput.LstickX[0] > 0.5f) { inputLstickXPlus = true; }
        else if (ControllerInput.LstickX[0] < -0.5f) { inputLstickXMinus = true; }
        else if (ControllerInput.LstickY[0] > 0.5f) { inputLstickYPlus = true; }
        else if (ControllerInput.LstickY[0] < -0.5f) { inputLstickYMinus = true; }
        //入力リセット
        if (lastLstickX[0] != 0)
        {
            inputLstickXPlus = false;
            inputLstickXMinus = false;
        }
        if (lastLstickY[0] != 0)
        {
            inputLstickYPlus = false;
            inputLstickYMinus = false;
        }
        if (lastCrossX != 0)
        {
            inputCrossXPlus = false;
            inputCrossXMinus = false;
        }
        if (lastCrossY != 0)
        {
            inputCrossYPlus = false;
            inputCrossYMinus = false;
        }


        /*矢印キー横*/
        if (Input.GetKeyDown(KeyCode.RightArrow) || inputCrossXPlus || inputLstickXPlus || lastPhase != GameStart.phase) 
        {
            for (int i = 0; i < activeButtons.Length; i++)
            {
                if (buttonPositions[i].x > buttonPositions[targetButton].x) 
                {
                   
                    Debug.Log(buttonsInTheScene[i].name + "と" + buttonsInTheScene[targetButton].name + "の距離は" + Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]) );
                    if (Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]) < buttonDistance)
                    {
                        newTarget = i;
                        buttonDistance = Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]);
                    }
                }
            }
            targetButton = newTarget;
            SoundEffect.soundTrigger[3] = 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || inputCrossXMinus || inputLstickXMinus)
        {
            for (int i = 0; i < activeButtons.Length; i++)
            {
                if (buttonPositions[i].x < buttonPositions[targetButton].x)
                {
                    if (Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]) < buttonDistance)
                    {
                        newTarget = i;
                        buttonDistance = Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]);
                    }
                }
            }
            targetButton = newTarget;
            SoundEffect.soundTrigger[3] = 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || inputCrossYPlus || inputLstickYPlus)
        {
            for (int i = 0; i < activeButtons.Length; i++)
            {
                if (buttonPositions[i].y > buttonPositions[targetButton].y)
                {
                    if (Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]) < buttonDistance)
                    {
                        newTarget = i;
                        buttonDistance = Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]);
                    }
                }
            }
            targetButton = newTarget;
            SoundEffect.soundTrigger[3] = 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || inputCrossYMinus || inputLstickYMinus)
        {
            for (int i = 0; i < activeButtons.Length; i++)
            {
                if (buttonPositions[i].y < buttonPositions[targetButton].y)
                {
                    if (Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]) < buttonDistance)
                    {
                        newTarget = i;
                        buttonDistance = Vector2.Distance(buttonPositions[targetButton], buttonPositions[i]);
                    }
                }
            }
            targetButton = newTarget;
            SoundEffect.soundTrigger[3] = 1;
        }

        lastCrossX = ControllerInput.crossX[0];
        lastCrossY = ControllerInput.crossY[0];
        lastPhase = GameStart.phase;
    }
 


}

