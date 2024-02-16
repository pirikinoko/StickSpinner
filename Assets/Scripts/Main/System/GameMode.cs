using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;
using Photon.Pun;   

public class GameMode : MonoBehaviourPunCallbacks
{
    //基本
    GameSetting gameSetting;
    public static string[] goaledPlayer { get; set; } = new string[GameStart.MaxPlayer];
    [SerializeField] GameObject[] resultTextGO = new GameObject[GameStart.MaxPlayer];
    // GameObject[] icons = new GameObject[GameStart.MaxPlayer];
    Text[] resultText = new Text[GameStart.MaxPlayer];
    [SerializeField] GameObject ResultPanel, ResultPanelFront, TextCanvas;
    //通常ステージ
    public static byte Goals = 0;
    public static float[] clearTime = new float[GameStart.MaxPlayer];
    public static bool Goaled;
    //バトルモード
    public GameObject KillLogBack, Plus1, Plus5;
    GameObject[] pointTextGO = new GameObject[4], chanceRespown = new GameObject[4];
    [SerializeField] GameObject[] ffaFrame, teamFrame, teamTag;
    [SerializeField] private Text[] teamTagText;
    string[] teamTagName = { "A", "B", "C", "D" };
    private Color[] teamColors = { Color.white, Color.red, Color.blue, Color.green };
    public static bool[] isDead = { false, false, false, false };
    public static float[] points = new float[4], pointsInOrder = new float[4], teamPoints = new float[4];
    public static float topPoint, KillLogTimer;
    public static string[] plasement = new string[4];
    Text[] pointText = new Text[4];
    public static string killer, died;
    public Text KillLogText = null;
    public static byte[] playParticle = new byte[4];
    Vector2[] teamFramePos = new Vector2[4];
    public static bool Finished;
    byte count = 0;
    float frameSpace = 10;
    private Vector2[] particlePos = new Vector2[4];
    public static float[,] killTimer = new float[4, 4];       // プレイヤー同士の衝突を記録(プレイヤー1～4とプレイヤー1～4の衝突)
    Renderer[] chanceRespown1Rend = new Renderer[4];
    //サッカーモード
    [SerializeField]
    GameObject ball;
    [SerializeField]
    Text ballCountText;
    Vector2 ballPosDefault = new Vector2(0, -2f);
    //無限モード
    public static bool isGameOver;
    int startTrigger = 0;

    void Start() 
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        startTrigger = 0;
    }
    void StartInUpdate()
    {   //基本
        for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
        {
            resultText[i] = resultTextGO[i].GetComponent<Text>();
            teamTag[i].gameObject.SetActive(false);
            //icons[i].gameObject.SetActive(false);
            isDead[i] = false;
            teamTag[i].gameObject.SetActive(false);
            for (int j = 0; j < 4; j++)
            {
                killTimer[i, j] = 0f;
            }
        }
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        TextCanvas.gameObject.SetActive(false);
        ballCountText = GameObject.Find("BallSpownCount").GetComponent<Text>();
        ballCountText.text = null;
        Finished = false;
        Goaled = false;
        isGameOver = false;
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





        //アーケード
        if ((GameStart.gameMode1 == "Multi" || GameStart.gameMode1 == "Online")&& GameStart.gameMode2 == "Arcade")
        {
            //チームタグ表示
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (GameStart.teamMode != "FreeForAll")
                {
                    teamTag[i].gameObject.SetActive(true);
                    teamTagText[i].text = teamTagName[GameStart.playerTeam[i]];
                }
            }
            AdjustTeamFramePos();
            //リセット等
            for (int i = 0; i < 4; i++)
            {
                plasement[i] = null;
                points[i] = 0;
                teamPoints[i] = 0;
                pointsInOrder[i] = 0;
                pointTextGO[i] = GameObject.Find("P" + (i + 1).ToString() + "Point");
                pointText[i] = pointTextGO[i].GetComponent<Text>();
                chanceRespown[i] = GameObject.Find("ChanceRespownPos" + (i + 1).ToString());
                chanceRespown1Rend[i] = chanceRespown[i].gameObject.GetComponent<SpriteRenderer>();
                chanceRespown1Rend[i].enabled = false;
            }
            count = 0;
            KillLogTimer = 0;
            // 画面上部スコア表示リセット
            for (int i = 0; i < 4; i++) { teamFrame[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
            for (int i = 0; i < 4; i++) { ffaFrame[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
            //旗取りモード
            if (GameStart.Stage == 1) 
            { 
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

            }
            //サッカーモード
            if (GameStart.Stage == 2) 
            {
                //チームレフト
                teamFrame[0].gameObject.SetActive(true);
                pointTextGO[0].gameObject.SetActive(true);
                //チームライト
                teamFrame[1].gameObject.SetActive(true);
                pointTextGO[1].gameObject.SetActive(true);
            }

        }

    }

    void Update()
    {
        if (GameStart.gameMode1 == "Online" && !GameSetting.allJoin)
        {
            return;
        }
        if (startTrigger == 0)
        {
            StartInUpdate();
            startTrigger = 1;
        }

        // 通常ステージ
        if (GameStart.gameMode2 != "Arcade")
        {
            CheckFinish();
        }
        //アーケード
        if ((GameStart.gameMode1 == "Multi" || GameStart.gameMode1 == "Online") && GameStart.gameMode2 == "Arcade")
        {
            //旗取りモード
            if (GameStart.Stage == 1) 
            {
                KillSystem();
                PointDisplay();
                PlayParticle();
                checkResult();
                ShowResult();
                LastChance();
            }
            //サッカーモード
            if (GameStart.Stage == 2)
            {
                PointDisplay();
            }
        }

        // 無限モード
        if (GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Arcade")
        {
            InfinityMode();
        }
    }
    private void FixedUpdate()
    {
        if (!GameSetting.allJoin)
        {
            return;
        }
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
                Vector2 tagPos = gameSetting.players[i].transform.position;
                tagPos.x += 0.7f;
                tagPos.y += 0.47f;
                teamTag[i].transform.position = tagPos;
            }
        }
    }
    public void GoalProcess(int playerid)
    {
        if (GameStart.gameMode1 == "Online" && Goals == 0)
        {
            ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (customProps.ContainsKey("winnings"))
            {
                int[] winningsLocal = (int[])customProps["winnings"];
                winningsLocal[playerid - 1]++;
                customProps["winnings"] = winningsLocal;
                Debug.Log("   customProps[winnings] " + winningsLocal[0]);
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

        }
        Debug.Log("playerId == " + playerid);
        if (gameSetting.players[playerid - 1].activeSelf)
        {
            Debug.Log("player" + playerid + "がゴールしました");
            // ゴールしたプレイヤーを表示する
            clearTime[Goals] = GameSetting.playTime;
            goaledPlayer[Goals] = "Player" + playerid.ToString();
            SoundEffect.soundTrigger[2] = 1;
            Goals++;
            gameSetting.players[playerid - 1].gameObject.SetActive(false);
            gameSetting.sticks[playerid - 1].gameObject.SetActive(false);
            gameSetting.nameTags[playerid - 1].gameObject.SetActive(false);
        }

    
    }
    void LastChance()
    {
        if (GameSetting.playTime < 30)
        {
            for (int i = 0; i < GameStart.MaxPlayer; i++)
            {
                GameSetting.respownPos[i] = chanceRespown[i].gameObject.transform.position;
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
        if (GameStart.Stage == 1) 
        {
            if (GameStart.teamMode == "FreeForAll")
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
       else if (GameStart.Stage == 2) 
        {
            pointText[0].text = String.Format("{0:#}", teamPoints[0].ToString());
            pointText[1].text = String.Format("{0:#}", teamPoints[1].ToString());
        }
    }

    void checkResult()
    {
        if (GameSetting.playTime <= 0)
        {
            Finished = true;
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                gameSetting.players[i].gameObject.SetActive(false);
                gameSetting.sticks[i].gameObject.SetActive(false);
                gameSetting.nameTags[i].gameObject.SetActive(false);

            }
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            TextCanvas.gameObject.SetActive(true);


            if (count == 0)
            {
                if (GameStart.teamMode == "FreeForAll")
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

                                    if (GameStart.gameMode1 == "Online" && j == 0)
                                    {
                                        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
                                        if (customProps.ContainsKey("winnings"))
                                        {
                                            int[] winningsLocal = (int[])customProps["winnings"];
                                            winningsLocal[i]++;
                                            customProps["winnings"] = winningsLocal;
                                        }
                                        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
                                    }
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
            particlePos[i] = gameSetting.nameTags[i].gameObject.transform.position;
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

    void InfinityMode() 
    {
        if (isGameOver) 
        {
            GameSetting.Playable = false;
            TextCanvas.gameObject.SetActive(true);
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            // 参加プレイヤー数分タイム表示
            resultText[0].text = "Score: " + (int)GenerateStage.maxHeight + "m";
        }
    }

    public IEnumerator BallReset()
    {
        ball.transform.position = ballPosDefault;
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0;
        ballRb.constraints = RigidbodyConstraints2D.FreezeAll;
        ball.GetComponent<CircleCollider2D>().enabled = false;

        Vector2 textPos = ballPosDefault;
        ballCountText.transform.position = textPos;
        float countdown = 3.0f; // 開始するカウントダウンの数
        while (countdown > 0)
        {
            if (Mathf.Floor(countdown) < Mathf.Floor(countdown + Time.deltaTime))
            {
                SoundEffect.soundTrigger[3] = 1;
            }
            ballCountText.text = Mathf.Ceil(countdown).ToString(); // カウントダウンの整数部分を表示
            countdown -= Time.deltaTime; // Time.deltaTime を使用して時間を減少させる
            yield return null; // 次のフレームまで待機
        }
        ballCountText.text = null;
        ball.GetComponent<CircleCollider2D>().enabled = true;
        ballRb.constraints = RigidbodyConstraints2D.None;
        ballRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        ballRb.AddForce(new Vector2(0, -0.1f)); 
    }

    void AdjustTeamFramePos()
    {
        for (int i = 0; i < teamFrame.Length; i++)
        {
            //チームフレーム位置設定
            //初期位置に設定
            teamFramePos[i] = teamFrame[0].transform.position;
            //右にずらす
            teamFramePos[i].x += (i * (frameSpace / (-1 + (float)GameStart.PlayerNumber)));
            teamFrame[i].transform.position = teamFramePos[i];
        }
    }
}


