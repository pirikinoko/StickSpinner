using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    GameMode gameMode;
    GameSetting gameSetting;
    int lastColId;
    public int count;

    [Networked]
    public bool IsOwned { get; set; } // オーナーシップ管理

    [Networked]
    public int OwnerPlayerId { get; set; } // 所有プレイヤーID

    void Start()
    {
        count = 0;
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
    }

    void Update()
    {
        if (gameMode == null)
        {
            gameMode = GameObject.Find("Scripts").GetComponent<GameMode>();
        }

        if (GameStart.gameMode1 == "Online" && Runner.IsServer)
        {
            if (!GameSetting.setupEnded) { return; }

            // 最も近いプレイヤーの初期化
            GameObject nearestPlayer = gameSetting.players[0];
            float minDistance = Vector2.Distance(this.transform.position, nearestPlayer.transform.position);

            // 全てのプレイヤーをチェックして最も近いプレイヤーを見つける
            for (int i = 0; i < GameStart.PlayerCount; i++)
            {
                float distance = Vector2.Distance(this.transform.position, gameSetting.players[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPlayer = gameSetting.players[i];
                }
            }

            int id = nearestPlayer.GetComponent<NetworkObject>().Owner.PlayerId;
            TransferOwnership(id);
        }
    }

    private void TransferOwnership(int newOwnerId)
    {
        if (Runner.IsServer)
        {
            OwnerPlayerId = newOwnerId;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            lastColId = col.gameObject.GetComponent<Body>().id;
        }
        if (col.gameObject.CompareTag("Stick"))
        {
            lastColId = col.gameObject.GetComponent<Controller>().id;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!GameSetting.Playable) { return; }

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
        if (GameStart.gameMode1 == "Online" && Runner.IsServer)
        {
            Runner.SendRpc(nameof(GoalProcess), RpcTarget.All, targetTeam);
        }
        else
        {
            ProcessGoal(targetTeam);
        }

        SoundEffect.soundTrigger[8] = 1;
        StartCoroutine(GameObject.Find("Scripts").GetComponent<GameMode>().BallReset(this.gameObject));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void GoalProcess(int targetTeam, RpcInfo info = default)
    {
        if (count >= 1) { return; }
        count++;

        ProcessGoal(targetTeam);
    }

    private void ProcessGoal(int targetTeam)
    {
        if (GameStart.teamMode == "FFA")
        {
            GameMode.points[targetTeam]++;
        }
        else
        {
            GameMode.teamPoints[targetTeam]++;
        }

        PlayPaperCanon(targetTeam);
    }

    void PlayPaperCanon(int goalTeam)
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        // パーティクル再生
        for (int i = 0; i < GameStart.PlayerCount; i++)
        {
            Vector2[] particlePos = new Vector2[4];
            particlePos[i] = gameSetting.players[i].gameObject.transform.position;
            if (GameStart.playerTeam[i] == goalTeam)
            {
                GameObject particleObj = (GameObject)Resources.Load("PaperCanon");
                Instantiate(particleObj, particlePos[i], Quaternion.identity); // パーティクル用ゲームオブジェクト生成
            }
        }
    }
}
