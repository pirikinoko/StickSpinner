using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    public GameObject Player1, Player2, Player3, Player4, Stick1, Stick2, Stick3, Stick4, p1Name, p2Name, p3Name, p4Name, CountDownGO, P1CountGO, P2CountGO, P3CountGO, P4CountGO, ControllerUI1, ControllerUI2, ControllerUI3;
    GameObject[] players     = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks      = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags    = new GameObject[GameStart.MaxPlayer];
    GameObject[] countTextGO = new GameObject[GameStart.MaxPlayer];
    Text[]    nameTagText    = new Text[   GameStart.MaxPlayer];
    Vector2[] nameTagPos     = new Vector2[GameStart.MaxPlayer];

    Vector2[,] startPos = new Vector2[GameStart.MaxPlayer, GameStart.MaxPlayer];
    public static bool Playable = false;
    public Text[] timer = new Text[GameStart.MaxPlayer];
    public Text CountDown, playTime;
    float elapsedTime;
    public static float PlayTime;
    public static float StartTime = 3f;
    float SoundTime = 1f;
    bool  StartFlag;
    //リスポーンタイマー
    float[] respownTimer = new float[GameStart.MaxPlayer];

    public static bool[] deathTimer = new bool[GameStart.MaxPlayer];
    //UI切り替え用
    int UIMode;
    const int KeyboardMode = 5;
    const int ControllerMode = 6;
    // Start is called before the first frame update
    void Start()
    {
        //配列に代入
        players = new GameObject[] { Player1, Player1, Player1, Player4 };
        sticks = new GameObject[] { Stick1, Stick2, Stick3, Stick4 };
        nameTags = new GameObject[] { p1Name, p2Name, p3Name, p4Name };
        countTextGO = new GameObject[] { P1CountGO, P2CountGO, P3CountGO, P4CountGO };
        startPos[0, 0] = new Vector2(-8f, -4); startPos[0, 1] = new Vector2(-7f, -4); startPos[0, 2] = new Vector2(-6f, -4); startPos[0, 3] = new Vector2(-5f, -4);
        startPos[1, 0] = new Vector2(-5.5f, -4); startPos[1, 1] = new Vector2(-5.5f, -4); startPos[1, 2] = new Vector2(-6.5f, -4); startPos[1, 3] = new Vector2(-7.5f, -4);
        startPos[2, 0] = new Vector2(-3f, -2); startPos[2, 1] = new Vector2(-2f, -2); startPos[2, 2] = new Vector2(-1f, -2); startPos[2, 3] = new Vector2(0, 2);
        startPos[3, 0] = new Vector2(-8f, -2); startPos[3, 1] = new Vector2(-7.84f, -2); startPos[3, 2] = new Vector2(-4.01f, -2); startPos[3, 3] = new Vector2(-3.84f, -2);
        for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
        {
            deathTimer[i] = false;
            respownTimer[i] = 3.0f;
            nameTags[i] = GameObject.Find("P" + (i + 1).ToString() + "Text");
            players[i] = GameObject.Find("Player" + (i + 1).ToString());
            sticks[i] = GameObject.Find("Stick" + (i + 1).ToString());
            nameTagText[i] = nameTags[i].GetComponent<Text>(); ;
            nameTagText[i].text = "Player" + (i + 1).ToString();
            countTextGO[i] = GameObject.Find("P" + (i + 1).ToString() + "CountDown");
            countTextGO[i].gameObject.SetActive(false);
        }

        //デバッグ用ステージ反映
        for (int i = 1; i < 5; i++)
        {
            if (SceneManager.GetActiveScene().name == "Stage" + i.ToString())
            {
                GameStart.Stage = i;
            }
        }

        if (GameStart.Stage != 4)
        {
            PlayTime = 0;
            elapsedTime = 0;
        }
        else if (GameStart.Stage == 4)
        {
            PlayTime = 90;
            elapsedTime = 90;
        }
        Goal.Goaled = false;
        BattleMode.Finished = false;
        SoundTime = 1f;
        StartTime = 3f;
        Playable = false;
        StartFlag = true;
        CountDown.text = ("3");
        SoundEffect.BunTrigger = 1;

        //プレイヤー人数の反映
        {
            int i = 0;
            for ( ; i < GameStart.PlayerNumber; i++)
            {
                nameTags[i].gameObject.SetActive(true);
                players[i].gameObject.SetActive(true);
                sticks[i].gameObject.SetActive(true);
                //初期位置
                players[i].gameObject.transform.position = startPos[GameStart.Stage - 1, i];
                sticks[i].gameObject.transform.position = startPos[GameStart.Stage - 1, i];
            }

            // プレイヤー人数の反映
            for ( ; i < GameStart.MaxPlayer; i++)
            {
                nameTags[i].gameObject.SetActive(false);
                players[i].gameObject.SetActive(false);
                sticks[i].gameObject.SetActive(false);
            }
        }
    }




    void FixedUpdate()
    {
        NameTagPos();
    }
    // Update is called once per frame
    void Update()
    {
        SwichUI();
    }
    void RespownTimer()
    {
        //リスポーンタイマー
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (deathTimer[i])
            {
                countTextGO[i].gameObject.SetActive(true);
                countTextGO[i].gameObject.transform.position = Thorn.col[i];
                respownTimer[i] -= Time.deltaTime;
                SoundTime -= Time.deltaTime;
                if (respownTimer[i] > 2)
                {
                    timer[i].text = ("3");
                }
                else if (respownTimer[i] > 1)
                {
                    timer[i].text = ("2");
                }
                else if (respownTimer[i] > 0)
                {
                    timer[i].text = ("1");
                }
                else if (respownTimer[i] < 0f)
                {
                    timer[i].text = ("");
                    deathTimer[i] = false;
                    respownTimer[i] = 3.0f;
                    Thorn.respownTrigger[i] = true;
                }
                if (SoundTime < 0)
                {
                    SoundEffect.BunTrigger = 1;
                    SoundTime = 1;
                }
            }
        }
    }
    void StartTimer()
    {
        //タイム
        if (GameStart.Stage != 4)
        {
            if (StartFlag)
            {
                StartTime -= Time.deltaTime;
                SoundTime -= Time.deltaTime;
                if (StartTime > 2)
                {
                    CountDownGO.gameObject.SetActive(true);
                    CountDown.text = ("3");
                }
                else if (StartTime > 1)
                {
                    CountDown.text = ("2");
                }
                else if (StartTime > 0)
                {
                    CountDown.text = ("1");
                }
                else if (StartTime < 0 && StartTime > -0.5f)
                {
                    CountDown.text = ("スタート");
                }
                else if (StartTime < 0.9f)
                {
                    CountDown.text = ("");
                    CountDownGO.gameObject.SetActive(false);
                    StartFlag = false;
                }
                if (SoundTime < 0)
                {
                    SoundEffect.BunTrigger = 1;
                    SoundTime = 1;
                }
            }
            if (StartTime < 0 && ButtonInGame.Paused != 1)
            {

                if (GameStart.Stage == 4 && PlayTime > 0)
                {
                    elapsedTime -= Time.deltaTime;
                }
                else
                {
                    elapsedTime += Time.deltaTime;
                }
                Playable = true;
                PlayTime = elapsedTime * 10;
                PlayTime = Mathf.Floor(PlayTime) / 10;
                playTime.text = ("タイム:" + PlayTime);
            }
        }
    }
    void NameTagPos()
    {
        //ネームタグの位置
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            nameTagPos[i] = players[i].transform.position;
            nameTagPos[i].y += 0.5f;
            nameTags[i].transform.position = nameTagPos[i];
        }
    }
    void SwichUI()
    {
        //キーボードマウス用UIとコントローラー用UIの切り替え
        if (Controller.usingController)
        {
            UIMode = ControllerMode;
        }
        else
        {
            UIMode = KeyboardMode;
        }

        if (UIMode == KeyboardMode)
        {
            ControllerUI1.gameObject.SetActive(false);
            ControllerUI2.gameObject.SetActive(false);
            ControllerUI3.gameObject.SetActive(false);
        }
        else
        {
            ControllerUI1.gameObject.SetActive(true);
            ControllerUI2.gameObject.SetActive(true);
            ControllerUI3.gameObject.SetActive(true);
        }
    }
}
