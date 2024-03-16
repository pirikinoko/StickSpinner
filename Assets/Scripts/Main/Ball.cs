using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Ball : MonoBehaviourPunCallbacks
{
    GameMode gameMode;
    GameSetting gameSetting;
    int lastColId;
    // Start is called before the first frame update
    void Start()
    {
        
        if(GameStart.gameMode1 != "Online") 
        {
            this.GetComponent<PhotonRigidbody2DView>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameMode == null) 
        {
            gameMode = GameObject.Find("Scripts").GetComponent<GameMode>();
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
                    if (lastColId == NetWorkMain.netWorkId)
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
                    if (lastColId == NetWorkMain.netWorkId)
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
                    if (lastColId == NetWorkMain.netWorkId)
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
                    if(lastColId == NetWorkMain.netWorkId) 
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

    void PlayPaperCanon(int goalTeam) 
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        //パーティクル再生
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            Vector2[] particlePos = new Vector2[4];
             particlePos[i] = gameSetting.players[i].gameObject.transform.position;
            if(GameStart.playerTeam[i] == goalTeam) 
            {
                GameObject particleObj = (GameObject)Resources.Load("PaperCanon2");
                Instantiate(particleObj, particlePos[i], Quaternion.identity); //パーティクル用ゲームオブジェクト生成
            }
        }
    }
    [PunRPC] 
    void GoalProcess(int targetTeam) 
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
        SoundEffect.soundTrigger[8] = 1;
        StartCoroutine(GameObject.Find("Scripts").GetComponent<GameMode>().BallReset(this.gameObject));
    }
}
