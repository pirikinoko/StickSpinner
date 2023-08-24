using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Trigger : MonoBehaviour
{

    float   pointTimer;
    int playerId;                   // プレイヤー番号(1～4)      
    GameMode gamemode;
    int checkNum = 0;
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


        if (this.gameObject.name.Contains("Player"))
        {
            if (other.gameObject.name.Contains("CheckPos"))
            {
                int tmp = int.Parse(Regex.Replace(other.gameObject.name, @"[^0-9]", ""));
                if (tmp > checkNum)
                {
                    GameSetting.respownPos[playerId - 1] = other.gameObject.transform.position;
                    checkNum = tmp;
                    SoundEffect.soundTrigger[2] = 1;
                }

            }

            if (other.gameObject.name.Contains("Point"))
            {

                {
                    if (GameSetting.playTime > 0 && ButtonInGame.Paused != 1)
                    {
                        pointTimer += Time.deltaTime;
                        if (pointTimer > 1)
                        {
                            SoundEffect.soundTrigger[6] = 1;
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
        if (GameStart.gameMode2 != "Arcade")　　//対戦モードを除き
        {
            //自身がプレイヤーならゴール処理
            if (this.gameObject.CompareTag("Player"))
            {
                if(other.gameObject.name  == "GoalFlag")
                {
                    gamemode.GoalProcess(playerId);
                }
            }
        }
        if (other.gameObject.CompareTag("thorn"))　//対戦モード時トゲに当たった時最後に触れていたプレイヤーキル(5秒)
        {
            for (int i = 0; i < 4; i++)
            {
                if (GameMode.killTimer[i, playerId - 1] > 0) //トゲに当たったプレイヤーに最後に5秒以内に触れていたプレイヤーにポイント付与
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
        if(GameStart.gameMode2 == "Arcade")
        {
            if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Stick"))
            {
                //敵に触れてから5秒間キル判定            
                for (int i = 0; i < 4; i++)
                {
                    GameMode.killTimer[i, other.gameObject.GetComponent<Trigger>().playerId - 1] = 0;  //一人死亡時にキルポイントを得られるのは最後に触れていた一人のみ
                }
                GameMode.killTimer[playerId - 1, other.gameObject.GetComponent<Trigger>().playerId - 1] = 3.0f;
            }       
        }
       
    }
}
