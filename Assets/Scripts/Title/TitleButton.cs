
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
    [SerializeField] GameObject[] titleObj, gameModeObj, singleObj, multiObj;
    [SerializeField] GameObject titleFrame, modeFrame, singleFrame, MultiFrame;
    bool InputCrossX, InputCrossY;
    public int targetNum { get; set; }
    int lastPhase;
    int min = 0, max;
    float[] lastLstickX = new float[4],  lastLstickY = new float[4];
    GameStart gameStart;
    void Start()
    {
        lastPhase = GameStart.phase;
        targetNum = 0;
        gameStart = GameObject.Find("Systems").GetComponent<GameStart>();
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
        for (int i = 0; i < 4; i++)
        {
            lastLstickX[i] = ControllerInput.LstickX[i];
            lastLstickY[i] = ControllerInput.LstickY[i];
        }

        //フェーズごとに選択を１にリセット
        if(GameStart.phase != lastPhase)
        {
            targetNum = 0;
            lastPhase = GameStart.phase;
        }
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
            if (ControllerInput.crossY[0] >= 0.1f) { targetNum += 4; InputCrossY = true; SoundEffect.soundTrigger[3] = 1; }
            else if (ControllerInput.crossY[0] <= -0.1f) { targetNum -= 4; InputCrossY = true; SoundEffect.soundTrigger[3] = 1; }
        }
        /*ボタン選択（横）*/
        //上限下限の設定
        targetNum = Mathf.Clamp(targetNum, min, max);

        /*ボタン選択（縦）*/
        if (lastLstickX[0] > 0.1f || lastLstickX[0] < -0.1f || lastLstickY[0] > 0.1f || lastLstickY[0] < -0.1f) { return; }

        /*Lスティック横*/
        if (ControllerInput.LstickX[0] > 0.5f) { targetNum++; SoundEffect.soundTrigger[3] = 1; }
        else if (ControllerInput.LstickX[0] < -0.5f) { targetNum--; SoundEffect.soundTrigger[3] = 1; }
        /*Lスティック横*/

        /*Lスティック縦*/
        if (ControllerInput.LstickY[0] > 0.5f) { targetNum -= 4; SoundEffect.soundTrigger[3] = 1; }
        else if (ControllerInput.LstickY[0] < -0.5f) { targetNum += 4; SoundEffect.soundTrigger[3] = 1; }
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
            //タイトル（シングルorマルチ選択）
            case 0:
                min = 0;
                max = 1;
                for (int i = 0; i < titleObj.Length; i++)
                {
                    Vector2 framePos = titleObj[targetNum].transform.position;
                    titleFrame.transform.position = framePos;
                }
                if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                {
                    ExecuteEvents.Execute(titleObj[targetNum], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
                break;

            case 1:
                //ゲームモード選択
                if (GameStart.gameMode1 == "Single")
                {
                    min = 0;
                    max = 1;
                    for (int i = 0; i < gameModeObj.Length; i++)
                    {
                        Vector2 framePos = gameModeObj[targetNum].transform.position;
                        modeFrame.transform.position = framePos;
                    }
                    if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        ExecuteEvents.Execute(gameModeObj[targetNum], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                    }
                }
                //人数選択
                if (GameStart.gameMode1 == "Multi")
                {
                    min = 2; max = 4;
                    GameStart.PlayerNumber = targetNum;
                    if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        GameStart.phase++;
                        return;
                    }

                }
                break;
            case 2:
                //ステージ選択
                if (GameStart.gameMode1 == "Single")
                {
                    if (GameStart.gameMode2 == "Nomal")
                    {
                        min = 1;
                        max = 4;
                    }
                    else
                    {
                        min = 1;
                        max = 2;
                    }
                    GameStart.Stage = targetNum;
                    if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        //ゲーム開始処理
                        SceneManager.LoadScene("Stage");
                    }
                }
                //ゲームモード選択
                if (GameStart.gameMode1 == "Multi")
                {
                    min = 0;
                    max = 1;
                    targetNum = Mathf.Clamp(targetNum, min, max);
                    for (int i = 0; i < gameModeObj.Length; i++)
                    {
                        Vector2 framePos = gameModeObj[targetNum].transform.position;
                        modeFrame.transform.position = framePos;
                    }
                    if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        ExecuteEvents.Execute(gameModeObj[targetNum], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                        return;
                    }

                }
                break;

            case 3:
                //ステージ選択
                if (GameStart.gameMode1 == "Multi")
                {
                    if (GameStart.gameMode2 == "Nomal")
                    {
                        min = 1;
                        max = 3;
                    }
                    else
                    {
                        min = 1;
                        max = 2;
                    }
                    GameStart.Stage = targetNum;

                    if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                    {
                        if (GameStart.Stage > 2)
                        {
                            //ゲーム開始処理
                            SceneManager.LoadScene("Stage");
                        }
                        else
                        {
                            GameStart.phase++;
                        }
                    }
                }
                break;

            case 4:
                //アーケードモードゲーム設定
                if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
                {
                    //ゲーム開始処理
                    SceneManager.LoadScene("Stage");
                }

                //制限時間増減
                if (ControllerInput.next[0])
                {
                    GameStart.flagTimeLimit += 10;
                    GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 180);
                }
                if (ControllerInput.back[0])
                {
                    GameStart.flagTimeLimit -= 10;
                    GameStart.flagTimeLimit = System.Math.Max(60, GameStart.flagTimeLimit);
                }
                //チーム選択
                for(int i = 0; i < GameStart.PlayerNumber; i++)
                {
                    /*ボタン選択（縦）*/
                    if (lastLstickX[i] > 0.1f || lastLstickX[i] < -0.1f || lastLstickY[i] > 0.1f || lastLstickY[i] < -0.1f) { return; }
                    /*Lスティック横*/
                    if (ControllerInput.LstickX[i] > 0.5f)
                    {
                        gameStart.playerTeam[i]++;
                        SoundEffect.soundTrigger[3] = 1;
                    }
                    else if (ControllerInput.LstickX[i] < -0.5f)
                    {
                        gameStart.playerTeam[i]--;
                        SoundEffect.soundTrigger[3] = 1; }
                    /*Lスティック横*/

                    /*Lスティック縦*/
                    if (ControllerInput.LstickY[i] > 0.5f)
                    {
                        gameStart.playerTeam[i] -= 2;
                        SoundEffect.soundTrigger[3] = 1;
                    }
                    else if (ControllerInput.LstickY[i] < -0.5f)
                    {
                        gameStart.playerTeam[i] += 2;
                        SoundEffect.soundTrigger[3] = 1;
                    }
                    /*Lスティック縦*/
                }
                /*キーボード*/
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    gameStart.playerTeam[0] -= 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    gameStart.playerTeam[0] += 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    gameStart.playerTeam[1] -= 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    gameStart.playerTeam[1] += 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    gameStart.playerTeam[2] -= 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    gameStart.playerTeam[2] += 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    gameStart.playerTeam[3] -= 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    gameStart.playerTeam[3] += 1;
                    SoundEffect.soundTrigger[3] = 1;
                }
                /*キーボード*/

                for (int i = 0; i < GameStart.PlayerNumber; i++)
                {
                    gameStart.playerTeam[i] = Mathf.Clamp(gameStart.playerTeam[i], 0, GameStart.PlayerNumber - 1);
                }
                break;
        }

        //毎回選択を１にリセット
        if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
        {
            targetNum = 0;
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

