
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class TitleButton : MonoBehaviour
{
    const int stage4 = 4, firstStage = 1, lastStage = 4;
    const float holdGoal = 0.85f;
    float holdTime = 0;
    int minPlayer = 1;
    int maxPlayer = 4;
    // ボタンの長押し時間
    [SerializeField]
    private Animator YButtonAnim;
    public GameObject YButtonGO;
    //New
    [SerializeField] GameObject[] phase1Obj, phase2SingleObj, phase2MultiObj;
    [SerializeField] GameObject phase1Frame, phase2SingleFrame, phase2MultiFrame;
    bool InputCrossX, InputCrossY;
    public int targetNum { get; set; }
    int min = 0, max;
    float lastLstickX, lastLstickY;
    void Start()
    {
        targetNum = 0;
    }
    void Update()
    {
        OpenSetting();
        SelectButton();
        Selected();
        if (ControllerInput.back[0] || Input.GetKeyDown(KeyCode.Backspace)) 
        {
            GameStart.phase--;         
        }
        lastLstickX = ControllerInput.LstickX[0];
        lastLstickY = ControllerInput.LstickY[0];

    }

    void SelectButton()
    {
        if (ControllerInput.crossX[0] == 0) { InputCrossX = false; }
        if (ControllerInput.crossY[0] == 0) { InputCrossY = false; }
      

        /*ボタン選択（横）*/
        if (InputCrossX == false)
        {
            if (ControllerInput.crossX[0] >= 0.1f) { targetNum++; InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
            else if (ControllerInput.crossX[0] <= -0.1f) { targetNum--; InputCrossX = true; SoundEffect.soundTrigger[3] = 1; }
        }
        /*ボタン選択（縦）*/
        if (InputCrossY == false)
        {
            if (ControllerInput.crossY[0] >= 0.1f) { targetNum+= 4;  InputCrossY = true; SoundEffect.soundTrigger[3] = 1; }
            else if (ControllerInput.crossY[0] <= -0.1f) { targetNum-= 4;  InputCrossY = true; SoundEffect.soundTrigger[3] = 1; }       
        }
        /*ボタン選択（横）*/
        //上限下限の設定
        targetNum = Mathf.Clamp(targetNum, min, max);

        /*ボタン選択（縦）*/
        if (lastLstickX > 0.1f || lastLstickX < -0.1f || lastLstickY > 0.1f || lastLstickY < -0.1f) { return; }

        /*Lスティック横*/
        if (ControllerInput.LstickX[0] > 0.5f) { targetNum++;  SoundEffect.soundTrigger[3] = 1; }
        else if (ControllerInput.LstickX[0] < -0.5f) { targetNum--;  SoundEffect.soundTrigger[3] = 1; }
        /*Lスティック横*/

        /*Lスティック縦*/
        if (ControllerInput.LstickY[0] > 0.5f) { targetNum-= 4; SoundEffect.soundTrigger[3] = 1; }
        else if (ControllerInput.LstickY[0] < -0.5f) { targetNum+= 4; SoundEffect.soundTrigger[3] = 1; }
        /*Lスティック縦*/

        /*矢印キー横*/
        if (Input.GetKeyDown(KeyCode.RightArrow)) { targetNum++; SoundEffect.soundTrigger[3] = 1; }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) { targetNum--; SoundEffect.soundTrigger[3] = 1; }
        /*矢印キー横*/

        /*矢印キー縦*/
        if (Input.GetKeyDown(KeyCode.UpArrow)) { targetNum -= 4; SoundEffect.soundTrigger[3] = 1; }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) { targetNum += 4; SoundEffect.soundTrigger[3] = 1; }
        /*矢印キー縦*/
          //上限下限の設定
        targetNum = Mathf.Clamp(targetNum, min, max);

    }


    void Selected()
    {
        switch (GameStart.phase)
        {
            case 0:
                min = 0;
                max = 1;
                for (int i = 0; i < phase1Obj.Length; i++)
                {
                    Vector2 framePos = phase1Obj[targetNum].transform.position;
                    phase1Frame.transform.position = framePos;
                }
                if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                {
                    ExecuteEvents.Execute(phase1Obj[targetNum], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
                break;
                    
            case 1:
                if(GameStart.gameMode1 == "Single") 
                {
                    min = 0;
                    max = 2;
                    for (int i = 0; i < phase2SingleObj.Length; i++)
                    {
                        Vector2 framePos = phase2SingleObj[targetNum].transform.position;
                        phase2SingleFrame.transform.position = framePos;
                    }
                    if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        ExecuteEvents.Execute(phase2SingleObj[targetNum], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                    }             
                }
                if (GameStart.gameMode1 == "Multi")
                {
                    min = 2; max = 4;
                    GameStart.PlayerNumber = targetNum;
                    if(ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        GameStart.phase++;
                    }

                }
                break;

            case 2:
                if (GameStart.gameMode1 == "Multi")
                {
                    min = 0; max = 5;
                    for (int i = 0; i < phase2MultiObj.Length; i++)
                    {
                        Vector2 framePos = phase2MultiObj[targetNum].transform.position;
                        phase2MultiFrame.transform.position = framePos;
                    }
                    if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        ExecuteEvents.Execute(phase2MultiObj[targetNum], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                    }

                }
                break;

     

        }

    }


    void OpenSetting()　//設定表示
    {
        if (ControllerInput.start[0])
        {
            Settings.SettingPanelActive = !(Settings.SettingPanelActive);
            Settings.inSetting = !(Settings.inSetting);
        }
    }




















    /*


    //
    // タイトル
    //
    void Title()
    {
        if(ControllerInput.next[0])
        {
            SoundEffect.soundTrigger[2] = 1;
            GameStart.phase = 1;
        }
	}

    //
    // ステージ選択
    //
    void SelectStage()
    {
        // 次へ
        if(ControllerInput.next[0])
        {
            SoundEffect.soundTrigger[2] = 1;
            GameStart.phase = 2;
        }
        // 戻る
        else if(ControllerInput.back[0])
        {
            // キャンセル音を鳴らす
            GameStart.phase = 0;
		}

        // LR でステージ選択
        if (ControllerInput.plus[0])
        {
            GameStart.Stage++;
        }
        else if (ControllerInput.minus[0])
        {
            GameStart.Stage--;
        }
        // 最大値を超えたら最大値を渡す
        GameStart.Stage = System.Math.Min(GameStart.Stage, lastStage);
        // 最小値を下回ったら最小値を渡す
        if (GameStart.Stage == 4) { minPlayer = 2; }
        GameStart.Stage = System.Math.Max(GameStart.Stage, firstStage);
    }

    //
    // プレイヤー数増減 
    //
    void ChangePlayerNumber()
    {
        // 次へ
        if (ControllerInput.next[0])
        {
            SoundEffect.soundTrigger[2] = 1;
            GameStart.phase = 3;
        }
        // 戻る
        else if (ControllerInput.back[0])
        {
            // キャンセル音を鳴らす
            GameStart.phase = 0;
        }

        // LR でプレイヤー数選択
        if (ControllerInput.plus[0])
        {
            GameStart.PlayerNumber++;
        }
        else if (ControllerInput.minus[0])
        {
            GameStart.PlayerNumber--;
        }
        // 最大値を超えたら最大値を渡す
        GameStart.PlayerNumber = System.Math.Min(GameStart.PlayerNumber, maxPlayer);
        // 最小値を下回ったら最小値を渡す
        if (GameStart.Stage == 4) { minPlayer = 2; } else { minPlayer = 1; }
        GameStart.PlayerNumber = System.Math.Max(GameStart.PlayerNumber, minPlayer);
    }
    //
    // ボタン押しっぱなしでゲーム開始
    //
    void HoldButtonGoToGame()
    {
        // ボタンを押した瞬間
        if (ControllerInput.next[0])
        {
            YButtonAnim.enabled = true; 
            YButtonAnim.SetTrigger("On");
            YButtonAnim.Play("YAnim", 0, 0);
        }
        // ボタンを押し続けたとき -> メーターが上がり続けてステージ開始
        if (ControllerInput.nextHold[0])
        {
            holdTime += Time.deltaTime;
            if (holdTime > holdGoal)
            {
                GameStart.inDemoPlay = false;
                SoundEffect.soundTrigger[2] = 1;
                SceneManager.LoadScene("Stage");
            }
        }
        // ボタンを放した時
        if (ControllerInput.nextHold[0] == false)
        {
            YButtonAnim.enabled = false;         
            holdTime = 0;
        }
        // 戻る
        if (ControllerInput.back[0])
        {
            // キャンセル音を鳴らす
            GameStart.phase = 1;
        }


    }
    */

}

