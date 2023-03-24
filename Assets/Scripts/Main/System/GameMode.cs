using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameMode : MonoBehaviour
{
    //基本
    public static string[] goaledPlayer{ get; set;} = new string[GameStart.MaxPlayer];
    GameObject[] players  = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks   = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    GameObject[] resultTextGO = new GameObject[GameStart.MaxPlayer];
    Text[] resultText = new Text[GameStart.MaxPlayer];
    [SerializeField] GameObject ResultPanel, ResultPanelFront;
    GameObject InputField;
    //通常ステージ
    public static byte Goals = 0;
    public static float[] clearTime = new float[GameStart.MaxPlayer];
    public static bool Goaled;
    //バトルモード
    public GameObject KillLogBack, Plus1, Plus5;
    GameObject[] pointTextGO = new GameObject[4], pointBox = new GameObject[4];
    [SerializeField] GameObject[] respownPos2 = new GameObject[GameStart.MaxPlayer];
    public static float[] points = new float[4], pointsInOrder = new float[4];
    public static float topPoint, KillLogTimer;
    float p1Points, p2Points, p3Points, p4Points;
    public static string[] plasement = new string [4];
    Text[] pointText = new Text[4];
    public static string killer, died;
    public Text KillLogText = null;
    public static byte[] playParticle = new byte[4];
    public static bool Finished;
    byte count = 0;
    private Vector2[] particlePos = new Vector2[4];
    public static float[,] killTimer = new float[4, 4];       // プレイヤー同士の衝突を記録(プレイヤー1～4とプレイヤー1～4の衝突)
    Renderer[] respownPos2Rend = new Renderer[4];

    void Start()
    {   //基本
        for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
        {        
            nameTags[       i] = GameObject.Find("P" +      (i + 1).ToString() + "Text");
            players[        i] = GameObject.Find("Player" + (i + 1).ToString());
            sticks[         i] = GameObject.Find("Stick"  + (i + 1).ToString());
            resultTextGO[   i] = GameObject.Find("resultText" + (i + 1).ToString());
            respownPos2Rend[i] = respownPos2[i].gameObject.GetComponent<SpriteRenderer>();
            respownPos2Rend[i].enabled = false;
            resultNameGO[   i].SetActive(false);
            resultPointGO[  i].SetActive(false);
            barObject[      i].SetActive(false);
            resultText[     i] = resultTextGO[i].GetComponent<Text>();
            for(int j = 0; j < 4; j++)
            {
                killTimer[i, j] = 0f;
            }
        }
        InputField = GameObject.Find("InputField");
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        InputField.gameObject.SetActive(false);

        // 通常ステージ
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
                pointBox[i] = GameObject.Find("PointFrame" + (i + 1).ToString());
                pointTextGO[i] = GameObject.Find("P" + (i + 1).ToString() +"Point");
                pointText[i] = pointTextGO[i].GetComponent<Text>();
            }
            count = 0;
            KillLogTimer = 0;
            Finished = false;
           
            // 画面上部スコアプレイヤー数分表示
            for (int i = 0; i < GameStart.PlayerNumber; i++) { pointBox[i].gameObject.SetActive(true); pointTextGO[i].gameObject.SetActive(true); }
            for (int i = 3; i >= GameStart.PlayerNumber; i--) { pointBox[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
        }
          
    }

    void Update()
    {
        // 通常ステージ
        if (GameStart.Stage != 4)
        {
            CheckFinish();
        }
        // バトルモード
        if (GameStart.Stage == 4)
        {
            KillSystem();
            PointDisplay();
            PlayParticle();
            checkResult();
            ShowResult();
            LastChance();
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

    public void GoalProcess(int playerid)
    {
        // ゴールしたプレイヤーを表示する
        clearTime[Goals] = GameSetting.PlayTime;
        goaledPlayer[Goals] = "Player" + playerid.ToString();
        SoundEffect.PironTrigger = 1;
        Goals++;
        players[playerid - 1].gameObject.SetActive(false);
        sticks[playerid - 1].gameObject.SetActive(false);
        nameTags[playerid - 1].gameObject.SetActive(false);
    }
    void LastChance()
    {
        if(GameSetting.PlayTime < 30)
        {
            for (int i = 0; i < GameStart.MaxPlayer; i++)
            {
                GameSetting.respownPos[i] = respownPos2[i].gameObject.transform.position;
            }
        }
    }
    //バトルモード
    void KillSystem()
    {
        //タイマー減少
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            for (int j = 0; j < GameStart.PlayerNumber; j++)
            {
                if (killTimer[i, j] >= 0)
                {
                    killTimer[i, j] -= Time.deltaTime;
                }
            }
        }
        //キルログ表示
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
            pointText[i].text = String.Format("{0:#}", points[i].ToString());
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


            if (count == 0)
            {
          
                //ポイント並び替え
                points.CopyTo(pointsInOrder, 0);
                Array.Sort(pointsInOrder);
                Array.Reverse(pointsInOrder);
                int num = 3;
                /*
          //順位計測
          for (int i = GameStart.PlayerNumber - 1; i >= 0; i--)
          {

              if (points[i] == 0)
              {
                  plasement[num] = "Player" + (i + 1).ToString();
                  num --;
              }
              else
              {
                  for (int j = 0; j < GameStart.PlayerNumber; j++)
                  {
                      if (pointsInOrder[j] == points[i])
                      {
                          plasement[j] = "Player" + (i + 1).ToString();
                      }

                  }
              }                   

          }
          topPoint = pointsInOrder[0];
          */
                StartCoroutine("ShowResult2");
                count = 1;
            }
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
        if (Finished)
        {
            //リザルト表示
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                resultText[i].text = (i + 1) + "位: " + plasement[i] + "  " + pointsInOrder[i] + "ポイント";
            }
        }
    }

    [SerializeField] GameObject[] resultNameGO, resultPointGO, barObject;
    [SerializeField] int barAspectRatioX, barAspectRatioY, barSizeX, barSizeY;
    [SerializeField] Color[] playerColor;
    Text[] resultNameText = new Text[4], resultPointText = new Text[4];
    Vector2 barScale, barPos, pointPos, namePos;
     IEnumerator ShowResult2()
    {
        float sumPoints = points[0] + points[1] + points[2] + points[3];
        float[] pointRatio = new float[4]; //全プレイヤーのポイントの合計に対するプレイヤーごとのポイントの割合
        barPos.x = -1000;
        barPos.y = -400 ;
        namePos.x = -1200;
        namePos.y = -500;
        for (int i = (GameStart.PlayerNumber - 1); i >= 0; i--)
        {
            int num = 0;
            string nameTagStr = nameTags[i].gameObject.GetComponent<Text>().text;
            int playerId = int.Parse(nameTagStr.Substring(6));
            pointRatio[i] = pointsInOrder[i] / sumPoints;
            barPos.x += 400;
            namePos.x += 400;
            barScale.x = barSizeX * (pointRatio[i] + 0.2f);
            pointPos = barPos;
            resultPointGO[i].transform.localPosition = barPos;
            resultNameText[i] = resultNameGO[i].GetComponent<Text>();
            resultPointText[i] = resultPointGO[i].GetComponent<Text>();
            SpriteRenderer spriteRenderer = barObject[i].GetComponent<SpriteRenderer>();
            spriteRenderer.color = playerColor[playerId - 1];
            resultNameText[i].text = nameTagStr;



            spriteRenderer.color = playerColor[i];
            while (num < pointsInOrder[i])
            {
                num++;
                resultNameGO[i].SetActive(true);
                resultPointGO[i].SetActive(true);
                barObject[i].SetActive(true);
               
                resultPointText[i].text = num.ToString();
                barScale.y = (num * 10) * pointRatio[i];
                barObject[i].transform.localScale = barScale;
                pointPos.y += 8;
                barPos.y += (5.0f * pointRatio[i]);
                barObject[i].transform.localPosition = barPos;
                resultPointGO[i].transform.localPosition = pointPos;
                resultNameGO[i].transform.localPosition = namePos;
                yield return new WaitForSeconds(1 / pointsInOrder[i]);
            }
        }
    }
}


