using Fusion;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerCollisionEvent : NetworkBehaviour
{
    [SerializeField]
    private int _playerId;

    private SessionManager _sessionManager;

    private float _pointTimer;
    private GameRuleManager _gameMode;
    private GamePlayHandler _gameSetting;
    private int _checkNum = 0;

    private void Start()
    {
        _sessionManager = SessionManager.Instance;
        _gameSetting = GameObject.Find("PlaySceneManager").GetComponent<GamePlayHandler>();
        _gameMode = GameObject.Find("PlaySceneManager").GetComponent<GameRuleManager>();

        _pointTimer = 0;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (gameObject.name.Contains("Player"))
        {
            if (other.gameObject.name.Contains("CheckPos"))
            {
                int tmp = int.Parse(Regex.Replace(other.gameObject.name, @"[^0-9]", ""));
                if (tmp > _checkNum)
                {
                    GamePlayHandler.RespownPos[_playerId - 1] = other.gameObject.transform.position;
                    _checkNum = tmp;

                    GameObject icon = GameObject.Find("P" + _playerId + "Icon");
                    icon.SetActive(true);
                    icon.GetComponent<SpriteRenderer>().enabled = true;

                    Vector2 iconPos = other.gameObject.transform.position;
                    iconPos.x -= Constants.CheckPosOffsetX;
                    iconPos.x += Constants.IconOffsetX * _playerId;
                    iconPos.y += Constants.IconOffsetY;
                    icon.transform.position = iconPos;
                    SoundEffect.soundTrigger[2] = 1;
                }
            }

            if (other.gameObject.name.Contains("Point"))
            {
                if (GamePlayHandler.ElapsedTime > 0 && !_gameSetting.IsPaused)
                {
                    _pointTimer += Time.deltaTime;
                    if (_pointTimer > Constants.PointInterval)
                    {
                        if (_sessionManager.PlayingType == "Online")
                        {
                            if (Runner.LocalPlayer.PlayerId == _playerId - 1)
                            {
                                RPCAddFlagPoints(_playerId);
                            }
                        }
                        else
                        {
                            SoundEffect.soundTrigger[6] = 1;
                            GameRuleManager.Points[_playerId - 1] += 1;
                            GameRuleManager.IsParticlePlayed[_playerId - 1] = 1;
                        }
                        _pointTimer = 0;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_sessionManager.GameMode != "Arcade") // 対戦モードを除き
        {
            // 自身がプレイヤーならゴール処理
            if (gameObject.name == "GoalFlag")
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    int playerID = other.gameObject.GetComponent<Body>().Id;
                    if (_sessionManager.PlayingType == "Online")
                    {
                        _gameMode.RPC_GoalProcess(playerID);
                    }
                    else
                    {
                        _gameMode.GoalProcess(playerID);
                    }
                }
            }
        }
        if (other.gameObject.CompareTag("thorn")) // 対戦モード時トゲに当たった時最後に触れていたプレイヤーキル(5秒)
        {
            for (int i = 0; i < GameStartHandler.MaxPlayerCount; i++)
            {
                if (GameRuleManager.KillableDuration[i, _playerId - 1] > 0) // トゲに当たったプレイヤーに最後に5秒以内に触れていたプレイヤーにポイント付与
                {
                    if (_sessionManager.PlayingType == "Online")
                    {
                        if (Runner.LocalPlayer.PlayerId == _playerId - 1)
                        {
                            RPCAddKillPoints(i, _playerId);
                        }
                    }
                    else
                    {
                        GameRuleManager.Points[i] += Constants.KillPoint;
                        GameRuleManager.IsParticlePlayed[i] = 2;
                        GameRuleManager.KillerName = "Player" + (i + 1).ToString();
                        GameRuleManager.DeadName = "Player" + _playerId.ToString();
                        GameRuleManager.KillableDuration[i, _playerId - 1] = 0;
                        GameRuleManager.KillLogTimer = Constants.KillLogDuration;
                    }
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPCAddKillPoints(int target, int myId)
    {
        GameRuleManager.Points[target] += Constants.KillPoint;
        GameRuleManager.IsParticlePlayed[target] = 2;
        GameRuleManager.KillerName = "Player" + (target + 1).ToString();
        GameRuleManager.DeadName = "Player" + myId.ToString();
        GameRuleManager.KillableDuration[target, myId - 1] = 0;
        GameRuleManager.KillLogTimer = Constants.KillLogDuration;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPCAddFlagPoints(int myId)
    {
        SoundEffect.soundTrigger[6] = 1;
        GameRuleManager.Points[myId - 1] += 1;
        GameRuleManager.IsParticlePlayed[myId - 1] = 1;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (_sessionManager.GameMode == "Arcade")
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Stick"))
            {
                // 自分以外に対象のプレイヤーのキル判定が入らないように全員分のキルタイマーを一度リセットする
                for (int i = 0; i < GameStartHandler.MaxPlayerCount; i++)
                {
                    GameRuleManager.KillableDuration[i, other.gameObject.GetComponent<PlayerCollisionEvent>()._playerId - 1] = 0;
                }

                GameRuleManager.KillableDuration[_playerId - 1, other.gameObject.GetComponent<PlayerCollisionEvent>()._playerId - 1] = Constants.KillTimerDuration;
            }
        }
    }
}

