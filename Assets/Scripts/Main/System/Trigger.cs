using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trigger : MonoBehaviour
{

    float   pointTimer;
    int playerId;                   // プレイヤー番号(1～4)      
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
                            GameMode.points[playerId - 1] += 1;
                            GameMode.playParticle[playerId - 1] = 1;
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
        if (other.gameObject.CompareTag("thorn"))
        {
            for (int i = 0; i < 4; i++)
            {
                if (GameMode.killTimer[i, playerId - 1] > 0)
                {
                    GameMode.points[i] += 5;
                    GameMode.playParticle[i] = 2;
                    GameMode.killer = "Player" + (i + 1).ToString();
                    GameMode.died = "Player" + playerId.ToString();
                    GameMode.killTimer[i, playerId - 1] = 0;
                    GameMode.KillLogTimer = 5.0f;
                }
            }
        }

    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if(GameStart.Stage == 4)
        {
            if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Stick"))
            {
                //敵に触れてから五秒間キル判定
                GameMode.killTimer[playerId - 1, other.gameObject.GetComponent<Trigger>().playerId - 1] = 5.0f;
            }       
        }
    }
}
