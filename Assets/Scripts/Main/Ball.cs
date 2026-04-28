using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    private NetworkObject _networkObject;
    private SessionManager _sessionManager;
    GameRuleManager gameMode;
    GamePlayHandler gameSetting;
    int lastColId;
    public int count;

    [Networked]
    public bool IsOwned { get; set; } // オーナーシップ管理

    [Networked]
    public int OwnerPlayerId { get; set; } // 所有プレイヤーID

    void Start()
    {
        _networkObject = GetComponent<NetworkObject>();
        _sessionManager = SessionManager.Instance;
        gameSetting = GameObject.Find("PlaySceneManager").GetComponent<GamePlayHandler>();
        count = 0;
    }

    void Update()
    {
        if (gameMode == null)
        {
            gameMode = GameObject.Find("PlaySceneManager").GetComponent<GameRuleManager>();
        }
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            lastColId = col.gameObject.GetComponent<Body>().Id;
        }
        if (col.gameObject.CompareTag("Stick"))
        {
            lastColId = col.gameObject.GetComponent<Controller>().id;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!GamePlayHandler.Playable) { return; }

        if (col.gameObject.name == "GoalZoneLeft")
        {
            HandleGoal(1);
        }
        else if (col.gameObject.name == "GoalZoneRight")
        {
            HandleGoal(0);
        }
    }

    private void HandleGoal(int targetTeam)
    {
        RPC_GoalProcess(targetTeam);

        SoundEffect.soundTrigger[8] = 1;
        StartCoroutine(GameObject.Find("PlaySceneManager").GetComponent<GameRuleManager>().BallReset(this.gameObject));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_GoalProcess(int targetTeam)
    {
        if (_sessionManager.PlayingType == "Online" && !_networkObject.HasStateAuthority)
        {
            return;
        }

        if (_sessionManager.TeamMode == "FFA")
        {
            GameRuleManager.Points[targetTeam]++;
        }
        else
        {
            GameRuleManager.TeamPoints[targetTeam]++;
        }

        PlayPaperCanon(targetTeam);
    }

    void PlayPaperCanon(int goalTeam)
    {
        gameSetting = GameObject.Find("PlaySceneManager").GetComponent<GamePlayHandler>();
        // パーティクル再生
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            Vector2[] particlePos = new Vector2[4];
            particlePos[i] = gameSetting.Players[i].gameObject.transform.position;
            if (_sessionManager.PlayerTeam[i] == goalTeam)
            {
                GameObject particleObj = (GameObject)Resources.Load("PaperCanon");
                Instantiate(particleObj, particlePos[i], Quaternion.identity); // パーティクル用ゲームオブジェクト生成
            }
        }
    }
}
