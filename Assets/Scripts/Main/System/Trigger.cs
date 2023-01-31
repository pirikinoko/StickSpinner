using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trigger : MonoBehaviour
{

    float   pointTimer;
    //public static float[,] killTimer = new float[4, 4];
    int playerId;                   // プレイヤー番号(1～4)
                                    //int otherId = 0;                // 当たった相手のプレイヤー(1～4)

    GameMode gamemode;
    void Start()
    {
        gamemode = GameObject.Find("Scripts").GetComponent<GameMode>();

        // ボディかスティックより ID を得る
        Controller cnt = GetComponent<Controller>();
        if (cnt)
		{   // Controller.cs
            playerId = cnt.id;
		}
        else
        {   // Body.cs
            Body bdy = GetComponent<Body>();
            playerId = bdy.id;
		}
        pointTimer = 0;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        //
        if (other.gameObject.name.Contains("CheckPos"))
        {
            GameSetting.respownPos[playerId - 1] = other.gameObject.transform.position;
            Debug.Log("P" + playerId.ToString() + "のCheckPointを設定");
        }

        if (this.gameObject.name.Contains("Player"))
        {
            if (other.gameObject.name.Contains("Point"))
            {

                {
                    if (GameSetting.PlayTime > 0 && ButtonInGame.Paused != 1)
                    {
                        pointTimer += Time.deltaTime;
                        if (pointTimer > 2)
                        {
                            SoundEffect.KinTrigger = 1;
                            GameMode.points[playerId] += 1;
                            GameMode.playParticle[playerId] = 1;
                            pointTimer = 0;
                        }
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameStart.Stage != 4)　　//対戦モードを除き
        {
            //自身がプレイヤーなら
            if (this.gameObject.CompareTag("Player"))
            {
                if(other.gameObject.name  == "GoalFlag")
                {
                    gamemode.GoalProcess(playerId);
                }
              
            }
        }
    }
    /*
    private void OnCollisionStay2D(Collision2D other)
    {
        if(GameStart.Stage == 4)
        {
            //敵に触れてから五秒間キル判定
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (this.gameObject.name == "Player" + (i + 1).ToString() || this.gameObject.name == "Stick" + (i + 1).ToString())
                {
                    for (int j = 0; i < GameStart.PlayerNumber; i++)
                    {
                        if (other.gameObject.name == "Player" + (j + 1).ToString() || other.gameObject.name == "Stick" + (j + 1).ToString())
                        {
                            killTimer[i, j] = 5.0f;
                        }
                    }
                }
            }
        }      
    }

    void KillTimer()
    {
        //敵に触れてから五秒間キル判定
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            for (int j = 0; i < GameStart.PlayerNumber; i++)
            {
                if (killTimer[i, j] > 0)
                {
                    killTimer[i, j] -= Time.deltaTime;
                }
            }

        }
    }*/
}
