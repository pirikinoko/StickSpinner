using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
//  プレイヤーの死亡処理は Controller.cs へ移動
//


public class Thorn : MonoBehaviour
{

    Vector2 CheckPos;
    public static byte[] triggerCheckPos{ get; set;} = new byte[GameStart.MaxPlayer];
    public static bool[] respownTrigger{  get; set;} = new bool[GameStart.MaxPlayer];

    GameObject[] players  = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    Rigidbody2D[] stickrb = new Rigidbody2D[GameStart.MaxPlayer];
   

    void Start()
    {
        // 配列に代入
        for (int i = 0; i < GameStart.MaxPlayer; i++)
        {
            players[ i] = GameObject.Find("Player" + (i + 1).ToString());
            nameTags[i] = GameObject.Find("P" + (i + 1).ToString() + "NameTag");
            // thorn21, 27 でエラーがでるので例外処理を書いておく
            try {
                stickrb[i]  = GameObject.Find("Stick"  + (i + 1).ToString()).GetComponent<Rigidbody2D>();
            } catch (System.NullReferenceException e) {
                //Debug.Log("Error:" + gameObject.name);
            } catch (System.IndexOutOfRangeException e) {
                //添字アクセス違反が発生した時の対処処理
                //このサンプルコードではこちらが実行される。
                //Debug.Log("Error:" + gameObject.name);
            } 
		}
    }

    void Update()
    {
    }

    // コリジョン
    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーとの判定
        if (other.gameObject.CompareTag("Player"))
        {
            Body bdy = other.gameObject.GetComponent<Body>();
            // Stick1 をとる
            Controller cnt = other.gameObject.transform.GetChild(0).GetComponent<Controller>();
            cnt.StartDead();
            int id = bdy.id;
            nameTags[id - 1].gameObject.SetActive(false);
            if(GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Arcade") 
            {
                //無限モードゲームオーバー
                GameMode.isGameOver = true;
            }
          
        }
    }
}
