using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Photon.Pun;

public class GameMode : MonoBehaviourPunCallbacks
{
    // 基本
    GameSetting gameSetting;

    public static string[] goaledPlayer { get; set; } = new string[GameStart.maxPlayer];

    [SerializeField]
    GameObject[] resultTextGO = new GameObject[GameStart.maxPlayer];

    [SerializeField]
    GameObject[] icons;

    [SerializeField]
    GameObject[] battleModeUIs;

    [SerializeField]
    GameObject ResultPanel;

    [SerializeField]
    GameObject ResultPanelFront;

    [SerializeField]
    GameObject TextCanvas;

    [SerializeField]
    GameObject backTitleButton;

    [SerializeField]
    GameObject ResultPanelArcade;

    Text[] resultText = new Text[GameStart.maxPlayer];

    bool showResultTriggerd = false;

    public static bool gameEnded;

    // 通常ステージ
    public static byte Goals = 0;

    public static float[] clearTime = new float[GameStart.maxPlayer];

    public static bool isGoaled;

    public static bool isGameEnded;
    //バトルモード
    [SerializeField]
    GameObject KillLogBack;

    [SerializeField]
    GameObject Plus1;

    [SerializeField]
    GameObject drawTextGO;

    [SerializeField]
    GameObject Plus5;

    GameObject[] pointTextGO = new GameObject[4];
    GameObject[] pointFrame = new GameObject[4];
    GameObject[] crownObj = new GameObject[4];

    [SerializeField]
    GameObject[] teamTag;

    [SerializeField]
    private Text[] teamTagText;

    string[] teamTagName = { "A", "B", "C", "D" };
    string[] teamsInOrder = new string[4];

    private Color[] teamColors = { Color.white, Color.red, Color.blue, Color.green };

    public static bool[] isDead = { false, false, false, false };
    public static float[] points = new float[4];
    public static float[] pointsInOrder = new float[4];
    public static float[] teamPoints = new float[4];
    public static float KillLogTimer;

    public static string[] playerNameByRank = new string[4];

    int[] playerRank = new int[4];

    Text[] pointText = new Text[4];

    public static string killer;
    public static string died;

    public Text KillLogText = null;

    public static byte[] playParticle = new byte[4];

    Vector2[] framePos = new Vector2[4];

    public static bool isTimeFinished;

    byte count = 0;

    bool isDraw = true;

    float frameSpace = 10;

    private Vector2[] particlePos = new Vector2[4];

    public static float[,] killTimer = new float[4, 4];  // プレイヤー同士の衝突を記録(プレイヤー1～4とプレイヤー1～4の衝突)

    // サッカーモード
    [SerializeField]
    Text ballCountText;

    Vector2 ballPosDefault = new Vector2(0, -2f);

    // 無限モード
    public static bool isGameOver;

    int startTrigger = 0;


    void Start()
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        startTrigger = 0;
        backTitleButton.gameObject.SetActive(false);
        showResultTriggerd = false;
        isDraw = true;
    }

    void StartInUpdate()
    {   
        for (int i = 0; i < GameStart.maxPlayer; i++)
        {
            resultText[i] = resultTextGO[i].GetComponent<Text>();
            teamTag[i].gameObject.SetActive(false);
            icons[i].gameObject.SetActive(false);
            isDead[i] = false;
            for (int j = 0; j < 4; j++)
            {
                killTimer[i, j] = 0f;
            }
        }
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        TextCanvas.gameObject.SetActive(false);
        KillLogBack.gameObject.SetActive(false);
        ballCountText = GameObject.Find("BallSpownCount").GetComponent<Text>();
        ballCountText.text = null;
        isTimeFinished = false;
        isGameEnded = false;
        isGoaled = false;
        isGameOver = false;
        Goals = 0;
        // 通常ステージ
        if (GameStart.gameMode2 != "Arcade")
        {
            for (int i = 0; i < GameStart.maxPlayer; i++)
            { 
                clearTime[i] = 0;
                goaledPlayer[i] = null;
            }
            for (int j = 0; j < battleModeUIs.Length; j++)
            {
                battleModeUIs[j].SetActive(false);
            }
        }

        //アーケード
        if ((GameStart.gameMode1 == "Multi" || GameStart.gameMode1 == "Online") && GameStart.gameMode2 == "Arcade")
        {
            for (int i = 0; i < 4; i++)
            {
                playerNameByRank[i] = null;
                points[i] = 0;
                teamPoints[i] = 0;
                pointsInOrder[i] = 0;
                if (GameStart.teamMode == "FFA")
                {
                    pointTextGO[i] = GameObject.Find("P" + (i + 1).ToString() + "Point");
                    pointFrame[i] = GameObject.Find("PointFrame" + (i + 1).ToString());
                    GameObject.Find("TeamFrame" + (i + 1).ToString()).SetActive(false);
                }
                else
                {
                    pointTextGO[i] = GameObject.Find("TeamPoint" + (i + 1).ToString());
                    pointFrame[i] = GameObject.Find("TeamFrame" + (i + 1).ToString());
                    GameObject.Find("PointFrame" + (i + 1).ToString()).SetActive(false);
                }
                pointFrame[i].SetActive(false);
                pointText[i] = pointTextGO[i].GetComponent<Text>();
            }
            count = 0;
            KillLogTimer = 0;

            AdjustTeamFramePos();

            //チームタグ表示
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (GameStart.teamMode != "FFA")
                {
                    teamTag[i].gameObject.SetActive(true);
                    teamTagText[i].text = teamTagName[GameStart.playerTeam[i]];
                    teamTagText[i].color = teamColors[GameStart.playerTeam[i]];
                }
            }

            //旗取りモード
            if (GameStart.stage == 1)
            {
                if (GameStart.teamMode == "FFA")
                {
                    for (int i = 0; i < GameStart.PlayerNumber; i++) { pointFrame[i].gameObject.SetActive(true);  }
                    for (int i = 3; i >= GameStart.PlayerNumber; i--) { pointFrame[i].gameObject.SetActive(false);}
                }
                else
                {
                    for (int i = 0; i < GameStart.PlayerNumber; i++)
                    {
                        if (GameStart.teamSize[i] >= 1)
                        {
                            pointFrame[i].gameObject.SetActive(true);
                        }
                    }
                }

            }
            //サッカーモード
            if (GameStart.stage == 2)
            {
                //チームレフト
                pointFrame[0].gameObject.SetActive(true);
                //チームライト
                pointFrame[1].gameObject.SetActive(true);
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
            if (GameStart.stage == 1)
            {
                KillSystem();
                PointDisplay();
                PlayParticle();
                checkResult();
                ShowResult();
            }
            //サッカーモード
            if (GameStart.stage == 2)
            {
                PointDisplay();
                checkResult();
                ShowResult();
                KillSystem();
            }
        }

        // 無限モード
        if (GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Arcade")
        {
            SettingInfinityMode();
        }
    }
    private void FixedUpdate()
    {
        if (!GameSetting.setupEnded)
        {
            return;
        }
        if(GameStart.gameMode1 != "Single" && GameStart.gameMode2 == "Arcade") 
        {
            ShowTeamTag();
        }

    }
    //通常ステージ
    void CheckFinish()
    {
        if (Goals == GameStart.PlayerNumber)
        {
            isGameEnded = true;
            GameSetting.Playable = false;
            TextCanvas.gameObject.SetActive(true);
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            Vector2[] iconPos = new Vector2[GameStart.maxPlayer];
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                int pId = int.Parse(Regex.Replace(goaledPlayer[i], @"[^0-9]", ""));
                icons[pId - 1].SetActive(true);
                iconPos[pId - 1] = resultTextGO[i].transform.position;
                iconPos[pId - 1].x -= 0.7f;
                icons[pId - 1].transform.position = iconPos[pId - 1];
                resultText[i].text = "#" + (i + 1).ToString() + "     " + clearTime[i] + "Sec";
            }
        }
    }

    void ShowTeamTag()
    {
        if (GameStart.teamMode != "FFA")
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
                tagPos.y += 0.68f;
                teamTag[i].transform.position = tagPos;
            }
        }
    }
    public void GoalProcess(int playerid)
    {
        //（オンライン）1位の時winningsプラス１
        if (GameStart.gameMode1 == "Online" && Goals == 0)
        {
            if(NetWorkMain.GetCustomProps<int[]>("winnings", out var ValueBArray))
            {
                ValueBArray[playerid - 1]++;
                NetWorkMain.SetCustomProps<int[]>("winnings", ValueBArray);
            }
        }
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
        if(playerid == NetWorkMain.netWorkId)
        {
            isGoaled = true;
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
        if (GameStart.stage == 2) { KillLogText.text = ""; KillLogBack.gameObject.SetActive(false); }
    }

    void PointDisplay() //ポイント小数点以下切り捨て＆表示
    {

        if (GameStart.teamMode == "FFA")
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
                if (GameStart.stage == 1)
                {
                    teamPoints[i] = 0;
                }           
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

    IEnumerator ShowBackToTitleButton() 
    {
        yield return new WaitForSeconds(3.0f);
        backTitleButton.gameObject.SetActive(true);
    }
    void checkResult()
    {
        if (GameSetting.playTime <= 0 && !isTimeFinished)
        {
            isTimeFinished = true;
            isGameEnded = true;
            GameSetting.Playable = false;
            if (GameStart.gameMode2 != "Arcade") 
            {
                for (int i = 0; i < GameStart.PlayerNumber; i++)
                {
                    gameSetting.players[i].gameObject.SetActive(false);
                    gameSetting.sticks[i].gameObject.SetActive(false);
                    gameSetting.nameTags[i].gameObject.SetActive(false);
                }
                ResultPanel.gameObject.SetActive(true);
                ResultPanelFront.gameObject.SetActive(true);
                TextCanvas.gameObject.SetActive(true);
            }
            else 
            {
                //ResultPanelArcade.gameObject.SetActive(true);
                StartCoroutine(ShowBackToTitleButton());
            }
          


            if (count == 0)
            {
                if (GameStart.teamMode == "FFA")
                {
                    //ポイント並び替え
                    points.CopyTo(pointsInOrder, 0);
                    Array.Sort(pointsInOrder);
                    Array.Reverse(pointsInOrder);
                    int num = GameStart.PlayerNumber - 1;
                    bool[] ignore = { false, false, false, false };
                    //順位計測
                    for (int i = GameStart.PlayerNumber - 1; i >= 0; i--)
                    {
                        for (int j = GameStart.PlayerNumber - 1; j >= 0; j--)
                        {
                            Debug.Log("j==" + j);
                            if (pointsInOrder[j] == points[i])
                            {

                                playerNameByRank[j] = "Player" + (i + 1).ToString();
                                playerRank[i] = j;
                                if (GameStart.gameMode1 == "Online" && j == 0)
                                {
                                    if (NetWorkMain.GetCustomProps<int[]>("winnings", out var ValueCArray))
                                    {
                                        ValueCArray[i]++;
                                        NetWorkMain.SetCustomProps<int[]>("winnings", ValueCArray);
                                    }
                                }
                            }
                        }
                    }
                    for (int i = GameStart.PlayerNumber - 1; i >= 0; i--)
                    {
                            if (points[i] != points[0])
                            {
                                isDraw = false;
                                break;
                            }
                    }
                }
                else
                {
                    //ポイント並び替え
                    teamPoints.CopyTo(pointsInOrder, 0);
                    Array.Sort(pointsInOrder);
                    Array.Reverse(pointsInOrder);
                    int num = GameStart.teamCount - 1;
                    bool[] ignore = { false, false, false, false };
                    //順位計測
                    for (int i = GameStart.PlayerNumber - 1; i >= 0; i--)
                    {
                        if (GameStart.teamSize[i] >= 1)
                        {
                            for (int j = GameStart.teamCount - 1; j >= 0; j--)
                            {
                                    if (pointsInOrder[j] == teamPoints[i])
                                    {
                                        teamsInOrder[j] = "Team" + teamTagName[i];
                                        for (int l = 0; l < GameStart.PlayerNumber; l++)
                                        {
                                            if (GameStart.playerTeam[l] == i)
                                            {
                                                playerRank[l] = j;
                                                Debug.Log("Player" + (l + 1).ToString() + "の順位を" + (j + 1).ToString() + "に");
                                            }
                                        }
                                    }
                            }
                        }
                        float firstValue = -1;
                        for (int k = GameStart.PlayerNumber - 1; k >= 0; k--)
                        {
                            if (GameStart.teamSize[k] >= 1)
                            {
                                if (firstValue == -1) { firstValue = teamPoints[k]; }
                                if (teamPoints[k] != firstValue)
                                {
                                    isDraw = false;
                                    break;
                                }
                            }
                        }
                    }
 
                }
                count = 1;
            }
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                //王冠表示
                if (!isDraw) 
                {
                    if (playerRank[i] < 3)
                    {
                        GameObject crownPrefab = (GameObject)Resources.Load("Crown" + (playerRank[i] + 1).ToString());
                        Vector2 crownPos = gameSetting.players[i].transform.position;
                        crownPos.y += 1f;
                        crownObj[i] = Instantiate(crownPrefab, crownPos, Quaternion.identity);
                        crownObj[i].name = "Crown" + (playerRank[i] + 1).ToString();
                        GameObject smokeAnim = (GameObject)Resources.Load("SmokeEffect");
                        Instantiate(smokeAnim, crownPos, Quaternion.identity);
                    }
                }
                else 
                {
                    drawTextGO.gameObject.SetActive(true);
                }
            }
        }
        if (count == 1 && !isDraw)
        {
            for (int h = 0; h < GameStart.PlayerNumber; h++)
            {
                if (playerRank[h] < 3) 
                {
                    Vector2 crownPos = gameSetting.players[h].transform.position;
                    crownPos.y += 1f;
                    crownObj[h].transform.position = crownPos;
                }
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
        if (isTimeFinished && !showResultTriggerd)
        {
            //リザルト表示
            Vector2[] iconPos = new Vector2[GameStart.maxPlayer];
            if (GameStart.teamMode == "FFA")
            {
                for (int i = 0; i < GameStart.PlayerNumber; i++)
                {
                    int pId = int.Parse(Regex.Replace(playerNameByRank[i], @"[^0-9]", ""));
                    icons[pId - 1].SetActive(true);
                    iconPos[pId - 1] = resultTextGO[i].transform.position;
                    iconPos[pId - 1].x -= 0.7f;
                    icons[pId - 1].transform.position = iconPos[pId - 1];
                    resultText[i].text = "#" + (i + 1) + "     " + pointsInOrder[i] + "point";          
                }
            }
            else
            {
                for (int i = 0; i < GameStart.teamCount; i++)
                {
                    resultText[i].text = "#" + (i + 1) + "   " + teamsInOrder[i] + "   " + pointsInOrder[i] + "point";
                }

            }
        }
        SetHighScore();
        showResultTriggerd = true;
    }

    void SettingInfinityMode()
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

    public IEnumerator BallReset(GameObject ball)
    {
        ball.transform.position = ballPosDefault;
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0;
        ballRb.constraints = RigidbodyConstraints2D.FreezeAll;
        ball.GetComponent<CircleCollider2D>().enabled = false;

        Vector2 textPos = ballPosDefault;
        ballCountText.transform.position = textPos;
        // 開始するカウントダウンの数
        float countdown = 3.0f;
        while (countdown > 0)
        {
            if (Mathf.Floor(countdown) < Mathf.Floor(countdown + Time.deltaTime))
            {
                SoundEffect.soundTrigger[3] = 1;
            }
            // カウントダウンの整数部分を表示
            ballCountText.text = Mathf.Ceil(countdown).ToString();
            // Time.deltaTime を使用して時間を減少させる
            countdown -= Time.deltaTime;
            // 次のフレームまで待機
            yield return null; 
        }
        ballCountText.text = null;
        ball.GetComponent<CircleCollider2D>().enabled = true;
        ballRb.constraints = RigidbodyConstraints2D.None;
        ballRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        ballRb.AddForce(new Vector2(0, -0.1f));
        ball.GetComponent<Ball>().count = 0;
    }

    void AdjustTeamFramePos()
    {
        //フラッグモードの時
        if (GameStart.stage == 1)
        {
            for (int i = 0; i < pointFrame.Length; i++)
            {
                //チームフレームを初期位置に設定
                framePos[i] = pointFrame[0].transform.position;
                //右にずらす
                framePos[i].x += (i * (frameSpace / (-1 + (float)GameStart.PlayerNumber)));
                pointFrame[i].transform.position = framePos[i];
            }
        }
        //サッカーモードの時
        if (GameStart.stage == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                //チームフレームを初期位置に設定
                framePos[i] = pointFrame[0].transform.position;
                //右にずらす
                framePos[i].x += frameSpace * i;
                pointFrame[i].transform.position = framePos[i];
            }
        }
    }


     void SetHighScore()
    {
        if (GameStart.gameMode1 == "Single")
        {
            if (GameStart.gameMode2 == "Nomal")
            {
                if (ShowHighScore.singleHighScore[GameStart.stage - 1] == 0) { ShowHighScore.singleHighScore[GameStart.stage - 1] = (int)(GameMode.clearTime[0]); }
                ShowHighScore.singleHighScore[GameStart.stage - 1] = Mathf.Min((int)(GameMode.clearTime[0]), (int)(ShowHighScore.singleHighScore[GameStart.stage - 1]));
            }
            else
            {
                if (ShowHighScore.singleHighScore[GameStart.stage - 1] == 0) { ShowHighScore.singleArcadeHighScore[GameStart.stage - 1] = (int)(GenerateStage.maxHeight); }
                ShowHighScore.singleArcadeHighScore[GameStart.stage - 1] = Mathf.Max((int)(GenerateStage.maxHeight), (int)(ShowHighScore.singleArcadeHighScore[GameStart.stage - 1]));
            }
        }
        else
        {
            if (GameStart.gameMode2 == "Nomal")
            {
                if (ShowHighScore.multiHighScore[GameStart.stage - 1] == 0) { ShowHighScore.multiHighScore[GameStart.stage - 1] = (int)(GameMode.clearTime[0]); }
                ShowHighScore.multiHighScore[GameStart.stage - 1] = Mathf.Min((int)(GameMode.clearTime[0]), (int)(ShowHighScore.multiHighScore[GameStart.stage - 1]));
            }
            else
            {
                ShowHighScore.multiArcadeHighScore[GameStart.stage - 1] = Mathf.Max((int)(GameMode.pointsInOrder[0]), (int)(ShowHighScore.multiHighScore[GameStart.stage - 1]));
            }
        }
    }
}


