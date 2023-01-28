using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    //基本
    [SerializeField]
    Text CountDown, playTime;
    public GameObject CountDownGO, ControllerUI1, ControllerUI2, ControllerUI3;
    GameObject[] players = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    GameObject[] countTextGO = new GameObject[GameStart.MaxPlayer];
    Text[]    nameTagText    = new Text[   GameStart.MaxPlayer];
    Vector2[] nameTagPos     = new Vector2[GameStart.MaxPlayer]; 
    Vector2[,] startPos = new Vector2[GameStart.MaxPlayer, GameStart.MaxPlayer];
    public static bool Playable = false;
    public Text[] timer = new Text[GameStart.MaxPlayer];
    //public Text CountDown, playTime;
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
        GameStart.Stage = 1;
        Debug.Log("PlayerNumber: " + GameStart.PlayerNumber + " Stage: " + GameStart.Stage);
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
                players[i].gameObject.transform.position = CheckPoint.respownPos[i];
                sticks[i].gameObject.transform.position =  CheckPoint.respownPos[i];
            }

            // プレイヤー人数の反映
            for ( ; i < GameStart.MaxPlayer; i++)
            {
                nameTags[i].gameObject.SetActive(false);
                players[i].gameObject.SetActive(false);
                sticks[i].gameObject.SetActive(false);
            }
        }

        //
        GameStart.inDemoPlay = false;
    }




    void FixedUpdate()
    {
        NameTagPos();
    }
    // Update is called once per frame
    void Update()
    {
        SwichUI();
        StartTimer();   
        //RespownTimer();
    }

    // プレイヤーの復活カウントダウンは Controller.cs へ移動
    /*void RespownTimer()
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
    }*/



    void StartTimer()
    {

        //タイム
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
