using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
//  プレイヤーの死亡処理は Controller.cs へ移動
//


public class Thorn : MonoBehaviour
{
    // コリジョン
    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーとの判定
        if (other.gameObject.CompareTag("Player"))
        {
            Body bdy = other.gameObject.GetComponent<Body>();
            // Stick1 をとる
            Controller cnt = other.gameObject.transform.GetChild(0).GetComponent<Controller>();
            //当たったプレイヤーを死亡させる
            cnt.StartDead();
            int id = bdy.id;
            if(GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Arcade") 
            {
                //無限モードの場合はゲームオーバー
                GameMode.isGameOver = true;
            }
          
        }
    }
}
