using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Text.RegularExpressions;
public class Ball : MonoBehaviourPunCallbacks
{
    GameMode gameMode;
    GameSetting gameSetting;
    int lastColId;
    public int count;
    PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        if (GameStart.gameMode1 != "Online") 
        {
            this.GetComponent<PhotonRigidbody2DView>().enabled = false;
        }
        else
        {
            photonView = GetComponent<PhotonView>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameMode == null) 
        {
            gameMode = GameObject.Find("Scripts").GetComponent<GameMode>();
        }
        if (GameStart.gameMode1 == "Online" && PhotonNetwork.IsMasterClient)
        {
            if (!GameSetting.setupEnded) { return; }
            // 最も近いプレイヤーの初期化
            GameObject nearestPlayer = gameSetting.players[0];
            float minDistance = Vector2.Distance(this.transform.position, nearestPlayer.transform.position);

            // 全てのプレイヤーをチェックして最も近いプレイヤーを見つける
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                float distance = Vector2.Distance(this.transform.position, gameSetting.players[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPlayer = gameSetting.players[i];
                }
            }
            int id = int.Parse(Regex.Replace(nearestPlayer.name, @"[^0-9]", ""));
            photonView.TransferOwnership(id);
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
         if(col.gameObject.name == "GoalZoneLeft")
        {

            if(GameStart.teamMode == "FFA") 
            {
                if (GameStart.gameMode1 != "Online")
                {
                    GameMode.points[1]++;
                }
                else
                {
                      if (photonView.IsMine)
                    {
                        photonView.RPC("GoalProcess", RpcTarget.All, 1);
                    }
                }
            }
            else 
            {
                if (GameStart.gameMode1 != "Online")
                {
                    GameMode.teamPoints[1]++;
                }
                else
                {
                    if (photonView.IsMine)
                    {
                        photonView.RPC("GoalProcess", RpcTarget.All, 1);
                    }
                }
            }
            SoundEffect.soundTrigger[8] = 1;
            StartCoroutine(GameObject.Find("Scripts").GetComponent<GameMode>().BallReset(this.gameObject));
            PlayPaperCanon(1);
        }
        if (col.gameObject.name == "GoalZoneRight")
        {
            if (GameStart.teamMode == "FFA")
            {
                if (GameStart.gameMode1 != "Online")
                {
                    GameMode.points[0]++;
                }
                else 
                {
                    if (photonView.IsMine)
                    {
                        photonView.RPC("GoalProcess", RpcTarget.All, 0);
                    }
                }
            }
            else
            {
                if (GameStart.gameMode1 != "Online")
                {
                    GameMode.teamPoints[0]++;
                }

                else 
                {
                    if (photonView.IsMine)
                    {
                        photonView.RPC("GoalProcess", RpcTarget.All, 0);
                    }
                }
            }
            if (GameStart.gameMode1 != "Online")
            {
                SoundEffect.soundTrigger[8] = 1;
                StartCoroutine(GameObject.Find("Scripts").GetComponent<GameMode>().BallReset(this.gameObject));
                PlayPaperCanon(0);
            }

        }
    }

    void PlayPaperCanon(int goalTeam ) 
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        //パーティクル再生
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            Vector2[] particlePos = new Vector2[4];
             particlePos[i] = gameSetting.players[i].gameObject.transform.position;
            if(GameStart.playerTeam[i] == goalTeam) 
            {
                GameObject particleObj = (GameObject)Resources.Load("PaperCanon");
                Instantiate(particleObj, particlePos[i], Quaternion.identity); //パーティクル用ゲームオブジェクト生成
            }
        }
    }
    [PunRPC] 
    void GoalProcess(int targetTeam) 
    {
        if (count >= 1) { return; }
        GetComponent<PhotonRigidbody2DView>().enabled = false;
        count++;
        if (GameStart.teamMode == "FFA")
        {
            GameMode.points[targetTeam]++;
        }
        else 
        {
            GameMode.teamPoints[targetTeam]++;
        }  
        PlayPaperCanon(targetTeam);
        SoundEffect.soundTrigger[8] = 1;
        StartCoroutine(GameObject.Find("Scripts").GetComponent<GameMode>().BallReset(this.gameObject));
    }
}
