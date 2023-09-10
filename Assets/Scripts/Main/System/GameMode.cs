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
    public static string[] goaledPlayer { get; set; } = new string[GameStart.MaxPlayer];
    GameObject[] players = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    GameObject[] resultTextGO = new GameObject[GameStart.MaxPlayer];
    Text[] resultText = new Text[GameStart.MaxPlayer];
    [SerializeField] GameObject ResultPanel, ResultPanelFront, TextCanvas;
    //通常ステージ
    public static byte Goals = 0;
    public static float[] clearTime = new float[GameStart.MaxPlayer];
    public static bool Goaled;
    //バトルモード
    public GameObject KillLogBack, Plus1, Plus5;
    GameObject[] pointTextGO = new GameObject[4];
    [SerializeField] GameObject[] chanceRespown1, chanceRespown2, ffaFrame, teamFrame, teamTag;
    [SerializeField] private Text[] teamTagText;
    string[] teamTagName = { "A", "B", "C", "D" };
    private Color[] teamColors = { Color.white, Color.red, Color.blue, Color.green };
    public static bool[] isDead = { false, false, false, false};
    public static float[] points = new float[4], pointsInOrder = new float[4], teamPoints = new float[4];
    public static float topPoint, KillLogTimer;
    float p1Points, p2Points, p3Points, p4Points;
    public static string[] plasement = new string[4];
    Text[] pointText = new Text[4];
    public static string killer, died;
    public Text KillLogText = null;
    public static byte[] playParticle = new byte[4];
    public static bool Finished;
    byte count = 0;
    private Vector2[] particlePos = new Vector2[4];
    public static float[,] killTimer = new float[4, 4];       // プレイヤー同士の衝突を記録(プレイヤー1～4とプレイヤー1～4の衝突)
    Renderer[] chanceRespown1Rend = new Renderer[4];
    Renderer[] chanceRespown2Rend = new Renderer[4];
    void Start()
    {   //基本
        for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
        {
            nameTags[i] = GameObject.Find("P" + (i + 1).ToString() + "NameTag");
            players[i] = GameObject.Find("Player" + (i + 1).ToString());
            sticks[i] = GameObject.Find("Stick" + (i + 1).ToString());
            resultTextGO[i] = GameObject.Find("resultText" + (i + 1).ToString());
            chanceRespown1Rend[i] = chanceRespown1[i].gameObject.GetComponent<SpriteRenderer>();
            chanceRespown1Rend[i].enabled = false;
            chanceRespown2Rend[i] = chanceRespown2[i].gameObject.GetComponent<SpriteRenderer>();
            chanceRespown2Rend[i].enabled = false;
            resultText[i] = resultTextGO[i].GetComponent<Text>();
            teamTag[i].gameObject.SetActive(false);
            isDead[i] = false;
            for (int j = 0; j < 4; j++)
            {
                killTimer[i, j] = 0f;
            }
        }
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        TextCanvas.gameObject.SetActive(false);
        Finished = false;
        Goaled = false;
        Goals = 0;
        // 通常ステージ
        if (GameStart.gameMode2 != "Arcade")
        {
            for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
            {
                clearTime[i] = 0;
                goaledPlayer[i] = null;
            }
        }

        //バトルモード
        if (GameStart.gameMode2 == "Arcade")
        {
            for (int i = 0; i < 4; i++)
            {
                plasement[i] = null;
                points[i] = 0;
                teamPoints[i] = 0;
                pointsInOrder[i] = 0;
                pointTextGO[i] = GameObject.Find("P" + (i + 1).ToString() + "Point");
                pointText[i] = pointTextGO[i].GetComponent<Text>();
            }
            count = 0;
            KillLogTimer = 0;


            // 画面上部スコアプレイヤー数分表示
            for (int i = 0; i < 4; i++) { teamFrame[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
            for (int i = 0; i < 4; i++) { ffaFrame[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
            if (GameStart.teamMode == "FreeForAll")
            {
                for (int i = 0; i < GameStart.PlayerNumber; i++) { ffaFrame[i].gameObject.SetActive(true); pointTextGO[i].gameObject.SetActive(true); }
                for (int i = 3; i >= GameStart.PlayerNumber; i--) { ffaFrame[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
            }
            else
            {            
                for (int i = 0; i < GameStart.PlayerNumber; i++)
                {
                    if (GameStart.teamSize[i] >= 1)
                    {
                        teamFrame[i].gameObject.SetActive(true);
                        pointTextGO[i].gameObject.SetActive(true);
                    }       
                }
            }
            //チームタグ表示
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (GameStart.teamMode != "FreeForAll")
                {
                    teamTag[i].gameObject.SetActive(true);
                    teamTagText[i].text = teamTagName[GameStart.playerTeam[i]];
                    teamTagText[i].color = teamColors[GameStart.playerTeam[i]];
                }
            }
            
        }

    }

    void Update()
    {
        //Debug.Log("teamPoints: " +  teamPoints[0] + "    " + teamPoints[1] + "    " + teamPoints[2] + "    " + teamPoints[3]);
        Debug.Log("teamCount " + GameStart.teamCount);
        // 通常ステージ
        if (GameStart.gameMode2 != "Arcade")
        {
            CheckFinish();
        }
        // バトルモード
        if (GameStart.gameMode2 == "Arcade")
        {
            KillSystem();
            PointDisplay();
            PlayParticle();
            checkResult();
            ShowResult();
            LastChance();
        }
    }
    private void FixedUpdate()
    {
        ShowTeamTag();
    }
    //通常ステージ
    void CheckFinish()
    {
        if (Goals == GameStart.PlayerNumber)
        {
            Goaled = true;
            GameSetting.Playable = false;
            TextCanvas.gameObject.SetActive(true);
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            // 参加プレイヤー数分タイム表示
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                resultText[i].text = "#" + (i + 1).ToString() + "  " + goaledPlayer[i] + "    " + clearTime[i] + "Sec";
            }
        }
    }

    void ShowTeamTag()
    {
        if (GameStart.teamMode != "FreeForAll")
        {
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (isDead[i])
                {
                    teamTag[i].gameObject.SetActive(false);
                }
                else
                {
                    teamTag[i].gameObject.SetActive(true);
                }
            }
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                Vector2 tagPos = players[i].transform.position;
                tagPos.x += 0.7f;
                tagPos.y += 0.47f;
                teamTag[i].transform.position = tagPos;
            }
        }
    }
    public void GoalProcess(int playerid)
    {
        // ゴールしたプレイヤーを表示する
        clearTime[Goals] = GameSetting.playTime;
        goaledPlayer[Goals] = "Player" + playerid.ToString();
        SoundEffect.soundTrigger[2] = 1;
        Goals++;
        players[playerid - 1].gameObject.SetActive(false);
        sticks[playerid - 1].gameObject.SetActive(false);
        nameTags[playerid - 1].gameObject.SetActive(false);
    }
    void LastChance()
    {
        if (GameSetting.playTime < 30)
        {
            switch (GameStart.Stage)
            {
                case 1:
                    for (int i = 0; i < GameStart.MaxPlayer; i++)
                    {
                        GameSetting.respownPos[i] = chanceRespown1[i].gameObject.transform.position;
                    }
                    break;
                case 2:
                    for (int i = 0; i < GameStart.MaxPlayer; i++)
                    {
                        GameSetting.respownPos[i] = chanceRespown2[i].gameObject.transform.position;
                    }
                    break;
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
        if(GameStart.teamMode == "FreeForAll")
        {
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                pointText[i].text = String.Format("{0:#}", points[i].ToString());
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                teamPoints[i] = 0;
            }
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                teamPoints[GameStart.playerTeam[i]] += points[i];
            }
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (GameStart.teamSize[i] >= 1)
                {
                    pointText[i].text = String.Format("{0:#}", teamPoints[i].ToString());
                }
            }

        }
    }

    void checkResult()
    {
        if (GameSetting.playTime <= 0)
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
            TextCanvas.gameObject.SetActive(true);


            if (count == 0)
            {
                if(GameStart.teamMode == "FreeForAll")
                {
                    //ポイント並び替え
                    points.CopyTo(pointsInOrder, 0);
                    Array.Sort(pointsInOrder);
                    Array.Reverse(pointsInOrder);
                    int num = GameStart.PlayerNumber - 1;

                    //順位計測
                    for (int i = GameStart.PlayerNumber - 1; i >= 0; i--)
                    {

                        if (points[i] == 0)
                        {
                            plasement[num] = "Player" + (i + 1).ToString();
                            num--;
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
                }
                else
                {
                    //ポイント並び替え
                    teamPoints.CopyTo(pointsInOrder, 0);
                    Array.Sort(pointsInOrder);
                    Array.Reverse(pointsInOrder);
                    int num = GameStart.teamCount - 1;

                    //順位計測
                    for (int i = 3; i >= 0; i--)
                    {
                        if (GameStart.teamSize[i] >= 1)
                        {
                            if (teamPoints[i] == 0)
                            {
                                plasement[num] = "Team" + teamTagName[i];
                                num--;
                            }
                            else
                            {
                                for (int j = 0; j < GameStart.teamCount; j++)
                                {
                                    if (pointsInOrder[j] == teamPoints[i])
                                    {
                                        plasement[j] = "Team" + teamTagName[i];
                                    }

                                }
                            }
                        }
                    }
                    topPoint = pointsInOrder[0];
                }
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
                SoundEffect.soundTrigger[7] = 1;
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
            if (GameStart.teamMode == "FreeForAll")
            {
                for (int i = 0; i < GameStart.PlayerNumber; i++)
                {
                    resultText[i].text = "#" + (i + 1) + "   " + plasement[i] + "   " + pointsInOrder[i] + "point";
                }
            }
            else
            {
                for (int i = 0; i < GameStart.teamCount; i++)
                {
                    resultText[i].text = "#" + (i + 1) + "   " + plasement[i] + "   " + pointsInOrder[i] + "point";
                }
            }
               
        }
    }

}


