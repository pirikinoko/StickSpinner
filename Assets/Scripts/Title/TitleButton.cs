    
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
    const float holdGoal = 0.85f;
    float holdTime = 0;
    // ボタンの長押し時間
    [SerializeField]
    private Animator YButtonAnim;
    public GameObject YButtonGO;
    //タイトル画面操作
    [SerializeField] GameObject cursor;
    bool InputCrossX, InputCrossY;
    bool inputCrossXPlus, inputCrossXMinus, inputCrossYPlus, inputCrossYMinus, inputLstickXPlus, inputLstickXMinus, inputLstickYPlus, inputLstickYMinus;
    float[] lastLstickX = new float[4], lastLstickY = new float[4];
    float lastCrossX, lastCrossY; 
    private Button[] activeButtons;
    const int maxButtonCount = 20;
    GameObject[] buttonsInTheScene = new GameObject[maxButtonCount];
    Vector2[] buttonPositions = new Vector2[maxButtonCount];
    int targetButton = 0;

    void Start()
    {


    }
    void Update()
    {
        FindAllButtons();
        OpenSetting();

        //次に進むボタン
        if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
        {
            if (Settings.inSetting) { return; }
            activeButtons[targetButton].onClick.Invoke();
        }

        //戻るボタン
        if (ControllerInput.back[0] || Input.GetKeyDown(KeyCode.Backspace))
        {

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
            //Debug.Log("ButtonName"+ i + activeButtons[i].name);
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
        else if (ControllerInput.crossY[0] >= 0.1f) { inputCrossYPlus = true; }
        else if (ControllerInput.crossY[0] <= -0.1f) { inputCrossYMinus = true; }

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
        if (Input.GetKeyDown(KeyCode.RightArrow) || inputCrossXPlus ||  inputLstickXPlus) 
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
    }
  


    [PunRPC]
        void ArcadeTeamChange()
        {
            if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
            {
                GameStart.phase = 5;
            }

            //制限時間増減
            if (ControllerInput.plus[0])
            {
                GameStart.flagTimeLimit += 10;
                GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 150);
            }
            if (ControllerInput.minus[0])
            {
                GameStart.flagTimeLimit -= 10;
                GameStart.flagTimeLimit = System.Math.Max(40, GameStart.flagTimeLimit);
            }
            //チーム選択
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                /*ボタン選択（縦）*/
                if (lastLstickX[i] > 0.1f || lastLstickX[i] < -0.1f || lastLstickY[i] > 0.1f || lastLstickY[i] < -0.1f) { return; }
                /*Lスティック横*/
                if (ControllerInput.LstickX[i] > 0.5f)
                {
                    if (GameStart.playerTeam[i] < 3)
                    {
                        GameStart.playerTeam[i]++;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] -= 1;
                        }
                    }
                }
                else if (ControllerInput.LstickX[i] < -0.5f)
                {
                    if (GameStart.playerTeam[i] > 0)
                    {
                        GameStart.playerTeam[i]--;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] += 1;
                        }
                    }
                }
                /*Lスティック横*/

                /*Lスティック縦*/
                if (ControllerInput.LstickY[i] > 0.5f)
                {
                    if (GameStart.playerTeam[i] > 1)
                    {
                        GameStart.playerTeam[i] -= 2;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] += 2;
                        }
                    }
                }
                else if (ControllerInput.LstickY[i] < -0.5f)
                {
                    if (GameStart.playerTeam[i] < 2)
                    {
                        GameStart.playerTeam[i] += 2;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] -= 2;
                        }
                    }
                }
                /*Lスティック縦*/
            }
    }
     
    

    void ArcadeCotroll() 
    {
        //アーケードモードゲーム設定
        if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return))
        {
            //ゲーム開始処理
            SceneManager.LoadScene("Stage");
        }

        //制限時間増減
        if (ControllerInput.plus[0])
        {
            GameStart.flagTimeLimit += 10;
            GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 150);
        }
        if (ControllerInput.minus[0])
        {
            GameStart.flagTimeLimit -= 10;
            GameStart.flagTimeLimit = System.Math.Max(40, GameStart.flagTimeLimit);
        }
        //チーム選択
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            /*ボタン選択（縦）*/
            if (lastLstickX[i] > 0.1f || lastLstickX[i] < -0.1f || lastLstickY[i] > 0.1f || lastLstickY[i] < -0.1f) { return; }
            /*Lスティック横*/
            if (ControllerInput.LstickX[i] > 0.5f)
            {
                if (GameStart.playerTeam[i] < 3)
                {
                    GameStart.playerTeam[i]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] -= 1;
                    }
                }
            }
            else if (ControllerInput.LstickX[i] < -0.5f)
            {
                if (GameStart.playerTeam[i] > 0)
                {
                    GameStart.playerTeam[i]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] += 1;
                    }
                }
            }
            /*Lスティック横*/

            /*Lスティック縦*/
            if (ControllerInput.LstickY[i] > 0.5f)
            {
                if (GameStart.playerTeam[i] > 1)
                {
                    GameStart.playerTeam[i] -= 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] += 2;
                    }
                }
            }
            else if (ControllerInput.LstickY[i] < -0.5f)
            {
                if (GameStart.playerTeam[i] < 2)
                {
                    GameStart.playerTeam[i] += 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] -= 2;
                    }
                }
            }
            /*Lスティック縦*/
        }
        /*キーボード*/
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (GameStart.playerTeam[0] > 0)
            {
                GameStart.playerTeam[0] -= 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[0]] > GameStart.PlayerNumber - 2)
                {
                    GameStart.playerTeam[0] += 1;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (GameStart.playerTeam[0] < 3)
            {
                GameStart.playerTeam[0] += 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[0]] > GameStart.PlayerNumber - 1)
                {
                    GameStart.playerTeam[0] -= 1;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (GameStart.playerTeam[1] > 0)
            {
                GameStart.playerTeam[1] -= 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[1]] > GameStart.PlayerNumber - 2)
                {
                    GameStart.playerTeam[1] += 1;
                }
            }

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (GameStart.playerTeam[1] < 3)
            {
                GameStart.playerTeam[1] += 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[1]] > GameStart.PlayerNumber - 2)
                {
                    GameStart.playerTeam[1] -= 1;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (GameStart.playerTeam[2] > 0)
            {
                GameStart.playerTeam[2] -= 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[2]] > GameStart.PlayerNumber - 2)
                {
                    GameStart.playerTeam[2] += 1;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            if (GameStart.playerTeam[2] < 3)
            {
                GameStart.playerTeam[2] += 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[2]] > GameStart.PlayerNumber - 2)
                {
                    GameStart.playerTeam[2] -= 1;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (GameStart.playerTeam[3] > 0)
            {
                GameStart.playerTeam[3] -= 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[3]] > GameStart.PlayerNumber - 2)
                {
                    GameStart.playerTeam[3] += 1;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            if (GameStart.playerTeam[3] < 3)
            {
                GameStart.playerTeam[3] += 1;
                SoundEffect.soundTrigger[3] = 1;
                if (GameStart.teamSize[GameStart.playerTeam[3]] > GameStart.PlayerNumber - 2)
                {
                    GameStart.playerTeam[3] -= 1;
                }
            }

        }
        /*キーボード*/
    }

    void OpenSetting()　//設定表示
    {
        if ((ControllerInput.start[0] || Input.GetKeyDown(KeyCode.Escape)) && Settings.exitPanelActive == false)
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

