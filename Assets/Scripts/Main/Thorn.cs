using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thorn : MonoBehaviour
{

    Vector2 CheckPos;
    public static byte[] triggerCheckPos{ get; set;} = new byte[GameStart.MaxPlayer];
    public static bool[] respownTrigger{  get; set;} = new bool[GameStart.MaxPlayer];
    public GameObject Player1, Player2, Player3, Player4, P1Name, P2Name, P3Name, P4Name;

    GameObject[] players  = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];

    /*@@ 保留  Renderer2D のことでは?
    public Renderer p1, p2 ,p3 ,p4 ,s1 ,s2, s3, s4;
    public Renderer[] playerrend = new Renderer[GameStart.MaxPlayer];
    public Renderer[] stickrend  = new Renderer[GameStart.MaxPlayer];
    */

    public Rigidbody2D stickrb1, stickrb2, stickrb3, stickrb4;
    public Rigidbody2D[] stickrb = new Rigidbody2D[GameStart.MaxPlayer];
    public static Vector2[] col  = new Vector2[GameStart.MaxPlayer];


    void Start()
    {
        //配列に代入
        players  = new GameObject[] {  Player1,  Player1,  Player1,  Player4 };
        nameTags = new GameObject[] {  P1Name,   P2Name,   P3Name,   P4Name };
        stickrb  = new Rigidbody2D[] { stickrb1, stickrb2, stickrb3, stickrb4 };
        /*@@
        playerrend[0] = p1;
        playerrend[1] = p2;
        playerrend[2] = p3;
        playerrend[3] = p4;
        stickrend[0] = s1;
        stickrend[1] = s2;
        stickrend[2] = s3;
        stickrend[3] = s4;

        for(int i = 0; i < GameStart.PlayerNumber; i++)
        {
            players[   i] = GameObject.Find("Player" + (i + 1).ToString());
            nameTags[  i] = GameObject.Find("P" + (i + 1).ToString() + "Text");
            playerrend[i].enabled = true;
            stickrend[ i].enabled = true;
            nameTags[  i].gameObject.SetActive(true); 
        }
        */
    }

    void Update()
    {
        PlayerRespown();
    }

    void PlayerRespown()
    {
    /*@@
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (respownTrigger[i])
            {
                players[i].gameObject.transform.position = CheckPoint.checkPos[i];
                nameTags[i].gameObject.SetActive(true);
                playerrend[i].enabled = true;
                stickrend[i].enabled = true;
                respownTrigger[i] = false;
            }
            if (playerrend[i].enabled == false)
            {
                players[i].gameObject.transform.position = col[i];
                stickrb[i].velocity = new Vector2(0f, 0f);
                stickrb[i].MoveRotation(0f);

            }
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    /*@@
        //トゲで死亡
        if (other.gameObject.CompareTag("Player"))
        {
            SoundEffect.DyukushiTrigger = 1;  //効果音
        }
        for (int i = 1; i < GameStart.PlayerNumber; i++)
        {
            if (other.gameObject.name == "Player" + (i + 1).ToString() && playerrend[i].enabled == true)　//リスポーンまでプレイヤーが見えなくなる
            {
                GameSetting.deathTimer[i] = true;
                col[i] = this.gameObject.transform.position;
                col[i].y += 0.5f;
                triggerCheckPos[i] = 1;
                playerrend[i].enabled = false;
                stickrend[i].enabled = false;
                nameTags[i].gameObject.SetActive(false);

                for (int j = 0; j < GameStart.PlayerNumber; j++)　   //死んだプレイヤーに五秒以内に触れていたプレイヤーに5ポイント(キル)
                {
                    if (other.gameObject.name == "Player" + (j + 1).ToString())
                    {
                       if(KillSystem.killTimer[i , j] > 0)
                        {
                            BattleMode.points[i] += 5;
                            KillSystem.killTimer[i, j] = 0;
                            BattleMode.KillLogTimer = 5;
                            BattleMode.playParticle[i]  = 2;
                            BattleMode.died.text = other.gameObject.name;
                            for(int k = 0; k < GameStart.PlayerNumber; k++)
                            {
                                BattleMode.killer.text = "Player" + (k + 1).ToString();
                            }
                        }
                    }
                }
            }
        }
        */
    }
}
