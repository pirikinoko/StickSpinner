
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleButton : MonoBehaviour
{

    //const int title = 0;
    //const int selectStage = 1;
    //const int selectPlayerNumber = 2;

    //const int stage1 = 1;
    //const int stage2 = 2;
    //const int stage3 = 3;
    const int stage4 = 4, firstStage = 1, lastStage = 4;
    const float holdGoal = 0.85f;
    float holdTime = 0;
    int minPlayer = 1;
    int maxPlayer = 4;
    // ボタンの長押し時間
    [SerializeField]
    private Animator YButtonAnim;


    void Update()
    {
        // タイトル画面は三つに分けて処理する
        switch (GameStart.phase)
        {
            // タイトル
            case 0:
                Title();
                break;
            // ステージ選択
            case 1:
                SelectStage();
                break;
            // プレイヤー数増減 
            case 2:
                ChangePlayerNumber();
                break;
            // ボタン押しっぱなしでゲーム開始
            case 3:
                HoldButtonGoToGame();
                break;
        }
    }

    //
    // タイトル
    //
    void Title()
    {
        if(Input.GetButtonDown("Next_1"))
        {
            SoundEffect.PironTrigger = 1;
            GameStart.phase = 1;
        }
	}

    //
    // ステージ選択
    //
    void SelectStage()
    {
        // 次へ
        if(Input.GetButtonDown("Next_1"))
        {
            SoundEffect.PironTrigger = 1;
            GameStart.phase = 2;
        }
        // 戻る
        else if(Input.GetButtonDown("XBack_1"))
        {
            // キャンセル音を鳴らす
            GameStart.phase = 0;
		}

        // LR でステージ選択
        if (Input.GetButtonDown("Plus_1"))
        {
            GameStart.Stage++;
        }
        else if (Input.GetButtonDown("Minus_1"))
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
        if (Input.GetButtonDown("Next_1"))
        {
            SoundEffect.PironTrigger = 1;
            GameStart.phase = 3;
        }
        // 戻る
        else if (Input.GetButtonDown("XBack_1"))
        {
            // キャンセル音を鳴らす
            GameStart.phase = 0;
        }

        // LR でプレイヤー数選択
        if (Input.GetButtonDown("Plus_1"))
        {
            GameStart.PlayerNumber++;
        }
        else if (Input.GetButtonDown("Minus_1"))
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
        if (Input.GetButtonDown("Next_1"))
        {
            YButtonAnim.enabled = true;
            YButtonAnim.SetTrigger("On");
        }
        // ボタンを押し続けたとき -> メーターが上がり続けてステージ開始
        else if (Input.GetButton("Next_1"))
        {
            holdTime += Time.deltaTime;
            if (holdTime > holdGoal)
            {
                GameStart.inDemoPlay = false;
                SoundEffect.PironTrigger = 1;
                SceneManager.LoadScene("Stage" + GameStart.Stage.ToString());
            }
        }
        // ボタンを放した時
        else if (Input.GetButtonUp("Next_1"))
        {
            YButtonAnim.SetTrigger("Off");
            YButtonAnim.enabled = false;
            holdTime = 0;
        }
        // 戻る
        else if (Input.GetButtonDown("XBack_1"))
        {
            // キャンセル音を鳴らす
            GameStart.phase = 1;
        }

      
    }
}

