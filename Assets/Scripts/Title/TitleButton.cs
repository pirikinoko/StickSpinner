
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
    const int stage4 = 4;
    const float holdGoal = 0.85f;
    float holdTime = 0;

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
            // プレイヤー数増減 & ボタン押しっぱなしでゲーム開始
            case 2:
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
            if (GameStart.Stage == stage4){ GameStart.PlayerNumber = 2;}    // これなんだろう? stage4 は強制的に２人プレイという意味?
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
            if (GameStart.Stage == stage4){ GameStart.PlayerNumber = 2;}
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
            if(GameStart.Stage > GameStart.MaxStage){ GameStart.Stage = GameStart.MaxStage - 1;}
        }
        else if (Input.GetButtonDown("Minus_1"))
        {
            GameStart.Stage--;
            if(GameStart.Stage < 0){ GameStart.Stage = 0;}
        }
	}

    //
    // プレイヤー数増減 & ボタン押しっぱなしでゲーム開始
    //
    void HoldButtonGoToGame()
    {
        // ボタンを押した瞬間
        if(Input.GetButtonDown("Next_1"))
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
                GameStart.InSelectPN = false;
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
        else if(Input.GetButtonDown("XBack_1"))
        {
            // キャンセル音を鳴らす
            GameStart.phase = 1;
		}

        // LR でプレイヤー数選択
        if (Input.GetButtonDown("Plus_1"))
        {
            GameStart.PlayerNumber++;
            if(GameStart.PlayerNumber > GameStart.MaxPlayer){ GameStart.PlayerNumber = GameStart.MaxPlayer - 1;}
        }
        else if (Input.GetButtonDown("Minus_1"))
        {
            GameStart.PlayerNumber--;
            if(GameStart.PlayerNumber < 1){ GameStart.PlayerNumber = 1;}
        }
    }
}

