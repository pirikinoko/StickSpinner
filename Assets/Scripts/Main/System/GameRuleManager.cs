using Cysharp.Threading.Tasks;
using Fusion;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameRuleManager : NetworkBehaviour
{
    // 基本
    private GamePlayHandler _gamePlayHandler;

    private SessionManager _sessionManager;

    public static string[] FinishedPlayer { get; set; } = new string[GameStartHandler.MaxPlayerCount];

    [SerializeField]
    GameObject[] _resultTextGO = new GameObject[GameStartHandler.MaxPlayerCount];

    [SerializeField]
    GameObject[] _resultIcons;

    [SerializeField]
    GameObject _resultPanel;

    [SerializeField]
    GameObject _resultPanelFront;

    [SerializeField]
    GameObject _textCanvas;

    [SerializeField]
    GameObject _uiBackToTitle;

    GameObject[] _crownObjects = new GameObject[3];

    Text[] _resultText = new Text[GameStartHandler.MaxPlayerCount];

    bool _isResultTriggered = false;

    public static bool IsGameEnded;

    // 通常ステージ
    public static byte Goals = 0;

    public static float[] clearTime = new float[GameStartHandler.MaxPlayerCount];

    public static bool isGoaled;

    //バトルモード
    [SerializeField]
    GameObject _killLogBack;

    [SerializeField]
    GameObject _plus1;

    [SerializeField]
    GameObject _drawTextGO;

    [SerializeField]
    GameObject _plus5;

    GameObject[] _pointTextGO = new GameObject[4];
    GameObject[] _pointFrame = new GameObject[4];
    GameObject[] _crownObj = new GameObject[4];

    [SerializeField]
    private Text[] _teamTagText;

    string[] _teamTagName = { "A", "B", "C", "D" };
    string[] _teamsInOrder = new string[4];

    private Color[] _teamColors = { Color.white, Color.red, Color.blue, Color.green };

    public static bool[] IsDead = { false, false, false, false };
    public static float[] Points = new float[4];
    public static float[] PointsInOrder = new float[4];
    public static float[] TeamPoints = new float[4];
    public static float KillLogTimer;

    public static string[] PlayerNamesByRank = new string[4];

    int[] _playerRanks = new int[4];

    Text[] _pointTexts = new Text[4];

    public static string KillerName;
    public static string DeadName;

    public Text KillLogText = null;

    public static byte[] IsParticlePlayed = new byte[4];

    Vector2[] _framePos = new Vector2[4];

    public static bool IsTimeExpired;

    byte _count = 0;

    bool _isDraw = true;

    float _spaceBetweenFrames = 10;

    private Vector2[] _particlePos = new Vector2[4];

    public static float[,] KillableDuration = new float[4, 4];  // プレイヤー同士の衝突を記録(プレイヤー1～4とプレイヤー1～4の衝突)

    // サッカーモード
    [SerializeField]
    Text _ballCountText;

    Vector2 _defaultBallPos = new Vector2(0, -2f);

    int startTrigger = 0;


    void Start()
    {
        _sessionManager = SessionManager.Instance;
        _gamePlayHandler = GameObject.Find("PlaySceneManager").GetComponent<GamePlayHandler>();
        for (int i = 0; i < _crownObjects.Length; i++)
        {
            _crownObjects[i] = (GameObject)Resources.Load("Crown" + (i + 1).ToString());
        }
        startTrigger = 0;
        _uiBackToTitle.gameObject.SetActive(false);
        _isResultTriggered = false;
        _isDraw = true;
    }

    void StartInUpdate()
    {
        for (int i = 0; i < GameStartHandler.MaxPlayerCount; i++)
        {
            _resultText[i] = _resultTextGO[i].GetComponent<Text>();
            _teamTagText[i].gameObject.SetActive(false);
            _resultIcons[i].gameObject.SetActive(false);
            IsDead[i] = false;
            for (int j = 0; j < 4; j++)
            {
                KillableDuration[i, j] = 0f;
            }
        }

        _resultPanel.gameObject.SetActive(false);
        _resultPanelFront.gameObject.SetActive(false);
        _textCanvas.gameObject.SetActive(false);
        _killLogBack.gameObject.SetActive(false);
        _ballCountText.text = null;
        IsTimeExpired = false;
        isGoaled = false;
        IsGameEnded = false;
        Goals = 0;

        // 通常ステージ
        if (_sessionManager.GameMode != "Arcade")
        {
            for (int i = 0; i < GameStartHandler.MaxPlayerCount; i++)
            {
                clearTime[i] = 0;
                FinishedPlayer[i] = null;
            }
        }

        //アーケード
        if ((_sessionManager.PlayingType == "Multi" || _sessionManager.PlayingType == "Online") && _sessionManager.GameMode == "Arcade")
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerNamesByRank[i] = null;
                Points[i] = 0;
                TeamPoints[i] = 0;
                PointsInOrder[i] = 0;
                if (_sessionManager.TeamMode == "FFA")
                {
                    _pointTextGO[i] = GameObject.Find("P" + (i + 1).ToString() + "Point");
                    _pointFrame[i] = GameObject.Find("PointFrame" + (i + 1).ToString());
                    GameObject.Find("TeamFrame" + (i + 1).ToString()).SetActive(false);
                }
                else
                {
                    _pointTextGO[i] = GameObject.Find("TeamPoint" + (i + 1).ToString());
                    _pointFrame[i] = GameObject.Find("TeamFrame" + (i + 1).ToString());
                    GameObject.Find("PointFrame" + (i + 1).ToString()).SetActive(false);
                }
                _pointFrame[i].SetActive(false);
                _pointTexts[i] = _pointTextGO[i].GetComponent<Text>();
            }
            _count = 0;
            KillLogTimer = 0;

            AdjustTeamFramePos();

            //チームタグ表示
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                if (_sessionManager.TeamMode != "FFA")
                {
                    _teamTagText[i].gameObject.SetActive(true);
                    _teamTagText[i].text = _teamTagName[_sessionManager.PlayerTeam[i]];
                    _teamTagText[i].color = _teamColors[_sessionManager.PlayerTeam[i]];
                }
            }

            //旗取りモード
            if (_sessionManager.Stage == 1)
            {
                if (_sessionManager.TeamMode == "FFA")
                {
                    for (int i = 0; i < GameStartHandler.PlayerCount; i++) { _pointFrame[i].gameObject.SetActive(true); }
                    for (int i = 3; i >= GameStartHandler.PlayerCount; i--) { _pointFrame[i].gameObject.SetActive(false); }
                }
                else
                {
                    for (int i = 0; i < GameStartHandler.PlayerCount; i++)
                    {
                        if (GameStartHandler.TeamSize[i] >= 1)
                        {
                            _pointFrame[i].gameObject.SetActive(true);
                        }
                    }
                }

            }
            //サッカーモード
            if (_sessionManager.Stage == 2)
            {
                //チームレフト
                _pointFrame[0].gameObject.SetActive(true);
                //チームライト
                _pointFrame[1].gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (_sessionManager.PlayingType == "Online" && !GamePlayHandler.AllJoin)
        {
            return;
        }
        if (startTrigger == 0)
        {
            StartInUpdate();
            startTrigger = 1;
        }

        // 通常ステージ
        if (_sessionManager.GameMode != "Arcade")
        {
            CheckFinish();
        }
        //アーケード
        if ((_sessionManager.PlayingType == "Multi" || _sessionManager.PlayingType == "Online") && _sessionManager.GameMode == "Arcade")
        {
            //旗取りモード
            if (_sessionManager.Stage == 1)
            {
                KillSystem();
                PointDisplay();
                PlayParticle();
                CheckResult();
                ShowResult();
            }
            //サッカーモード
            if (_sessionManager.Stage == 2)
            {
                PointDisplay();
                CheckResult();
                ShowResult();
                KillSystem();
            }
        }

        // 無限モード
        if (_sessionManager.PlayingType == "Single" && _sessionManager.GameMode == "Arcade")
        {
            SettingInfinityMode();
        }
    }
    private void FixedUpdate()
    {
        if (!GamePlayHandler.IsSetupEnded)
        {
            return;
        }
        if (_sessionManager.PlayingType != "Single" && _sessionManager.GameMode == "Arcade")
        {
            ShowTeamTag();
        }

    }
    //通常ステージ
    void CheckFinish()
    {
        if (Goals == GameStartHandler.PlayerCount)
        {
            IsGameEnded = true;
            GamePlayHandler.Playable = false;
            _textCanvas.gameObject.SetActive(true);
            _resultPanel.gameObject.SetActive(true);
            _resultPanelFront.gameObject.SetActive(true);
            Vector2[] iconPos = new Vector2[GameStartHandler.MaxPlayerCount];
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                int pId = int.Parse(Regex.Replace(FinishedPlayer[i], @"[^0-9]", ""));
                _resultIcons[pId - 1].SetActive(true);
                iconPos[pId - 1] = _resultTextGO[i].transform.position;
                iconPos[pId - 1].x -= 0.7f;
                _resultIcons[pId - 1].transform.position = iconPos[pId - 1];
                _resultText[i].text = "#" + (i + 1).ToString() + "     " + clearTime[i] + "Sec";
            }
        }
    }

    void ShowTeamTag()
    {
        if (_sessionManager.TeamMode != "FFA")
        {
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                if (IsDead[i])
                {
                    _teamTagText[i].gameObject.SetActive(false);
                }
                else
                {
                    _teamTagText[i].gameObject.SetActive(true);
                }
            }
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                Vector2 tagPos = _gamePlayHandler.Players[i].transform.position;
                tagPos.y += 0.68f;
                _teamTagText[i].transform.position = tagPos;
            }
        }
    }


    public void GoalProcess(int playerid)
    {
        //（オンライン）1位の時winningsプラス１
        if (_sessionManager.PlayingType == "Online" && Goals == 0)
        {
            _sessionManager.WinningsToSend[playerid - 1]++;
        }
        if (_gamePlayHandler.Players[playerid - 1].activeSelf)
        {
            clearTime[Goals] = GamePlayHandler.ElapsedTime;
            FinishedPlayer[Goals] = "Player" + playerid.ToString();
            SoundEffect.soundTrigger[2] = 1;
            Goals++;
            _gamePlayHandler.Players[playerid - 1].gameObject.SetActive(false);
            _gamePlayHandler.Sticks[playerid - 1].gameObject.SetActive(false);
        }
        if (_sessionManager.PlayingType == "Online" && playerid == Runner.LocalPlayer.PlayerId)
        {
            isGoaled = true;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_GoalProcess(int playerId)
    {
        GoalProcess(playerId);
    }

    //バトルモード
    void KillSystem()
    {
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            for (int j = 0; j < GameStartHandler.PlayerCount; j++)
            {
                if (KillableDuration[i, j] >= 0)
                {
                    KillableDuration[i, j] -= Time.deltaTime;
                }
            }
        }

        KillLogText.text = KillerName + "　→　" + DeadName;
        if (KillLogTimer > 0)
        {
            _killLogBack.gameObject.SetActive(true);
            KillLogTimer -= Time.deltaTime;
        }

        if (KillLogTimer <= 0)
        {
            _killLogBack.gameObject.SetActive(false);
            KillLogText.text = null;
        }

        if (_sessionManager.Stage == 2) { KillLogText.text = ""; _killLogBack.gameObject.SetActive(false); }
    }

    void PointDisplay()
    {

        if (_sessionManager.TeamMode == "FFA")
        {
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                _pointTexts[i].text = String.Format("{0:#}", Points[i].ToString());
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (_sessionManager.Stage == 1)
                {
                    TeamPoints[i] = 0;
                }
            }
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                TeamPoints[_sessionManager.PlayerTeam[i]] += Points[i];
            }
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                if (GameStartHandler.TeamSize[i] >= 1)
                {
                    _pointTexts[i].text = String.Format("{0:#}", TeamPoints[i].ToString());
                }
            }
        }
    }

    private async UniTask ShowBackToTitleButton()
    {
        await UniTask.Delay(3000);
        _uiBackToTitle.gameObject.SetActive(true);
    }

    void CheckResult()
    {
        if (GamePlayHandler.ElapsedTime <= 0 && !IsTimeExpired)
        {
            IsTimeExpired = true;
            IsGameEnded = true;
            GamePlayHandler.Playable = false;
            if (_sessionManager.GameMode != "Arcade")
            {
                for (int i = 0; i < GameStartHandler.PlayerCount; i++)
                {
                    _gamePlayHandler.Players[i].gameObject.SetActive(false);
                }
                _resultPanel.gameObject.SetActive(true);
                _resultPanelFront.gameObject.SetActive(true);
                _textCanvas.gameObject.SetActive(true);
            }
            else
            {
                ShowBackToTitleButton().Forget();
            }

            if (_count == 0)
            {
                if (_sessionManager.TeamMode == "FFA")
                {
                    //ポイント並び替え
                    Points.CopyTo(PointsInOrder, 0);
                    Array.Sort(PointsInOrder);
                    Array.Reverse(PointsInOrder);
                    int num = GameStartHandler.PlayerCount - 1;
                    bool[] ignore = { false, false, false, false };
                    //順位計測
                    for (int i = GameStartHandler.PlayerCount - 1; i >= 0; i--)
                    {
                        for (int j = GameStartHandler.PlayerCount - 1; j >= 0; j--)
                        {
                            Debug.Log("j==" + j);
                            if (PointsInOrder[j] == Points[i])
                            {

                                PlayerNamesByRank[j] = "Player" + (i + 1).ToString();
                                _playerRanks[i] = j;
                                if (_sessionManager.PlayingType == "Online" && j == 0)
                                {
                                    _sessionManager.WinningsToSend[i]++;
                                }
                            }
                        }
                    }
                    for (int i = GameStartHandler.PlayerCount - 1; i >= 0; i--)
                    {
                        if (Points[i] != Points[0])
                        {
                            _isDraw = false;
                            break;
                        }
                    }
                }
                else
                {
                    //ポイント並び替え
                    TeamPoints.CopyTo(PointsInOrder, 0);
                    Array.Sort(PointsInOrder);
                    Array.Reverse(PointsInOrder);
                    int num = GameStartHandler.TeamCount - 1;
                    bool[] ignore = { false, false, false, false };
                    //順位計測
                    for (int i = GameStartHandler.PlayerCount - 1; i >= 0; i--)
                    {
                        if (GameStartHandler.TeamSize[i] >= 1)
                        {
                            for (int j = GameStartHandler.TeamCount - 1; j >= 0; j--)
                            {
                                if (PointsInOrder[j] == TeamPoints[i])
                                {
                                    _teamsInOrder[j] = "Team" + _teamTagName[i];
                                    for (int l = 0; l < GameStartHandler.PlayerCount; l++)
                                    {
                                        if (_sessionManager.PlayerTeam[l] == i)
                                        {
                                            _playerRanks[l] = j;
                                            Debug.Log("Player" + (l + 1).ToString() + "の順位を" + (j + 1).ToString() + "に");
                                        }
                                    }
                                }
                            }
                        }
                        float firstValue = -1;
                        for (int k = GameStartHandler.PlayerCount - 1; k >= 0; k--)
                        {
                            if (GameStartHandler.TeamSize[k] >= 1)
                            {
                                if (firstValue == -1) { firstValue = TeamPoints[k]; }
                                if (TeamPoints[k] != firstValue)
                                {
                                    _isDraw = false;
                                    break;
                                }
                            }
                        }
                    }

                }
                _count = 1;
            }
            for (int i = 0; i < GameStartHandler.PlayerCount; i++)
            {
                if (!_isDraw)
                {
                    if (_playerRanks[i] < 3)
                    {
                        Vector2 Pos = _gamePlayHandler.Players[i].transform.position;
                        if (_playerRanks[i] == 0) { Instantiate((GameObject)Resources.Load("PaperCanon"), Pos, Quaternion.identity); }
                        Pos.y += 1f;
                        _crownObj[i] = Instantiate(_crownObjects[_playerRanks[i]], Pos, Quaternion.identity);
                        _crownObj[i].name = "Crown" + (_playerRanks[i] + 1).ToString();
                    }
                }
                else
                {
                    _drawTextGO.gameObject.SetActive(true);
                }
            }
        }
        if (_count == 1 && !_isDraw)
        {
            for (int h = 0; h < GameStartHandler.PlayerCount; h++)
            {
                if (_playerRanks[h] < 3)
                {
                    Vector2 crownPos = _gamePlayHandler.Players[h].transform.position;
                    crownPos.y += 1f;
                    _crownObj[h].transform.position = crownPos;
                }
            }
        }
    }

    void PlayParticle()
    {
        //パーティクル再生
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            _particlePos[i] = _gamePlayHandler.Players[i].gameObject.transform.position;
            _particlePos[i].y += 0.3f;
            if (IsParticlePlayed[i] == 1)
            {
                Instantiate(_plus1, _particlePos[i], Quaternion.identity);
                IsParticlePlayed[i] = 0;
            }
            if (IsParticlePlayed[i] == 2)
            {
                SoundEffect.soundTrigger[7] = 1;
                Instantiate(_plus5, _particlePos[i], Quaternion.identity);
                IsParticlePlayed[i] = 0;
            }
        }
    }

    void ShowResult()
    {
        if (IsTimeExpired && !_isResultTriggered)
        {
            //リザルト表示
            Vector2[] iconPos = new Vector2[GameStartHandler.MaxPlayerCount];
            if (_sessionManager.TeamMode == "FFA")
            {
                for (int i = 0; i < GameStartHandler.PlayerCount; i++)
                {
                    int pId = int.Parse(Regex.Replace(PlayerNamesByRank[i], @"[^0-9]", ""));
                    _resultIcons[pId - 1].SetActive(true);
                    iconPos[pId - 1] = _resultTextGO[i].transform.position;
                    iconPos[pId - 1].x -= 0.7f;
                    _resultIcons[pId - 1].transform.position = iconPos[pId - 1];
                    _resultText[i].text = "#" + (i + 1) + "     " + PointsInOrder[i] + "point";
                }
            }
            else
            {
                for (int i = 0; i < GameStartHandler.TeamCount; i++)
                {
                    _resultText[i].text = "#" + (i + 1) + "   " + _teamsInOrder[i] + "   " + PointsInOrder[i] + "point";
                }
            }
        }
        SetHighScore();
        _isResultTriggered = true;
    }

    void SettingInfinityMode()
    {
        if (IsGameEnded)
        {
            GamePlayHandler.Playable = false;
            _textCanvas.gameObject.SetActive(true);
            _resultPanel.gameObject.SetActive(true);
            _resultPanelFront.gameObject.SetActive(true);
            _resultText[0].text = "Score: " + (int)GenerateStage.maxHeight + "m";
        }
    }

    public IEnumerator BallReset(GameObject ball)
    {
        ball.transform.position = _defaultBallPos;
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0;
        ballRb.constraints = RigidbodyConstraints2D.FreezeAll;
        ball.GetComponent<CircleCollider2D>().enabled = false;

        Vector2 textPos = _defaultBallPos;
        _ballCountText.transform.position = textPos;

        float countdown = 3.0f;
        while (countdown > 0)
        {
            if (Mathf.Floor(countdown) < Mathf.Floor(countdown + Time.deltaTime))
            {
                SoundEffect.soundTrigger[3] = 1;
            }

            _ballCountText.text = Mathf.Ceil(countdown).ToString();
            countdown -= Time.deltaTime;
            yield return null;
        }
        _ballCountText.text = null;

        ball.GetComponent<CircleCollider2D>().enabled = true;
        ballRb.constraints = RigidbodyConstraints2D.None;
        ballRb.AddForce(new Vector2(0, -0.1f));
        ball.GetComponent<Ball>().count = 0;
    }

    void SetHighScore()
    {
        if (_sessionManager.PlayingType == "Single")
        {
            if (_sessionManager.GameMode == "Nomal")
            {
                if (ShowHighScore.singleHighScore[_sessionManager.Stage - 1] == 0) { ShowHighScore.singleHighScore[_sessionManager.Stage - 1] = (int)GameRuleManager.clearTime[0]; }
                ShowHighScore.singleHighScore[_sessionManager.Stage - 1] = Mathf.Min((int)GameRuleManager.clearTime[0], (int)ShowHighScore.singleHighScore[_sessionManager.Stage - 1]);
            }
            else
            {
                if (ShowHighScore.singleHighScore[_sessionManager.Stage - 1] == 0) { ShowHighScore.singleArcadeHighScore[_sessionManager.Stage - 1] = (int)GenerateStage.maxHeight; }
                ShowHighScore.singleArcadeHighScore[_sessionManager.Stage - 1] = Mathf.Max((int)GenerateStage.maxHeight, (int)ShowHighScore.singleArcadeHighScore[_sessionManager.Stage - 1]);
            }
        }
        else
        {
            if (_sessionManager.GameMode == "Nomal")
            {
                if (ShowHighScore.multiHighScore[_sessionManager.Stage - 1] == 0) { ShowHighScore.multiHighScore[_sessionManager.Stage - 1] = (int)GameRuleManager.clearTime[0]; }
                ShowHighScore.multiHighScore[_sessionManager.Stage - 1] = Mathf.Min((int)GameRuleManager.clearTime[0], (int)ShowHighScore.multiHighScore[_sessionManager.Stage - 1]);
            }
            else
            {
                ShowHighScore.multiArcadeHighScore[_sessionManager.Stage - 1] = Mathf.Max((int)GameRuleManager.PointsInOrder[0], (int)ShowHighScore.multiHighScore[_sessionManager.Stage - 1]);
            }
        }
    }
}


