using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Goal : MonoBehaviour
{
    //基本
    public static string[] goaledPlayer{ get; set;} = new string[GameStart.MaxPlayer];
    GameObject[] players  = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks   = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    public Text[] resultText = new Text[GameStart.MaxPlayer];
    GameObject ResultPanel;
    GameObject ResultPanelFront, InputField;
    //通常ステージ
    public static byte Goals = 0;
    public static float[] clearTime = new float[GameStart.MaxPlayer];
    public static bool Goaled;
    //バトルモード
    public GameObject KillLogBack, Plus1, Plus5;
    public GameObject[] pointTextGO, pointBox;
    public static float[] points = new float[4], pointsInOrder = new float[4];
    public static float KillLogTimer;
    float[] pointCut;
    public static string[] plasement;
    public Text[] pointText;
    public static Text killer, died;
    public Text KillLogText = null;
    public static byte[] playParticle;
    byte count = 0;
    public static bool Finished;
    private Vector2[] particlePos;


    void Start()
    {   //基本
        for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
        {        
            nameTags[  i] = GameObject.Find("P" +      (i + 1).ToString() + "Text");
            players[   i] = GameObject.Find("Player" + (i + 1).ToString());
            sticks[    i] = GameObject.Find("Stick"  + (i + 1).ToString());
        }
   
        ResultPanel = GameObject.Find("ResultPanel");
        ResultPanelFront = GameObject.Find("ResultPanelFront");
        InputField = GameObject.Find("InputField");
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        InputField.gameObject.SetActive(false);

        //通常ステージ
        if(GameStart.Stage != 4)
        {
            for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
            {
                clearTime[i] = 0;
                goaledPlayer[i] = null;
            }
            Goaled = false;
            Goals = 0;
        }


        //バトルモード
        if (GameStart.Stage == 4) 
        {
            for (int i = 0; i < 4; i++)
            {
                plasement[i] = null;
                points[i] = 0;
                pointsInOrder[i] = 0;
            }
            count = 0;
            KillLogTimer = 0;
            Finished = false;
            //画面上部スコアプレイヤー数分表示
            for (int i = 0; i < GameStart.PlayerNumber; i++) { pointBox[i].gameObject.SetActive(true); pointTextGO[i].gameObject.SetActive(true); }
            for (int i = 4; i > GameStart.PlayerNumber; i--) { pointBox[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
        }
          
    }

    void Update()
    {
        //通常ステージ
        if (GameStart.Stage != 4)
        {
            CheckFinish();
        }

        //バトルモード
        if (GameStart.Stage == 4)
        {
            KillLog();
            PointDisplay();
            checkResult();
            PlayParticle();
            ShowResult();
        }

    }

    //通常ステージ
    void CheckFinish()
    {
        if (Goals == GameStart.PlayerNumber)
        {
            Goaled = true;
            GameSetting.Playable = false;
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            InputField.gameObject.SetActive(true);
            // 参加プレイヤー数分タイム表示
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                resultText[i].text = (i + 1).ToString() + "位: " + goaledPlayer[i] + " タイム:" + clearTime[i] + "秒";
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameStart.Stage != 4)
        {
            //接触したオブジェクトのタグが"Player"のとき
            if (other.CompareTag("Player"))
            {
                // ゴールしたプレイヤーを表示する
                clearTime[Goals] = GameSetting.PlayTime;
                goaledPlayer[Goals] = other.gameObject.name;
                SoundEffect.PironTrigger = 1;
                Goals++;
                int num = int.Parse(other.gameObject.name.Substring(6)) - 1;
                players[num].gameObject.SetActive(false);
                sticks[num].gameObject.SetActive(false);
                nameTags[num].gameObject.SetActive(false);
            }
        }         
    }

    //バトルモード
    void KillLog()
    {
        KillLogText.text = killer + "　→　" + died;
        if (KillLogTimer > 0)
        {
            KillLogBack.gameObject.SetActive(true);
            KillLogTimer -= Time.deltaTime;
        }
        if (KillLogTimer <= 0)
        {
            KillLogBack.gameObject.SetActive(false);
            KillLogText.text = null;
        }
    }
    void PointDisplay() //ポイント小数点以下切り捨て＆表示
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            pointCut[i] = Mathf.Floor(points[i]);
            pointText[i].text = pointCut[i].ToString();
        }
    }
    void checkResult()
    {

        if (GameSetting.PlayTime <= 0)
        {
            Finished = true;
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                players[i].gameObject.SetActive(false);
                sticks[i].gameObject.SetActive(false);
                nameTags[i].gameObject.SetActive(false);

            }
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            InputField.gameObject.SetActive(true);


        }
        if (count == 0)
        {
            //ポイント並び替え
            pointsInOrder = points;
            Array.Sort(pointsInOrder);
            //順位計測
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                for (int j = 0; j < GameStart.PlayerNumber; j++)
                {
                    if (points[i] != 0 && pointsInOrder[j] == points[i])
                    {
                        plasement[j] = "Player" + (i + 1).ToString();
                    }
                }

            }
            count = 1;
        }
    }

    void PlayParticle()
    {
        //パーティクル再生
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            particlePos[i] = nameTags[i].gameObject.transform.position;
            particlePos[i].y += 0.3f;
            if (playParticle[i] == 1)
            {
                Instantiate(Plus1, particlePos[i], Quaternion.identity); //パーティクル用ゲームオブジェクト生成
                playParticle[i] = 0;
            }
            if (playParticle[i] == 2)
            {
                SoundEffect.KirarinTrigger = 1;
                Instantiate(Plus5, particlePos[i], Quaternion.identity); //パーティクル用ゲームオブジェクト生成
                playParticle[i] = 0;
            }
        }
    }
    void ShowResult()
    {
        //リザルト表示
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            resultText[i].text = "1位: " + plasement[i] + " " + points[0] + "ポイント";
        }
    }
}


